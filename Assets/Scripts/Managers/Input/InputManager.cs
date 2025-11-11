using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	public class PlayerInputHandler
	{
		public event Action DashEvent;
		public event Action PrimarySkillEvent;
		// public event Action SecondarySkillEvent;
		public event Action WeaponSkillEvent;
		public event Action InteractEvent;

		public Vector3 InputDirection { get; private set; }

		public PlayerInputHandler()
		{
			Instance.PlayerActions.Dash.performed += ctx => DashEvent?.Invoke();
			Instance.PlayerActions.PrimarySkill.performed += ctx => PrimarySkillEvent?.Invoke();
			// Instance.PlayerActions.SecondarySkill.performed += ctx => SecondarySkillEvent?.Invoke();
			Instance.PlayerActions.WeaponSkill.performed += ctx => WeaponSkillEvent?.Invoke();
			Instance.PlayerActions.Interact.performed += ctx => InteractEvent?.Invoke();
		}

		public void DoUpdate()
		{
			InputDirection = Instance.PlayerActions.Move.ReadValue<Vector2>().normalized;
		}

		public void ClearSignals()
		{
			DashEvent = null;
			PrimarySkillEvent = null;
			// SecondarySkillEvent = null;
			WeaponSkillEvent = null;
			InteractEvent = null;
		}
	}

	public enum ControlInputFlags : byte
	{
		None = 0,
		Ctrl = 1 << 0,
		Shift = 1 << 1,
		Alt = 1 << 2,
	}

	public static InputManager Instance { get; private set; }

	public event Action OnControlInputFlagChanged;

	private InputSystem_Actions inputSystem;
	public InputActionAsset InputActionAsset => inputSystem.asset;
	public InputSystem_Actions.PlayerActions PlayerActions => inputSystem.Player;
	public InputSystem_Actions.UIActions UIActions => inputSystem.UI;

	public PlayerInputHandler PlayerInput { get; private set; }

	private ControlInputFlags tempFlags = ControlInputFlags.None;
	private ControlInputFlags currentFlags = ControlInputFlags.None;
	public ControlInputFlags ControlInputFlag
	{
		get => currentFlags;
		private set
		{
			if (currentFlags != value)
			{
				currentFlags = value;
				OnControlInputFlagChanged?.Invoke();
			}
		}
	}

	void Awake()
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

		inputSystem = new();
		PlayerInput = new();
	}

	void OnEnable()
	{
		inputSystem.Enable();
		PlayerActions.Enable();
		UIActions.Enable();
	}

	void OnDisable()
	{
		if (inputSystem != null)
		{
			inputSystem.Disable();
			PlayerActions.Disable();
			UIActions.Disable();
		}
	}

	void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}

		inputSystem?.Dispose();
		inputSystem = null;
	}

	void Update()
	{
		PlayerInput.DoUpdate();

		if (Keyboard.current != null)
		{
			Keyboard keyboard = Keyboard.current;
			tempFlags = ControlInputFlags.None;
			if (keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed)
			{
				tempFlags |= ControlInputFlags.Ctrl;
			}
			if (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed)
			{
				tempFlags |= ControlInputFlags.Shift;
			}
			if (keyboard.leftAltKey.isPressed || keyboard.rightAltKey.isPressed)
			{
				tempFlags |= ControlInputFlags.Alt;
			}

			ControlInputFlag = tempFlags;
		}
	}

	public void ClearSignals()
	{
		OnControlInputFlagChanged = null;

		PlayerInput.ClearSignals();
	}
}