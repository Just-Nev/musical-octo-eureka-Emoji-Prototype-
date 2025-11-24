using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private float yOffset = 0.2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (hitEffect != null)
            {
                Vector3 spawnPos = transform.position + new Vector3(0, yOffset, 0);
                Instantiate(hitEffect, spawnPos, Quaternion.identity);
            }

            // ADD SCORE
            ScoreManagerSpace.Instance.AddScore(453);

            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}






