using UnityEngine;
using UnityEngine.InputSystem;

public class PrimaryBtnHold : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    public MicRecorder micRecorder;

    public WhisperTranscriber whisperTranscriber;
    public ChatLoop chatLoop;

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

    private async void OnRelease(InputAction.CallbackContext context)
    {
        if (!this.isRecording)
        {
            Debug.LogWarning("Primary button released without recording started.");
            return;
        }

        this.isRecording = false;
        micRecorder.StopRecording();
        // micRecorder.PlayRecording();
        micRecorder.SaveRecording();
        string transcription = await whisperTranscriber.TranscribeRecording();

        Debug.Log("transcription in PrimaryBtnHold: " + transcription);

        chatLoop.SendUserMessage(transcription);
    }
}
