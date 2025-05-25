using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabActivatedButton : MonoBehaviour
{
    private XRSimpleInteractable simpleInteractable;

    private void Awake()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
    }

    public void ActivateButton()
    {
        if (simpleInteractable != null)
        {
            simpleInteractable.interactionManager.SelectExit(simpleInteractable.firstInteractorSelecting, simpleInteractable);
        }
    }
}