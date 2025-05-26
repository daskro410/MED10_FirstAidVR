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
                HandleIntroductionState();
                break;
            case GameState.Interacting:
                HandleInteractingState();
                break;
            case GameState.AmbulanceCalled:
                HandleAmbulanceCalledState();
                break;
            case GameState.AmbulanceArrived:
                HandleAmbulanceArrivedState();
                break;
            case GameState.Finished:
                HandleFinishedState();
                break;
        }
        
    }

    public void HandleIntroductionState(){

    }

    public void HandleInteractingState(){

    }

    public void HandleAmbulanceCalledState(){
        Debug.Log("Ambulance called!");
        StartCoroutine(ambulanceMover.CallAmbulance(ambulanceArrivalTime));
        Debug.Log("Game State: " + gameState);
    }

    public void HandleAmbulanceArrivedState(){
        customInteractionManager.SetInteractionState(InteractionState.AmbulanceArrived);
        StartCoroutine(FadeToFinish());
    }

    public void HandleFinishedState(){

    }

    private IEnumerator FadeToFinish(){
        yield return new WaitForSeconds(fadeOutTime);
        FindFirstObjectByType<FadeBehavior>().FadeOut();
        
        ambulanceMover.GetComponent<AudioSource>().Stop();
    }
}
