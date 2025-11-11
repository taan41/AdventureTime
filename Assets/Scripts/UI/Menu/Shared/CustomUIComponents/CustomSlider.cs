using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSlider : CustomUIComponent, IDragHandler, IEndDragHandler
{
	public event Action<float> OnValueChanged;

	[SerializeField] private Image fillImage;
	[SerializeField] private SliderHandle handle;
	[SerializeField] private float handleOffsetLeft;
	[SerializeField] private float handleOffsetRight;
	[SerializeField] private Text percent;

	public float Value { get; private set; }

	protected override void Awake()
	{
		handle.SetSliderRect(rectTransform, handleOffsetLeft, handleOffsetRight);
		handle.OnValueChanged += SetValue;

		base.Awake();
	}

	public void SetValue(float value) => SetValue(value, false);
	public void SetValue(float value, bool moveHandle)
	{
		value = Mathf.Clamp01(value);
		Value = value;

		if (fillImage != null)
		{
			fillImage.fillAmount = value;
		}

		if (percent != null)
		{
			percent.text = (int)(value * 100f) + "%";
		}

		if (moveHandle)
		{
			float handleX = handleOffsetLeft + value * (rectTransform.rect.width - handleOffsetRight - handleOffsetLeft);
			handle.RectTransform.localPosition = new Vector2(handleX, handle.RectTransform.localPosition.y);
		}

		OnValueChanged?.Invoke(value);
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);

		handle.OnDrag(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		handle.OnDrag(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		handle.OnEndDrag(eventData);
	}

	public override void AutoFillFields()
	{
		base.AutoFillFields();

		if (fillImage == null)
		{
			Transform fillTransform = rectTransform.Find("Fill");
			if (fillTransform != null)
			{
				fillImage = fillTransform.GetComponent<Image>();
				if (fillImage == null)
				{
					fillImage = fillTransform.gameObject.AddComponent<Image>();
				}
				fillImage.type = Image.Type.Filled;
				fillImage.fillMethod = Image.FillMethod.Horizontal;
				fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
			}
			else
			{
				var fillObject = new GameObject("Fill");
				fillObject.transform.SetParent(transform, false);
				fillImage = fillObject.AddComponent<Image>();
				fillTransform = fillObject.transform;
			}

			fillTransform.SetAsLastSibling();
			if (handle != null)
			{
				handle.RectTransform.SetAsLastSibling();
			}
		}
		
		if (percent == null)
		{
			Transform percentTransform = rectTransform.Find("Percent");
			if (percentTransform != null)
			{
				percent = percentTransform.GetComponent<Text>();
				if (percent == null)
				{
					percent = percentTransform.gameObject.AddComponent<Text>();
				}
			}
			else
			{
				var percentObject = new GameObject("Percent");
				percentObject.transform.SetParent(transform, false);
				percent = percentObject.AddComponent<Text>();
			}

			percent.transform.SetAsLastSibling();
			if (handle != null)
			{
				handle.RectTransform.SetAsLastSibling();
			}
		}

		if (handle == null)
		{
			var handleTransform = transform.Find("Handle");
			if (handleTransform != null)
			{
				handle = handleTransform.GetComponent<SliderHandle>();
				if (handle == null)
				{
					handle = handleTransform.gameObject.AddComponent<SliderHandle>();
				}
			}
			else
			{
				var handleObject = new GameObject("Handle");
				handleObject.transform.SetParent(transform, false);
				handle = handleObject.AddComponent<SliderHandle>();
			}

			handle.AutoFillFields();
			handle.transform.SetAsLastSibling();
		}
	}
}