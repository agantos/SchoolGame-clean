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

	void OnGrabClick()
	{
		Disable();

		_playerManager.GrabItem(grabbableObject);
		_rightSideButtonsHandler.ToggleGrabButton(false);
		_rightSideButtonsHandler.ToggleReleaseButton(true);
	}

	protected override void OnPlayerExit()
	{
		_rightSideButtonsHandler?.ToggleGrabButton(false);
	}
}
