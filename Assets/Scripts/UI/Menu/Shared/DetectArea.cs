using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DetectArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public event Action<bool> OnPointerOverChanged;
	public event Action OnPointerEntered;
	public event Action OnPointerExited;

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnPointerOverChanged?.Invoke(true);
		OnPointerEntered?.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnPointerOverChanged?.Invoke(false);
		OnPointerExited?.Invoke();
	}
}