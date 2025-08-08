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

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("Missing SpriteRenderer on Player!");

        // Store the original scale for squash/stretch animation
        originalScale = transform.localScale;

        // Get the animator for total score text and set it to unscaled time (not affected by timeScale)
        if (totalScoreText != null)
        {
            totalScoreAnimator = totalScoreText.GetComponent<Animator>();
            if (totalScoreAnimator != null)
                totalScoreAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        // Get the animator for score text
        if (scoreText != null)
        {
            scoreAnimator = scoreText.GetComponent<Animator>();
            if (scoreAnimator != null)
                scoreAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        // Set initial score display
        UpdateScoreUI();
    }

    void Update()
    {
        HandleInput();

        // Move player toward the touch position if allowed
        if (isTouching && !stopMovement && !IsTouchOverUI())
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        // Handle sprite and movement reset after cooldown ends
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.unscaledDeltaTime;
            if (cooldownTimer <= 0)
            {
                if (spriteRenderer != null)
                    spriteRenderer.sprite = normalSprite;

                stopMovement = false;
            }
        }
    }

    void HandleInput()
    {
#if UNITY_EDITOR
        // In editor: use mouse input
        isTouching = Input.GetMouseButton(0);
        if (isTouching)
        {
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchWorldPos.z = 0f;
            targetPosition = touchWorldPos;
        }
#else
        // On device: use touch input
        isTouching = Input.touchCount > 0;
        if (isTouching)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
            touchWorldPos.z = 0f;
            targetPosition = touchWorldPos;
        }
#endif

        // Hide UI buttons when user is touching (and not over a UI element)
        bool shouldHideButtons = isTouching && !IsTouchOverUI();

        foreach (GameObject button in uiButtonsToHide)
        {
            if (button != null)
                button.SetActive(!shouldHideButtons);
        }
    }

    // Check if the current touch is over a UI element
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

    // Detect collision with food (enter or stay)
    private void OnTriggerEnter2D(Collider2D other) => TryConsumeFood(other);
    private void OnTriggerStay2D(Collider2D other) => TryConsumeFood(other);

    // Logic for eating food or bad food
    private void TryConsumeFood(Collider2D other)
    {
        // Skip if still in cooldown
        if (cooldownTimer > 0f) return;

        if (other.CompareTag("Food"))
        {
            Vector3 floatingPos = other.transform.position + Vector3.up * 0.5f;

            // Destroy food and update player state
            Destroy(other.gameObject);
            cooldownTimer = eatCooldown;
            if (spriteRenderer != null)
                spriteRenderer.sprite = cooldownSprite;

            stopMovement = false;
            score++;
            UpdateScoreUI();

            // Show floating visual effect
            if (floatingEffectPrefab != null)
                Instantiate(floatingEffectPrefab, floatingPos, Quaternion.identity);

            // Play squash and stretch animation
            StopAllCoroutines();
            StartCoroutine(SquashAndStretch());
        }
        else if (other.CompareTag("BadFood"))
        {
            // Destroy bad food and stop movement
            Destroy(other.gameObject);
            cooldownTimer = badFoodCooldown;
            if (spriteRenderer != null)
                spriteRenderer.sprite = badCooldownSprite;

            stopMovement = true;

            // Play squash and stretch animation
            StopAllCoroutines();
            StartCoroutine(SquashAndStretch());
        }
    }

    // Update UI elements with current score values
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "+" + score.ToString();

            // Trigger score pop animation
            if (scoreAnimator != null)
                scoreAnimator.Play("ScorePop", -1, 0f);
        }

        if (totalScoreText != null)
        {
            // Calculate a "random" total score for visual effect
            int totalScore = score * Random.Range(77, 96);
            totalScoreText.text = "+" + totalScore.ToString();

            // Trigger total score pop animation
            if (totalScoreAnimator != null)
                totalScoreAnimator.Play("TotalScorePop", -1, 0f);
        }
    }

    // Animate a squash and stretch effect for visual feedback
    private IEnumerator SquashAndStretch()
    {
        float duration = 0.1f;

        // Squash the object
        transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);
        yield return new WaitForSeconds(duration);

        // Stretch the object
        transform.localScale = new Vector3(originalScale.x * 0.9f, originalScale.y * 1.1f, originalScale.z);
        yield return new WaitForSeconds(duration);

        // Return to normal scale
        transform.localScale = originalScale;
    }
}







