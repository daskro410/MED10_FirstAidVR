using System.Collections;
using UnityEngine;

public enum GameState
{
    Introduction,
    Interacting,
    AmbulanceCalled,
    AmbulanceArrived,
    Finished
}

public class GameManager : MonoBehaviour
{
    CustomInteractionManager customInteractionManager; // Reference to the CustomInteractionManager script

    public GameObject bodyPrefab;

    public GameState gameState;
    private GameState previousState; // Store the previous game state

    public WaypointMover ambulanceMover; // Reference to the ambulance prefab

    public int ambulanceArrivalTime = 5; // Time in seconds for the ambulance to arrive

    public float fadeOutTime = 2f; // Time in seconds for the fade out effect

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>(); // Find the CustomInteractionManager in the scene

        SetGameState(GameState.Interacting);
        Debug.Log("Game State: " + gameState); // Log the initial game state

        bodyPrefab.GetComponent<Animator>().SetTrigger("TriggerLayingAnim");
        
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG: Check if the interaction state has changed
        if (gameState != previousState){
            SetGameState(gameState);
            previousState = gameState;
        }  
    }

    public void SetGameState(GameState state)
    {
        gameState = state; // Update the game state

        switch (state){
            case GameState.Introduction:
                HandleIntroductionState(); // Call the method to handle the Introduction state
                break;
            case GameState.Interacting:
                HandleInteractingState(); // Call the method to handle the Interacting state
                break;
            case GameState.AmbulanceCalled:
                HandleAmbulanceCalledState(); // Call the method to handle the AmbulanceCalled state
                break;
            case GameState.AmbulanceArrived:
                HandleAmbulanceArrivedState(); // Call the method to handle the AmbulanceArrived state
                break;
            case GameState.Finished:
                HandleFinishedState(); // Call the method to handle the Finished state
                break;
        }
        
    }

    public void HandleIntroductionState(){

    }

    public void HandleInteractingState(){

    }

    public void HandleAmbulanceCalledState(){
        Debug.Log("Ambulance called!"); // Log that the ambulance has been called
        StartCoroutine(ambulanceMover.CallAmbulance(ambulanceArrivalTime)); // Call the ambulance with the specified arrival time
        Debug.Log("Game State: " + gameState); // Log the current game state
    }

    public void HandleAmbulanceArrivedState(){
        customInteractionManager.SetInteractionState(InteractionState.AmbulanceArrived); // Set the interaction state to AmbulanceArrived
        StartCoroutine(FadeToFinish()); // Start the fade out effect
    }

    public void HandleFinishedState(){

    }

    private IEnumerator FadeToFinish(){
        yield return new WaitForSeconds(fadeOutTime); // Wait for 2 seconds before fading to finish
        FindFirstObjectByType<FadeBehavior>().FadeOut(); // Call the FadeToFinish method from the FadeBehavior script
        
        ambulanceMover.GetComponent<AudioSource>().Stop(); // Stop the ambulance sound
    }
}
