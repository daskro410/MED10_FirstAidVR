using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CustomInteractions : MonoBehaviour
{
    public PhoneState phoneState;

    public XRBaseInteractor leftControllerInteractor;
    public XRGrabInteractable phoneGrabInteractable;
    public GameObject phonePrefab;

    public bool isPhoneInHand;
    Rigidbody phoneRigidbody;
    PhoneBehavior phoneBehavior;
    MeshRenderer phoneMeshRenderer;
    BoxCollider phoneCollider;

    bool isPhoneDropped;

    public InputActionReference phoneAction;

    void Start(){
        phoneRigidbody = phonePrefab.GetComponent<Rigidbody>();
        phoneBehavior = phonePrefab.GetComponent<PhoneBehavior>();
        phoneMeshRenderer = phonePrefab.GetComponent<MeshRenderer>();
        phoneCollider = phonePrefab.GetComponent<BoxCollider>();
        phoneGrabInteractable = phonePrefab.GetComponent<XRGrabInteractable>();

        //phoneRigidbody.useGravity = false;

/*         phoneAction.action.started += GripPressed;
        phoneAction.action.canceled += GripReleased; */

        //phonePrefab.SetActive(false);
        //phoneMeshRenderer.enabled = false;
        //phoneCollider.enabled = false;
        //isPhoneActive = false;

        // Attach phone to hand on start
        AttachPhoneToHand();

    }

    void AttachPhoneToHand(){
        Debug.Log("Attaching phone to hand at the start now...");

        leftControllerInteractor.interactionManager.SelectEnter(
            (IXRSelectInteractor)leftControllerInteractor, 
            (IXRSelectInteractable)phoneGrabInteractable
            );

        phoneState = PhoneState.InHandInactive;
    }

    void Update(){
        
    }

/*     public void GripPressed(InputAction.CallbackContext context){
        if (!isPhoneActive && !isPhoneDropped){
            TogglePhone();
        }
    }

    public void GripReleased(InputAction.CallbackContext context){
        if (isPhoneActive){

            // Drop phone if it is droppable
            if (phoneBehavior.isPhoneDroppable && !isPhoneDropped){
                DropPhone();
                Debug.Log("Phone dropped");
            }

            if (!isPhoneDropped){
                TogglePhone();
                Debug.Log("Phone dismissed");
            }
        }
    } */

/*     public void TogglePhone(){
        if(isPhoneInHand){
            isPhoneInHand = false;
            //phonePrefab.SetActive(false);

            phoneMeshRenderer.enabled = false;
            //phoneCollider.enabled = false;

            Debug.Log("Phone hidden");

        } else {
            isPhoneInHand = true;
            //phonePrefab.SetActive(true);

            phoneMeshRenderer.enabled = true;
            //phoneCollider.enabled = true;

            Debug.Log("Phone shown");
        }
    } */

// REPLACED WITH STICKY INTERACTION WITH INTERACTION TOOLKIT
/*     public void SnapToHand(){
        if (phoneBehavior.isPhoneDropped){
            phoneBehavior.isPhoneDropped = false;
            phoneBehavior.isPhoneDroppable = false;

            phoneCollider.isTrigger = true;

            // Reset Rigidbody physics
            phoneRigidbody.useGravity = false;
            phoneRigidbody.linearVelocity = Vector3.zero;
            phoneRigidbody.angularVelocity = Vector3.zero;

            phoneBehavior.grabInteractable.enabled = false;
            phoneBehavior.grabTransformer.enabled = false;

            //phonePrefab.transform.SetParent(phoneBehavior.originalParent);
            //phonePrefab.transform.SetPositionAndRotation(phoneBehavior.originalPhonePos, phoneBehavior.originalPhoneRot);
            //phonePrefab.transform.rotation = phoneBehavior.originalPhoneRot;

            Debug.Log("Phone snapped to hand");
        }

        Debug.Log("Phone is not dropped, can't snap to hand");
    } */

    public void FirstSelectEntered(){
        // if the phone is dropped, then pick it up
        if (phoneState == PhoneState.Dropped){
            phoneState = PhoneState.InHandActive;
            Debug.Log("Phone picked up from the ground. State = " + phoneState);

            SetPhoneGrabbable(false);
        }

        // if the phone is hidden, then show it
        else if (phoneState == PhoneState.InHandInactive){
            phoneState = PhoneState.InHandActive;
            Debug.Log("Phone is hidden, show it now. State = " + phoneState);
        }
    }

    public void LastSelectExit(){
        // ASK TO DROP PHONE OR NOT 

        // if already in hand is can be dropped, drop it
        if (phoneState == PhoneState.InHandActive && phoneBehavior.isPhoneDroppable){
            phoneState = PhoneState.Dropped;
            Debug.Log("Phone dropped. State = " + phoneState);

            SetPhoneGrabbable(true);
        }

        // if already in hand but can't be dropped, toggle it away (this can maybe just be an else statement)
        else if (phoneState == PhoneState.InHandActive && !phoneBehavior.isPhoneDroppable){
            phoneState = PhoneState.InHandInactive;
            Debug.Log("Phone not droppable, phone hidden. State = " + phoneState);

            SetPhoneGrabbable(false);
        }

/*      phoneBehavior.isPhoneDropped = true;

        phoneCollider.isTrigger = false;
                
        phonePrefab.transform.parent = null;
        phoneRigidbody.useGravity = true;

        phoneBehavior.grabInteractable.enabled = true;
        phoneBehavior.grabTransformer.enabled = true; */
    }

    public void SetPhoneGrabbable(bool canGrab){
        phoneGrabInteractable.firstSelectEntered.RemoveAllListeners();
        phoneGrabInteractable.lastSelectExited.RemoveAllListeners();

        if (canGrab){
            phoneGrabInteractable.firstSelectEntered.AddListener(_ => FirstSelectEntered());
            phoneGrabInteractable.lastSelectExited.AddListener(_ => LastSelectExit());
        }

        Debug.Log("Phone grabbability set to " + canGrab);
    }

    public void PersonGrabbed(){
        Debug.Log("Person grabbed!");
    }

    public void PersonDropped(){
        Debug.Log("Person dropped!");
    }
}
