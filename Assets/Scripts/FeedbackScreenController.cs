using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FeedbackScreenController : MonoBehaviour
{
    public GameObject screen1;  // Assign Screen 1 panel in the Inspector
    public GameObject screen2;  // Assign Screen 2 panel in the Inspector
    public Button nextButton;   // Assign the "Next" button in the Inspector
    public Button doneButton;   // Assign the "Done" button in the Inspector

    void Start()
    {
        // Initially show only the first screen
        screen1.SetActive(true);
        screen2.SetActive(false);

        // Attach the button event listeners
        nextButton.onClick.AddListener(ShowSecondScreen);
        doneButton.onClick.AddListener(ChangeToMenuScene);
    }

    // Show Screen 2 and hide Screen 1
    void ShowSecondScreen()
    {
        screen1.SetActive(false);
        screen2.SetActive(true);
    }

    // Change to the menu scene when the Done button is pressed
    void ChangeToMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }
}
