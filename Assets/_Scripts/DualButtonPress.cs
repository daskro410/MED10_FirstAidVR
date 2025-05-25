using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DualButtonPress : MonoBehaviour
{
    private int activeControllers = 0;

    private XRBaseInteractable interactable; // The XRBaseInteractable component for the button

    // UnityEvent to trigger the button press
    public UnityEngine.Events.UnityEvent onDualPress;

    void Start()
    {
        // Get the XRBaseInteractable component attached to the button
        interactable = GetComponent<XRBaseInteractable>();

        if (interactable == null)
        {
            Debug.LogError("XRBaseInteractable is missing on " + gameObject.name);
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is an XR controller
        if (other.CompareTag("XRController"))
        {
            activeControllers++;
            CheckPressCondition();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting the trigger is an XR controller
        if (other.CompareTag("XRController"))
        {
            activeControllers--;
            CheckPressCondition();
        }
    }

    // Check if both controllers are interacting with the button
    private void CheckPressCondition()
    {
        if (activeControllers >= 2)
        {
            // Trigger the press event when both controllers are interacting with the button
            onDualPress.Invoke();
            Debug.Log("Both controllers are pressing the button!");
        }
    }
}