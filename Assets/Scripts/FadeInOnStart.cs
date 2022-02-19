using UnityEngine;

public class FadeInOnStart : MonoBehaviour
{
    public CanvasGroup canvasGroup = null!;

    public float time = 1f;

    private void Start()
    {
        StartCoroutine(new Fade().Begin(
            fadeTime: time,
            start: 0f,
            end: 1f,
            canvasGroup
        ));
    }
}