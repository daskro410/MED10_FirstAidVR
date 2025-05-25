using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InteractionLogger : MonoBehaviour
{
    private Dictionary<InteractionState, float> timeSpentPerState = new Dictionary<InteractionState, float>();
    private InteractionState currentState;
    private float stateStartTime;

    private string logFileName;

    void Awake()
    {
        foreach (InteractionState state in System.Enum.GetValues(typeof(InteractionState)))
        {
            timeSpentPerState[state] = 0f;
        }

        logFileName = Application.persistentDataPath + "/InteractionLog_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
        //Debug.Log("Log file will be saved at: " + logFileName);
    }

    public void StartTrackingState(InteractionState newState)
    {
        if (currentState != newState)
        {
            // End timing for previous state
            float timeInState = Time.time - stateStartTime;
            timeSpentPerState[currentState] += timeInState;

            //Debug.Log($"Ended {currentState}, duration: {timeInState:F2} seconds");

            // Start timing new state
            currentState = newState;
            stateStartTime = Time.time;

            //Debug.Log($"Started {currentState}");
        }
    }

    public void StopTracking()
    {
        // Stop timing current state when exiting
        float timeInState = Time.time - stateStartTime;
        timeSpentPerState[currentState] += timeInState;

        //Debug.Log($"Final end of {currentState}, duration: {timeInState:F2} seconds");
    }

    public void SaveLogToFile()
    {
        using (StreamWriter writer = new StreamWriter(logFileName))
        {
            writer.WriteLine("Interaction State Log");
            writer.WriteLine("---------------------------");
            foreach (var entry in timeSpentPerState)
            {
                writer.WriteLine($"{entry.Key}: {entry.Value:F2} seconds");
            }
        }

        //Debug.Log("Interaction log saved to: " + logFileName);
    }

    void OnApplicationQuit()
    {
        SaveLogToFile();
    }
}