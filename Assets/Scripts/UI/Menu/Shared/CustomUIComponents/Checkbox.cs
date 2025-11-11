using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Checkbox : CustomUIComponent
{
	public event Action<bool> OnChecked;

	public bool IsChecked { get; private set; } = false;

	[SerializeField] private Image checkboxImage;
	[SerializeField] private Sprite checkedSprite;
	[SerializeField] private Sprite uncheckedSprite;

	public void Toggle() => SetChecked(!IsChecked);
	public void SetChecked(bool value)
	{
		if (IsChecked != value)
		{
			IsChecked = value;
			RefreshVisuals();
			OnChecked?.Invoke(IsChecked);
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		Toggle();

		base.OnPointerUp(eventData);
	}

	public override void RefreshVisuals()
	{
		checkboxImage.sprite = IsChecked ? checkedSprite : uncheckedSprite;
		if (checkboxImage.sprite == null)
		{
			checkboxImage.enabled = false;
		}
		else
		{
			checkboxImage.SetNativeSize();
			checkboxImage.enabled = true;
		}

		base.RefreshVisuals();
	}

	public override void AutoFillFields()
	{
		base.AutoFillFields();

		if (checkboxImage == null)
		{
			Transform checkboxTransform = rectTransform.Find("Checkbox");
			if (checkboxTransform != null)
			{
				checkboxImage = checkboxTransform.GetComponent<Image>();
				if (checkboxImage == null)
				{
					checkboxImage = checkboxTransform.gameObject.AddComponent<Image>();
				}
			}
			else
			{
				var checkboxObj = new GameObject("Checkbox");
				checkboxObj.transform.SetParent(transform, false);
				checkboxImage = checkboxObj.AddComponent<Image>();
			}

			checkboxImage.maskable = false;
			checkboxImage.enabled = false;
		}
	}
}