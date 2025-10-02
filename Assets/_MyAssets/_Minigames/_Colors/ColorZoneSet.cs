using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ColorZoneSet : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
	[Header("Brush Color")]
	public Color zoneColor = Color.black;

	[Header("Drawing Reference")]
	public TransparentOverlayDraw overlayDraw;
	public bool _enabled = true;

	public Action onEnter;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (overlayDraw != null && _enabled)
		{
			if (onEnter != null)
			{
				onEnter.Invoke();
			}

			overlayDraw.SetBrushColor(zoneColor);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (_enabled && overlayDraw != null && eventData.pointerPress != null) // finger/pen is pressed and sliding over
		{
			if(onEnter != null)
			{
				onEnter.Invoke();
			}
			overlayDraw.SetBrushColor(zoneColor);
		}
	}
}
