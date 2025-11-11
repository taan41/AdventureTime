using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Bookmark : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public enum BookmarkType
	{
		Inventory,
		Settings,
	}

	public event Action<Bookmark> OnClick;

	public RectTransform bookmarkTransform;
	public Image iconImage;
	public GameObject highlightPointer;
	public GameObject selectedPointer;
	public bool isSelected = false;

	public void OnPointerClick(PointerEventData eventData)
	{
		SetSelected(true);

		OnClick?.Invoke(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((MouseManager.Instance.MouseInputFlag & MouseManager.MouseInputFlags.LeftClick) != 0)
		{
			SetSelected(true);

			OnClick?.Invoke(this);
		}
		else if (!isSelected)
		{
			highlightPointer.SetActive(true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!isSelected)
		{
			ClearVisuals();
		}
	}

	public void SetSelected(bool selected)
	{
		isSelected = selected;

		selectedPointer.SetActive(isSelected);
		highlightPointer.SetActive(false);
	}

	public void ClearVisuals()
	{
		isSelected = false;
		selectedPointer.SetActive(false);
		highlightPointer.SetActive(false);
	}
}