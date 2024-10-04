using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

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
    private async Task HandlePressAsync()
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


        var scoreString = await scorer.GetScore();
        if (scoreString == null)
        {
            Debug.LogError("Error: scoreString is null");
            return;
        }
        Debug.Log("Score before parsing: " + scoreString);

        var scoreResult = ScoreResult.Parse(scoreString);
        Debug.Log("Score after parsing: " + scoreResult.NumberOfErrors + ", " + scoreResult.Accuracy);

        // send the score to the points system
        var points = ScoringSystem.CalculatePoints(scoreResult);
        Debug.Log("Points: " + points);

        // Save to static data class to be access by results UI
        ResultsData.points = points;
        ResultsData.errors = scoreResult.NumberOfErrors;
        ResultsData.relevanceScore = scoreResult.Accuracy;

        // TODO: response time and feedback point
        //ResultsData.feedback = scoreResult.;
        //ResultsData.feedback = scoreResult.;


    }
}
