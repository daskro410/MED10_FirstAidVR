using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PhoneSocketBehavior : MonoBehaviour
{
    GameManager gameManager;
    public CustomInteractionManager customInteractionManager; // Reference to the CustomInteractionManager script

    public PhoneInteractionBehavior phoneInteractionBehavior;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>(); // Find the CustomInteractionManager in the scene
        phoneInteractionBehavior = FindFirstObjectByType<PhoneInteractionBehavior>(); // Find the PhoneInteractionBehavior in the scene

        gameObject.SetActive(false); // Disable the phone socket at the start
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhoneSocketEnter(){
        phoneInteractionBehavior.SetPhoneInteractionState(PhoneInteractionState.OnGround);

        //gameManager.SetGameState(GameState.AmbulanceCalled); // Set the game state to AmbulanceCalled. Moved to breath to ensure interaction!
        
        

        // Phone is in socket now, so we are calling the ambulance
        customInteractionManager.SetInteractionState(InteractionState.HeartPumping);
    }

    public void OnPhoneSocketExit(){
        phoneInteractionBehavior.SetPhoneInteractionState(PhoneInteractionState.InHand);
    }
}
