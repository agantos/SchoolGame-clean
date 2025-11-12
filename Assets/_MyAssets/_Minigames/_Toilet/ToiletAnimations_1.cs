using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;

public class ToiletAnimations_1 : MonoBehaviour
{
	public SpriteRenderer[] WaterSprites;
	public GameObject Shampoo;

	[Header("Sink")]
	public GameObject OpenSinkHot;
	public GameObject OpenSinkCold;
	public GameObject Sink;

	public float waterAnimation = 0.5f;

	private CancellationTokenSource _cts_WaterAnimation;

	public bool startWaterAnimation;
	public bool stopWaterAnimation;
	public bool pressShampoo;

	private CinemachineCameraChanger _cameraChanger;

	[Header("Cameras")]

	public CinemachineCamera shampooCamera;
	public CinemachineCamera waterCamera;
	public CinemachineCamera normalCamera;


	private void Start()
	{
		foreach (var s in WaterSprites)
		{
			s.gameObject.SetActive(false);
		}

		ShampooHand.SetActive(false);
	}

	private async void Update()
	{
		if (startWaterAnimation)
		{
			startWaterAnimation = false;
			await StartWater();
			await UniTask.Delay(200);
		}

		if (stopWaterAnimation)
		{
			stopWaterAnimation = false;
			await ResetHandles();
			StopWaterAnimation();
		}

		if (pressShampoo) { 
			pressShampoo = false;
			PressShampoo();
		}
	}


	private void OnDisable()
	{
		StopWaterAnimation();
	}

	public async UniTask ShampooAnimation()
	{
		await PressShampoo();
	}

	public async UniTask StopWater()
	{
		await ResetHandles();
		StopWaterAnimation();
	}

	#region Water Animation


	[Header("Sink Animation Settings")]
	public float rotationAngle_sink = 90f;
	public float rotationDuration_sink = 0.4f;
	public float delayBetweenHandles = 0.3f;

	private Sequence _sequence;



	public async UniTask StartWater()
	{
		if (OpenSinkCold == null || OpenSinkHot == null)
			return;

		// Kill any currently running sequence
		_sequence?.Kill();

		_sequence = DOTween.Sequence();


		// Cold handle first
		_sequence.Append(
			OpenSinkCold.transform
				.DOLocalRotate(
					new Vector3(0, rotationAngle_sink, 0),
					rotationDuration_sink,
					RotateMode.LocalAxisAdd
				)
				.SetEase(Ease.OutBack)
				.OnStart(async () =>
				{
					await UniTask.Delay(300);
					StartWaterAnimation();
				})
		);

		// Wait before Hot handle
		_sequence.AppendInterval(delayBetweenHandles);

		// Then Hot handle
		_sequence.Append(
			OpenSinkHot.transform
				.DOLocalRotate(
					new Vector3(0, rotationAngle_sink, 0),
					rotationDuration_sink,
					RotateMode.LocalAxisAdd
				)
				.SetEase(Ease.OutBack)
		);

		// Await sequence completion (DOTween async integration)
		await _sequence.AsyncWaitForCompletion();
	}

	public async UniTask ResetHandles()
	{
		_sequence?.Kill();
		await OpenSinkCold.transform.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
		await OpenSinkHot.transform.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
	}

	public async UniTask StartWaterAnimation()
	{
		if (_cts_WaterAnimation != null)
			return;

		_cts_WaterAnimation = new CancellationTokenSource();
		AnimateWaterSprites(_cts_WaterAnimation.Token).Forget();
	}

	public void StopWaterAnimation()
	{
		foreach (var s in WaterSprites)
		{
			if(s != null )
				if(s.gameObject.activeSelf) 
					s.gameObject.SetActive(false);
		}

		if (_cts_WaterAnimation == null)
			return;

		_cts_WaterAnimation.Cancel();
		_cts_WaterAnimation.Dispose();
		_cts_WaterAnimation = null;
	}

