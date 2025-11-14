using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TMP_Text matchText;
    public TMP_Text TryText;
    public GameObject EndGame;
    public GameObject panel;
    private int matchCount = 0;
    private int tryCount = 0;
    private Card firstCard;
    private Card secondCard;
    private bool isChecking = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Update()
    {
        if (matchCount == 6)
        {
            EndGame.SetActive(true);
            panel.SetActive(true);
        }
    }

    public void CardRevealed(Card card)
    {
        if (isChecking) return;

        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null)
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        isChecking = true;

        // Wait one second AFTER both are shown
        yield return new WaitForSeconds(1f);

        if (firstCard.cardID == secondCard.cardID)
        {
            // Match
            firstCard.DisableCard();
            secondCard.DisableCard();

            // Increase match count
            matchCount++;
            matchText.text = "" + matchCount;

            //Increase Trys 
            tryCount++;
            TryText.text = "" + tryCount;

            
        }
        else
        {
            // No match
            firstCard.HideCard();
            secondCard.HideCard();

            //Increase Trys 
            tryCount++;
            TryText.text = "" + tryCount;
        }

        firstCard = null;
        secondCard = null;
        isChecking = false;
    }

    public void LoadScene1()
    {
        SceneManager.LoadScene(1);
    }
}








