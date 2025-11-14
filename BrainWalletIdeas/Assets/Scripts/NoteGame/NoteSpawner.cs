using UnityEngine;

public class MultiLaneSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject notePrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 1f;

    [Header("Sprites in Order")]
    public Sprite[] noteSprites; // Assign sprites in the order you want

    private int spriteIndex = 0;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnNote();
        }
    }

    void SpawnNote()
    {
        if (notePrefab == null || spawnPoints.Length == 0 || noteSprites.Length == 0)
            return;

        // Random lane instead of ordered laneIndex
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject note = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);

        // Set sprite in your exact order
        SpriteRenderer sr = note.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = noteSprites[spriteIndex];
        }

        // Advance sprite index (loops)
        spriteIndex++;
        if (spriteIndex >= noteSprites.Length)
            spriteIndex = 0;
    }
}



