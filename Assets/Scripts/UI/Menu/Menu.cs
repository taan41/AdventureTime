using System;
using UnityEngine;

public abstract class MenuBase : MonoBehaviour
{
	public event Action OnClosed;
	protected void InvokeOnClosed() => OnClosed?.Invoke();

	public enum InputFlags : byte
	{
		None = 0,
		LeftClick = 1 << 0,
		RightClick = 1 << 1,
		MiddleClick = 1 << 2,
		Ctrl = 1 << 3,
		Shift = 1 << 4,
		Alt = 1 << 5,
	}

	protected static InputFlags GetInputFlags()
	{
	    InputFlags flags = (InputFlags)((int)MouseManager.Instance.MouseInputFlag & 0x7);
	    
		flags |= (InputFlags)((int)InputManager.Instance.ControlInputFlag << 3);
	    
    	return flags;
	}

	public virtual bool IsOpened => menuPanel.activeSelf;
	[SerializeField] protected GameObject menuPanel;

	public abstract void Initialize();

	public virtual bool Open(Character focusedCharacter)
	{
		if (!GameStateManager.Instance.CheckState(GameStateManager.GameState.GameMenu)) return false;

		menuPanel.SetActive(true);
		GameStateManager.Instance.AddState(GameStateManager.GameState.GameMenu);
		return true;
	}

	public virtual bool Close()
	{
		if (!IsOpened) return false;

		menuPanel.SetActive(false);
		GameStateManager.Instance.RemoveState(GameStateManager.GameState.GameMenu);

		OnClosed?.Invoke();
		return true;
	}

	public virtual void OnItemDisplayClicked(ItemDisplay itemDisplay) { }
	public virtual void OnItemDisplayDragged(ItemDisplay itemDisplay, bool startDragging = true) { }
	public virtual void OnItemDisplayDropped(ItemDisplay itemDisplay) { }
	public virtual void OnItemDisplayPointed(ItemDisplay itemDisplay, bool pointing = true) { }
	public virtual void OnBlankAreaClicked() { }
	public virtual void OnBlankAreaDropped() { }
}