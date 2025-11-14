using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardID;          // Matching ID
    public GameObject cover;    // Overlay hiding the face

    private bool isRevealed = false;

    public void OnCardClicked()
    {
        if (isRevealed) return;

        isRevealed = true;

        // Reveal immediately
        cover.SetActive(true);

        // Force Unity to redraw UI right now
        Canvas.ForceUpdateCanvases();

        // Tell GameManager
        GameManager.Instance.CardRevealed(this);
    }

    public void HideCard()
    {
        isRevealed = false;
        cover.SetActive(false);
    }

    public void DisableCard()
    {
        GetComponent<Button>().interactable = false;
    }
}







