using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class ScreenFader : MonoBehaviour
{
    public CanvasGroup fadeGroup;      // Reference to the black panel
    public TextMeshProUGUI fadeText;   // Optional: text to show

    private void Awake()
    {
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;

        if (fadeText != null)
            fadeText.gameObject.SetActive(false);
    }

    public async UniTask FadeIn(float duration, CancellationToken token = default)
    {
        fadeGroup.blocksRaycasts = true;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.SmoothStep(0f, 1f, t / duration);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        fadeGroup.alpha = 1f;
    }

    public async UniTask FadeOut(float duration, CancellationToken token = default)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.SmoothStep(1f, 0f, t / duration);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
    }

    public async UniTask FadeTransition(float fadeInTime, float waitTime, float fadeOutTime, string message = "", CancellationToken token = default, Action callBack = null)
    {
        await FadeIn(fadeInTime, token);

        if (!string.IsNullOrEmpty(message) && fadeText != null)
        {
            fadeText.text = message;
            fadeText.gameObject.SetActive(true);
        }

        callBack?.Invoke();
        await UniTask.Delay(System.TimeSpan.FromSeconds(waitTime), cancellationToken: token);

        if (fadeText != null)
            fadeText.gameObject.SetActive(false);

        await FadeOut(fadeOutTime, token);
    }
}