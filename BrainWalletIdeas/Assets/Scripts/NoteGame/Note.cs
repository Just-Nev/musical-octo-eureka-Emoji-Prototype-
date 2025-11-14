using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed = 4f;

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }
}

