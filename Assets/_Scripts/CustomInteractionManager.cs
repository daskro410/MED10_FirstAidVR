using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public enum InteractionState
{
    None,
    BodyDragging,
    CheckSurroundings,
    CheckConciousness,
    TiltHeadUpright,
    CheckBreath,
    CallAmbulance,
    HeartPumping,
    MouthBreathing,
    AmbulanceArrived
}

public class CustomInteractionManager : MonoBehaviour
{
    UIManager uiManager;
    GameManager gameManager;
    private InteractionLogger interactionLogger;

    public GameObject leftHandModel;
    public GameObject rightHandModel;

    // INTERACTABLES
    public GameObject heartInteractable;
    public GameObject[] mouthInteractables;
    public GameObject[] shoulderInteractables;
    public GameObject[] bodyInteractables;
    public GameObject[] headInteractables;

    public GameObject phoneSocket;

    public GameObject sittingArea;

    public InteractionState interactionState;

    [Header("Heart Interaction")]
    public int currentChestPressCounter = 0;
    public int maxChestPressCounter = 30;

    [Header("Mouth Interaction")]
    public float currentMouthListeningTime = 0f;
    public float maxMouthListeningTime = 2f;

    public float currentMouthBreathTime = 0f;
    public float maxMouthBreathTime = 2f;
    public float minHMDDistance = 0.5f;

    [Header("Body Shake")]
    public float currentBodyShakeTime = 0f;
    public float maxBodyShakeTime = 5f;

    [Header("Check Surroundings")]
    public float currentCheckSurroundingsTime = 0f;
    public float maxCheckSurroundingsTime = 5f;

    public InteractionState previousState;
    private bool isAmbulanceArrivalAllowed = false;

