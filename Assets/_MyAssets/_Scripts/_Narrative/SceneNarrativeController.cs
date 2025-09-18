using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class SceneNarrativeController : MonoBehaviour
{
    public Func<UniTask> onLoadScene;
    public Func<UniTask> onUnloadScene;
}
