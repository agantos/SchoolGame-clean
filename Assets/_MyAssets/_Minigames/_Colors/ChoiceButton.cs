using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
	Button _button;
	Image _selectedView;

	[SerializeField] Sprite correctSprite;
	[SerializeField] Sprite wrongSprite;

	public Func<UniTask> callback;

	public bool isCorrect;

	[SerializeField] AudioSource audioSource;

	public Func<UniTask> onGameCompleted;


	private void Awake()
	{
		_button = GetComponent<Button>();
		_button.onClick.AddListener(onClick);

		isCorrect = false;
		_selectedView = GetComponentInChildren<Image>();
		_selectedView.GetComponent<CanvasGroup>().alpha = 0f;
	}

	public void Initialize(bool isCorrect, Func<UniTask> callback = null)
	{
		if (callback != null)
		{
			this.callback = callback;
		}

		this.isCorrect = isCorrect;

		if (isCorrect)
		{
			_selectedView.sprite = correctSprite;
		}
		else
		{
			_selectedView.sprite= wrongSprite;
		}
	}

	async void onClick()
	{
		var canvasGroup = _selectedView.GetComponent<CanvasGroup>();
		
		callback.Invoke();

		if (isCorrect)
		{

			await canvasGroup.DOFade(1, 0.2f).AsyncWaitForCompletion();
			await canvasGroup.DOFade(0.75f, 0.2f).AsyncWaitForCompletion();
		}
		else
		{
			await canvasGroup.DOFade(1, 0.2f).AsyncWaitForCompletion();
			await canvasGroup.DOFade(0f, 0.2f).AsyncWaitForCompletion();
		}

	}
}
