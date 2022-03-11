using Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public SceneNames sceneName = SceneNames.Core;

    public CanvasGroup menuCanvas = null!;

    public float fadeOutTime = 1f;

    private bool isBegin;

    public void Begin()
    {
        if (isBegin) return;
        isBegin = true;

        StartCoroutine(new Fade().Begin(
            fadeTime: fadeOutTime,
            start: 1f,
            end: 0f,
            menuCanvas,
            () => { SceneManager.LoadScene(sceneName.ToString()); }
        ));
    }
}