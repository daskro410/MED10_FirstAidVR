using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class PlayerMovementLocker : MonoBehaviour
{
    CustomInteractionManager customInteractionManager; // Reference to the CustomInteractionManager script

    public GameObject XRRig;
    public GameObject sittingSpot;

    public float fadeDuration = 0.5f;

    private float kneelOffset;
    private ContinuousMoveProvider moveProvider;
    private XROrigin xrOrigin;

    public TeleportationProvider teleportationProvider;
    public Camera mainCamera;

    // New boolean flag to toggle height adjustment
    public bool adjustPlayerHeight = true;

    void Start()
    {
        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>();

        xrOrigin = XRRig.GetComponent<XROrigin>();
        moveProvider = XRRig.GetComponentInChildren<ContinuousMoveProvider>();

        if (moveProvider == null)
            Debug.LogError("ContinuousMoveProvider not found.");
    }

    public void LockMovement()
    {
        if (moveProvider) moveProvider.enabled = false;
        Debug.Log("Movement Locked");

        GetComponent<Collider>().enabled = false;
    }

    public void UnlockMovement()
    {
        if (moveProvider) moveProvider.enabled = true;
        Debug.Log("Movement Unlocked");

        GetComponent<Collider>().enabled = true;
    }

    // Don't need this anymore as it does not work hahaha
    public void SnapPlayerToSittingSpot()
    {
        FadeBehavior fadeBehavior = XRRig.GetComponentInChildren<FadeBehavior>();

        fadeBehavior.FadeOut(fadeDuration);

        // Get the player's camera height
        float playerHeight = xrOrigin.CameraInOriginSpaceHeight;

        // Get the sitting spot's position and rotation
        Vector3 sittingSpotPosition = sittingSpot.transform.position;
        Quaternion sittingSpotRotation = sittingSpot.transform.rotation;

        // Calculate the target camera position
        Vector3 targetCameraPosition = sittingSpotPosition;

        // Apply vertical offset depending on whether we want to sit or stand
        if (adjustPlayerHeight)
        {
            // Simulate sitting: place camera ~0.5x height above seat
            targetCameraPosition += Vector3.up * (playerHeight * 0.5f);
        }
        else
        {
            // Simulate standing: place camera ~1x height above seat
            targetCameraPosition += Vector3.up * playerHeight;
        }

        // Calculate the offset between the camera's current position and the target position
        Vector3 cameraOffset = targetCameraPosition - xrOrigin.Camera.transform.position;

        // Move the XR Rig by the calculated offset
        XRRig.transform.position += cameraOffset;

        // Align the XR Rig's rotation with the sitting spot's rotation (only Y-axis)
        Vector3 rigEuler = XRRig.transform.eulerAngles;
        rigEuler.y = sittingSpotRotation.eulerAngles.y;
        XRRig.transform.eulerAngles = rigEuler;

        // Disable the sitting spot after use
        sittingSpot.SetActive(false);

        // Update the interaction state
        customInteractionManager.SetInteractionState(InteractionState.CheckConciousness);

        fadeBehavior.FadeIn(fadeDuration);
    }

    private void NewSnapPlayerToSpot()
    {
        FadeBehavior fadeBehavior = XRRig.GetComponentInChildren<FadeBehavior>();

        fadeBehavior.FadeOut(fadeDuration);

        XRRig.transform.position = new Vector3(sittingSpot.transform.position.x, XRRig.transform.position.y, sittingSpot.transform.position.z);
        XRRig.transform.rotation = new Quaternion(sittingSpot.transform.rotation.x, sittingSpot.transform.rotation.y, sittingSpot.transform.rotation.z, sittingSpot.transform.rotation.w);

        fadeBehavior.FadeOut(fadeDuration);
    }

    // THIS WORKS THX GADE :D
    public void MoveToSeat(GameObject target)
    {
        FadeBehavior fadeBehavior = XRRig.GetComponentInChildren<FadeBehavior>();

        fadeBehavior.FadeOut(fadeDuration);

        // Get the target rotation
        Quaternion targetRotation = target.transform.rotation;

        // Create a TeleportRequest with the target position and rotation
        TeleportRequest teleportRequest = new TeleportRequest()
        {
            destinationPosition = new Vector3(target.transform.position.x, XRRig.transform.position.y, target.transform.position.z),
            destinationRotation = targetRotation
        };

        //Quaternion cameraRotation = Quaternion.Euler(0f, mainCamera.transform.localEulerAngles.y, 0f);
        //TeleportationProvider teleportationProvider = player.GetComponent<TeleportationProvider>();
        teleportationProvider.QueueTeleportRequest(teleportRequest);

        // Get the current camera rotation
        Quaternion cameraRotation = Quaternion.Euler(0f, mainCamera.transform.localEulerAngles.y, 0f);

        // Rotate the XR Rig's XROrigin to face the correct direction after teleportation, considering camera rotation
        XRRig.transform.rotation = targetRotation;// * Quaternion.Inverse(cameraRotation);

        // Disable the sitting spot after use
        sittingSpot.SetActive(false);

        // Update the interaction state
        customInteractionManager.SetInteractionState(InteractionState.CheckConciousness);

        fadeBehavior.FadeIn(fadeDuration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LockMovement();
            //SnapPlayerToSittingSpot();
            MoveToSeat(sittingSpot);
        }

        Debug.Log(other.name + " entered the trigger with tag: " + other.tag);
    }
}