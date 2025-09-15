using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraTransition : MonoBehaviour
{
	[SerializeField] CinemachineCamera[] cameras;
	[SerializeField] float[] delays;

	CinemachineCameraChanger _changer;

	private void Start()
	{
		_changer = FindAnyObjectByType<CinemachineCameraChanger>();
	}

	public async UniTask PerformTransitions()
	{
		int i = 0;

		foreach(var c in cameras)
		{
			await _changer.TransitionToCam(c);
			
			if(delays.Length > i)
				await UniTask.Delay(Mathf.CeilToInt(delays[i] * 1000));
			
			i++;
		}
	}
}
