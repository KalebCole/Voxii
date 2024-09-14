using UnityEngine;
using UnityEngine.InputSystem;

public class SecondaryBtnPress : MonoBehaviour
{
    public InputActionAsset inputActionAsset;

    public ChatLoop chatLoop;
    public PrimaryBtnHold primaryBtnHold;

    private InputAction secondaryButton;

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

        // TODO: call scorer here
    }
}
