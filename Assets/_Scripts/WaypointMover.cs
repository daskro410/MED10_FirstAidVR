using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    GameManager gameManager; // Reference to the GameManager script

    [SerializeField] private Route route; // Reference to the Route script
    [SerializeField] private float speed = 5f; // Speed of the object
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation towards the waypoint

    public bool canMove = false;

    private Transform currentWaypoint; // Index of the current waypoint
    private Transform targetWaypoint; // Current target waypoint

    public float distanceThreshold = 0.1f; // Distance threshold to consider the waypoint reached

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>(); // Find the GameManager in the scene

        if (route == null || route.points.Length == 0)
        {
            Debug.LogError("Route is not assigned or has no waypoints.");
            return;
        }

        currentWaypoint = route.GetNextWaypoint(currentWaypoint); // Get the first waypoint
        transform.position = currentWaypoint.position; // Set the initial position to the first waypoint

        currentWaypoint = route.GetNextWaypoint(currentWaypoint); // Get the next waypoint
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove){
            MoveTowardsPoint();
            RotateTowardsPoint();
        }
    }

    public IEnumerator CallAmbulance(int arrivalTime)
    {
        yield return new WaitForSeconds(arrivalTime); // Wait for the specified arrival time

        GetComponent<AudioSource>().Play(); // Play the car sound

        canMove = true; // Allow the ambulance to move after the wait

        currentWaypoint = route.GetNextWaypoint(currentWaypoint); // Get the next waypoint after the wait
        
        if (currentWaypoint == null)
        {
            Debug.Log("Reached the end of the route.");
            yield break; // Stop moving if there are no more waypoints
        }

    }

    private void MoveTowardsPoint(){
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, speed * Time.deltaTime); // Move towards the target waypoint

        if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
        {
            currentWaypoint = route.GetNextWaypoint(currentWaypoint); // Get the next waypoint

            if (currentWaypoint == null)
            {
                Debug.Log("Reached the end of the route.");

                canMove = false; // Stop moving if there are no more waypoints
                //GetComponent<AudioSource>().Stop(); // Stop the car sound
                gameManager.SetGameState(GameState.AmbulanceArrived);

                return; // Stop moving if there are no more waypoints
            }
        }
    }

    private void RotateTowardsPoint()
    {
        if (currentWaypoint != null)
        {
            Vector3 direction = currentWaypoint.position - transform.position; // Calculate the direction to the waypoint
            Quaternion targetRotation = Quaternion.LookRotation(direction); // Calculate the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // Smoothly rotate towards the waypoint
        }
    }
}
