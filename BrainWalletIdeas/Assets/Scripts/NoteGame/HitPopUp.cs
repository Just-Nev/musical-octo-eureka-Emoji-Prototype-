using UnityEngine;

public class HitPopUp : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float fadeDuration = 0.6f;

    private SpriteRenderer sr;
    private Color originalColor;
    private Vector3 startPos;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        startPos = transform.localPosition;
    }

    void OnEnable()
    {
        sr.color = originalColor; // reset alpha
        transform.localPosition = startPos;
        StopAllCoroutines();
        StartCoroutine(FadeAndRise());
    }

    public void SetSprite(Sprite sprite)
    {
        sr.sprite = sprite;
    }

    private System.Collections.IEnumerator FadeAndRise()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
