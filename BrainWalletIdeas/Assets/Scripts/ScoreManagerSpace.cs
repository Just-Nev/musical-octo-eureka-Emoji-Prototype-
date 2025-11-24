using UnityEngine;
using TMPro;

public class ScoreManagerSpace : MonoBehaviour
{
    public static ScoreManagerSpace Instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    private int score = 0;

    void Awake()
    {
        // basic singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }
}

