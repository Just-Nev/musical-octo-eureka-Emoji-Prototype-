using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float fadeDuration = 0.6f;

    private TextMeshProUGUI textMesh;
    private CanvasGroup canvasGroup;
    private Vector3 startPos;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        startPos = transform.position;
    }

    void OnEnable()
    {
        canvasGroup.alpha = 1f;
        transform.position = startPos;
        StopAllCoroutines();
        StartCoroutine(FadeAndRise());
    }

    public void SetText(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
    }

    private System.Collections.IEnumerator FadeAndRise()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}

