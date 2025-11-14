using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float spawnChance = 0.1f; // chance between 0 and 1
    public bool spawnOnlyOnce = false; // for unique items like Finger
    [HideInInspector] public bool hasSpawned = false;
}

public class FoodSpawner : MonoBehaviour
{
    [Header("Spawnable Objects (Add as many as you like)")]
    public List<SpawnableObject> spawnables = new List<SpawnableObject>();

    [Header("Spawn Settings")]
    public float spawnInterval = 0.8f;
    public float minSpawnInterval = 0.3f;

    [Header("Fall Speed Settings")]
    public float fallSpeed = 3f;
    public float maxFallSpeed = 8f;

    [Header("Difficulty Scaling")]
    public float difficultyRampTime = 60f;

    [Header("Boost Settings (for Finger Mode, etc.)")]
    public float boostedSpawnInterval = 0.1f;

    private float difficultyTimer = 0f;
    private Camera mainCam;
    private bool boosted = false;
    private Coroutine boostRoutine;

    void Start()
    {
        mainCam = Camera.main;
        UpdateDifficultyParameters();
        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        difficultyTimer += Time.deltaTime;
        UpdateDifficultyParameters();
    }

    void UpdateDifficultyParameters()
    {
        float progress = Mathf.Clamp01(difficultyTimer / difficultyRampTime);

        if (!boosted)
            spawnInterval = Mathf.Lerp(1f, minSpawnInterval, progress);
        else
            spawnInterval = boostedSpawnInterval;

        fallSpeed = Mathf.Lerp(3f, maxFallSpeed, progress);
    }

    IEnumerator SpawnLoop()
    {
        SpawnFood();

        while (true)
        {
            float timer = 0f;
            while (timer < spawnInterval)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            SpawnFood();
        }
    }

    void SpawnFood()
    {
        if (spawnables == null || spawnables.Count == 0) return;

        // Screen boundaries
        float screenLeft = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        float screenRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        float offset = 0.5f;
        float randomX = Random.Range(screenLeft + offset, screenRight - offset);
        float spawnY = mainCam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y + 1f;
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

        // Determine which prefab to spawn based on probabilities
        GameObject prefabToSpawn = ChooseRandomPrefab();

        if (prefabToSpawn == null) return;

        GameObject food = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        Rigidbody2D rb = food.GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = food.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.down * fallSpeed;
    }

    GameObject ChooseRandomPrefab()
    {
        float totalChance = 0f;

        // calculate total chance only for valid prefabs
        foreach (var item in spawnables)
        {
            if (item.prefab != null && (!item.spawnOnlyOnce || !item.hasSpawned))
                totalChance += item.spawnChance;
        }

        if (totalChance <= 0f) return null;

        float roll = Random.value * totalChance;
        float cumulative = 0f;

        foreach (var item in spawnables)
        {
            if (item.prefab == null) continue;
            if (item.spawnOnlyOnce && item.hasSpawned) continue;

            cumulative += item.spawnChance;

            if (roll <= cumulative)
            {
                if (item.spawnOnlyOnce)
                    item.hasSpawned = true;
                return item.prefab;
            }
        }

        return null;
    }

    // Called when finger mode is activated
    public void ActivateFingerBoost(float duration)
    {
        if (boostRoutine != null)
            StopCoroutine(boostRoutine);
        boostRoutine = StartCoroutine(FingerBoostRoutine(duration));
    }

    private IEnumerator FingerBoostRoutine(float duration)
    {
        boosted = true;
        yield return new WaitForSeconds(duration);
        boosted = false; // return to normal spawn rate
    }
}








