using UnityEngine;
using UnityEngine.InputSystem;

public class BtnVoiceRecord : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    public MicRecorder micRecorder;

    private InputAction primaryButton;
    private bool isRecording = false;

    void OnEnable()
    {
        // Get the primary button action from the XRButtonDebug script
        var actionMap = inputActionAsset.FindActionMap("XRControls");

        this.primaryButton = actionMap.FindAction("PrimaryButton");

        this.primaryButton.Enable();

        // Subscribe to the action events
        this.primaryButton.performed += this.OnPress;
        this.primaryButton.canceled += this.OnRelease;
    }

    void OnDisable()
    {
        // Unsubscribe from action events when disabled
        this.primaryButton.performed -= this.OnPress;
        this.primaryButton.canceled -= this.OnRelease;

        this.primaryButton.Disable();
    }

    private void OnPress(InputAction.CallbackContext context)
    {
        if (!this.isRecording) // The performer event might be sent multiple times while holding down
        {
            this.isRecording = true;
            micRecorder.StartRecording();
        }
    }

    private void OnRelease(InputAction.CallbackContext context)
    {
        if (this.isRecording) // For safety
        {
            this.isRecording = false;
            micRecorder.StopRecording();
            micRecorder.PlayRecording();
        }
    }
}
