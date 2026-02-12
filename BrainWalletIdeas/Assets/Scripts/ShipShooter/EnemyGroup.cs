using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDownAmount = 0.5f;

    [Header("Boundaries")]
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;

    private int direction = 1; // 1 = right, -1 = left

    void Update()
    {
        // Move the whole group horizontally
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

        // Check each enemy's position relative to boundaries
        foreach (Transform enemy in transform)
        {
            if (enemy != null)
            {
                if (enemy.position.x > rightBoundary.position.x && direction == 1 ||
                    enemy.position.x < leftBoundary.position.x && direction == -1)
                {
                    MoveDownAndReverse();
                    break;
                }
            }
        }
    }

    void MoveDownAndReverse()
    {
        direction *= -1; // reverse direction
        transform.position += Vector3.down * moveDownAmount; // move formation down
    }
}


