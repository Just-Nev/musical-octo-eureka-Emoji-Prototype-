using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;

    //public GameObject pauseMenuUI; // O

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Pause the game
            //if (pauseMenuUI != null)
                //pauseMenuUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
            //if (pauseMenuUI != null)
                //pauseMenuUI.SetActive(false);
        }
    }
}
