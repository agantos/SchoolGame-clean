using Cysharp.Threading.Tasks;
using UnityEngine;

public class GrabItemArea : TriggerArea
{
	private RightSideButtonsHandler _rightSideButtonsHandler;
	private PlayerManager _playerManager;
	private BinArea _recycleGameBinArea;
	public GameObject grabbableObject;

	private void Awake()
	{
		_playerManager = FindAnyObjectByType<PlayerManager>();
		_rightSideButtonsHandler = FindAnyObjectByType<RightSideButtonsHandler>();
		_recycleGameBinArea = FindAnyObjectByType<BinArea>();

	}
	protected override void OnObjectEnter(GameObject obj = null)
	{
		if(_playerManager.grabbedObject == null)
		{
			_rightSideButtonsHandler?.ToggleGrabButton(true);
			_rightSideButtonsHandler.GrabButton.onClick.RemoveAllListeners();

			_rightSideButtonsHandler.GrabButton.onClick.AddListener(OnGrabClick);
		}		
	}

	async void OnGrabClick()
	{
		Disable();

		_playerManager.GrabItem(grabbableObject, this);
		_rightSideButtonsHandler.ToggleGrabButton(false);

		await UniTask.Delay(300);
		_rightSideButtonsHandler.ToggleReleaseButton(true);
		_rightSideButtonsHandler.ReleaseButton.onClick.AddListener(OnReleaseClick);
		await UniTask.Delay(300);
		_recycleGameBinArea?.Enable();
	}

	public async void OnReleaseClick()
	{
		_playerManager?.ReleaseItem(grabbableObject);
		_rightSideButtonsHandler.ReleaseButton.gameObject.SetActive(false);
		_rightSideButtonsHandler.ReleaseButton.onClick.RemoveAllListeners();


		await UniTask.Delay(500);
		transform.position = grabbableObject.transform.position;
		Enable();

		_recycleGameBinArea?.Disable();
	}

	protected override void OnObjectExit(GameObject obj = null)
	{
		_rightSideButtonsHandler?.ToggleGrabButton(false);
	}
}
