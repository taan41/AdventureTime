using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour, IUpdatable
{
	public enum MouseInputFlags : byte
	{
		None = 0,
		LeftClick = 1 << 0,
		RightClick = 1 << 1,
		MiddleClick = 1 << 2,
		ScrollUp = 1 << 3,
		ScrollDown = 1 << 4
	}

	public static MouseManager Instance { get; private set; } = null;

	public event System.Action<bool> OnEnabledChanged;

	public bool Enabled { get; private set; } = false;
	public bool UseUpdate { get; private set; } = false;
	public bool UseFixedUpdate { get; private set; } = false;
	public bool UseUnscaledTime { get; private set; } = true;

	public Vector2 MouseScreenPosition { get; private set; }
	public Vector2 MouseWorldPosition { get; private set; }
	public MouseInputFlags MouseInputFlag { get; private set; }
	private MouseInputFlags tempInputFlags = MouseInputFlags.None;

	public Item MouseItem { get; private set; } = new();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		UpdaterManager.StaticRegisterLastUpdater(this);

		Enable(true);
	}

	public void Enable(bool enabled)
	{
		Enabled = enabled;
		OnEnabledChanged?.Invoke(enabled);
	}

	public void DoUpdate(float deltaTime) { }

	public void DoFixedUpdate(float fixedDeltaTime) { }

	public void DoUnscaledUpdate(float unscaledDeltaTime)
	{
		MouseScreenPosition = Mouse.current.position.ReadValue();
		MouseWorldPosition = Camera.main.ScreenToWorldPoint(MouseScreenPosition);

		tempInputFlags = MouseInputFlags.None;
		if (Mouse.current.leftButton.isPressed)
		{
			tempInputFlags |= MouseInputFlags.LeftClick;
		}
		if (Mouse.current.rightButton.isPressed)
		{
			tempInputFlags |= MouseInputFlags.RightClick;
		}
		if (Mouse.current.middleButton.isPressed)
		{
			tempInputFlags |= MouseInputFlags.MiddleClick;
		}
		float scrollValue = Mouse.current.scroll.ReadValue().y;
		if (scrollValue > 0f)
		{
			tempInputFlags |= MouseInputFlags.ScrollUp;
		}
		else if (scrollValue < 0f)
		{
			tempInputFlags |= MouseInputFlags.ScrollDown;
		}

		MouseInputFlag = tempInputFlags;
	}

	// public void SetShopMouse(float goldDelta, bool show)
	// 	=> shopCursor.SetGoldText(goldDelta, show && MouseItem.Data == null);

	// private void RefreshItemMouse()
	// {
	// 	if (MouseItem.Data != null)
	// 	{
	// 		itemCursor.SetImage(MouseItem.Data.icon, MouseItem.Quantity, MouseItem.Data.stackable);
	// 		shopCursor.SetGoldText(0f, false);
	// 	}
	// 	else
	// 	{
	// 		itemCursor.SetImage(null);
	// 	}
	// }
}