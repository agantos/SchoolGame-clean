using UnityEngine;

public class _GrabItemArea : TriggerArea
{
	private RightSideButtonsHandler _rightSideButtonsHandler;
	private PlayerManager _playerManager;
	private GameObject grabbableObject;

	private void Awake()
	{
		_playerManager = FindAnyObjectByType<PlayerManager>();
		_rightSideButtonsHandler = FindAnyObjectByType<RightSideButtonsHandler>();
	}
	protected override void OnPlayerEnter()
	{
		_rightSideButtonsHandler?.ToggleGrabButton(true);
		_rightSideButtonsHandler.GrabButton.onClick.RemoveAllListeners();
	}

	void OnGrabClick()
	{
		Disable();

		_playerManager.GrabItem(grabbableObject);
		_rightSideButtonsHandler.ToggleGrabButton(false);
		_rightSideButtonsHandler.ToggleReleaseButton(true);

		// Set Up releaseButton
		_rightSideButtonsHandler.ReleaseButton.onClick.RemoveAllListeners();
		
		_rightSideButtonsHandler.ReleaseButton.onClick.AddListener(() =>
		{

		});
	}

	protected override void OnPlayerExit()
	{
		_rightSideButtonsHandler?.ToggleGrabButton(false);
	}
}
