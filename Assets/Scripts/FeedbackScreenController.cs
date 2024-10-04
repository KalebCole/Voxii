using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FeedbackScreenController : MonoBehaviour
{
    public GameObject screen1;
    public GameObject screen2;
    public Button nextButton;
    public Button doneButton;
    public TextMeshProUGUI pointValue;
    public TextMeshProUGUI grammarErrorValue;
    public TextMeshProUGUI responseTimeValue;
    public TextMeshProUGUI relevanceValue;
    public TextMeshProUGUI feedbackPoint;

    void Start()
    {
        // Initially show only the first screen
        screen1.SetActive(true);
        screen2.SetActive(false);

        // Initalise the text values
        pointValue.text = ResultsData.points.ToString(); ;
        grammarErrorValue.text = ResultsData.errors.ToString();
        responseTimeValue.text = "3" + "%";//ResultsData..ToString();
        relevanceValue.text = ResultsData.relevanceScore.ToString() + "%";
        feedbackPoint.text = "t";//ResultsData.feedback.ToString();
    }

    // Show Screen 2 and hide Screen 1
    public void ShowSecondScreen()
    {
        screen1.SetActive(false);
        screen2.SetActive(true);
    }

    // Change to the menu scene when the Done button is pressed
    public void ChangeToMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }
}
