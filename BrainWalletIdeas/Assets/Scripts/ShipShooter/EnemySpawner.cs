using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int rows = 1;
    [SerializeField] private int columns = 8;
    [SerializeField] private float spacing = 0.8f;

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = new Vector3(col * spacing, row * -spacing, 0);
                Instantiate(enemyPrefab, transform.position + pos, Quaternion.identity, transform);
            }
        }
    }
}

