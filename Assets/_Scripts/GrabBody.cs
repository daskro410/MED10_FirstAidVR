using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabBody : MonoBehaviour
{
    public CustomInteractionManager customInteractionManager;
    //public Animator bodyAnimator; // The animator component to control the animation

    public XRSimpleInteractable topBodyInteractable;
    public XRSimpleInteractable bottomBodyInteractable;

    public Animator bodyAnimator; // The animator component to control the animation

    private bool isTopGrabbed = false;
    private bool isBottomGrabbed = false;

    [SerializeField] public bool isBothBodyInteractablesGrabbed => isTopGrabbed && isBottomGrabbed; // Property to check if both interactables are grabbed

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>(); // Find the CustomInteractionManager in the scene
        topBodyInteractable = customInteractionManager.bodyInteractables[0].GetComponent<XRSimpleInteractable>();
        bottomBodyInteractable = customInteractionManager.bodyInteractables[1].GetComponent<XRSimpleInteractable>();

        //bodyAnimator = GetComponent<Animator>(); // Get the Animator component attached to this GameObject

        if (topBodyInteractable != null)
        {
            topBodyInteractable.selectEntered.AddListener(OnTopBodyGrabbed);
            topBodyInteractable.selectExited.AddListener(OnTopBodyReleased);
        }

        if (bottomBodyInteractable != null)
        {
            bottomBodyInteractable.selectEntered.AddListener(OnBottomBodyGrabbed);
            bottomBodyInteractable.selectExited.AddListener(OnBottomBodyReleased);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTopBodyGrabbed(SelectEnterEventArgs args)
    {
        isTopGrabbed = true;
        CheckBothGrabbed();
    }

    private void OnTopBodyReleased(SelectExitEventArgs args)
    {
        isTopGrabbed = false;
        CheckBothGrabbed();
    }

    private void OnBottomBodyGrabbed(SelectEnterEventArgs args)
    {
        isBottomGrabbed = true;
        CheckBothGrabbed();
    }

    private void OnBottomBodyReleased(SelectExitEventArgs args)
    {
        isBottomGrabbed = false;
        CheckBothGrabbed();
    }

    private void CheckBothGrabbed()
    {
        if (isTopGrabbed && isBottomGrabbed)
        {
            Debug.Log("Both body objects are grabbed!");

            // Check if the interaction state is set to BodyDragging
            customInteractionManager.StartCoroutine(customInteractionManager.TrackBodyShakingTime());

            // Trigger the animation
            bodyAnimator.SetTrigger("TriggerShakingAnim");
        }
        else
        {
            Debug.Log("One or both body objects are not grabbed.");

            // Stop the coroutine if one of the interactables is released
            customInteractionManager.StopCoroutine(customInteractionManager.TrackBodyShakingTime());

            bodyAnimator.SetTrigger("TriggerLayingAnim");
        }
    }



    private void OnDestroy()
    {
        // Remove listeners when the script is destroyed
        if (topBodyInteractable != null)
        {
            topBodyInteractable.selectEntered.RemoveListener(OnTopBodyGrabbed);
            topBodyInteractable.selectExited.RemoveListener(OnTopBodyReleased);
        }

        if (bottomBodyInteractable != null)
        {
            bottomBodyInteractable.selectEntered.RemoveListener(OnBottomBodyGrabbed);
            bottomBodyInteractable.selectExited.RemoveListener(OnBottomBodyReleased);
        }
    }
}
