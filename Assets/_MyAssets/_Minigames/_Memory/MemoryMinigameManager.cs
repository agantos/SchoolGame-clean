using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class MemoryMinigameManager : MonoBehaviour
{
	public GameObject Cabinet;

	CinemachineCameraChanger _changer;
	public CinemachineCamera farCamera;

	public CabinetContentsAnimations_Base[] cabinetContents;

	private void Awake()
	{
		_changer = FindAnyObjectByType<CinemachineCameraChanger>();

		foreach (var item in cabinetContents)
		{
			item.gameObject.SetActive(false);
		}
	}

	public void TriggerRandomIntroduction()
	{
		int index = Random.Range(0, cabinetContents.Length);
		PlayItemIntroduction(cabinetContents[index]);
	}

	async UniTask PlayItemIntroduction(CabinetContentsAnimations_Base item)
	{
		item.gameObject.SetActive(true);

		_changer.TransitionToCam(item.viewCamera);
		await UniTask.Delay(300);
		await item.OpenCabinetDoor();
		
		await item.PlayIntroductionAnimation();


		_changer.TransitionToCam(farCamera);
		await UniTask.Delay(300);
		await item.CloseCabinetDoor();
	}

	private void PlayIntroduction()
	{
		foreach (var item in cabinetContents)
		{
			
		}
	}


}
