using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.XR.CoreUtils;
using System.Collections.Generic;

[System.Serializable]
public class UICanvasWithTimer
{
    public GameObject canvas;
    public float switchInterval = 5f;
}

[System.Serializable]
public class UIStateGroup
{
    public InteractionState state;
    public UICanvasWithTimer[] canvases;
}

public class UIManager : MonoBehaviour
{
    public CustomInteractionManager customInteractionManager;
    AudioManager audioManager;

    //public GameObject[] canvases;
    private GameObject currentCanvas;

    public UIStateGroup[] uiStateGroups;
    private int currentCanvasIndex = 0;

    public GameObject miniHeartDisplayCanvas;

    private InteractionState currentState;

    public float uiTimer = 0f; // Delay before switching UI


    // HEART PUMPING UI
    public TextMeshProUGUI heartCounterText;
    public Image pulseCircle;
    public TextMeshProUGUI miniHeartCounterText;
    public Image miniPulseCircle;
    public float bpm = 110f;
    public float pulseScale = 1.2f;
    private float timer = 0f;
    private float interval;

    public TextMeshProUGUI breathCounterText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        customInteractionManager = FindFirstObjectByType<CustomInteractionManager>(); // Find the CustomInteractionManager in the scene
        audioManager = FindFirstObjectByType<AudioManager>(); // Find the AudioManager in the scene

        UpdateUI(customInteractionManager.interactionState);

        UpdateHeartCounterText();
        UpdateBreathCounterText();

        // Set the beats per min to beats per secs
        interval = 60f / bpm;
    }

    void Update()
    {
        UpdateUI(customInteractionManager.interactionState);

        if (customInteractionManager.interactionState == InteractionState.HeartPumping){
            UpdatePulseCircle();
        }

        currentState = customInteractionManager.interactionState;
        var group = Array.Find(uiStateGroups, g => g.state == currentState);
        
        if (group != null && group.canvases.Length > 1)
        {
            uiTimer += Time.deltaTime;
            var currentCanvasWithTimer = group.canvases[currentCanvasIndex];

            if (uiTimer >= currentCanvasWithTimer.switchInterval)
            {
                // Only switch if not at the last canvas
                if (currentCanvasIndex < group.canvases.Length - 1)
                {
                    // Hide current
                    currentCanvasWithTimer.canvas.SetActive(false);

                    // Move to next
                    currentCanvasIndex++;
                    uiTimer = 0f;

                    // Show next
                    var nextCanvas = group.canvases[currentCanvasIndex].canvas;
                    nextCanvas.SetActive(true);
                    currentCanvas = nextCanvas;
                    audioManager.Play("UI_Ping");
                }
                // Else: we stay on the last canvas and do nothing
}
        }
    }

    public void UpdateUI(InteractionState state)
    {
        if (currentState == state && currentCanvas != null)
            return; // No state change

        currentState = state;
        currentCanvasIndex = 0;
        uiTimer = 0f;

        // Disable all canvases
        foreach (var g in uiStateGroups)
        {
            if (g.canvases == null) continue;

            foreach (var c in g.canvases)
            {
                if (c.canvas != null)
                    c.canvas.SetActive(false);
            }
        }

        // Show the mini heart display if the state is HeartPumping
        miniHeartDisplayCanvas.SetActive(state == InteractionState.HeartPumping);


        var group = Array.Find(uiStateGroups, g => g.state == state);
        if (group == null || group.canvases.Length == 0)
        {
            Debug.LogWarning("No canvases found for state: " + state);
            return;
        }

        // Enable the first canvas
        currentCanvas = group.canvases[currentCanvasIndex].canvas;
        currentCanvas.SetActive(true);
        audioManager.Play("UI_Ping");
    }

    public void UpdateHeartCounterText(){
        string counterText = customInteractionManager.currentChestPressCounter.ToString() + " / " + customInteractionManager.maxChestPressCounter.ToString();

        heartCounterText.text = counterText;
        miniHeartCounterText.text = counterText;
    }

    public void UpdateBreathCounterText(){
        breathCounterText.text = Math.Round((decimal)customInteractionManager.currentMouthBreathTime, 1).ToString() + " / " + customInteractionManager.maxMouthBreathTime.ToString();
    }

    public void UpdatePulseCircle(){
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            
            // PLAY BEAT SOUND HERE
            audioManager.Play("UI_Heartbeat");
        }

        float t = timer / interval;
        // Use sine to ease in and out (smoother than PingPong/linear)
        float eased = Mathf.Sin(t * Mathf.PI); // Goes from 0 -> 1 -> 0
        float scale = Mathf.Lerp(1f, pulseScale, eased);
        pulseCircle.transform.localScale = new Vector3(scale, scale, 1f);
        miniPulseCircle.transform.localScale = new Vector3(scale, scale, 1f);
    }

    public bool IsCurrentUICycleComplete()
    {
        var group = Array.Find(uiStateGroups, g => g.state == currentState);
        if (group == null || group.canvases.Length == 0)
            return true; // No UI to cycle through

        // If we’re on the last canvas, and it's been shown for its full interval
        var currentCanvasWithTimer = group.canvases[currentCanvasIndex];
        return currentCanvasIndex == group.canvases.Length - 1 && uiTimer >= currentCanvasWithTimer.switchInterval;
    }
}
