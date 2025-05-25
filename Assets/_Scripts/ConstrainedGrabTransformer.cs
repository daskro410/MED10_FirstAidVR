using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class ConstrainedGrabTransformer : XRBaseGrabTransformer
{
    public float minYOffset = -0.03f; // Minimum offset from the initial position
    public float maxYOffset = 0.0f;  // Maximum offset from the initial position

    private float initialYPosition; // Initial Y position of the interactable

    public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
    {
        if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            return;

        // If this is the first frame of the grab, store the initial Y position
        if (initialYPosition == 0)
        {
            initialYPosition = targetPose.position.y;
        }

        // Constrain the Y position of the target pose
        Vector3 constrainedPosition = targetPose.position;
        constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, initialYPosition + minYOffset, initialYPosition + maxYOffset);
        targetPose.position = constrainedPosition;
    }
}