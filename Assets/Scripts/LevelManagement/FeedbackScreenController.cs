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
    public TextMeshProUGUI feedbackCategory;
    public TextMeshProUGUI feedbackIncorrect;
    public TextMeshProUGUI feedbackCorrected;
    public TextMeshProUGUI feedbackReasoning;

    void Start()
    {
        // Initially show only the first screen
        screen1.SetActive(true);
        screen2.SetActive(false);

        // debug
        Debug.Log("ResultsData.points: " + ResultsData.points);
        // Initalise the text values
        pointValue.text = ResultsData.points.ToString();
        grammarErrorValue.text = ResultsData.errors.ToString();
        responseTimeValue.text = ResultsData.responseTime.ToString() + "s";
        relevanceValue.text = ResultsData.relevanceScore.ToString() + "%";
        feedbackCategory.text = ResultsData.feedbackCategory;
        feedbackIncorrect.text = ResultsData.feedbackIncorrect;
        feedbackCorrected.text = ResultsData.feedbackCorrected;
        feedbackReasoning.text = ResultsData.feedbackReasoning;
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
