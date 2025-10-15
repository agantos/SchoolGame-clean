using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public abstract class CabinetContentsAnimations_Base : MonoBehaviour
{
	[Header("Base Class Variables")]
	[SerializeField] ParticleEffect CorrectEffect;
	public CinemachineCamera viewCamera;
	public GameObject cabinetDoor;


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

	public async UniTask PlayCorrectEffect() { 
		CorrectEffect.Play();
		await UniTask.Delay(1000);
	}

	public async UniTask OpenCabinetDoor()
	{

	}

	public async UniTask CloseCabinetDoor()
	{

	}

	public abstract UniTask PlayCorrectAnimation();
	public abstract UniTask PlayWrongAnimation();
	public abstract UniTask PlayIntroductionAnimation();

}
