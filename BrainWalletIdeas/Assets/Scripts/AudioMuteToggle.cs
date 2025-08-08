using UnityEngine;
using UnityEngine.UI;

public class AudioMuteToggle : MonoBehaviour
{
    public Button muteButton;
    public Sprite muteIcon;
    public Sprite unmuteIcon;

    private bool isMuted = false;
    private Image buttonImage;

    void Start()
    {
        if (muteButton != null)
        {
            muteButton.onClick.AddListener(ToggleMute);
            buttonImage = muteButton.GetComponent<Image>();
            UpdateButtonIcon();
        }
    }

    void ToggleMute()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0f : 1f;
        UpdateButtonIcon();
    }

    void UpdateButtonIcon()
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = isMuted ? muteIcon : unmuteIcon;
        }
    }
}

