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
        // Find the ChatLoop GameObject and get its ChatLoop component
        var chatLoopGameObject = GameObject.Find("ChatLoop");
        if (chatLoopGameObject == null)
        {
            Debug.LogError("ChatLoop GameObject not found!");
            return;
        }

        var chatLoop = chatLoopGameObject.GetComponent<ChatLoop>();
        if (chatLoop == null)
        {
            Debug.LogError("ChatLoop component not found on ChatLoop GameObject!");
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
