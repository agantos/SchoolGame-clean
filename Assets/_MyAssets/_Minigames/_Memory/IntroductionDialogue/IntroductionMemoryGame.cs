using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IntroductionMemoryGame : DialogueEventPlanner_Base
{
	[Header("Images")]
	public Image books;
	public Image globe;
	public Image geometry;
	public Image music;

	[Header("Images")]
	public SCR_DialogueNode dialogueNode;
	DialogueManager _dialogueManager;

	private void Start()
	{
		_dialogueManager = FindAnyObjectByType<DialogueManager>();
		_dialogueManager.EventPlanner = this;
		_dialogueManager.DialogueToStart = dialogueNode;

		CreateEvent("IntroductionMemoryGame", DialogueEvent.OnDialogueEvent.STEP, async () => { await PlayImageAnimation(music); }, 1);
		CreateEvent("IntroductionMemoryGame", DialogueEvent.OnDialogueEvent.STEP, async () => { await PlayImageAnimation(geometry); }, 2);
		CreateEvent("IntroductionMemoryGame", DialogueEvent.OnDialogueEvent.STEP, async () => { await PlayImageAnimation(books); }, 3);
		CreateEvent("IntroductionMemoryGame", DialogueEvent.OnDialogueEvent.STEP, async () => { await PlayImageAnimation(globe); }, 4);
		CreateEvent("IntroductionMemoryGame", DialogueEvent.OnDialogueEvent.END_NODE, StartGame);

		_dialogueManager.StartDialogue();

	}

	public async UniTask StartGame()
	{
		await UniTask.Delay(1000);
		var minigame = FindAnyObjectByType<MemoryMinigameManager>();
		await minigame.LoadLevel_Index(0);
	}

	public async UniTask PlayImageAnimation(Image image)
	{
		// Start invisible
		image.transform.localScale = Vector3.zero;
		image.gameObject.SetActive(true);

		// Create a sequence for layered animation
		Sequence seq = DOTween.Sequence();

		seq.Append(image.transform
			.DOScale(1.2f, 0.4f) // pop a bit past full size
			.SetEase(Ease.OutBack, 2f)); // strong anticipation bounce

		seq.Append(image.transform
			.DOScale(1f, 0.1f) // settle back to normal
			.SetEase(Ease.InOutSine));

		await seq.AsyncWaitForCompletion();

		// Hold for a bit
		await UniTask.Delay(1000);

		// Fun pop-out (optional)
		await image.transform
			.DOScale(0f, 0.3f)
			.SetEase(Ease.InBack)
			.AsyncWaitForCompletion();

		image.gameObject.SetActive(false);
	}
}
