using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusicController : MonoBehaviour
{
    private AudioSource audioSource;
    public Button muteButton;                 // Reference to the button
    public GameObject symbol;                 // Reference to the child GameObject with the Image component
    public Sprite unmutedImage;              // Image for unmuted state
    public Sprite mutedImage;                 // Image for muted state
    private bool isMuted = false;            // Track mute state

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();                   // Start playing the background music
        UpdateButtonImage();                  // Update button image on start
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;                   // Toggle the mute state
        audioSource.mute = isMuted;           // Mute or unmute the audio
        UpdateButtonImage();                  // Update the button image
    }

    private void UpdateButtonImage()
    {
        // Change the button image based on the mute state
        Image symbolImage = symbol.GetComponent<Image>();
        symbolImage.sprite = isMuted ? mutedImage : unmutedImage;
    }
}
