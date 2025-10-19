using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MemoryMinigameManager : MonoBehaviour
{

	CinemachineCameraChanger _cameraChanger;
	public GameObject Clicks;
	Button[] _buttons;

	MemoryGameLevel _currentLevel;
	int _currentLevelIndex;



	[Space]
	[Space]
	[Header("Level 1")]
	public MemoryGameLevel level1;

	[Space]
	[Space]
	[Header("Level 2")]
	[Space]

	public MemoryGameLevel level2;
	[Space]
	[Header("Level 3")]
	[Space]
	
	public MemoryGameLevel level3;

	private async void Awake()
	{
		_cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
		_buttons = Clicks.GetComponentsInChildren<Button>();
		ToggleButtons(true);

		await UniTask.Delay(2000);
		LoadLevel_Index(0);
	}

	public async UniTask LoadLevel(MemoryGameLevel level) {
		ToggleButtons(false);
		await _cameraChanger.TransitionToCam(level.farCamera);
		level.InitializeLevel_Random();		

		await UniTask.Delay(1000);
		await PlayIntroduction(level);
	}

	void ResetButtons()
	{
		foreach (Button button in _buttons) { 
			button.onClick.RemoveAllListeners();
		}
	}

	void ToggleButtons(bool b)
	{
		foreach (Button button in _buttons)
		{
			button.gameObject.SetActive(b);
		}
	}

	public async void LoadLevel_Index(int levelIndex)
	{
		MemoryGameLevel[] levels = {level1, level2, level3};

		_currentLevel = levels[levelIndex];
		_currentLevelIndex = levelIndex;

		await LoadLevel(_currentLevel);
	}

	public async void LoadNextLevel()
	{
		MemoryGameLevel[] levels = { level1, level2, level3 };

		if (_currentLevelIndex < levels.Length - 1)
		{
			_currentLevelIndex++;
			LoadLevel_Index(_currentLevelIndex);
		}
		else
		{
			_cameraChanger.TransitionBackToPlayerCamera();
			await UniTask.Delay(3000);
			gameObject.SetActive(false);
		}

	}

	#region Animations
	async UniTask PlayItemIntroduction(CabinetContentsAnimations_Base item)
	{
		item.gameObject.SetActive(true);

		await _cameraChanger.TransitionToCam(item.viewCamera);
		await item.PlayIntroductionAnimation();
	}

	async UniTask PlayItemCorrect(CabinetContentsAnimations_Base item)
	{
		item.gameObject.SetActive(true);

		item.OpenCabinetDoor();
		await _cameraChanger.TransitionToCam(item.viewCamera);
		await item.PlayCorrectAnimation();
		await UniTask.Delay(500);

		_cameraChanger.TransitionToCam(_currentLevel.farCamera);
		await UniTask.Delay(500);
		await item.CloseCabinetDoor();
	}

	async UniTask PlayItemCorrectWrong(CabinetContentsAnimations_Base item)
	{
		item.gameObject.SetActive(true);

		item.OpenCabinetDoor();
		await _cameraChanger.TransitionToCam(item.viewCamera);
		await item.PlayWrongAnimation();
		await UniTask.Delay(500);

		_cameraChanger.TransitionToCam(_currentLevel.farCamera);
		await UniTask.Delay(500);
		await item.CloseCabinetDoor();
	}

	#endregion

	#region
	#endregion

	#region
	#endregion

	async UniTask PlayIntroductionShort(MemoryGameLevel level)
	{
		ToggleButtons(false);

		// Text Animation 
		level.roundText.gameObject.SetActive(true);
		level.roundText.transform.localScale = Vector3.zero;
		await level.roundText.transform
			.DOScale(Vector3.one, 0.8f) // final size, duration
			.SetEase(Ease.OutBack, 2.5f) // back ease = cartoon bounce
			.SetDelay(0.1f).AsyncWaitForCompletion(); // optional small delay for timing polish

		await UniTask.Delay(100);

		await level.roundText.transform
		.DOScale(Vector3.zero, 0.8f) // final size, duration
		.SetEase(Ease.OutBack, 2.5f) // back ease = cartoon bounce
		.SetDelay(0.1f).AsyncWaitForCompletion(); // optional small delay for timing polish


		foreach (var c in level.initializedItems)
		{
			c.item.cabinetDoor.SetActive(false);
		}

		await UniTask.Delay(3000);


		foreach (var c in level.initializedItems)
		{
			c.item.cabinetDoor.SetActive(true);
		}

		ToggleButtons(true);
	}

	async UniTask PlayIntroduction(MemoryGameLevel level)
	{
		ToggleButtons(false);

		// Text Animation 
		level.roundText.gameObject.SetActive(true);
		level.roundText.transform.localScale = Vector3.zero;
		await level.roundText.transform
			.DOScale(Vector3.one, 0.8f) // final size, duration
			.SetEase(Ease.OutBack, 2.5f) // back ease = cartoon bounce
			.SetDelay(0.1f).AsyncWaitForCompletion(); // optional small delay for timing polish

		await UniTask.Delay(100);

		await level.roundText.transform
		.DOScale(Vector3.zero, 0.8f) // final size, duration
		.SetEase(Ease.OutBack, 2.5f) // back ease = cartoon bounce
		.SetDelay(0.1f).AsyncWaitForCompletion(); // optional small delay for timing polish


		// Open Doors
		foreach (var c in level.initializedItems)
		{
			c.item.OpenCabinetDoor();
			await UniTask.Delay(300);
		}

		await UniTask.Delay(3000);

		foreach (var c in level.initializedItems)
		{
			c.item.cabinetDoor.SetActive(false);
		}

		foreach (var cabinet in level._cabinetPositions)
		{
			foreach(var c in level.initializedItems)
			{
				if(cabinet == c.cabinetPosition)
				{
					await PlayItemIntroduction(c.item);
				}
			}
		}
		
		await _cameraChanger.TransitionToCam(level.farCamera);
		await UniTask.Delay(2000);

		foreach (var c in level.initializedItems)
		{
			c.item.cabinetDoor.SetActive(true);
		}

		foreach (var c in level.initializedItems)
		{
			c.item.CloseCabinetDoor();
			await UniTask.Delay(300);
		}

		await UniTask.Delay(600);
		ToggleButtons(true);
	}



	async UniTask OnClickCabinet(CabinetContentsAnimations_Base item, bool isCorrect, MemoryGame_CabinetPosition cabinet)
	{
		ToggleButtons(false);

		if (isCorrect)
		{
			await item.PrepareForCorrect();
			await PlayItemCorrect(item);
		}
		else {
			await item.PrepareForWrong();
			await PlayItemCorrectWrong(item);
		}
		await UniTask.Delay(500);
		ToggleButtons(true);

		item.gameObject.SetActive(false);

		item.Cleanup();
		cabinet.clickButton.gameObject.SetActive(false);

	}

	[Serializable]
	public class MemoryGameLevel
	{
		public CabinetContentsAnimations_Base[] items;
		public GameObject CabinetPositionParent;
		public bool[] correctItemIndexes;
		public CinemachineCamera farCamera;
		public TextMeshProUGUI roundText;

		public MemoryGame_CabinetPosition[] _cabinetPositions;
		MemoryMinigameManager _minigameManager;

		public List<MemoryGame_InitializedItem> initializedItems = new List<MemoryGame_InitializedItem>();

		int _correctAnswersTotal = 0;
		int _correctAnswersFound = 0;

		public void InitializeLevel_Random()
		{
			_minigameManager = FindAnyObjectByType<MemoryMinigameManager>();
			_cabinetPositions = CabinetPositionParent.GetComponentsInChildren<MemoryGame_CabinetPosition>();

			foreach (var correct in correctItemIndexes) { 
				if(correct) _correctAnswersTotal++;
			}

			// Safety checks
			if (items == null || correctItemIndexes == null || _cabinetPositions == null)
			{
				Debug.LogError("MemoryGameLevel: Missing setup data!");
				return;
			}
			if (items.Length != correctItemIndexes.Length)
			{
				Debug.LogError("MemoryGameLevel: items and correctItemIndexes must have the same length!");
				return;
			}
			if (_cabinetPositions.Length < items.Length)
			{
				Debug.LogWarning("MemoryGameLevel: Not enough cabinet positions for all items!");
			}

			// Clear previous data
			initializedItems.Clear();

			// Create a list of indexes and shuffle them for random placement
			List<int> randomIndices = new List<int>();
			for (int i = 0; i < _cabinetPositions.Length; i++)
				randomIndices.Add(i);

			// Fisher–Yates shuffle
			for (int i = randomIndices.Count - 1; i > 0; i--)
			{
				int j = UnityEngine.Random.Range(0, i + 1);
				(randomIndices[i], randomIndices[j]) = (randomIndices[j], randomIndices[i]);
			}

			// Assign items randomly to positions
			for (int i = 0; i < items.Length; i++)
			{
				var initItem = new MemoryGame_InitializedItem
				{
					isCorrect = correctItemIndexes[i],
					item = items[i],
					cabinetPosition = _cabinetPositions[randomIndices[i]],
				};

				initItem.item.viewCamera = initItem.cabinetPosition.closeCamera;
				initItem.item.cabinetDoor = initItem.cabinetPosition.door;
				initItem.item.transformParent.SetParent(initItem.cabinetPosition.objectPosition);
				initItem.item.transformParent.localPosition = Vector3.zero;
				initItem.item.PrepareIntroduction();

				initializedItems.Add(initItem);

				// init button
				initItem.cabinetPosition.clickButton.onClick.AddListener(() =>
				{
					// Fire the async handler without holding up Unity events
					_ = HandleCabinetClick(initItem);
				});
			}

		}

		private async UniTask HandleCabinetClick(MemoryGame_InitializedItem initItem)
		{
			await _minigameManager.OnClickCabinet(initItem.item, initItem.isCorrect, initItem.cabinetPosition);

			if (initItem.isCorrect) _correctAnswersFound++;

			if (_correctAnswersFound == _correctAnswersTotal)
			{
				_minigameManager.ToggleButtons(false);
				_minigameManager.ResetButtons();
				await UniTask.Delay(2000);
				_minigameManager.LoadNextLevel();
			}
		}

		public struct MemoryGame_InitializedItem
		{
			public bool isCorrect;
			public CabinetContentsAnimations_Base item;
			public MemoryGame_CabinetPosition cabinetPosition;
		}
	}


}
