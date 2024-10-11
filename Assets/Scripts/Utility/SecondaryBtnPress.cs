using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System.Collections.Generic;

public class SecondaryBtnPress : MonoBehaviour
{
    public InputActionAsset inputActionAsset;

    public ChatLoop chatLoop;
    public PrimaryBtnHold primaryBtnHold;

    private InputAction secondaryButton;

    // Enable or disable mock data
    private bool useMockData = false;

    void OnEnable()
    {
        // Get the primary button action from the XRButtonDebug script
        var actionMap = inputActionAsset.FindActionMap("XRControls");

        this.secondaryButton = actionMap.FindAction("SecondaryButton");

        this.secondaryButton.Enable();

        // Subscribe to the action events
        this.secondaryButton.performed += this.OnPress;
    }

    void OnDisable()
    {
        // Unsubscribe from action events when disabled
        this.secondaryButton.performed -= this.OnPress;
        this.secondaryButton.Disable();
    }

    private void OnPress(InputAction.CallbackContext context)
    {
        HandlePressAsync();
    }

    public async Task HandlePressAsync()
    {
        if (chatLoop == null)
        {
            Debug.LogError("ChatLoop component not found on GameObject!");
            return;
        }

        // Check if the AI is responding
        if (chatLoop.isResponding)
        {
            Debug.Log("Cannot get score, AI is still responding.");
            return;
        }

        if (primaryBtnHold.isRecording)
        {
            Debug.Log("Cannot get score, recording in progress.");
            return;
        }

        // Create the Scorer with mock mode based on the useMockData flag
        Scorer scorer = new Scorer(chatLoop.chatLogFilePath, useMockData);

        // Get the points from the scorer
        int points = await scorer.CalculatePointsAsync();
        (ScoreResult, float, List<ErrorExample>) values = await scorer.GetResultsAndResponseTimeAsync();

        Debug.Log($"Points in Scdareybutn pres: {points}");

        
        // Save to static data class to be access by results UI
        ResultsData.points = points;
        // debug
        Debug.Log("ResultsData.points in SCNDARYBTNPRESS: " + ResultsData.points);
        ResultsData.errors = values.Item1.NumberOfErrors;
        ResultsData.relevanceScore = values.Item1.Accuracy;
        ResultsData.responseTime = ((int)values.Item2);
        ResultsData.feedbackCategory = values.Item3[0].Category;
        ResultsData.feedbackIncorrect = values.Item3[0].Incorrect;
        ResultsData.feedbackCorrected = values.Item3[0].Corrected;
        ResultsData.feedbackReasoning = values.Item3[0].Reasoning;

    }
}
