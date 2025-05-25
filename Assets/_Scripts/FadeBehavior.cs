using System.Collections;
using UnityEngine;

public class FadeBehavior : MonoBehaviour
{
    public float defaultFadeDuration = 2f;

    private Material fadeMaterial;
    private Coroutine currentFade;
    private Renderer rend;

    public bool fadeOnStart = true;

    void Start()
    {
        rend = GetComponent<Renderer>();
        fadeMaterial = rend.material;

        if (fadeOnStart){
            FadeIn();
        }
    }

    public void FadeOut(float duration = -1f)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(Fade(0f, 1f, duration < 0 ? defaultFadeDuration : duration));
    }

    public void FadeIn(float duration = -1f)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(Fade(1f, 0f, duration < 0 ? defaultFadeDuration : duration));
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float time = 0f;
        Color color = fadeMaterial.color;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(from, to, time / duration);
            rend.material.color = new Color(color.r, color.g, color.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        rend.material.color = new Color(color.r, color.g, color.b, to);
    }
}
