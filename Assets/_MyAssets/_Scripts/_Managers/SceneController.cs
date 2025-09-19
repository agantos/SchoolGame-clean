using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public SCR_Scene[] scenes;
    private int currentLoadedScene = 0;

    public int sceneToStart = 0;

    private AsyncOperation _preloadedSceneOp;

	private async void Start()
	{
        await LoadSceneAsFirst(sceneToStart);
	}

	public async UniTask LoadSceneAsFirst(int index = 0)
    {
        currentLoadedScene = index;
        var sceneToLoad = scenes[currentLoadedScene];

        await SceneManager.LoadSceneAsync(sceneToLoad.ScenePath, LoadSceneMode.Additive);

        // Call onLoad tasks
        if (sceneToLoad?.narrativeController?.onLoadScene != null)
        {          
            await sceneToLoad.narrativeController.onLoadScene.Invoke();
        }

        int nextIndex = currentLoadedScene + 1;
        if (nextIndex < scenes.Length)
        {
            var nextPreload = scenes[nextIndex];
            _preloadedSceneOp = SceneManager.LoadSceneAsync(nextPreload.ScenePath, LoadSceneMode.Additive);
            _preloadedSceneOp.allowSceneActivation = false;
        }
    }

    public async UniTask LoadNextScene()
    {
        var toLoadIndex = currentLoadedScene + 1; 

        if (toLoadIndex < 0 || toLoadIndex >= scenes.Length)
        {
            Debug.LogWarning("Scene index out of range");
            return;
        }

        var currentScene = scenes[currentLoadedScene];
        var sceneToLoad = scenes[toLoadIndex];

        // Call unload tasks on current scene
        if (currentScene?.narrativeController?.onUnloadScene != null)
        {
            await currentScene.narrativeController.onUnloadScene.Invoke();
        }

		// Activate the preloaded scene
		if (_preloadedSceneOp != null)
		{
			_preloadedSceneOp.allowSceneActivation = true;

			await UniTask.WaitUntil(() =>
				SceneManager.GetSceneByPath(sceneToLoad.ScenePath).isLoaded
			);

			currentLoadedScene = toLoadIndex;
            _preloadedSceneOp = null;
		}
		else
        {
            Debug.LogError("Should not be here");
            return;
        }


        // Call load tasks on the new scene
        if (sceneToLoad?.narrativeController?.onLoadScene != null)
        {
            await UniTask.Yield();
            await sceneToLoad.narrativeController.onLoadScene.Invoke();
        }

        // Unload previous scene
        if (currentScene != null)
        {
            await SceneManager.UnloadSceneAsync(currentScene.ScenePath);
        }


        // Preload the next scene in the background (if it exists)
        int nextIndex = currentLoadedScene + 1;
        if (nextIndex < scenes.Length)
        {
            var nextPreload = scenes[nextIndex];
            _preloadedSceneOp = SceneManager.LoadSceneAsync(nextPreload.ScenePath, LoadSceneMode.Additive);
            _preloadedSceneOp.allowSceneActivation = false;
        }
    }
}
