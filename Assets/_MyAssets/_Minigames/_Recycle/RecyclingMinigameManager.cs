using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class RecyclingMinigameManager : MonoBehaviour
{
	public GameObject[] waste;
	public GameObject[] recycleTrash;
	public GameObject allTrashParent;

	int _completedTrash = 0;
	int _maxTrash = 0;

	public BinAnimation wasteBin;
	public BinAnimation recycleBin;

	private CinemachineCameraChanger _cameraChanger;
	private PlayerManager _playerManager;

	public RecycleGameIntroduction gameIntroduction;

	private void Awake()
	{
		_maxTrash = waste.Length + recycleTrash.Length;
		_projectorController = FindAnyObjectByType<ProjectorController>(findObjectsInactive: FindObjectsInactive.Include);
		_cameraChanger = FindAnyObjectByType<CinemachineCameraChanger>();
		_playerManager = FindAnyObjectByType<PlayerManager>();

		//Disable components
		UI_Canvas.gameObject.SetActive(false);
		allTrashParent.SetActive(false);
	}

	#region Projector

	[Header("Projector")]
	ProjectorController _projectorController;
	public VideoClip recycleVideo;

	#endregion

	#region Game Functionality

	public async UniTask StartRecyclingGame()
	{
		await UniTask.Delay(2000);
		allTrashParent.SetActive(true);
		UI_Canvas.gameObject.SetActive(true);
		await gameIntroduction.PlayIntroduction();

		//Enable Components
		_playerManager.PlayerMovementController.EnableMovement();

		UpdateText();

		_isRunning = true;
		LoopBounceAsync().Forget();

		await UniTask.Delay(2000);
	}

	public void CompleteTrash()
	{
		_completedTrash++;
		UpdateUIonCorrect();
	}

	public bool GetIsWaste(GameObject obj)
	{
		foreach (GameObject w in waste)
		{
			if (w == obj) return true;
		}

		return false;
	}

	public bool IsTrash(GameObject obj)
	{
		foreach (GameObject w in waste)
		{
			if (w == obj) return true;
		}

		foreach (GameObject w in recycleTrash)
		{
			if (w == obj) return true;
		}

		return false;
	}

	#endregion

	#region UI
	public TextMeshProUGUI completedText;
	public Canvas UI_Canvas;


	private string coloredTitle =
	"<color=#FF6B6B>S</color>" +  // red
	"<color=#FFD93D>o</color>" +  // yellow
	"<color=#6BCB77>r</color>" +  // green
	"<color=#4D96FF>t</color>" +  // blue
	"<color=#FF6EC7>e</color>" +  // pink
	"<color=#FF9F40>d</color> " + // orange
	"<color=#A66BFF>T</color>" +  // purple
	"<color=#4D96FF>r</color>" +  // blue
	"<color=#6BCB77>a</color>" +  // green
	"<color=#FFD93D>s</color>" +  // yellow
	"<color=#FF6B6B>h</color>";   // red

	float minInterval = 5f;      
	float maxInterval = 10f;
	float duration = 0.3f;       

	private bool _isRunning;

	void AnimateText(TextMeshProUGUI text, float scaleAmount = 1.05f)
	{
		if (text == null) return;

		// Reset scale
		text.rectTransform.localScale = Vector3.one;

		// Play the punch/bounce animation
		text.rectTransform
			.DOPunchScale(Vector3.one * (scaleAmount - 1f), duration, vibrato: 2, elasticity: 0.7f)
			.SetEase(Ease.OutBack);
	}

	void UpdateUIonCorrect()
	{
		UpdateText();
		AnimateText(completedText, 1.5f);
	}

	void UpdateText()
	{
		completedText.gameObject.SetActive(true);
		string completedColor = "#6BCB77"; // green
		string maxColor = "#FFD93D";       // yellow

		completedText.text = $"{coloredTitle} " +
							 $"<color={completedColor}>{_completedTrash}</color>" +
							 "<color=#FFFFFF>/</color>" +
							 $"<color={maxColor}>{_maxTrash}</color>";
	}

	async UniTaskVoid LoopBounceAsync()
	{
		while (_isRunning && this != null)
		{
			float waitTime = Random.Range(minInterval, maxInterval);
			await UniTask.Delay((int)(waitTime * 1000), cancellationToken: this.GetCancellationTokenOnDestroy());
			PlayBounce();
		}
	}

	void PlayBounce()
	{
		AnimateText(completedText);
	}

	string GetColoredText(string text)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder(text.Length * 20);

		foreach (char c in text)
		{
			// Skip coloring spaces
			if (c == ' ')
			{
				sb.Append(' ');
				continue;
			}

			// Generate random color (pastel or bright)
			Color randomColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
			string hexColor = ColorUtility.ToHtmlStringRGB(randomColor);

			sb.Append($"<color=#{hexColor}>{c}</color>");
		}

		return sb.ToString();
	}



	void OnDestroy()
	{
		_isRunning = false;
	}

	#endregion
}