	private async UniTask AnimateWaterSprites(CancellationToken token)
	{
		foreach (var s in WaterSprites)
		{
			s.gameObject.SetActive(true);
		}

		// --- Start sprite flipping loop ---
		while (!token.IsCancellationRequested)
		{
			bool flipX = Random.value > 0.5f;
			bool flipY = Random.value > 0.5f;

			foreach (var sprite in WaterSprites)
			{
				if (sprite == null) continue;

				if (flipX == sprite.flipX && flipY == sprite.flipY)
				{
					sprite.flipY = !sprite.flipY;
					continue;
				}
				else
				{
					sprite.flipX = flipX;
					sprite.flipY = flipY;
				}
			}

			await UniTask.Delay(System.TimeSpan.FromSeconds(waterAnimation), cancellationToken: token);
		}
	}
	#endregion

	#region Shampoo Animation
	[Header("Shampoo Animation References")]
	public GameObject ShampooContainer;
	public Transform ShampooNozzle;
	public GameObject ShampooBlobPrefab;
	public GameObject ShampooHand;

	[Header("Animation Settings")]
	public float squeezeScale_Shampoo = 0.8f;
	public float squeezeDuration_Shampoo = 0.15f;
	public float blobForce_Shampoo = 4f;
	public float blobArcHeight_Shampoo = 2f;
	public float blobLifetime_Shampoo = 1.2f;

	private bool _isAnimating_Shampoo;



	async UniTask PressShampoo()
	{
		ShampooHand.SetActive(true);

		if (_isAnimating_Shampoo || ShampooContainer == null) return;
		_isAnimating_Shampoo = true;

		var token = this.GetCancellationTokenOnDestroy();

		// --- Step 1: Squeeze the bottle ---
		Vector3 originalScale = ShampooContainer.transform.localScale;
		Vector3 squeezedScale = new Vector3(originalScale.x * squeezeScale_Shampoo, originalScale.y * 0.7f, originalScale.z);

		await ScaleOverTime(ShampooContainer.transform, squeezedScale, squeezeDuration_Shampoo, token);

		// --- Step 2: Squirt shampoo blob ---
		if (ShampooBlobPrefab != null && ShampooNozzle != null)
		{
			GameObject blob = Instantiate(ShampooBlobPrefab, ShampooNozzle.position, Quaternion.identity);
			AnimateBlob(blob, token).Forget();
		}

		// --- Step 3: Return to normal scale ---
		await ScaleOverTime(ShampooContainer.transform, originalScale, squeezeDuration_Shampoo * 1.5f, token);

		_isAnimating_Shampoo = false;

		await UniTask.Delay(500);

		ShampooHand.SetActive(false);
	}
	private async UniTask ScaleOverTime(Transform target, Vector3 targetScale, float duration, CancellationToken token)
	{
		Vector3 startScale = target.localScale;
		float elapsed = 0f;

		while (elapsed < duration && !token.IsCancellationRequested)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.SmoothStep(0, 1, elapsed / duration);
			target.localScale = Vector3.Lerp(startScale, targetScale, t);
			await UniTask.Yield(cancellationToken: token);
		}

		target.localScale = targetScale;
	}

	private async UniTask AnimateBlob(GameObject blob, CancellationToken token)
	{
		Vector3 startPos = blob.transform.position;
		Vector3 targetPos = startPos + new Vector3(0, -0.15f, 0f);

		float elapsed = 0f;
		while (elapsed < blobLifetime_Shampoo && !token.IsCancellationRequested)
		{
			elapsed += Time.deltaTime;
			float t = elapsed / blobLifetime_Shampoo;

			// Parabolic arc (cartoony motion)
			float height = Mathf.Sin(t * Mathf.PI) * blobArcHeight_Shampoo;
			blob.transform.position = Vector3.Lerp(startPos, targetPos, t) + Vector3.up * height;

			await UniTask.Yield(cancellationToken: token);
		}

		if (blob != null)
			Destroy(blob);
	}
	#endregion

	#region
	#endregion
}
