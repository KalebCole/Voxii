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

        // Get the points from the scorer
        int points = await scorer.CalculatePointsAsync();

        Debug.Log($"Points: {points}");    

    

        // TODO: display it to a UI element that will pop up as soon as you click the secondary button, and wait until the score is calculated (through loading circle), then displayed
        // Then the user can exit out of it and continue with the conversation
    }
}
