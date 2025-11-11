// using System;
// using UnityEngine;

// public abstract class IngameWindow : MonoBehaviour, IInitializable
// {
// 	public event Action<bool> OnToggle;

// 	public static int StaticPriority => InitBootstrapper.DEFAULT_PRIORITY;
// 	public virtual int Priority => StaticPriority;

// 	protected virtual void Awake()
// 	{
// 		gameObject.SetActive(false);

// 		InitBootstrapper.StaticRegister(this);
// 	}

// 	public virtual void Initialize()
// 	{
// 		transform.SetParent(UIReferences.Instance.npcCanvas.transform, false);
// 	}

// 	public virtual void Toggle()
// 	{
// 		Toggle(!gameObject.activeSelf);
// 	}

// 	public virtual void Toggle(bool enable)
// 	{
// 		if (enable)
// 		{
// 			if (GameStateManager.Instance.CheckState(GameStateManager.GameState.Menu)) return;

// 			GameStateManager.Instance.SetState(GameStateManager.GameState.Menu);
// 		}
// 		else
// 		{
// 			GameStateManager.Instance.RemoveState(GameStateManager.GameState.Menu);
// 		}

// 		gameObject.SetActive(enable);

// 		OnToggle?.Invoke(enable);
// 	}
// }