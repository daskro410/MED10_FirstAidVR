using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HeadTiltInteraction : MonoBehaviour
{
    private GameManager gameManager;
    public CustomInteractionManager customInteractionManager;
    //public Animator bodyAnimator;

    public Animator headAnimator; 

    private GameObject topHeadInteractableObject; 
    private GameObject bottomHeadInteractableObject; 

    public XRSimpleInteractable topHeadInteractable; 
    public XRSimpleInteractable bottomHeadInteractable; 

    private bool isTopGrabbed = false;
    private bool isBottomGrabbed = false;

    [SerializeField] public bool isBothBodyInteractablesGrabbed => isTopGrabbed && isBottomGrabbed; // Property to check if both interactables are grabbed

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>();

        topHeadInteractableObject = customInteractionManager.headInteractables[0];
        bottomHeadInteractableObject = customInteractionManager.headInteractables[1];

        topHeadInteractable = topHeadInteractableObject.GetComponent<XRSimpleInteractable>();
        bottomHeadInteractable = bottomHeadInteractableObject.GetComponent<XRSimpleInteractable>();
        //bodyAnimator = GetComponent<Animator>();

        if (topHeadInteractable != null)
        {
            //Debug.Log("Top head interactable assigned: " + topHeadInteractable.name);
            topHeadInteractable.selectEntered.AddListener(OnTopGrabbed);
            topHeadInteractable.selectExited.AddListener(OnTopReleased);
        }

        if (bottomHeadInteractable != null)
        {
            //Debug.Log("Bottom head interactable assigned: " + bottomHeadInteractable.name);
            bottomHeadInteractable.selectEntered.AddListener(OnBottomGrabbed);
            bottomHeadInteractable.selectExited.AddListener(OnBottomReleased);
        }

        // Set the head to tilt at start
        headAnimator.SetTrigger("TriggerHeadTiltAnim");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTopGrabbed(SelectEnterEventArgs args)
    {
        //Debug.Log("Top grabbed!");
        isTopGrabbed = true;

        // Hide the interactable when grabbed
        HideInteractable(topHeadInteractableObject);

        CheckBothGrabbed();
    }

    private void OnTopReleased(SelectExitEventArgs args)
    {
        //Debug.Log("Top released!");
        isTopGrabbed = false;

        // Show the interactable when released
        ShowInteractable(topHeadInteractableObject);

        CheckBothGrabbed();
    }

    private void OnBottomGrabbed(SelectEnterEventArgs args)
    {
        //Debug.Log("Bottom grabbed!");
        isBottomGrabbed = true;

        // Hide the interactable when grabbed
        HideInteractable(bottomHeadInteractableObject);

        CheckBothGrabbed();
    }

    private void OnBottomReleased(SelectExitEventArgs args)
    {
        //Debug.Log("Bottom released!");
        isBottomGrabbed = false;

        // Show the interactable when released
        ShowInteractable(bottomHeadInteractableObject);

        CheckBothGrabbed();
    }

    private void CheckBothGrabbed()
    {
        if (isTopGrabbed && isBottomGrabbed)
        {
            //Debug.Log("Both body objects are grabbed!");

            headAnimator.SetTrigger("TriggerHeadUprightAnim");

            // Disable the interactables to prevent further interaction
            ForceInteractionRelease();
            DisableInteraction();

            // Forcing this bs
            //topHeadInteractableObject.GetComponent<MeshRenderer>().enabled = true; // Hide the top interactable
            //bottomHeadInteractableObject.GetComponent<MeshRenderer>().enabled = true; // Hide the bottom interactable

            customInteractionManager.SetInteractionState(InteractionState.CheckBreath);
        }
    }

    private void HideInteractable(GameObject interactable){
        if (customInteractionManager.interactionState != InteractionState.TiltHeadUpright){
            return;
        }

        interactable.GetComponent<MeshRenderer>().enabled = false;
    }

    private void ShowInteractable(GameObject interactable){
        if (customInteractionManager.interactionState != InteractionState.TiltHeadUpright){
            return;
        }

        interactable.GetComponent<MeshRenderer>().enabled = true;
    }

    public void ForceInteractionRelease(){
        Debug.Log("Forcing interaction release...");

        OnTopReleased(null);
        OnBottomReleased(null);
    }

    public void DisableInteraction()
    {
        Debug.Log("Disabling HeadTiltInteraction...");

        if (topHeadInteractable != null)
        {
            topHeadInteractable.selectEntered.RemoveListener(OnTopGrabbed);
            topHeadInteractable.selectExited.RemoveListener(OnTopReleased);
        }

        if (bottomHeadInteractable != null)
        {
            bottomHeadInteractable.selectEntered.RemoveListener(OnBottomGrabbed);
            bottomHeadInteractable.selectExited.RemoveListener(OnBottomReleased);
        }
    }

    private void OnDestroy()
    {
        // Remove listeners when the script is destroyed
        if (topHeadInteractable != null)
        {
            topHeadInteractable.selectEntered.RemoveListener(OnTopGrabbed);
            topHeadInteractable.selectExited.RemoveListener(OnTopReleased);
        }

        if (bottomHeadInteractable != null)
        {
            bottomHeadInteractable.selectEntered.RemoveListener(OnBottomGrabbed);
            bottomHeadInteractable.selectExited.RemoveListener(OnBottomReleased);
        }
    }
}
