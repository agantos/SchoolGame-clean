using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TraceableShapePart : MonoBehaviour
{
    public Func<UniTask> onTrace;
    public List<GameObject> traceableList;
    public List<TraceableShapePart> fullList;

    public bool isTraced;
    public Tween currentTween;

    public void Fill()
    {
        gameObject.GetComponent<CanvasGroup>().alpha = 1f;
    }

    public void Init(List<GameObject> traceableList, List<TraceableShapePart> fullList)
    {
        this.traceableList = traceableList;
        this.fullList = fullList;

        gameObject.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
