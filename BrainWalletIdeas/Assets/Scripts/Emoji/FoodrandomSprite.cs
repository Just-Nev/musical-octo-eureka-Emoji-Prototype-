using UnityEngine;

public class FoodRandomSprite : MonoBehaviour
{
    public Sprite[] foodSprites;                // Array of food sprites
    public Vector2 sizeRange = new Vector2(0.5f, 1.5f); // Random size range
    public Vector2 rotationSpeedRange = new Vector2(10f, 50f); // Degrees per second

    private SpriteRenderer spriteRenderer;
    private float rotationSpeed;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Choose a random sprite
        if (foodSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, foodSprites.Length);
            spriteRenderer.sprite = foodSprites[randomIndex];
        }
        else
        {
            Debug.LogWarning("No sprites assigned to foodSprites array.");
        }

        // Random scale
        float randomScale = Random.Range(sizeRange.x, sizeRange.y);
        transform.localScale = new Vector3(randomScale, randomScale, 1f);

        // Random rotation speed and direction (positive or negative)
        float speed = Random.Range(rotationSpeedRange.x, rotationSpeedRange.y);
        rotationSpeed = Random.value < 0.5f ? speed : -speed;
    }

    void Update()
    {
        // Rotate smoothly over time
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}


