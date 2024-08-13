using UnityEngine;
using UnityEngine.XR;

public class DisableMovement : MonoBehaviour
{
    private Transform xrRigTransform;
    private Vector3 initialPosition;

    void Start()
    {
        xrRigTransform = GetComponent<Transform>();
        if (xrRigTransform != null)
        {
            initialPosition = xrRigTransform.position;
        }

    }

    void Update()
    {
        if (xrRigTransform != null)
        {
            xrRigTransform.position = initialPosition;
        }
    }

}
