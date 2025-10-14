using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class CabinetContentsAnimations_Base : MonoBehaviour
{
	[SerializeField] ParticleEffect CorrectEffect;

	public bool playIntroduction;
	public bool playCorrect;
	public bool playWrong;

	private void Update()
	{
		if (playCorrect)
		{
			PlayCorrectAnimation();
			playCorrect = false;
		}

		if (playWrong)
		{
			PlayWrongAnimation();
			playWrong = false;
		}

		if (playIntroduction)
		{
			PlayIntroductionAnimation();
			playIntroduction = false;
		}
	}

	public abstract UniTask PlayCorrectAnimation();
	public abstract UniTask PlayWrongAnimation();
	public abstract UniTask PlayIntroductionAnimation();

}
