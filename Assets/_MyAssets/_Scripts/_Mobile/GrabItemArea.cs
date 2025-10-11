using Cysharp.Threading.Tasks;
using UnityEngine;

public class GrabItemArea : TriggerArea
{
	private RightSideButtonsHandler _rightSideButtonsHandler;
	private PlayerManager _playerManager;
	public GameObject grabbableObject;

	private void Awake()
	{
		_playerManager = FindAnyObjectByType<PlayerManager>();
		_rightSideButtonsHandler = FindAnyObjectByType<RightSideButtonsHandler>();
	}
	protected override void OnPlayerEnter()
	{
		_rightSideButtonsHandler?.ToggleGrabButton(true);
		_rightSideButtonsHandler.GrabButton.onClick.RemoveAllListeners();

		_rightSideButtonsHandler.GrabButton.onClick.AddListener(OnGrabClick);
	}

	async void OnGrabClick()
	{
		Disable();

		_playerManager.GrabItem(grabbableObject);
		_rightSideButtonsHandler.ToggleGrabButton(false);

		await UniTask.Delay(300);
		_rightSideButtonsHandler.ToggleReleaseButton(true);
		_rightSideButtonsHandler.ReleaseButton.onClick.AddListener(OnReleaseClick);
	}

	async void OnReleaseClick()
	{
		_playerManager?.ReleaseItem(grabbableObject);
		_rightSideButtonsHandler.ReleaseButton.gameObject.SetActive(false);
		_rightSideButtonsHandler.ReleaseButton.onClick.RemoveAllListeners();


		await UniTask.Delay(500);
		transform.position = grabbableObject.transform.position;
		Enable();
	}

	protected override void OnPlayerExit()
	{
		_rightSideButtonsHandler?.ToggleGrabButton(false);
	}
}
