using UnityEngine;
using TMPro;

public class FloatingEffect : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float fadeDuration = 1f;
    public float floatHeight = 1f;

    private float timer = 0f;
    private Vector3 startPos;
    private Color originalColor;

    private SpriteRenderer spriteRenderer;
    private TextMeshPro textMesh;

    void Start()
    {
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMesh = GetComponent<TextMeshPro>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        else if (textMesh != null)
            originalColor = textMesh.color;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Move upward
        float progress = timer / fadeDuration;
        transform.position = startPos + Vector3.up * floatHeight * progress;

        // Fade out
        float alpha = Mathf.Lerp(originalColor.a, 0f, progress);
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }
        else if (textMesh != null)
        {
            Color c = textMesh.color;
            c.a = alpha;
            textMesh.color = c;
        }

        // Destroy when done
        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
}

