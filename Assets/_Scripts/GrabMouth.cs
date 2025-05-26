using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrabMouth : MonoBehaviour
{
    CustomInteractionManager customInteractionManager;
    JawHandler jawHandler;

    private GameObject foreHeadInteractableObject;
    private GameObject noseInteractableObject;
    private GameObject jawInteractableObject;

    public XRSimpleInteractable foreheadInteractable;
    public XRSimpleInteractable noseInteractable;
    public XRSimpleInteractable jawInteractable;

    public XRSimpleInteractable topInteractable;
    public XRSimpleInteractable bottomInteractable;
    
    public bool isTopGrabbed = false;
    public bool isBottomGrabbed = false;

    [SerializeField] public bool isBothBodyInteractablesGrabbed => isTopGrabbed && isBottomGrabbed; // Property to check if both interactables are grabbed

    void Start()
    {
        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>();
        jawHandler = FindFirstObjectByType<JawHandler>();

        foreHeadInteractableObject = customInteractionManager.headInteractables[0];
        noseInteractableObject = customInteractionManager.mouthInteractables[0];
        jawInteractableObject = customInteractionManager.mouthInteractables[1];

        foreheadInteractable = foreHeadInteractableObject.GetComponent<XRSimpleInteractable>();
        noseInteractable = noseInteractableObject.GetComponent<XRSimpleInteractable>();
        jawInteractable = jawInteractableObject.GetComponent<XRSimpleInteractable>();

        // Add listeners for jaw (as it is the bottom)
        bottomInteractable = jawInteractable;
        bottomInteractable.selectEntered.AddListener(OnBottomGrabbed);
        bottomInteractable.selectExited.AddListener(OnBottomReleased);
    }

    void Update()
    {
        /* // Check if the interaction state has changed and update the interactables accordingly
        // Switch top interactable based on the interaction state
        if (customInteractionManager.interactionState != customInteractionManager.previousState){
            Debug.Log("GRABMOUTH: Interaction state changed! Updating interactables...");

            CheckToSwitchTop();
        } */
    }

    private void OnTopGrabbed(SelectEnterEventArgs args)
    {
        isTopGrabbed = true;

        HideInteractable(topInteractable.gameObject);

        CheckBothGrabbed();
    }

    private void OnTopReleased(SelectExitEventArgs args)
    {
        Debug.Log($"OnTopReleased called. Args: {args}, Interactor: {args?.interactorObject}, Interactable: {args?.interactableObject}");

        // Ensure the top interactable is actually released
        if (args == null || args.interactorObject == null || (Object)args.interactableObject != topInteractable)
        {
            Debug.Log("Top interactable release ignored because it was not the correct interactor or interactable.");
            return;
        }

        // Check if the interactor is still interacting with the top interactable
        if (topInteractable.isSelected)
        {
            Debug.Log("Top interactable is still selected. Ignoring release.");
            return;
        }

        isTopGrabbed = false;

        ShowInteractable(topInteractable.gameObject);

        CheckBothGrabbed();
    }

    private void OnBottomGrabbed(SelectEnterEventArgs args)
    {
        isBottomGrabbed = true;

        HideInteractable(bottomInteractable.gameObject);

        CheckBothGrabbed();
    }

    private void OnBottomReleased(SelectExitEventArgs args)
    {
        Debug.Log($"OnBottomReleased called. Args: {args}, Interactor: {args?.interactorObject}, Interactable: {args?.interactableObject}");

        // Ensure the bottom interactable is actually released
        if (args == null || args.interactorObject == null || (Object)args.interactableObject != bottomInteractable)
        {
            Debug.Log("Bottom interactable release ignored because it was not the correct interactor or interactable.");
            return;
        }

        // Check if the interactor is still interacting with the bottom interactable
        if (bottomInteractable.isSelected)
        {
            Debug.Log("Bottom interactable is still selected. Ignoring release.");
            return;
        }

        isBottomGrabbed = false;

        ShowInteractable(bottomInteractable.gameObject);

        CheckBothGrabbed();
    }

    private void CheckBothGrabbed()
    {
        if (isTopGrabbed && isBottomGrabbed)
        {
            Debug.Log("Both head objects are grabbed!");

            if (customInteractionManager.interactionState == InteractionState.CheckBreath)
            {
                customInteractionManager.StartMouthBreathCoroutine(customInteractionManager.maxMouthListeningTime);
            }

            else if (customInteractionManager.interactionState == InteractionState.MouthBreathing)
            {
                customInteractionManager.StartMouthBreathCoroutine(customInteractionManager.maxMouthBreathTime);
            }

            // Trigger the animation
            jawHandler.OpenJaw(); // Open the jaw when both interactables are grabbed
        }

        else
        {
            Debug.Log("One or both head objects are not grabbed.");

            // Stop the coroutine if one of the interactables is released
            customInteractionManager.StopMouthBreathCoroutine();

            jawHandler.CloseJaw();
        }
    }

    public void CheckToSwitchTop(){
        if (customInteractionManager.interactionState == InteractionState.CheckBreath)
        {
            if (topInteractable == null){
                topInteractable = foreheadInteractable;
                Debug.Log("Top interactable was null, assigned foreheadInteractable.");

                // Add listeners for forehead (as it is the top)
                topInteractable.selectEntered.AddListener(OnTopGrabbed);
                topInteractable.selectExited.AddListener(OnTopReleased);
            }

            else if (topInteractable != foreheadInteractable){
                Debug.Log("Top interactable was not foreheadInteractable, assigned foreheadInteractable.");

                // Remove listeners from the previous top interactable
                topInteractable.selectEntered.RemoveListener(OnTopGrabbed);
                topInteractable.selectExited.RemoveListener(OnTopReleased);

                // assign new top
                topInteractable = foreheadInteractable;

                // Add listeners for forehead (as it is the top)
                topInteractable.selectEntered.AddListener(OnTopGrabbed);
                topInteractable.selectExited.AddListener(OnTopReleased);
            }

            Debug.Log("New top interactable: " + topInteractable.name);
        }

        else if (customInteractionManager.interactionState == InteractionState.MouthBreathing)
        {
            if (topInteractable == null){
                topInteractable = noseInteractable;
                Debug.Log("Top interactable was null, assigned noseInteractable.");

                // Add listeners for forehead (as it is the top)
                topInteractable.selectEntered.AddListener(OnTopGrabbed);
                topInteractable.selectExited.AddListener(OnTopReleased);
            }

            else if (topInteractable != noseInteractable){
                Debug.Log("Top interactable was not noseInteractable, assigned noseInteractable.");
                
                // Remove listeners from the previous top interactable
                topInteractable.selectEntered.RemoveListener(OnTopGrabbed);
                topInteractable.selectExited.RemoveListener(OnTopReleased);

                // assign new top
                topInteractable = noseInteractable;

                // Add listeners for forehead (as it is the top)
                topInteractable.selectEntered.AddListener(OnTopGrabbed);
                topInteractable.selectExited.AddListener(OnTopReleased);
            }

            Debug.Log("New top interactable: " + topInteractable.name);
        }

        Debug.Log("New top interactable: " + topInteractable.name);
    }

    public void ForceInteractionRelease(){
        Debug.Log("Forcing interaction release...");

        OnTopReleased(null);
        OnBottomReleased(null);
    }

    private void HideInteractable(GameObject interactable){
        if (customInteractionManager.interactionState != InteractionState.CheckBreath && customInteractionManager.interactionState != InteractionState.MouthBreathing){
            return;
        }

        interactable.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    private void ShowInteractable(GameObject interactable){
        if (customInteractionManager.interactionState != InteractionState.CheckBreath && customInteractionManager.interactionState != InteractionState.MouthBreathing){
            return;
        }

        interactable.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}