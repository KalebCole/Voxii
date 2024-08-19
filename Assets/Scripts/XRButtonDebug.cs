using UnityEngine;
using UnityEngine.InputSystem;

public class XRButtonDebug : MonoBehaviour
{
    public InputActionAsset inputActionAsset;

    private InputAction primaryButton;

    void OnEnable()
    {
        // Find the action map and actions in the Input Action Asset
        var actionMap = inputActionAsset.FindActionMap("XRControls");

        primaryButton = actionMap.FindAction("PrimaryButton");

        // Enable all actions
        primaryButton.Enable();

        // Subscribe to the action events
        primaryButton.performed += ctx => Debug.Log("Primary Button pressed");
    }

    void OnDisable()
    {
        // Unsubscribe from action events when disabled
        primaryButton.performed -= ctx => Debug.Log("Primary Button pressed");

        // Disable actions
        primaryButton.Disable();
    }
}
