using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderHandle : CustomUIComponent, IDragHandler, IEndDragHandler
{
	public event Action<float> OnValueChanged;

	public float Value { get; private set; }

	private float minX;
	private float maxX;
	private RectTransform sliderRect;

	protected override void Awake()
	{
		base.Awake();
	}

	public void SetSliderRect(RectTransform rect, float leftOffset, float rightOffset)
	{
		sliderRect = rect;
		minX = leftOffset;
		maxX = sliderRect.sizeDelta.x - rightOffset;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (sliderRect == null) return;

		isPressed = true;
		RefreshVisuals();

		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(sliderRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
		{
			rectTransform.anchoredPosition = new Vector2(Mathf.Clamp(localPoint.x, minX, maxX), rectTransform.anchoredPosition.y);
			Value = (rectTransform.anchoredPosition.x - minX) / (maxX - minX);
			OnValueChanged?.Invoke(Value);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isPressed = false;
		RefreshVisuals();
	}
}