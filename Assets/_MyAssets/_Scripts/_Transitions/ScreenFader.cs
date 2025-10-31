using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
		await fadeGroup.DOFade(1f, 1.5f).AsyncWaitForCompletion();
    }

    public async UniTask FadeOut(float duration, CancellationToken token = default)
    {
		fadeText.DOFade(0f, 1.5f);
		await fadeGroup.DOFade(0f, 1.5f).AsyncWaitForCompletion();
        fadeGroup.blocksRaycasts = false;
        fadeText.gameObject.SetActive(false);
    }

    public async UniTask PerformFadeTransition(float fadeInTime, float waitTime, float fadeOutTime, string message = "", CancellationToken token = default, Action callBack = null)
    {
        await FadeIn(fadeInTime, token);
        ShowMessage(message);

		callBack?.Invoke();
        await UniTask.Delay(System.TimeSpan.FromSeconds(waitTime), cancellationToken: token);

        await FadeOut(fadeOutTime, token);
    }

	private float _textAnimationDuration = 1.6f;
	public async UniTask ShowMessage(string message)
	{
		if (string.IsNullOrEmpty(message) || fadeText == null)
			return;

		// Prepare text
		fadeText.text = message;
		fadeText.maxVisibleCharacters = message.Length; // full text visible
		fadeText.gameObject.SetActive(true);

		// Reset transform state
		fadeText.transform.localScale = Vector3.zero;

		// Kill any previous tweens that target this transform
		DOTween.Kill(fadeText.transform);

		// Timing split (tunable)
		float popDuration = Mathf.Max(0.02f, _textAnimationDuration * 0.55f);   // scale up
		float settleDuration = Mathf.Max(0.02f, _textAnimationDuration - popDuration); // settle to 1

		// Build sequence: quick pop (slightly overshoot) then settle with a small bounce
		Sequence seq = DOTween.Sequence();
		seq.SetTarget(fadeText.transform);

		// Pop to slightly bigger than final (gives snappy feel)
		seq.Append(fadeText.transform.DOScale(1.15f, popDuration).SetEase(Ease.OutBack));

		// Settle to final scale with a softer bounce
		seq.Append(fadeText.transform.DOScale(1f, settleDuration).SetEase(Ease.OutBounce));

		// Await completion
		await seq.AsyncWaitForCompletion();
	}
}