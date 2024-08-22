using UnityEngine;
using UnityEngine.InputSystem;

public class BtnVoiceRecord : MonoBehaviour
{
    public InputActionAsset inputActionAsset;

    private InputAction primaryButton;
    private bool isRecording = false;

    void OnEnable()
    {
        // Get the primary button action from the XRButtonDebug script
        var actionMap = inputActionAsset.FindActionMap("XRControls");

        this.primaryButton = actionMap.FindAction("PrimaryButton");

        this.primaryButton.Enable();

        // Subscribe to the action events
        this.primaryButton.performed += this.StartRecording;
        this.primaryButton.canceled += this.StopRecording;
    }

    void OnDisable()
    {
        // Unsubscribe from action events when disabled
        this.primaryButton.performed -= this.StartRecording;
        this.primaryButton.canceled -= this.StopRecording;

        this.primaryButton.Disable();
    }

    private void StartRecording(InputAction.CallbackContext context)
    {
        if (!this.isRecording) // The performer event might be sent multiple times while holding down
        {
            this.isRecording = true;
            Debug.Log("Recording started");
            // Start your voice recording logic here
        }
    }

    private void StopRecording(InputAction.CallbackContext context)
    {
        if (this.isRecording) // For safety
        {
            this.isRecording = false;
            Debug.Log("Recording stopped");
            // Stop your voice recording logic here
        }
    }
}
