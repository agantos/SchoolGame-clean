using UnityEngine;

public class BinArea : TriggerArea
{
	RightSideButtonsHandler _rightSideButtonsHandler;
	PlayerManager _playerManager;
	RecyclingMinigameManager _recyclingMinigameManager;

	public BinAnimation wasteBin;
	public BinAnimation recycleBin;

	private void Awake()
	{
		_rightSideButtonsHandler = FindAnyObjectByType<RightSideButtonsHandler>();
		_playerManager = FindAnyObjectByType<PlayerManager>();	
		_recyclingMinigameManager = FindAnyObjectByType<RecyclingMinigameManager>();
	}

	async void CorrectObjectThrown(BinAnimation bin)
	{
		GameObject heldObject = _playerManager.grabbedObject;

		heldObject.transform.SetParent(null, true);
		_playerManager.grabbedObject = null;
		_rightSideButtonsHandler.ToggleReleaseButton(false);

		await bin.PlayCorrectAnimation(heldObject);
		_recyclingMinigameManager.CompleteTrash();

		//Reset grabbed Item
		GrabItemArea grabArea = _playerManager.grabbedObjectArea;
		grabArea.Disable();
	}

	async void WrongObjectThrown(BinAnimation bin)
	{
		GameObject heldObject = _playerManager.grabbedObject;

		heldObject.transform.SetParent(null, true);
		_rightSideButtonsHandler.ToggleReleaseButton(false);

		await bin.PlayWrongAnimation(heldObject);

		//Reset grabbed Item
		GrabItemArea area = _playerManager.grabbedObjectArea;
		area.OnReleaseClick();
	}

	async void OnRecycleButtonClick()
	{
		ResetBinButtons();

		GameObject heldObject = _playerManager.grabbedObject;

		if (_recyclingMinigameManager.GetIsWaste(heldObject))
		{
			WrongObjectThrown(recycleBin);
		}
		else
		{
			CorrectObjectThrown(recycleBin);
		}

		Disable();
	}

	async void OnWasteButtonClick()
	{
		ResetBinButtons();

		GameObject heldObject = _playerManager.grabbedObject;

		if (_recyclingMinigameManager.GetIsWaste(heldObject))
		{
			CorrectObjectThrown(wasteBin);
		}
		else
		{
			WrongObjectThrown(wasteBin);
		}

		Disable();
	}

	async void ResetBinButtons()
	{
		_rightSideButtonsHandler.RecycleBinButton.onClick.RemoveAllListeners();
		_rightSideButtonsHandler.WasteBinButton.onClick.RemoveAllListeners();

		_rightSideButtonsHandler.ToggleWasteButton(false);
		_rightSideButtonsHandler.ToggleRecycleButton(false);
	}


	public void Disable()
	{
		OnPlayerExit();
		base.Disable();
	}

	protected override void OnPlayerEnter()
	{
		if (_playerManager.grabbedObject != null) {
			if (_recyclingMinigameManager.IsTrash(_playerManager.grabbedObject))
			{
				_rightSideButtonsHandler.ToggleRecycleButton(true);
				_rightSideButtonsHandler.ToggleWasteButton(true);

				_rightSideButtonsHandler.RecycleBinButton.onClick.AddListener(() =>
				{
					OnRecycleButtonClick();
					_rightSideButtonsHandler.RecycleBinButton.onClick.RemoveAllListeners();
				});

				_rightSideButtonsHandler.WasteBinButton.onClick.AddListener(() =>
				{
					OnWasteButtonClick();
					_rightSideButtonsHandler.WasteBinButton.onClick.RemoveAllListeners();
				});

			}
		}

	}

	protected override void OnPlayerExit() {

		ResetBinButtons();
	}

}
