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

	private void Start()
	{
	}

	async void OnRecycleButtonClick() {
		GameObject heldObject = _playerManager.grabbedObject;

		if (_recyclingMinigameManager.GetIsWaste(heldObject))
		{
			heldObject.transform.SetParent(null, true);
			_playerManager.grabbedObject = null;	
			recycleBin.PlayWrongAnimation(heldObject);
		}
		else
		{
			heldObject.transform.SetParent(null, true);
			_playerManager.grabbedObject = null;
			recycleBin.PlayCorrectAnimation(heldObject);
		}

		ResetButtons();
	}

	async void ResetButtons()
	{
		_rightSideButtonsHandler.RecycleBinButton.onClick.RemoveAllListeners();
		_rightSideButtonsHandler.WasteBinButton.onClick.RemoveAllListeners();

		_rightSideButtonsHandler.ToggleWasteButton(false);
		_rightSideButtonsHandler.ToggleRecycleButton(false);

	}

	async void OnWasteButtonClick()
	{
		GameObject heldObject = _playerManager.grabbedObject;

		if (_recyclingMinigameManager.GetIsWaste(heldObject))
		{
			heldObject.transform.SetParent(null, true);
			_rightSideButtonsHandler.ReleaseButton.enabled = false;

			await wasteBin.PlayCorrectAnimation(heldObject);		

		}
		else
		{
			heldObject.transform.SetParent(null, true);
			_rightSideButtonsHandler.ReleaseButton.enabled = false;

			await wasteBin.PlayWrongAnimation(heldObject);

		}
		ResetButtons();
	}


	protected override void OnPlayerEnter()
	{
		if (_recyclingMinigameManager.IsTrash(_playerManager.grabbedObject) )
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

	protected override void OnPlayerExit() { 
	
		_rightSideButtonsHandler?.ToggleRecycleButton(false);
		_rightSideButtonsHandler.ToggleGrabButton(false);
	}

}
