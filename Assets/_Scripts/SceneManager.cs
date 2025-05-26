using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public FadeBehavior fadeBehavior;

    void Start()
    {

    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        fadeBehavior.FadeOut();

        yield return new WaitForSeconds(fadeBehavior.defaultFadeDuration);

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName); 
    }
}
