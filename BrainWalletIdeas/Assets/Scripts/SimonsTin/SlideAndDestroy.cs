using UnityEngine;

public class SlideAndDestroy : MonoBehaviour
{
    public float slideSpeed = 2f;
    public float lifetime = 5f;
    public float startDelay = 0f;  // optional delay before sliding

    private float elapsed = 0f;

    void Update()
    {
        if (startDelay > 0f)
        {
            startDelay -= Time.deltaTime;
            return;
        }

        transform.position += Vector3.left * slideSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        elapsed += Time.deltaTime;
        if (elapsed >= lifetime)
            Destroy(gameObject);
    }
}



