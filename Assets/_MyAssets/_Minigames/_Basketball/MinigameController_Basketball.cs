using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MinigameController_Basketball : MonoBehaviour
{
	public TextMeshProUGUI[] texts;

	[Header("Levels")]
	public Level level1;
	public Level level2;
	public Level level3;
	public Level level4;
	public Level level5;
	public Level level6;

	[Space]
	[Header("Sprites")]
	public ColorShapeSprite[] colorShape;
	public FeelingSprite[] feelingSprites;

	[Space]
	[Header("Shapes GameObjects")]
	public GameObject triangleGameObject;
	public GameObject squareGameObject;
	public GameObject circleGameObject;

	[Space]
	[Header("UI GameObjects")]
	public Button ShootButton;
	public GameObject HelpUI;

	private CinemachineCameraChanger _cameraChanger;
	private BasketBallShotAnimator _basketShotAnimator;
	private DialogueManager _dialogueManager;

	private Level _shownLevel;
	private Level _currentLevel;
	private int _currentLevelIndex;


	private void Awake()
	{
		_cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
		_basketShotAnimator = FindAnyObjectByType<BasketBallShotAnimator>();
		_dialogueManager = FindAnyObjectByType<DialogueManager>();
	}

	private async void Start()
	{
		HelpUI.gameObject.SetActive(false);
		ShootButton.gameObject.SetActive(false);
		ShootButton.onClick.AddListener(async () => { await TryShoot(); });
		
		Level[] levels = { level1, level2, level3, level4, level5, level6 };

		foreach (var level in levels)
		{
			level.ball.SetActive(false);
		}

		foreach(var text in texts)
		{
			text.text = "";
		}

		squareGameObject.SetActive(false);
		triangleGameObject.SetActive(false);
		circleGameObject.SetActive(false);

		await UniTask.Delay(1000);
		_ = StartGame();
	}

	#region Game Logic
	public async UniTask StartGame()
	{
		_dialogueManager.DialogueToStart = introductionDialogue;
		_dialogueManager.StartDialogue(true);

		await UniTask.Delay(15500);

		InitializeRandomLevels();
		_currentLevelIndex = 0;
		await LoadLevel(level1);
		StartSpawnLoop();
	}

	private CancellationTokenSource _spawnLoopCTS;

	public async UniTask StartSpawnLoop()
	{
		StopSpawnLoop(); // ensure no duplicate loops
		_spawnLoopCTS = new CancellationTokenSource();

		try
		{
			while (true)
			{
				_shownLevel = GenerateRandomAnswer();
				SetShapeVisuals(_shownLevel);

				await UniTask.Delay(1500, cancellationToken: _spawnLoopCTS.Token);
			}
		}
		catch (OperationCanceledException)
		{
			// Loop stopped safely
		}
	}

	public void StopSpawnLoop()
	{
		_spawnLoopCTS?.Cancel();
		_spawnLoopCTS?.Dispose();
		_spawnLoopCTS = null;
	}

	async UniTask TryShoot()
	{
		StopSpawnLoop();
		ShootButton.enabled = false;


		if (_currentLevel.CheckWith(_shownLevel))
		{
			// make correct shot
			await _basketShotAnimator.ShootBallInside();

			// change level
			squareGameObject.SetActive(false);
			triangleGameObject.SetActive(false);
			circleGameObject.SetActive(false);

			foreach (var t in texts)
			{
				t.text = "";
			}

			await LoadNextLevel();
		}
		else
		{
			// make wrong shot
			// start loop again
			await _basketShotAnimator.ShootBallMiss();
			_basketShotAnimator.ResetBall();

			StartSpawnLoop();
		}

		ShootButton.enabled = true;
	}

	async UniTask LoadNextLevel()
	{
		Level[] levels = { level1, level2, level3, level4, level5, level6 };
		if (_currentLevelIndex >= levels.Length)
		{
			return;
		}
		else
		{
			_currentLevelIndex++;
			await LoadLevel(levels[_currentLevelIndex]);
		}
	}

	async UniTask LoadLevel(Level level)
	{
		HelpUI.gameObject.SetActive(false);
		ShootButton.gameObject.SetActive(false);

		_currentLevel = level;
		await _cameraChanger.TransitionToCam(level.camera);

		level.ball.SetActive(true);
		_basketShotAnimator.ball = level.ball.transform;

		//Start ball animation

		// Write texts
		string[] levelAnswers = { level.correctColor.ToString(), level.correctFeelings.ToString(), level.correctShapes.ToString() };
		int answerIndex = 0;

		for (int i = 0; i < texts.Length; i++)
		{
			while (answerIndex < levelAnswers.Length && levelAnswers[answerIndex] == "none")
			{
				answerIndex++;
			}

			if (answerIndex < levelAnswers.Length)
			{
				texts[i].text = levelAnswers[answerIndex];
				answerIndex++;
			}
			else
			{
				texts[i].text = "";
			}
		}

		await PlayLevelDialogue(level);
		HelpUI.gameObject.SetActive(true);
		ShootButton.gameObject.SetActive(true);

		// Start spawining levels
		StartSpawnLoop();
	}

	void SetShapeVisuals(Level shapeToSpawn)
	{
		_shownLevel = shapeToSpawn;

		squareGameObject.SetActive(false);
		triangleGameObject.SetActive(false);
		circleGameObject.SetActive(false);

		Sprite shapeColorSprite = null;
		Sprite feelingsSprite = null;

		foreach (ColorShapeSprite s in colorShape)
		{
			if (s.color == shapeToSpawn.correctColor && s.shape == shapeToSpawn.correctShapes)
			{
				shapeColorSprite = s.sprite;
				break;
			}
		}

		foreach (FeelingSprite s in feelingSprites)
		{
			if (s.feeling == shapeToSpawn.correctFeelings)
			{
				feelingsSprite = s.sprite;
				break;
			}
		}

		GameObject objectToChange = null;

		switch (shapeToSpawn.correctShapes)
		{
			case Shapes.Square:
				objectToChange = squareGameObject;
				break;
			case Shapes.Circle:
				objectToChange = circleGameObject;
				break;
			case Shapes.Triangle:
				objectToChange = triangleGameObject;
				break;

		}

		objectToChange.GetComponent<Image>().sprite = shapeColorSprite;
		objectToChange.transform.GetChild(0).GetComponent<Image>().sprite = feelingsSprite;
		objectToChange.SetActive(true);
	}
	#endregion

	#region Animations

	#endregion

	#region Dialogue and Sounds

	[Space]
	[Header("Dialog & Sounds")]
	public AudioSource audioSource;
	public SCR_DialogueNode basketBallBaseDialogue;
	public SCR_DialogueNode introductionDialogue;
	public ColorSound[] colorSounds;
	public FeelingSound[] feelingSounds;
	public ShapeSound[] shapeSounds;

	public async UniTask PlayLevelDialogue(Level level)
	{
		string color = level.correctColor.ToString();
		string feeling = level.correctFeelings.ToString();
		string shape = level.correctShapes.ToString();

		int secondsToWait = 1000;

		string dialogueText = "Shoot when you spot any ";

		List<AudioClip>	clipsToPlay = new List<AudioClip>();

		if(color != "none")
		{
			dialogueText += color;
			dialogueText += " ";
			clipsToPlay.Add(colorSounds.FirstOrDefault(f => f.color == level.correctColor).sound);
			secondsToWait += 1000;
		}

		if(feeling != "none")
		{
			dialogueText += feeling;
			dialogueText += " ";
			clipsToPlay.Add(feelingSounds.FirstOrDefault(f => f.feeling == level.correctFeelings).sound);

			secondsToWait += 1000;
		}

		if (shape != "none")
		{
			dialogueText += shape;
			dialogueText += " ";
			clipsToPlay.Add(shapeSounds.FirstOrDefault(f => f.shape == level.correctShapes).sound);

			secondsToWait += 1000;
		}

		dialogueText += "shape";

		SCR_DialogueNode runtimeNode = Instantiate(basketBallBaseDialogue);
		runtimeNode.steps[0].stepText = dialogueText;
		_dialogueManager.DialogueToStart = runtimeNode;
		_dialogueManager.StartDialogue(isUnskippable: true);
		await UniTask.Delay(500);

		// PlaySounds
		foreach (var clip in clipsToPlay)
		{
			audioSource.clip = clip;
			audioSource.Play();

			await UniTask.Delay((int)(clip.length * 1000));
			await UniTask.Delay(500);
		}

		_dialogueManager.CloseDialogue();		
	}	

	#endregion

	#region GetRandom
	List<int> pool = new List<int>();

	void CreateRandomPool(int maxNumber)
	{
		for (int i = 0; i < maxNumber; i++)
			pool.Add(i);
	}

	int GetUniqueRandom()
	{
		if (pool.Count == 0)
		{
			Debug.Log("No more numbers left!");
			return -1;
		}

		int index = UnityEngine.Random.Range(0, pool.Count);
		int value = pool[index];
		pool.RemoveAt(index);
		return value;
	}
	#endregion

	#region Level class and Management
	[Serializable]
	public class Level
	{
		public Colors correctColor;
		public Shapes correctShapes;
		public Feelings correctFeelings;
		public GameObject ball;
		public CinemachineCamera camera;

		public bool CheckWith(Level level)
		{
			bool colorCheck = (correctColor == Colors.none || level.correctColor == correctColor);
			bool shapesCheck = (correctShapes == Shapes.none || level.correctShapes == correctShapes);
			bool feelingsCheck = (correctFeelings == Feelings.none || level.correctFeelings == correctFeelings);

			return colorCheck && shapesCheck && feelingsCheck;
		}
	}

	public void InitializeRandomLevels()
	{
		CreateRandomPool(3);

		Level[] firstLevels = { level1, level2, level3 };

		for (int i = 0; i < 3; i++)
		{
			int level1Index = GetUniqueRandom();
			if (level1Index == 0)
			{
				firstLevels[i].correctColor = GetRandomEnumValue<Colors>();
				firstLevels[i].correctFeelings = Feelings.none;
				firstLevels[i].correctShapes = Shapes.none;
			}
			if (level1Index == 1)
			{
				firstLevels[i].correctFeelings = GetRandomEnumValue<Feelings>();
				firstLevels[i].correctColor = Colors.none;
				firstLevels[i].correctShapes = Shapes.none;
			}
			if (level1Index == 2)
			{
				firstLevels[i].correctShapes = GetRandomEnumValue<Shapes>();
				firstLevels[i].correctFeelings = Feelings.none;
				firstLevels[i].correctColor = Colors.none;
			}
		}

		level4.correctColor = GetRandomEnumValue<Colors>();
		level4.correctShapes = GetRandomEnumValue<Shapes>();
		level4.correctFeelings = Feelings.none;

		level5.correctColor = GetRandomEnumValue<Colors>();
		level5.correctFeelings = GetRandomEnumValue<Feelings>();
		level5.correctShapes = Shapes.none;


		level6.correctColor = GetRandomEnumValue<Colors>();
		level6.correctShapes = GetRandomEnumValue<Shapes>();
		level6.correctFeelings = GetRandomEnumValue<Feelings>();
	}

	public Level GenerateRandomAnswer()
	{
		Level asnwer = new Level();
		asnwer.correctColor = GetRandomEnumValue<Colors>();
		asnwer.correctShapes = GetRandomEnumValue<Shapes>();
		asnwer.correctFeelings = GetRandomEnumValue<Feelings>();

		return asnwer;
	}
	#endregion

	#region Enums
	public enum Colors
	{
		Yellow,
		Green,
		Blue,
		Purple,
		Red,
		none
	}

	public enum Shapes
	{
		Square,
		Circle,
		Triangle,
		none
	}

	public enum Feelings
	{
		Sad,
		Happy,
		Angry,
		none
	}

	[System.Serializable]
	public class ColorShapeSprite
	{
		public Colors color;
		public Shapes shape;
		public Sprite sprite;
	}

	[System.Serializable]
	public class FeelingSprite
	{
		public Feelings feeling;
		public Sprite sprite;
	}

	[System.Serializable]
	public class FeelingSound
	{
		public Feelings feeling;
		public AudioClip sound;
	}

	[System.Serializable]
	public class ShapeSound
	{
		public Shapes shape;
		public AudioClip sound;
	}

	[System.Serializable]
	public class ColorSound
	{
		public Colors color;
		public AudioClip sound;

	}

	public static T GetRandomEnumValue<T>() where T : Enum
	{
		// Get all enum values
		T[] values = (T[])Enum.GetValues(typeof(T));

		// Filter out "none"
		var filtered = Array.FindAll(values, v => !v.ToString().Equals("none", StringComparison.OrdinalIgnoreCase));

		// Pick a random one
		int index = UnityEngine.Random.Range(0, filtered.Length);
		return filtered[index];
	}
	#endregion


}
