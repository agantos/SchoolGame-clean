using Cysharp.Threading.Tasks;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
	[Header("Cameras")]
	[Space]
	[Space]
	public CinemachineCamera originalCamera;
	public CinemachineCamera waterCamera;
	public CinemachineCamera towelCamera;
	public CinemachineCamera soapCamera;
	public CinemachineCamera binCamera;

	[Header("Buttons")]
	[Space]
	[Space]
	public Button dryButton;
	public Button wetHandsButton;
	public Button waterButton_On;
	public Button waterButton_Off;
	public Button washHandsButton;
	public Button soapButton;

	[Header("Dialogues")]
	[Space]
	[Space]
	public SCR_DialogueNode HandsNoWashedYet;
	public SCR_DialogueNode HandsNotWet;
	public SCR_DialogueNode NoSoap;
	public SCR_DialogueNode ShouldRubHands;
	public SCR_DialogueNode ShouldTurnWaterOff;
	public SCR_DialogueNode WaterIsNotOn;

	
	private ToiletAnimations_1 _toiletAnimations_1;
	private ToiletAnimations_2 _toiletAnimations_2;
	private CinemachineCameraChanger _cameraChanger;
	private DialogueManager _dialogueManager;

	private bool _handsSoaped;
	private bool _handsAreWet;
	private bool _waterIsOn;
	private bool _handsReadyForDry;
	private bool _rubbedHands;


	private void Awake()
	{
		_toiletAnimations_1 = FindAnyObjectByType<ToiletAnimations_1>();
		_toiletAnimations_2 = FindAnyObjectByType<ToiletAnimations_2>();
		_cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
		_dialogueManager = FindAnyObjectByType<DialogueManager>();

		// Register button listeners
		dryButton.onClick.AddListener(() => RunButtonRoutine(PlayDryHandsAnimation));
		wetHandsButton.onClick.AddListener(() => RunButtonRoutine(PlayWetHandsAnimation));
		waterButton_On.onClick.AddListener(() => RunButtonRoutine(StartWater));
		waterButton_Off.onClick.AddListener(() => RunButtonRoutine(StopWater));
		washHandsButton.onClick.AddListener(() => RunButtonRoutine(RubHands));
		soapButton.onClick.AddListener(() => RunButtonRoutine(PutSoap));

		waterButton_Off.gameObject.SetActive(false);
	}

	private async void RunButtonRoutine(Func<UniTask> animationFunc)
	{
		Button[] buttons = { dryButton, wetHandsButton, waterButton_Off, washHandsButton, soapButton };

		// Hide buttons again
		dryButton.transform.parent.gameObject.SetActive(false);

		// Play requested animation
		await animationFunc();

		// Show buttons
		dryButton.transform.parent.gameObject.SetActive(true);
	}

	// ------------------ ANIMATIONS ------------------

	public async UniTask PutSoap()
	{
		if (!_handsAreWet)
		{
			// I shouldn't do that with dry hands
			_dialogueManager.DialogueToStart = HandsNotWet;
			Debug.Log("WHY!");
			_dialogueManager.StartDialogue();
			Debug.Log("WHY!2");

			return;
		}

		if (_handsAreWet && !_handsSoaped)
		{
			await _cameraChanger.TransitionToCam(soapCamera);
			await _toiletAnimations_1.ShampooAnimation();
			await _cameraChanger.TransitionToCam(originalCamera);
			_handsSoaped = true;
			soapButton.gameObject.SetActive(false);

		}
	}

	public async UniTask StartWater()
	{
		waterButton_On.gameObject.SetActive(false);
		waterButton_Off.gameObject.SetActive(true);

		await _cameraChanger.TransitionToCam(waterCamera);
		await _toiletAnimations_1.StartWaterAnimation();
		await _cameraChanger.TransitionToCam(originalCamera);
		
		_waterIsOn = true;
	}

	public async UniTask PlayWetHandsAnimation()
	{
		if (_waterIsOn) {
			if (!_handsSoaped)
			{
				await _cameraChanger.TransitionToCam(originalCamera);
				await UniTask.Delay(300);
				await _toiletAnimations_2.HandsTogetherAnimation_Sink();
				await UniTask.Delay(2000);
				await _toiletAnimations_2.HandsApartAnimation_Sink();
				_handsAreWet = true;
			}
			else if (_rubbedHands)
			{
				await _cameraChanger.TransitionToCam(originalCamera);
				await UniTask.Delay(300);
				await _toiletAnimations_2.WaterShampooedHands();
				_handsReadyForDry = true;
				wetHandsButton.gameObject.SetActive(false);
			}
			else
			{
				_dialogueManager.DialogueToStart = ShouldRubHands;
				_dialogueManager.StartDialogue();
				// I should rub my hands first
			}
		}
		else
		{
			_dialogueManager.DialogueToStart = WaterIsNotOn;
			_dialogueManager.StartDialogue();
			// message water is not on!
		}

	}

	public async UniTask RubHands()
	{
		if (_handsSoaped && _handsAreWet)
		{
			await _cameraChanger.TransitionToCam(originalCamera);
			await _toiletAnimations_2.ShampooHands();
			await UniTask.Delay(700);
			await _toiletAnimations_2.HandsApartAnimation_Sink();
			
			_rubbedHands = true;
			washHandsButton.gameObject.SetActive(false);
		}
		else
		{
			_dialogueManager.DialogueToStart = NoSoap;
			_dialogueManager.StartDialogue();
			// I don't have soap on my hands!
		}
	}

	public async UniTask StopWater()
	{
		// enable water button (Only if you are not ready for drying)
		waterButton_Off.gameObject.SetActive(false);
		if (!_handsReadyForDry)
		{
			waterButton_On.gameObject.SetActive(true);
		}

		await _cameraChanger.TransitionToCam(waterCamera);
		await _toiletAnimations_1.StopWaterAnimation();
		await UniTask.Delay(1000);
		await _cameraChanger.TransitionToCam(originalCamera);

		_waterIsOn = false;
	}

	public async UniTask PlayDryHandsAnimation()
	{
		if (!_handsReadyForDry)
		{
			_dialogueManager.DialogueToStart = HandsNoWashedYet;
			_dialogueManager.StartDialogue();
			// Hands not washed yet!
			return;			
		}

		if (_waterIsOn)
		{
			_dialogueManager.DialogueToStart = ShouldTurnWaterOff;
			_dialogueManager.StartDialogue();
			return;
			// Should close the water first
		}
		

		if (_handsReadyForDry && !_waterIsOn)
		{
			await _cameraChanger.TransitionToCam(towelCamera);
			await _toiletAnimations_2.DryHandsAnimation();
			await UniTask.Delay(700);

			_cameraChanger.TransitionToCam(binCamera);
			await UniTask.Delay(900);

			await _toiletAnimations_2.ThrowTrashAnimation();
			await UniTask.Delay(1500);

			await _cameraChanger.TransitionToCam(originalCamera);
		}
	}
}
