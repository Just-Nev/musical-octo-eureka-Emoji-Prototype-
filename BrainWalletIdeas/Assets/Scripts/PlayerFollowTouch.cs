using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class PlayerFollowTouch : MonoBehaviour
{
    // Movement and cooldown settings
    public float moveSpeed = 5f;
    public float eatCooldown = 1f;
    public float badFoodCooldown = 2f;

    // Sprites for different player states
    public Sprite normalSprite;
    public Sprite cooldownSprite;
    public Sprite badCooldownSprite;

    // Score and UI references
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI totalScoreText;
    public GameObject floatingEffectPrefab;

    // UI elements to hide while player is touching screen
    public GameObject[] uiButtonsToHide;

    // Internal variables
    private SpriteRenderer spriteRenderer;
    private float cooldownTimer = 0f;
    private Vector3 targetPosition;
    private bool isTouching = false;
    private Vector3 originalScale;
    private bool stopMovement = false;

    private Animator scoreAnimator;
    private Animator totalScoreAnimator;

    // Finger mode
    private bool fingerMode = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("Missing SpriteRenderer on Player!");

        originalScale = transform.localScale;

        if (totalScoreText != null)
        {
            totalScoreAnimator = totalScoreText.GetComponent<Animator>();
            if (totalScoreAnimator != null)
                totalScoreAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        if (scoreText != null)
        {
            scoreAnimator = scoreText.GetComponent<Animator>();
            if (scoreAnimator != null)
                scoreAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        UpdateScoreUI();
    }

    void Update()
    {
        HandleInput();

        if (!fingerMode)
        {
            // Normal player-following mode
            if (isTouching && !stopMovement && !IsTouchOverUI())
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            // In finger mode: tap directly on food
            HandleFingerTapFood();
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.unscaledDeltaTime;
            if (cooldownTimer <= 0)
            {
                if (spriteRenderer != null && !fingerMode)
                    spriteRenderer.sprite = normalSprite;

                stopMovement = false;
            }
        }
    }

    void HandleInput()
    {
#if UNITY_EDITOR
        isTouching = Input.GetMouseButton(0);
        if (isTouching)
        {
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchWorldPos.z = 0f;
            targetPosition = touchWorldPos;
        }
#else
        isTouching = Input.touchCount > 0;
        if (isTouching)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
            touchWorldPos.z = 0f;
            targetPosition = touchWorldPos;
        }
#endif

        bool shouldHideButtons = isTouching && !IsTouchOverUI();
        foreach (GameObject button in uiButtonsToHide)
        {
            if (button != null)
                button.SetActive(!shouldHideButtons);
        }
    }

    private bool IsTouchOverUI()
    {
#if UNITY_EDITOR
        return EventSystem.current.IsPointerOverGameObject();
#else
        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        else
            return false;
#endif
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finger"))
        {
            EnterFingerMode();
            Destroy(other.gameObject);
        }
        else if (!fingerMode)
        {
            TryConsumeFood(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!fingerMode)
            TryConsumeFood(other);
    }

    private void TryConsumeFood(Collider2D other)
    {
        if (cooldownTimer > 0f) return;

        if (other.CompareTag("Food"))
        {
            Vector3 floatingPos = other.transform.position + Vector3.up * 0.5f;

            Destroy(other.gameObject);
            cooldownTimer = eatCooldown;
            if (spriteRenderer != null)
                spriteRenderer.sprite = cooldownSprite;

            stopMovement = false;
            score++;
            UpdateScoreUI();

            if (floatingEffectPrefab != null)
                Instantiate(floatingEffectPrefab, floatingPos, Quaternion.identity);

            StopAllCoroutines();
            StartCoroutine(SquashAndStretch());
        }
        else if (other.CompareTag("BadFood"))
        {
            Destroy(other.gameObject);
            cooldownTimer = badFoodCooldown;
            if (spriteRenderer != null)
                spriteRenderer.sprite = badCooldownSprite;

            stopMovement = true;

            StopAllCoroutines();
            StartCoroutine(SquashAndStretch());
        }
        else if (other.CompareTag("Timer"))
        {
            Destroy(other.gameObject);
            Time.timeScale = 0.5f;
            Invoke("ResetTime", 4f);

            cooldownTimer = eatCooldown;
            if (spriteRenderer != null)
                spriteRenderer.sprite = cooldownSprite;

            stopMovement = false;

            StopAllCoroutines();
            StartCoroutine(SquashAndStretch());
        }
    }

    void ResetTime() 
    {
        Time.timeScale = 1;
    }


    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "+" + score.ToString();
            if (scoreAnimator != null)
                scoreAnimator.Play("ScorePop", -1, 0f);
        }

        if (totalScoreText != null)
        {
            int totalScore = score * Random.Range(77, 96);
            totalScoreText.text = "+" + totalScore.ToString();

            if (totalScoreAnimator != null)
                totalScoreAnimator.Play("TotalScorePop", -1, 0f);
        }
    }

    private IEnumerator SquashAndStretch()
    {
        float duration = 0.1f;
        transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);
        yield return new WaitForSeconds(duration);

        transform.localScale = new Vector3(originalScale.x * 0.9f, originalScale.y * 1.1f, originalScale.z);
        yield return new WaitForSeconds(duration);

        transform.localScale = originalScale;
    }

    // ---------------- Finger Mode ----------------

    private void EnterFingerMode()
    {
        fingerMode = true;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false; // Hide player sprite

        // Start countdown to exit finger mode
        StartCoroutine(FingerModeTimer(10f));

        // Boost spawner during finger mode
        FoodSpawner spawner = FindObjectOfType<FoodSpawner>();
        if (spawner != null)
        {
            spawner.ActivateFingerBoost(10f);
        }
    }

    private IEnumerator FingerModeTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Exit finger mode after duration
        fingerMode = false;
        if (spriteRenderer != null)
            spriteRenderer.enabled = true; // Show player sprite again
    }

    private void HandleFingerTapFood()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && !IsTouchOverUI())
        {
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchWorldPos.z = 0f;

            RaycastHit2D hit = Physics2D.Raycast(touchWorldPos, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Food"))
            {
                TapConsumeFood(hit.collider.gameObject);
            }
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !IsTouchOverUI())
        {
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            touchWorldPos.z = 0f;

            RaycastHit2D hit = Physics2D.Raycast(touchWorldPos, Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Food"))
            {
                TapConsumeFood(hit.collider.gameObject);
            }
        }
#endif
    }

    private void TapConsumeFood(GameObject foodObj)
    {
        Vector3 floatingPos = foodObj.transform.position + Vector3.up * 0.5f;

        Destroy(foodObj);
        score++;
        UpdateScoreUI();

        if (floatingEffectPrefab != null)
            Instantiate(floatingEffectPrefab, floatingPos, Quaternion.identity);
    }
}


