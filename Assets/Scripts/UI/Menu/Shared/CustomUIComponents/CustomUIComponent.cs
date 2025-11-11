using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class CustomUIComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public event Action OnClicked;
	public event Action OnHighlighted;
	protected virtual void InvokeOnClicked() => OnClicked?.Invoke();
	protected virtual void InvokeOnHighlighted() => OnHighlighted?.Invoke();

	[SerializeField] protected RectTransform rectTransform;
	[SerializeField] protected Image image;
	[SerializeField] protected Sprite defaultSprite;
	[SerializeField] protected Sprite highlightedSprite;
	[SerializeField] protected Sprite pressedSprite;
	[SerializeField] protected GameObject highlightedObject;
	[SerializeField] protected GameObject pressedObject;

	public RectTransform RectTransform => rectTransform;

	protected bool isHighlighted = false;
	protected bool isPressed = false;

	protected virtual void Awake()
	{
		isHighlighted = false;
		isPressed = false;
		RefreshVisuals();
	}

	public virtual void ListenToMenuClose(MenuBase menu)
	{
		menu.OnClosed += ClearVisuals;
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		isHighlighted = true;
		RefreshVisuals();
		OnHighlighted?.Invoke();
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		isHighlighted = false;
		isPressed = false;
		RefreshVisuals();
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		isPressed = true;
		RefreshVisuals();
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		if (isPressed)
		{
			OnClicked?.Invoke();
		}
		isPressed = false;
		RefreshVisuals();
	}

	public virtual void RefreshVisuals()
	{
		if (isPressed)
		{
			image.sprite = pressedSprite;

			SetGameObjectActive(pressedObject, true);
			SetGameObjectActive(highlightedObject, false);
		}
		else if (isHighlighted)
		{
			image.sprite = highlightedSprite;

			SetGameObjectActive(pressedObject, false);
			SetGameObjectActive(highlightedObject, true);
		}
		else
		{
			ClearVisuals();
		}

		image.enabled = image.sprite != null;
	}

	public virtual void ClearVisuals()
	{
		image.sprite = defaultSprite;

		SetGameObjectActive(pressedObject, false);
		SetGameObjectActive(highlightedObject, false);
	}

	protected static void SetGameObjectActive(GameObject obj, bool active)
	{
		if (obj != null)
		{
			obj.SetActive(active);
		}
	}

	[ContextMenu("Auto Fill Fields")]
	private void ContextAutoFillFields() => AutoFillFields();

	public virtual void AutoFillFields()
	{
		if (rectTransform == null)
		{
			rectTransform = GetComponent<RectTransform>();
		}

		if (image == null)
		{
			image = GetComponent<Image>();
		}

		if (defaultSprite == null && image != null)
		{
			defaultSprite = image.sprite;
		}

		if (highlightedObject == null)
		{
			var highlightedObjTransform = transform.Find("Highlighted Object");
			if (highlightedObjTransform != null)
			{
				highlightedObject = highlightedObjTransform.gameObject;
			}
			else
			{
				highlightedObject = new GameObject("Highlighted Object");
				highlightedObject.transform.SetParent(transform, false);
			}

			Image highlightImage = highlightedObject.GetOrAddComponent<Image>();
			highlightImage.raycastTarget = false;

			highlightedObject.SetActive(false);
		}

		if (pressedObject == null)
		{
			var pressedObjTransform = transform.Find("Pressed Object");
			if (pressedObjTransform != null)
			{
				pressedObject = pressedObjTransform.gameObject;
			}
			else
			{
				pressedObject = new GameObject("Pressed Object");
				pressedObject.transform.SetParent(transform, false);
			}

			Image pressedImage = pressedObject.GetOrAddComponent<Image>();
			pressedImage.raycastTarget = false;

			pressedObject.SetActive(false);
		}
	}
}