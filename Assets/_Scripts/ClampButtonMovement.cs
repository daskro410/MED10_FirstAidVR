using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ClampButtonMovement : MonoBehaviour
{
    public CustomInteractionManager customInteractionManager;

    public Vector3 initialPosition; // World position if no parent, local if parented
    public Vector3 initialLocalPosition; // Local position if parented
    private Transform initialParent;
    private bool hasParent;
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    public GameObject leftHandPose;
    public GameObject rightHandPose;

    public float minYOffset = -0.05f;
    public float maxYOffset = 0.0f;
    public float snapSpeed = 5f;

    [Range(0, 1)] public float hapticIntensity = 0.5f;
    public float hapticDuration = 0.1f;
    private bool hasTriggeredHaptic = false;

    public bool isHighlighted = false; // Flag to check if the jaw is highlighted

    [SerializeField] private bool requireBothControllers = true;
    private HashSet<XRBaseInteractor> activeInteractors = new HashSet<XRBaseInteractor>();

    private void Start()
    {
        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>(); // Find the CustomInteractionManager in the scene

        // Get the initial parent if it exists
        initialParent = transform.parent;
        if (initialParent != null)
        {
            initialLocalPosition = transform.localPosition; // Store local position if parented
        }

        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.useGravity = false;

        grabInteractable = GetComponent<XRGrabInteractable>();

        // Store initial position relative to parent if parent exists
        hasParent = transform.parent != null;
        initialPosition = hasParent ? transform.localPosition : transform.position;

        //Debug.Log("Initial Position FROM START: " + initialPosition);

        leftHandPose.SetActive(false);
        rightHandPose.SetActive(false);
    }

    private void Update()
    {
        // Keep track of whether the parent changes dynamically
        bool currentlyHasParent = transform.parent != null;

        if (customInteractionManager.interactionState == InteractionState.HeartPumping){
            if (currentlyHasParent != hasParent)
            {
                hasParent = currentlyHasParent;
                //initialPosition = hasParent ? transform.localPosition : transform.position;
            }

            if (CanMove())
            {
                HandleMovement();
                TriggerHapticFeedback();
            }
            else
            {
                ResetToInitialPosition();
            }

            // Safety check to ensure the object stays within the range
            if (hasParent)
            {
                transform.localPosition = new Vector3(
                    initialPosition.x,
                    Mathf.Clamp(transform.localPosition.y, initialPosition.y + minYOffset, initialPosition.y + maxYOffset),
                    initialPosition.z
                );
            }
            else
            {
                transform.position = new Vector3(
                    initialPosition.x,
                    Mathf.Clamp(transform.position.y, initialPosition.y + minYOffset, initialPosition.y + maxYOffset),
                    initialPosition.z
                );
            }
        }
    }

    public void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log($"Grabbed by: {args.interactorObject?.transform.name}");

        if (args.interactorObject is XRBaseInteractor interactor)
        {
            activeInteractors.Add(interactor);
        }

        if (CanMove()){
            leftHandPose.SetActive(true);
            rightHandPose.SetActive(true);
            customInteractionManager.leftHandModel.SetActive(false);
            customInteractionManager.rightHandModel.SetActive(false);

            //GetComponent<MeshRenderer>().enabled = false;

            grabInteractable.trackPosition = true; // Enable position tracking
        }
    }

    public void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log($"Released by: {args.interactorObject?.transform.name}");

        if (args.interactorObject is XRBaseInteractor interactor)
        {
            activeInteractors.Remove(interactor);
        }

        if (initialParent != null)
        {
            // Reattach to the original parent
            transform.SetParent(initialParent);
        }

        SnapToTop();

        leftHandPose.SetActive(false);
        rightHandPose.SetActive(false);
        customInteractionManager.leftHandModel.SetActive(true);
        customInteractionManager.rightHandModel.SetActive(true);

        //GetComponent<MeshRenderer>().enabled = false;

        grabInteractable.trackPosition = false; // Disable position tracking
    }

    private bool CanMove()
    {
        return requireBothControllers ? activeInteractors.Count >= 2 : activeInteractors.Count >= 1;
    }

    private void HandleMovement()
    {
        Vector3 targetPosition;

        if (hasParent)
        {
            // Keep the movement within range in local space if parented
            targetPosition = transform.localPosition;
            targetPosition.y = Mathf.Clamp(targetPosition.y, initialPosition.y + minYOffset, initialPosition.y + maxYOffset);
            transform.localPosition = targetPosition;  // Use local position if parented
        }
        else
        {
            // Keep the movement within range in world space if not parented
            targetPosition = transform.position;
            targetPosition.y = Mathf.Clamp(targetPosition.y, initialPosition.y + minYOffset, initialPosition.y + maxYOffset);
            transform.position = targetPosition;  // Use world position if not parented
        }

        // Ensure the rigidbody stays within the range
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Stop any unintended movement
            rb.angularVelocity = Vector3.zero; // Stop any unintended rotation
        }
    }

    public void ResetToInitialPosition()
    {
/*         if (hasParent)
            transform.localPosition = initialPosition;
        else
            transform.position = initialPosition; */

        if (initialParent != null)
        {
            transform.SetParent(initialParent); // Reset to the original parent
            transform.localPosition = initialLocalPosition; // Reset to the original local position
        }
    }

    public bool IsAtBottom()
    {
        // Get the current Y position
        float currentPositionY = hasParent ? transform.localPosition.y : transform.position.y;
        // Check if the current Y position is close to the minimum Y position (with a tolerance)
        return currentPositionY <= initialPosition.y + minYOffset + 0.001f; // Added small tolerance
    }

    private bool IsAtTop()
    {
        // Get the current Y position
        float currentPositionY = hasParent ? transform.localPosition.y : transform.position.y;
        // Check if the current Y position is close to the maximum Y position (with a tolerance)
        return currentPositionY >= initialPosition.y + maxYOffset - 0.001f; // Added small tolerance
    }

    private void TriggerHapticIfAtBottom(XRBaseInputInteractor controllerInteractor)
    {
        // Check if the object is at the bottom and haptic hasn't been triggered yet
        if (IsAtBottom() && !hasTriggeredHaptic)
        {
            hasTriggeredHaptic = true;  // Set to true to prevent multiple triggers
            controllerInteractor.SendHapticImpulse(hapticIntensity, hapticDuration);
            customInteractionManager.AddToHeartPressCounter();
            Debug.Log("Triggering haptic and adding to counter...");
        }
    }

    private void ResetHapticTriggerIfAtTop()
    {
        // Check if the object is at the top
        if (IsAtTop())
        {
            hasTriggeredHaptic = false;  // Reset the trigger when at the top
            Debug.Log("Reached top. Resetting haptic trigger...");
        }
    }

    private void TriggerHapticFeedback()
    {
        // Iterate over all active interactors
        foreach (var interactor in activeInteractors.ToArray())
        {
            if (interactor is XRBaseInputInteractor controllerInteractor)
            {
                TriggerHapticIfAtBottom(controllerInteractor);
                ResetHapticTriggerIfAtTop();
            }
        }
    }

    public void SnapToTop()
    {
        Vector3 targetPosition;

        if (hasParent)
        {
            // Snap to the top position in local space relative to the parent
            targetPosition = new Vector3(initialPosition.x, initialPosition.y + maxYOffset, initialPosition.z);
            transform.localPosition = targetPosition;  // Use local position if parented
        }
        else
        {
            // Snap to the top position in world space
            targetPosition = new Vector3(initialPosition.x, initialPosition.y + maxYOffset, initialPosition.z);
            transform.position = targetPosition;  // Use world position if not parented
        }

        Debug.Log($"SnapToTop called. Target position: {targetPosition}, Current position: {transform.position}");
    }

    public void ToggleHighlight(){
        if (isHighlighted){
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false; // Disable the collider when highlighted
            isHighlighted = false;
        }

        else if (!isHighlighted){
            GetComponent<Renderer>().enabled = true;
            GetComponent<Collider>().enabled = true; // Enable the collider when not highlighted
            isHighlighted = true;
        }
    }

    private void OnDrawGizmos()
    {
        // Ensure initialPosition is properly initialized
        if (initialPosition == Vector3.zero)
        {
            hasParent = transform.parent != null;
            initialPosition = hasParent ? transform.localPosition : transform.position;
        }

        // Draw gizmos relative to the correct initial position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            hasParent ? transform.parent.TransformPoint(initialPosition + new Vector3(0, minYOffset, 0)) 
                    : initialPosition + new Vector3(0, minYOffset, 0), 
            0.01f
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            hasParent ? transform.parent.TransformPoint(initialPosition + new Vector3(0, maxYOffset, 0)) 
                    : initialPosition + new Vector3(0, maxYOffset, 0), 
            0.01f
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            hasParent ? transform.parent.TransformPoint(initialPosition + new Vector3(0, minYOffset, 0)) 
                    : initialPosition + new Vector3(0, minYOffset, 0),
            hasParent ? transform.parent.TransformPoint(initialPosition + new Vector3(0, maxYOffset, 0)) 
                    : initialPosition + new Vector3(0, maxYOffset, 0)
        );
    }
}