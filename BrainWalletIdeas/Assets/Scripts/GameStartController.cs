using TMPro;
using UnityEngine;

public class GameStartController : MonoBehaviour
{
    // UI elements
    public TextMeshProUGUI startText;
    public TextMeshProUGUI timerText;
    public Animator timerAnimator;

    // Countdown settings
    public float countdownStart = 10f;

    // UI panels
    public GameObject endScreenObject;
    public GameObject Game;
    public GameObject GameUi;

    // Internal state
    private float countdownTimer;
    private bool gameStarted = false;
    private bool timerRunning = false;
    private bool hasStartedLoopingAnimation = false;

    void Start()
    {
        // Pause game at the beginning
        Time.timeScale = 0f;

        // Initialize timer
        countdownTimer = countdownStart;
        UpdateTimerText();

        // Hide end screen initially
        if (endScreenObject != null)
            endScreenObject.SetActive(false);

        // Reset animation trigger
        hasStartedLoopingAnimation = false;

        // Set timer animator to idle (neutral state)
        if (timerAnimator != null)
        {
            timerAnimator.Play("Idle");
        }
    }

    void Update()
    {
        // Start game on first touch/click
        if (!gameStarted && Input.GetMouseButtonDown(0))
        {
            StartGame();
        }

        // Run countdown timer if active
        if (timerRunning && countdownTimer > 0f)
        {
            countdownTimer -= Time.unscaledDeltaTime;

            // Play looping animation when timer reaches 10s or less
            if (!hasStartedLoopingAnimation && countdownTimer <= 10f)
            {
                hasStartedLoopingAnimation = true;

                if (timerAnimator != null)
                {
                    timerAnimator.Play("TimerSquashStretchLoop");
                    // Ensure this animation is set to loop in the Animator
                }
            }

            // End game when timer runs out
            if (countdownTimer <= 0f)
            {
                countdownTimer = 0f;
                timerRunning = false;
                EndGame();
            }

            UpdateTimerText();
        }
    }

    void StartGame()
    {
        // Resume game time
        Time.timeScale = 1f;

        gameStarted = true;
        timerRunning = true;

        // Hide start prompt
        if (startText != null)
            startText.gameObject.SetActive(false);
    }

    void EndGame()
    {
        // Show end screen
        if (endScreenObject != null)
            endScreenObject.SetActive(true);

        // Hide gameplay objects
        Game.SetActive(false);
        GameUi.SetActive(false);

        // Reset timer animation
        if (timerAnimator != null)
        {
            timerAnimator.Play("Idle");
        }
    }

    void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(countdownTimer / 60f);
            int seconds = Mathf.FloorToInt(countdownTimer % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}







