using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public enum PhoneInteractionState{
    Invisible,
    InPocket,
    InHand,
    OnGround
}

public class PhoneInteractionBehavior : MonoBehaviour
{
    CustomInteractionManager customInteractionManager; // Reference to the CustomInteractionManager script
    AudioManager audioManager; // Reference to the AudioManager script

    public GameObject phoneObject;
    public XRSocketInteractor pocketSocketInteractor;
    public GameObject phoneBodySocketAttachPoint;

    public PhoneInteractionState phoneInteractionState;

    public Sprite[] phoneScreens;
    public Image phoneScreenImage; // Reference to the UI Image component for the phone screen

    public bool isFirstTimeInPocket = true;

    private PhoneInteractionState previousState;

    private XRInteractionManager interactionManager;

    public InteractionLayerMask initialLayerMask;
    public InteractionLayerMask currentLayerMask;

    public bool isAllowedToPutPhoneDown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>(); // Find the CustomInteractionManager in the scene
        audioManager = FindFirstObjectByType<AudioManager>(); // Find the AudioManager in the scene
        // Find the XRInteractionManager in the scene
        interactionManager = FindFirstObjectByType<XRInteractionManager>();

        // Initialize the phone interaction state to Invisible
        SetPhoneInteractionState(PhoneInteractionState.Invisible);

        previousState = phoneInteractionState;

        initialLayerMask = phoneObject.GetComponent<XRGrabInteractable>().interactionLayers;
        currentLayerMask = initialLayerMask;

        //phoneScreenImage = phoneObject.GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG: Check if the phone interaction state has changed
        if (phoneInteractionState != previousState){
            SetPhoneInteractionState(phoneInteractionState);
            previousState = phoneInteractionState;
        }
    }

    public void SetPhoneInteractionState(PhoneInteractionState state)
    {
        phoneInteractionState = state;

        switch (state)
        {
            case PhoneInteractionState.Invisible:
                HandleInvisibleState();
                break;
            case PhoneInteractionState.InPocket:
                HandleInPocketState();
                break;
            case PhoneInteractionState.InHand:
                HandleInHandState();
                break;
            case PhoneInteractionState.OnGround:
                HandleOnGroundState();
                break;
        }
    }

    public void HandleInvisibleState(){
        // Disable phone entirely
        phoneObject.SetActive(false);
    }

    public void HandleInPocketState(){
        phoneObject.SetActive(true);
        phoneObject.GetComponent<XRGrabInteractable>().enabled = true;

        if (isFirstTimeInPocket)
        {
            isFirstTimeInPocket = false;
            StartCoroutine(SnapPhoneToPocket());
        }
    }

    public void HandleInHandState(){
        phoneObject.SetActive(true);
    }

    public void HandleOnGroundState(){
        phoneObject.SetActive(true);

        // Wait a bit before disabling the interaction, for some reason it doesn't work if we do it immediately
        currentLayerMask = InteractionLayerMask.GetMask("UNINTERACTABLE");
        phoneObject.GetComponent<XRGrabInteractable>().interactionLayers = currentLayerMask;
        customInteractionManager.phoneSocket.GetComponent<XRSocketInteractor>().interactionLayers = currentLayerMask;

        // Remove visuals from phone socket
        if (customInteractionManager.phoneSocket.GetNamedChild("Socket Visuals")){
            customInteractionManager.phoneSocket.GetNamedChild("Socket Visuals").gameObject.SetActive(false);
        }
    }

    public void OnPhoneGrab(){
        if (phoneInteractionState == PhoneInteractionState.InPocket){
            SetPhoneInteractionState(PhoneInteractionState.InHand);
        }
    }

    public void OnPhoneRelease(){
        if (phoneInteractionState == PhoneInteractionState.OnGround){
            Debug.Log("Phone is on the ground, disabling grab.");

            return;
        }

        else if (phoneInteractionState == PhoneInteractionState.InHand){
            Debug.Log("Tried to drop phone in air, returning to pocket.");
            phoneObject.transform.position = phoneBodySocketAttachPoint.transform.position;
            phoneObject.transform.rotation = phoneBodySocketAttachPoint.transform.rotation;
        }
    }

    public void OnPhoneActivate(){
        Debug.Log("Activation button pressed on phone.");

        if (phoneInteractionState == PhoneInteractionState.InHand)
        {
            int currentScreenIndex = System.Array.IndexOf(phoneScreens, phoneScreenImage.sprite);

            if (currentScreenIndex < phoneScreens.Length - 1)
            {
                int nextScreenIndex = currentScreenIndex + 1;
                phoneScreenImage.sprite = phoneScreens[nextScreenIndex];

                Debug.Log("Phone screen changed to: " + phoneScreens[nextScreenIndex].name);

                audioManager.Play("Phone_Beep");

                // Check if the last screen is now active
                if (nextScreenIndex == phoneScreens.Length - 1)
                {
                    Debug.Log("Reached the last screen. Allowing to put down phone.");
                    isAllowedToPutPhoneDown = true;
                    customInteractionManager.phoneSocket.SetActive(true); // Enable the phone socket

                    audioManager.Play("Phone_Ringing");
                }
            }
        }
    }

    public IEnumerator SnapPhoneToPocket()
    {
        yield return new WaitForEndOfFrame(); // Or WaitForSeconds(0.1f) if needed

        var interactable = phoneObject.GetComponent<XRGrabInteractable>();
        if (interactable == null || pocketSocketInteractor == null || interactionManager == null)
        {
            Debug.LogError("Interactable, SocketInteractor, or InteractionManager is null.");
            yield break;
        }

        // Move the phone to the pocket's attach point
        phoneObject.transform.position = pocketSocketInteractor.attachTransform.position;
        phoneObject.transform.rotation = pocketSocketInteractor.attachTransform.rotation;

        // Ensure the same InteractionManager is being used
        interactable.interactionManager = interactionManager;

        // Manually trigger the selection
        interactionManager.SelectEnter((IXRSelectInteractor)pocketSocketInteractor, (IXRSelectInteractable)interactable);

        Debug.Log("Phone snapped to pocket via XR action.");
    }
}
