using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrabHandPose : MonoBehaviour
{
    CustomInteractionManager customInteractionManager;

    public float poseTransitionDuration = 0.2f;

    private HandData leftHandModelHandData;
    private HandData rightHandModelHandData;

    public HandData leftHandPose; // Reference to the HandData scriptable object
    public HandData rightHandPose; // Reference to the HandData scriptable object

    public bool animatePosition = true; // Flag to enable/disable position animation
    public bool animateRotation = true; // Flag to enable/disable rotation animation
    public bool animateFingerRotations = true; // Flag to enable/disable finger rotation animation

    private Vector3 startingHandPosition;
    private Vector3 finalHandPosition;
    private Quaternion startingHandRotation;
    private Quaternion finalHandRotation;
    private Quaternion[] startingFingerRotations;
    private Quaternion[] finalFingerRotations;

    XRGrabInteractable grabInteractable;
    XRSimpleInteractable simpleInteractable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>(); // Find the CustomInteractionManager in the scene

        leftHandModelHandData = customInteractionManager.leftHandModel.GetComponent<HandData>();
        rightHandModelHandData = customInteractionManager.rightHandModel.GetComponent<HandData>();

        grabInteractable = GetComponent<XRGrabInteractable>();
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(SetupPose);
            grabInteractable.selectExited.AddListener(UnsetPose);
        }
        else if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.AddListener(SetupPose);
            simpleInteractable.selectExited.AddListener(UnsetPose);
        }
        else
        {
            Debug.LogError("No XRGrabInteractable or XRSimpleInteractable found on the object.");
        }

        leftHandPose.gameObject.SetActive(false);
        rightHandPose.gameObject.SetActive(false);
    }

    public void SetupPose(BaseInteractionEventArgs arg)
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.Log("Skipping SetupPose because object is not active.");
            return;
        }

        if (arg.interactorObject == null)
        {
            Debug.LogError("Interactor object is null in SetupPose.");
            return;
        }

        HandData handData = null;

        if (arg.interactorObject is XRBaseInteractor interactor)
        {
            // Check if the interactor is the left or right hand based on the original references
            if (interactor == customInteractionManager.leftHandModel.GetComponentInParent<XRBaseInteractor>())
            {
                handData = leftHandModelHandData;
            }
            else if (interactor == customInteractionManager.rightHandModel.GetComponentInParent<XRBaseInteractor>())
            {
                handData = rightHandModelHandData;
            }
            else
            {
                Debug.LogError("Interactor is not associated with the left or right hand.");
                return;
            }
        }

        if (handData == null)
        {
            Debug.LogError("HandData component not found on interactor's hierarchy. Parent is " + arg.interactorObject.transform.name);
            return;
        }

        handData.animator.enabled = false;

        if (handData.handType == HandData.HandType.Right)
        {
            SetHandDataValues(handData, rightHandPose);
        }
        else if (handData.handType == HandData.HandType.Left)
        {
            SetHandDataValues(handData, leftHandPose);
        }

        StartCoroutine(SetHandDataRoutine(
            handData,
            finalHandPosition,
            finalHandRotation,
            finalFingerRotations,
            startingHandPosition,
            startingHandRotation,
            startingFingerRotations
        ));
    }

    public void UnsetPose(BaseInteractionEventArgs arg){
        /* if (!gameObject.activeInHierarchy)
        {
            Debug.Log("Skipping UnsetPose because object is not active.");
            return;
        } */

        if (arg.interactorObject is XRBaseInputInteractor)
        {
            //Debug.Log("Interactor is XRBaseInputInteractor");
    
            var handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            if (handData == null)
            {
                //Debug.LogWarning("HandData is NULL");
            }
            else if (!handData.gameObject.activeInHierarchy)
            {
                //Debug.LogWarning("HandData is INACTIVE");
            }
            else
            {
                //Debug.Log("Re-enabling animator");
                handData.animator.enabled = true;
            }

            StartCoroutine(SetHandDataRoutine(handData, startingHandPosition, startingHandRotation, startingFingerRotations, finalHandPosition, finalHandRotation, finalFingerRotations));
        }
    }

    public void SetHandDataValues(HandData h1, HandData h2)
    {
        startingHandPosition = new Vector3(
            h1.root.localPosition.x / h1.root.localScale.x, 
            h1.root.localPosition.y / h1.root.localScale.y, 
            h1.root.localPosition.z / h1.root.localScale.z);
        finalHandPosition = new Vector3(
            h2.root.localPosition.x / h2.root.localScale.x, 
            h2.root.localPosition.y / h2.root.localScale.y, 
            h2.root.localPosition.z / h2.root.localScale.z);

        startingHandRotation = h1.root.localRotation;
        finalHandRotation = h2.root.localRotation;

        startingFingerRotations = new Quaternion[h1.fingerBones.Length];
        finalFingerRotations = new Quaternion[h1.fingerBones.Length];

        for (int i = 0; i < h1.fingerBones.Length; i++){
            startingFingerRotations[i] = h1.fingerBones[i].localRotation;
            finalFingerRotations[i] = h2.fingerBones[i].localRotation;
        }
    }

    public void SetHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBoneRotations){
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;

        for (int i = 0; i < newBoneRotations.Length; i++){
            h.fingerBones[i].localRotation = newBoneRotations[i];
        }
    }

    public IEnumerator SetHandDataRoutine(
        HandData h, 
        Vector3 newPosition, 
        Quaternion newRotation, 
        Quaternion[] newBoneRotations, 
        Vector3 startingPosition, 
        Quaternion startingRotation, 
        Quaternion[] startingBoneRotations)
    {
        float timer = 0;

        while (timer < poseTransitionDuration)
        {
            // check if hand is still available
            if (h == null || !h.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("SetHandDataRoutine skipped: HandData is null or inactive.");
                yield break;
            }

            float t = timer / poseTransitionDuration;

            if (animatePosition) h.root.localPosition = Vector3.Lerp(startingPosition, newPosition, t);

            if (animateRotation) h.root.localRotation = Quaternion.Lerp(startingRotation, newRotation, t);

            if (animateFingerRotations)
            {
                for (int i = 0; i < newBoneRotations.Length; i++)
                {
                    h.fingerBones[i].localRotation = Quaternion.Lerp(startingBoneRotations[i], newBoneRotations[i], t);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Snap to final values at end
        if (animatePosition) h.root.localPosition = newPosition;
        if (animateRotation) h.root.localRotation = newRotation;
        if (animateFingerRotations)
        {
            for (int i = 0; i < newBoneRotations.Length; i++)
            {
                h.fingerBones[i].localRotation = newBoneRotations[i];
            }
        }
    }


#if UNITY_EDITOR
    [MenuItem("Tools/Mirror Right Pose")]
    public static void MirrorRightPose(){
        GrabHandPose handPose = Selection.activeGameObject.GetComponent<GrabHandPose>();
        if (handPose == null){
            Debug.LogError("GrabHandPose component not found on selected object.");
            return;
        }

        Debug.Log("#MIRRORED POSE#");
        handPose.MirrorPose(handPose.leftHandPose, handPose.rightHandPose);
    }
#endif

    public void MirrorPose(HandData poseToMirror, HandData poseUsedToMirror){
        Vector3 mirroredPostion = poseUsedToMirror.root.localPosition;
        mirroredPostion.x *= -1;

        Quaternion mirroredRotation = poseUsedToMirror.root.localRotation;
        mirroredRotation.y *= -1;
        mirroredRotation.z *= -1;

        poseToMirror.root.localPosition = mirroredPostion;
        poseToMirror.root.localRotation = mirroredRotation;

        for (int i = 0; i < poseToMirror.fingerBones.Length; i++){
            poseToMirror.fingerBones[i].localRotation = poseUsedToMirror.fingerBones[i].localRotation;
        }
    }

    private void OnDisable()
    {
        if (grabInteractable && grabInteractable.isSelected)
        {
            foreach (var interactor in grabInteractable.interactorsSelecting)
            {
                if (interactor is XRBaseInteractor baseInteractor)
                {
                    Debug.Log("Manually calling UnsetPose from OnDisable for " + name);
                    UnsetPoseFromInteractor(baseInteractor);
                }
                else
                {
                    Debug.LogWarning("Interactor is not of type XRBaseInteractor.");
                }
            }
        }
    }

    private void UnsetPoseFromInteractor(XRBaseInteractor interactor)
    {
        var handData = interactor.transform.GetComponentInChildren<HandData>();
        if (handData == null)
        {
            Debug.LogWarning("HandData is NULL");
            return;
        }

        // Ensure the animator is re-enabled when interactable is disabled
        handData.animator.enabled = true;

        // Now manually call the logic to unset the pose, just like in UnsetPose()
        StartCoroutine(SetHandDataRoutine(handData, startingHandPosition, startingHandRotation, startingFingerRotations, finalHandPosition, finalHandRotation, finalFingerRotations));
    }
}
