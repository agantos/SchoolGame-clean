using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class BasketBallShotAnimator : MonoBehaviour
{
	[Header("References")]
	public Transform ball;      
	public Transform transformShotIn;
	public Transform basketOutTransform;
	public Transform missPosition;      


	[Header("Shot Settings")]
	public float jumpPower = 1f;  // Arc height
	public int numJumps = 1;      // Number of jumps (1 = normal arc)
	public float duration = 1f;    // Time to reach hoop
	public float spinSpeed = 720f; // Degrees per second while flying
	public float groundY = 0.5f;   // Y position for the ground after landing

	[Header("Optional Bounce")]
	public bool bounceAfterHoop = true; // Let the ball fall after hoop
	public float bounceDuration = 0.5f; // Duration of fall/bounce

	public bool startAnimation = false;
	public bool startAnimation_In = false;

	private Vector3 startingPosition;

	private async void Update()
	{
		if (startAnimation)
		{
			startAnimation = false;
			await ShootBallMiss();
			ResetBall(startingPosition);
		}

		if (startAnimation_In)
		{
			startAnimation_In = false;
			await ShootBallInside();
			ResetBall(startingPosition);
		}
	}

	public async UniTask ShootBallInside()
	{
		startingPosition = ball.position;

		// Kill any existing tweens on the ball
		ball.DOKill();

		// Spin the ball continuously during flight
		var spinTween = ball.DOLocalRotate(new Vector3(spinSpeed, 0, 0), duration, RotateMode.FastBeyond360)
							.SetEase(Ease.Linear)
							.SetLoops(-1); // continuous spin

		// Create a sequence for smooth animation
		Sequence shotSequence = DOTween.Sequence();

		// Jump to the hoop
		shotSequence.Append(ball.DOJump(transformShotIn.position, jumpPower, numJumps, duration)
								.SetEase(Ease.Linear));

		// Optional: bounce to the ground after hoop
		if (bounceAfterHoop)
		{
			Vector3 groundPos = new Vector3(transformShotIn.position.x, groundY, transformShotIn.position.z);
			shotSequence.Append(ball.DOMove(groundPos, bounceDuration).SetEase(Ease.InQuad));
		}

		// Wait for the full sequence to complete
		await shotSequence.AsyncWaitForCompletion();

		// Kill spinning at the end
		spinTween.Kill();
	}


	public void ResetBall()
	{
		ResetBall(startingPosition);
	}

	public void ResetBall(Vector3 startPosition)
	{
		ball.position = startPosition;
		ball.rotation = Quaternion.identity;
		ball.DOKill(); // stop any ongoing tweens
	}

	public async UniTask ShootBallMiss()
	{
		startingPosition = ball.position;

		Vector3 missTarget = missPosition.position;

		// Kill any existing tweens on the ball
		ball.DOKill();

		// Spin the ball continuously during flight
		var spinTween = ball.DOLocalRotate(new Vector3(spinSpeed, 0, 0), duration, RotateMode.FastBeyond360)
							.SetEase(Ease.Linear)
							.SetLoops(-1); // continuous spin

		// Create a sequence for smooth chained animation
		Sequence missSequence = DOTween.Sequence();

		// 1. Jump to the rim
		missSequence.Append(ball.DOJump(basketOutTransform.position, jumpPower, numJumps, duration)
								.SetEase(Ease.Linear));

		// 2. Bounce off to the miss target
		missSequence.Append(ball.DOJump(missTarget, 0.5f, 1, duration/2)
								.SetEase(Ease.InOutQuad));

		// Wait for the full sequence to finish
		await missSequence.AsyncWaitForCompletion();

		// Kill spinning at the end
		spinTween.Kill();
	}
}
