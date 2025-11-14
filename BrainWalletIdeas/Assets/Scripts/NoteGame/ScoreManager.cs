using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI (TMP)")]
    public TMP_Text scoreText;      
    public TMP_Text comboText;      

    [Header("Scoring Values")]
    public int perfectPoints = 150;
    public int goodPoints = 50;
    public int badPoints = 25;

    private int score = 0;
    private int combo = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddPerfect()
    {
        combo++;
        AddScore(perfectPoints * combo);
    }

    public void AddGood()
    {
        combo++;
        AddScore(goodPoints * combo);
    }

    public void AddBad()
    {
        combo = 0;
        AddScore(badPoints);
    }

    void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"{score:N0}";

        if (comboText != null)
        {
            if (combo > 1)
                comboText.text = $"{combo}x";
            else
                comboText.text = "0x";
        }
    }

    public int GetScore()
    {
        return score;
    }
}


