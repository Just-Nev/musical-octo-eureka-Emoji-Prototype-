using System.Collections;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Header("Food Prefabs")]
    public GameObject foodPrefab;
    public GameObject badFoodPrefab;

    [Header("Spawn Settings")]
    public float spawnInterval = 0.8f;         // Initial spawn interval
    public float minSpawnInterval = 0.3f;    // Fastest spawn rate
    public float badFoodChance = 0.2f;       // 20% chance to spawn bad food

    [Header("Fall Speed Settings")]
    public float fallSpeed = 3f;             // Initial fall speed
    public float maxFallSpeed = 8f;          // Max fall speed

    [Header("Difficulty Scaling")]
    public float difficultyRampTime = 60f;   // Time (in seconds) to reach max difficulty

    private float difficultyTimer = 0f;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        // Initialize difficulty-based parameters at start
        UpdateDifficultyParameters();

        // Start spawning
        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        // Update difficulty timer
        difficultyTimer += Time.deltaTime;

        // Update spawn interval and fall speed based on difficulty progress
        UpdateDifficultyParameters();
    }

    void UpdateDifficultyParameters()
    {
        float progress = Mathf.Clamp01(difficultyTimer / difficultyRampTime);
        spawnInterval = Mathf.Lerp(1f, minSpawnInterval, progress);
        fallSpeed = Mathf.Lerp(3f, maxFallSpeed, progress);
    }

    IEnumerator SpawnLoop()
    {
        // Spawn immediately at start
        SpawnFood();

        while (true)
        {
            float timer = 0f;
            while (timer < spawnInterval)
            {
                timer += Time.deltaTime;
                yield return null; // wait next frame
            }
            SpawnFood();
        }
    }

    void SpawnFood()
    {
        // Screen boundaries
        float screenLeft = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        float screenRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

        float offset = 0.5f; // padding from edges
        float randomX = Random.Range(screenLeft + offset, screenRight - offset);
        float spawnY = mainCam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y + 1f;

        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

        // Pick food type
        GameObject prefabToSpawn = Random.value < badFoodChance ? badFoodPrefab : foodPrefab;

        GameObject food = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // Give it downward movement
        Rigidbody2D rb = food.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = food.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.down * fallSpeed;
    }
}



