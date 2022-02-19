using System;
using System.Collections;
using UnityEngine;

public class Fade
{
    public IEnumerator Begin(float fadeTime, float start, float end, CanvasGroup canvasGroup, Action? callback = null)
    {
        var count = 0f;
        while (count < fadeTime)
        {
            count += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, count);

            yield return null;
        }

        callback?.Invoke();
    }
}