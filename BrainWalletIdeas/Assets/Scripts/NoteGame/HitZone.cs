using UnityEngine;

public class HitZone : MonoBehaviour
{
    [Header("Hit Timing")]
    public float perfectRange = 0.1f;
    public float goodRange = 0.25f;

    [Header("Popup System")]
    public GameObject popupPrefab; // prefab with SpriteRenderer + HitPopup.cs
    public Transform popupSpawnPoint;
    public Sprite perfectSprite;
    public Sprite goodSprite;
    public Sprite badSprite;

    [Header("Touch Detection")]
    public LayerMask hitZoneLayer; // assign HitZone layer

    private GameObject noteInZone;
    private bool hasHitThisTap = false;

    void Update()
    {
        bool tapped = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
            tapped = true;
#else
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        tapped = true;
#endif

        if (tapped)
        {
            hasHitThisTap = false;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CheckTouch(worldPos);
        }
    }

    void CheckTouch(Vector2 worldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPos, hitZoneLayer);

        if (hit != null && hit.gameObject == gameObject)
            TryHitNote();
    }

    void TryHitNote()
    {
        if (hasHitThisTap) return;
        hasHitThisTap = true;

        if (noteInZone == null)
        {
            SpawnHitPopup(badSprite);
            ScoreManager.Instance.AddBad();
            return;
        }

        float distance = Mathf.Abs(noteInZone.transform.position.y - transform.position.y);

        if (distance <= perfectRange)
        {
            SpawnHitPopup(perfectSprite);
            ScoreManager.Instance.AddPerfect();
        }
        else if (distance <= goodRange)
        {
            SpawnHitPopup(goodSprite);
            ScoreManager.Instance.AddGood();
        }
        else
        {
            SpawnHitPopup(badSprite);
            ScoreManager.Instance.AddBad();
        }

        Destroy(noteInZone);
        noteInZone = null;
    }



    void SpawnHitPopup(Sprite sprite)
    {
        if (popupPrefab == null || popupSpawnPoint == null) return;

        GameObject go = Instantiate(popupPrefab, popupSpawnPoint.position, Quaternion.identity, transform.parent);
        go.GetComponent<HitPopUp>().SetSprite(sprite);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
            noteInZone = collision.gameObject;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Note") && noteInZone == collision.gameObject)
            noteInZone = null;
    }
}







