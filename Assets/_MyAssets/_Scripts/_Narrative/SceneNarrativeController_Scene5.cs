using Cysharp.Threading.Tasks;
using UnityEngine;

public class SceneNarrativeController_Scene5 : SceneNarrativeController
{
	SceneController controller;

	private async void Start()
	{
		controller = FindAnyObjectByType<SceneController>();

		await UniTask.Delay(10000);
		//await controller.LoadNextScene();
	}
}