    private GameObject[] allInteractables;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the UIManager in the scene
        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null) { Debug.LogError("UIManager not found in the scene."); return;}
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null) { Debug.LogError("GameManager not found in the scene."); return;}
        interactionLogger = FindFirstObjectByType<InteractionLogger>();
        if (interactionLogger == null) { Debug.LogError("InteractionLogger not found in the scene."); return;}

        // Initialize the interaction state to None
        SetInteractionState(InteractionState.CheckSurroundings);

        previousState = interactionState;
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG: Check if the interaction state has changed
        if (interactionState != previousState){
            SetInteractionState(interactionState);
            previousState = interactionState;
        }  
    }

    public void SetInteractionState(InteractionState state)
    {
        interactionLogger?.StartTrackingState(state);

        interactionState = state;

        Debug.Log("Setting interaction state to: " + state);

        switch (state)
        {
            case InteractionState.None:
                HandleNoneState();
                break;
            case InteractionState.BodyDragging:
                HandleBodyDraggingState();
                break;
            case InteractionState.CheckSurroundings:
                HandleCheckSurroundingsState();
                break;
            case InteractionState.CheckConciousness:
                HandleCheckConsciousnessState();
                break;
            case InteractionState.TiltHeadUpright:
                HandleTiltHeadUprightState();
                break;
            case InteractionState.CheckBreath:
                HandleCheckBreathState();
                break;
            case InteractionState.CallAmbulance:
                HandleCallAmbulanceState();
                break;
            case InteractionState.HeartPumping:
                HandleHeartPumpingState();
                break;
            case InteractionState.MouthBreathing:
                HandleMouthBreathingState();
                break;
            case InteractionState.AmbulanceArrived:
                HandleAmbulanceArrivedState();
                break;
        }
    }

    public void HandleNoneState()
    {
        //Debug.Log("Changed state to: None");

        HideAllInteractables(); // Disable all interactables
    }

    public void HandleBodyDraggingState()
    {
        //Debug.Log("Changed state to: BodyDragging");

        HideAllInteractables(); // Disable all interactables
        ShowInteractables(shoulderInteractables); // Enable body dragging interactables
    }

    public void HandleCheckSurroundingsState()
    {
        HideAllInteractables(); // Disable all interactables

        StartCoroutine(TrackCheckingSurroundingsTime());
    }

    public void HandleCheckConsciousnessState()
    {
        //Debug.Log("Changed state to: CheckConsciousness");

        HideAllInteractables(); // Disable all interactables
        ShowInteractables(bodyInteractables);

        //sittingArea.SetActive(true); // Enable the sitting area interactable
    }

    public void HandleTiltHeadUprightState()
    {
        //Debug.Log("Changed state to: TiltHeadUpright");

        HideAllInteractables(); // Disable all interactables

        // Enable the head tilt interactable
        GetComponent<HeadTiltInteraction>().enabled = true; // Enable the HeadTiltInteraction script to allow interaction
        GetComponent<HeadTiltInteraction>().ForceInteractionRelease(); // Force release of the interactables

        ShowInteractables(headInteractables); // Enable head tilting interactables
    }

    public void HandleCheckBreathState()
    {
        //Debug.Log("Changed state to: CheckBreath");

        HideAllInteractables(); // Disable all interactables

        GetComponent<GrabMouth>().enabled = true;
        GetComponent<GrabMouth>().CheckToSwitchTop(); // Check to switch the top interactable
        GetComponent<GrabMouth>().ForceInteractionRelease(); // Force release of the interactables

        ShowInteractables(headInteractables); // Enable breath listening interactables
    }

    public void HandleCallAmbulanceState()
    {
        //Debug.Log("Changed state to: CallAmbulance");

        HideAllInteractables(); // Disable all interactables
        
        // Enable phone interactable and make sure it changes state :D
        GetComponent<PhoneInteractionBehavior>().isFirstTimeInPocket = true;
        GetComponent<PhoneInteractionBehavior>().SetPhoneInteractionState(PhoneInteractionState.InPocket);

        /* if (GetComponent<PhoneInteractionBehavior>().isAllowedToPutPhoneDown){
            phoneSocket.SetActive(true); // Enable the phone socket
        } */
    }

    public void HandleHeartPumpingState()
    {
        //Debug.Log("Changed state to: HeartPumping");

        HideAllInteractables(); // Disable all interactables

        // Reset the heart interactable to its original y postion
        heartInteractable.GetComponent<ClampButtonMovement>().ResetToInitialPosition();

        ShowInteractables(new GameObject[] { heartInteractable }); // Enable heart pumping interactables
    }

    public void HandleMouthBreathingState()
    {        
        //Debug.Log("Changed state to: MouthBreathing");

        HideAllInteractables(); // Disable all interactables

        GetComponent<GrabMouth>().enabled = true;
        GetComponent<GrabMouth>().CheckToSwitchTop(); // Check to switch the top interactable
        GetComponent<GrabMouth>().ForceInteractionRelease(); // Force release of the interactables
        
        ShowInteractables(mouthInteractables); // Enable mouth breathing interactables

        //leftHandModel.SetActive(true);
        //rightHandModel.SetActive(true);
    }

    public void HandleAmbulanceArrivedState()
    {
        //Debug.Log("Changed state to: AmbulanceArrived");

        HideAllInteractables(); // Disable all interactables

        // Stop tracking interactions and save the log to a file
        interactionLogger.StopTracking();
        interactionLogger.SaveLogToFile();
    }

    public void AddToHeartPressCounter()
    {
        currentChestPressCounter++;
        //Debug.Log("Heart Press Counter: " + currentChestPressCounter);

        if (currentChestPressCounter >= maxChestPressCounter)
        {
            Debug.Log("Max heart press counter reached: " + maxChestPressCounter + "Switching to mouth breathing state.");
            SetInteractionState(InteractionState.MouthBreathing);

            ResetHeartPressCounter();
            heartInteractable.GetComponent<MeshRenderer>().enabled = true; // Disable the heart interactable mesh renderer
            
            return;
        }

        // Update UI
        uiManager.UpdateHeartCounterText();
    }

    public void ResetHeartPressCounter()
    {
        currentChestPressCounter = 0;
        Debug.Log("Heart Press Counter reset.");

        uiManager.UpdateHeartCounterText();
    }

    private Coroutine mouthBreathCoroutine;

    public void StartMouthBreathCoroutine(float maxTime)
    {
        if (mouthBreathCoroutine == null)
        {
            Debug.Log("Starting mouthBreathCoroutine...");
            mouthBreathCoroutine = StartCoroutine(TrackMouthBreathTime(maxTime));
        }
    }

    public void StopMouthBreathCoroutine()
    {
        if (mouthBreathCoroutine != null)
        {
            Debug.Log("Stopping mouthBreathCoroutine...");
            StopCoroutine(mouthBreathCoroutine);
            mouthBreathCoroutine = null;
        }
    }

    public IEnumerator TrackMouthBreathTime(float maxTime)
    {
        GrabMouth grabMouth = GetComponent<GrabMouth>();

        if (!(interactionState == InteractionState.CheckBreath) && !(interactionState == InteractionState.MouthBreathing))
        {
            Debug.Log("Mouth breath time tracking is not allowed in the current state.");
            yield break;
        }

        Debug.Log("Tracking mouth breath time...");
        currentMouthListeningTime = 0f;
        currentMouthBreathTime = 0f;

        while (currentMouthBreathTime < maxTime)
        {
            // Only add time if the HMD is close enough
            if (mouthInteractables[1].GetComponent<JawHandler>().IsHMDCloseWithinDistance()
                && grabMouth.isTopGrabbed && grabMouth.isBottomGrabbed)
            {
                currentMouthBreathTime += Time.deltaTime;
                //Debug.Log($"HMD is close. Current time: {currentMouthBreathTime}/{maxTime}");

                uiManager.UpdateBreathCounterText(); // Update the UI with the current breath time
            }
            else
            {
                Debug.Log("HMD not close enough. Pausing timer.");
            }

            yield return null;
        }

        Debug.Log("Max mouth breath/listening time reached: " + maxTime + "Switching state.");
        ResetMouthBreathTime(); // Reset the timer if the HMD is close enough

        if (interactionState == InteractionState.MouthBreathing)
        {
            // Only switch to heart pumping state if the current state is MouthBreathing
            SetInteractionState(InteractionState.HeartPumping);

            // Ensures the interaction states have been done at least once before the ambulance arrives
            if (!isAmbulanceArrivalAllowed)
            {
                isAmbulanceArrivalAllowed = true;
                gameManager.SetGameState(GameState.AmbulanceCalled); // Call the ambulance if allowed
            }
        }
        else if (interactionState == InteractionState.CheckBreath)
        {
            // If the current state is CheckBreath, switch to heart pumping state
            SetInteractionState(InteractionState.CallAmbulance);
        }
    }

    public void ResetMouthBreathTime()
    {
        currentMouthBreathTime = 0f;
        currentMouthListeningTime = 0f;
        Debug.Log("Mouth Breath Time reset.");

        uiManager.UpdateBreathCounterText(); // Update the UI with the reset breath time
    }

    public IEnumerator TrackBodyShakingTime(){
        if (!(interactionState == InteractionState.CheckConciousness))
        {
            Debug.Log("Body shake time tracking is not allowed in the current state.");
            yield break;
        }

        Debug.Log("Tracking body shake time...");
        currentBodyShakeTime = 0f;

        while (currentBodyShakeTime < maxBodyShakeTime)
        {
            if (GetComponent<GrabBody>().isBothBodyInteractablesGrabbed)
            {
                currentBodyShakeTime += Time.deltaTime;
            }
            else
            {
                Debug.Log("Not both body interactables grabbed. Pausing body shake timer.");
            }

            yield return null;
        }

        Debug.Log("Max body shake time reached: " + maxBodyShakeTime + "Switching to breath state.");
        SetInteractionState(InteractionState.TiltHeadUpright);
    }

    public IEnumerator TrackCheckingSurroundingsTime(){
        if (!(interactionState == InteractionState.CheckSurroundings))
        {
            Debug.Log("Checking surroundings time tracking is not allowed in the current state.");
            yield break;
        }

        Debug.Log("Tracking checking surroundings time...");
        currentCheckSurroundingsTime = 0f;

        while (currentCheckSurroundingsTime < maxCheckSurroundingsTime)
        {
            currentCheckSurroundingsTime += Time.deltaTime;

            yield return null;
        }

        Debug.Log("Waiting for UI cycle to complete...");
        while (!uiManager.IsCurrentUICycleComplete())
        {
            yield return null;
        }

        Debug.Log("Checked surroundings for: " + maxCheckSurroundingsTime + " seconds. Switching to check consciousness state.");
        //SetInteractionState(InteractionState.CheckConciousness);
        sittingArea.SetActive(true); // Enable the sitting area interactable
    }

    public void ShowInteractables(GameObject[] interactables)
    {
        //Debug.Log("Showing interactables.");

        foreach (GameObject interactable in interactables)
        {
            var meshRenderer = interactable.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true; // Enable visuals
            }

            var simpleInteractable = interactable.GetComponent<XRSimpleInteractable>();
            if (simpleInteractable != null)
            {
                simpleInteractable.enabled = true; // Enable interaction
            }

            var grabInteractable = interactable.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.enabled = true; // Enable interaction
            }
        }
    }

    public void HideAllInteractables()
    {
        //Debug.Log("Hiding all interactables.");

        // Disable heart interactable
        if (heartInteractable != null)
        {
            heartInteractable.GetComponent<MeshRenderer>().enabled = false;
            heartInteractable.GetComponent<XRGrabInteractable>().enabled = false;
        }

        // Disable mouth interactables
        foreach (GameObject mouth in mouthInteractables)
        {
            var interactable = mouth.GetComponent<XRSimpleInteractable>();
            if (interactable != null)
            {
                interactable.enabled = false; // Disable interaction
            }
            mouth.GetComponent<MeshRenderer>().enabled = false; // Disable visuals
        }

        // Disable shoulder interactables
        foreach (GameObject shoulder in shoulderInteractables)
        {
            var interactable = shoulder.GetComponent<XRSimpleInteractable>();
            if (interactable != null)
            {
                interactable.enabled = false; // Disable interaction
            }
            shoulder.GetComponent<MeshRenderer>().enabled = false; // Disable visuals
        }

        // Disable body interactables
        foreach (GameObject body in bodyInteractables)
        {
            var interactable = body.GetComponent<XRSimpleInteractable>();
            if (interactable != null)
            {
                interactable.enabled = false; // Disable interaction
            }
            body.GetComponent<MeshRenderer>().enabled = false; // Disable visuals
        }

        // Disable head interactables
        foreach (GameObject head in headInteractables)
        {
            var interactable = head.GetComponent<XRSimpleInteractable>();
            if (interactable != null)
            {
                interactable.enabled = false; // Disable interaction
            }
            head.GetComponent<MeshRenderer>().enabled = false; // Disable visuals
        }

        // Disable sitting area
        if (sittingArea != null)
        {
            sittingArea.SetActive(false);
        }

        // CRAZY DUCT TAPE HERE
        //GetComponent<HeadTiltInteraction>().enabled = false; // Disable the HeadTiltInteraction script to prevent interaction
        GetComponent<GrabMouth>().enabled = false; // Disable the GrabMouth script to prevent interaction
    }
}
