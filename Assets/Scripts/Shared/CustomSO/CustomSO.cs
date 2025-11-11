using System;
using UnityEngine;

public abstract class CustomSO<T> : ScriptableObject where T : CustomSO<T>
{
	public event Action OnDataChanged;
	public void SignalDataChange()
	{
		if (Application.isPlaying) OnDataChanged?.Invoke();
		else OnDataChanged = null;
	}

	[Header("Other SO to copy from")]
	[SerializeField] protected T copyFrom;

	public abstract void CopyFrom(T other);

	public virtual bool IsValid() => true;


	protected virtual void OnValidate()
	{
		if (copyFrom != null)
		{
			CopyFrom(copyFrom);
			copyFrom = null;
		}

		SignalDataChange();
	}
}