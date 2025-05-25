using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public enum PhoneState
{
    InHandActive,
    InHandInactive,
    Dropped
}

public class PhoneBehavior : MonoBehaviour
{
    public XRGrabInteractable grabInteractable;
    //public XRBaseGrabTransformer grabTransformer;

    public bool isPhoneDroppable;

    //public Vector3 originalPhonePos;
    //public Quaternion originalPhoneRot;
    //public Transform originalParent;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        //grabTransformer = GetComponent<XRBaseGrabTransformer>();

        //grabInteractable.enabled = false;
        //grabTransformer.enabled = false;

        //originalParent = transform.parent;
        //originalPhonePos = transform.localPosition;
        //originalPhoneRot = transform.localRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Phone collided with " + other.gameObject.tag);

        if (other.gameObject.CompareTag("PhonePlacementArea")){
            isPhoneDroppable = true;

            Debug.Log("Phone is droppable");
        }
    }

    void Update()
    {

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PhonePlacementArea")){
            isPhoneDroppable = false;

            Debug.Log("Phone is no longer droppable");
        }
    }
}
