using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public FadeBehavior fadeBehavior; // Reference to the FadeBehavior script

    void Start()
    {

    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName)); // Start the coroutine to load the scene
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        fadeBehavior.FadeOut();

        yield return new WaitForSeconds(fadeBehavior.defaultFadeDuration); // Wait for the fade duration before loading the scene

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName); // Load the new scene
    }
}
