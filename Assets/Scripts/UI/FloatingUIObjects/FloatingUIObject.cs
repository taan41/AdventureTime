using System;
using UnityEngine;

public abstract class FloatingUIObject : MonoBehaviour, IUpdatable
{
	public event Action<bool> OnEnabledChanged;
	protected void InvokeOnUpdatingChanged(bool updating) => OnEnabledChanged?.Invoke(updating);

	public bool Enabled { get; protected set; } = false;
	public bool UseUpdate { get; protected set; } = true;
	public bool UseFixedUpdate { get; protected set; } = false;
	public bool UseUnscaledTime { get; private set; } = false;

	public virtual void Enable(bool enabled)
	{
		Enabled = enabled;
		OnEnabledChanged?.Invoke(enabled);

		gameObject.SetActive(enabled);

		if (enabled) FloatingUIObjectManager.Instance.Register(this);
	}

	public virtual void DoUpdate(float deltaTime) { }
	public virtual void DoFixedUpdate(float deltaTime) { }
	public virtual void DoUnscaledUpdate(float unscaledDeltaTime) { }
}