using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ButtonFollowVisual : MonoBehaviour
{
    public Transform visualTarget;  // Button visual target
    
    private Vector3 offset;
    private Transform pokeAttachTransform;

    private XRBaseInteractable interactable;
    private bool isFollowing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();

    }

    public void Follow(BaseInteractionEventArgs hover){
        if (hover.interactorObject is XRPokeInteractor)
        {
            XRPokeInteractor interactor = (XRPokeInteractor)hover.interactorObject;
            isFollowing = true;

            pokeAttachTransform = interactor.attachTransform;

            offset = visualTarget.position - pokeAttachTransform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFollowing){
            visualTarget.position = pokeAttachTransform.position + offset;
        }
    }
}
