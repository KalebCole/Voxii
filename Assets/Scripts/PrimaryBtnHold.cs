using UnityEngine;
using UnityEngine.InputSystem;

public class PrimaryBtnHold : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    public MicRecorder micRecorder;

    public WhisperTranscriber whisperTranscriber;
    public ChatLoop chatLoop;
    public bool isRecording = false;

    private InputAction primaryButton;

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
        if (chatLoop.msgsSent >= 10)
        {
            Debug.Log("Can't send more messages, limit reached.");
            return;
        }
        if (chatLoop == null)
        {
            Debug.LogError("ChatLoop component not found on GameObject!");
            return;
        }

        // Check if the AI is responding
        if (chatLoop.isResponding)
        {
            Debug.Log("Cannot record, AI is still responding.");
            return;
        }

        if (!this.isRecording) // The performer event might be sent multiple times while holding down
        {
            chatLoop.isResponding = true;
            this.isRecording = true;
            micRecorder.StartRecording();
        }
    }

    private async void OnRelease(InputAction.CallbackContext context)
    {
        if (chatLoop.msgsSent >= 10)
        {
            Debug.Log("It shouldn't go here but added for safety");
            return;
        }
        if (!this.isRecording)
        {
            return;
        }

        micRecorder.StopRecording();
        // micRecorder.PlayRecording();
        micRecorder.SaveRecording();
        this.isRecording = false;
        string transcription = await whisperTranscriber.TranscribeRecording();

        Debug.Log("transcription in PrimaryBtnHold: " + transcription);
        await chatLoop.SendUserMessage(transcription);
    }
}
