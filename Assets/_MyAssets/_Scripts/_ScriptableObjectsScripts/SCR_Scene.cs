using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "ScriptableObjects/Scene Reference")]
public class SCR_Scene : ScriptableObject
{
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif
    [SerializeField, HideInInspector] private string scenePath;
    public string ScenePath => scenePath;
    public SceneNarrativeController narrativeController;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            EditorUtility.SetDirty(this);
        }
    }
#endif
}
