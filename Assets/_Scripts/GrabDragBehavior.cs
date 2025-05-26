using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabDragBehavior : MonoBehaviour
{
    GameManager gameManager;
    public CustomInteractionManager customInteractionManager;

    public GameObject body;
    private Animator bodyAnimator;
    private Rigidbody bodyRigidbody; 
    private BoxCollider bodyCollider; 
    

    public XRSimpleInteractable leftShoulderInteractable;
    public XRSimpleInteractable rightShoulderInteractable;

    private bool isLeftShoulderGrabbed = false;
    private bool isRightShoulderGrabbed = false;

    private Transform leftHand;
    private Transform rightHand;

    public float followSpeed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>(); // Find the GameManager in the scene
        body = gameManager.bodyPrefab;

        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>();
        leftShoulderInteractable = customInteractionManager.shoulderInteractables[0].GetComponent<XRSimpleInteractable>();
        rightShoulderInteractable = customInteractionManager.shoulderInteractables[1].GetComponent<XRSimpleInteractable>();

        bodyAnimator = body.GetComponent<Animator>(); 
        bodyRigidbody = body.GetComponent<Rigidbody>(); 
        bodyCollider = body.GetComponent<BoxCollider>();

        if (leftShoulderInteractable != null)
        {
            leftShoulderInteractable.selectEntered.AddListener(OnLeftShoulderGrabbed);
            leftShoulderInteractable.selectExited.AddListener(OnLeftShoulderReleased);
        }

        if (rightShoulderInteractable != null)
        {
            rightShoulderInteractable.selectEntered.AddListener(OnRightShoulderGrabbed);
            rightShoulderInteractable.selectExited.AddListener(OnRightShoulderReleased);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isLeftShoulderGrabbed && isRightShoulderGrabbed && leftHand != null && rightHand != null)
        {
            Vector3 targetPosition = (leftHand.position + rightHand.position) / 2f;
            targetPosition.y = transform.position.y; // Keep body grounded
            GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * followSpeed));
        }
    }

    private void OnLeftShoulderGrabbed(SelectEnterEventArgs args)
    {
        isLeftShoulderGrabbed = true;
        leftHand = args.interactorObject.transform; // Get the left hand transform from the interactor
        CheckBothGrabbed();
    }

    private void OnLeftShoulderReleased(SelectExitEventArgs args)
    {
        isLeftShoulderGrabbed = false;
        leftHand = null; // Reset the left hand transform
        CheckBothGrabbed();
    }

    private void OnRightShoulderGrabbed(SelectEnterEventArgs args)
    {
        isRightShoulderGrabbed = true;
        rightHand = args.interactorObject.transform; // Get the right hand transform from the interactor
        CheckBothGrabbed();
    }

    private void OnRightShoulderReleased(SelectExitEventArgs args)
    {
        isRightShoulderGrabbed = false;
        rightHand = null; // Reset the right hand transform
        CheckBothGrabbed();
    }

    private void CheckBothGrabbed()
    {
        if (isLeftShoulderGrabbed && isRightShoulderGrabbed)
        {
            Debug.Log("Both shoulder objects are grabbed!");

            // Trigger the animation
            bodyAnimator.SetTrigger("TriggerUprightAnim");

            bodyCollider.enabled = true;
            bodyRigidbody.isKinematic = false; // Enable physics when both are grabbed
        }
        else
        {
            Debug.Log("One or both shoulder objects are not grabbed.");

            bodyAnimator.SetTrigger("TriggerLayingAnim");

            bodyCollider.enabled = false;
            bodyRigidbody.isKinematic = true; // Disable physics when not grabbed
        }
    }



    private void OnDestroy()
    {
        // Remove listeners when the script is destroyed
        if (leftShoulderInteractable != null)
        {
            leftShoulderInteractable.selectEntered.RemoveListener(OnLeftShoulderGrabbed);
            leftShoulderInteractable.selectExited.RemoveListener(OnLeftShoulderReleased);
        }

        if (rightShoulderInteractable != null)
        {
            rightShoulderInteractable.selectEntered.RemoveListener(OnRightShoulderGrabbed);
            rightShoulderInteractable.selectExited.RemoveListener(OnRightShoulderReleased);
        }
    }
}