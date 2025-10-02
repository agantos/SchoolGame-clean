using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class ColorZoneCheck : MonoBehaviour, IPointerEnterHandler
{
	[Header("Check Color")] 
	public Color zoneColor = Color.black;

	[Header("Drawing Reference")]
	public TransparentOverlayDraw overlayDraw;

	public Func<UniTask> onWrongMatch;
	public Func<UniTask> onCorrectMatch;

	public async void OnPointerEnter(PointerEventData eventData)
	{
		if (overlayDraw != null)
		{
			if (overlayDraw.drawColor == zoneColor)
			{
				await onCorrectMatch.Invoke();
				Debug.Log("Correct");
			}
			else
			{
				await onWrongMatch.Invoke();
				Debug.Log("Wrong");
			}
		}
	}
}
