using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[System.Serializable]
public class Haptic
{
    [Range(0,1)]
    public float hapticIntensity = 0.5f;
    public float hapticDuration = 0.1f;

    public void TriggerHaptic(BaseInteractionEventArgs eventArgs)
    {
        if (eventArgs.interactorObject is XRDirectInteractor directInteractor)
        {
            TriggerHaptic(directInteractor);
        }
    }

    public void TriggerHaptic(XRDirectInteractor interactor){
        if (interactor != null)
        {
            interactor.SendHapticImpulse(hapticIntensity, hapticDuration);
        }
    }
}

public class HapticInteractable : MonoBehaviour
{
    public Haptic hapticOnHover;
    public Haptic hapticOnActivate;
    public Haptic hapticOnSelect;

    void Start()
    {
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();

        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(hapticOnHover.TriggerHaptic);
            interactable.activated.AddListener(hapticOnActivate.TriggerHaptic);
            interactable.selectEntered.AddListener(hapticOnSelect.TriggerHaptic);
        }
    }

}
