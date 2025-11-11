using System.Collections.Generic;

/// <summary>
/// Use UpdaterManager instead
/// </summary>
public class Updater<T> : IUpdater where T : IUpdatable
{
	public int Order { get; set; } = 0;

	private readonly List<T> updateList = new();
	private readonly List<T> fixedUpdateList = new();
	private readonly List<T> unscaledUpdateList = new();

	public void DoUpdate(float deltaTime)
	{
		T updatable;
		for (int i = updateList.Count - 1; i >= 0; i--)
		{
			updatable = updateList[i];
			if (!updatable.Enabled) continue;
			updatable.DoUpdate(deltaTime);
		}
	}

	public void DoFixedUpdate(float fixedDeltaTime)
	{
		T updatable;
		for (int i = fixedUpdateList.Count - 1; i >= 0; i--)
		{
			updatable = fixedUpdateList[i];
			if (!updatable.Enabled) continue;
			updatable.DoFixedUpdate(fixedDeltaTime);
		}
	}

	public void DoUnscaledUpdate(float unscaledDeltaTime)
	{
		T updatable;
		for (int i = unscaledUpdateList.Count - 1; i >= 0; i--)
		{
			updatable = unscaledUpdateList[i];
			if (!updatable.Enabled) continue;
			updatable.DoUnscaledUpdate(unscaledDeltaTime);
		}
	}

	public void Register(T updatable)
	{
		if (updatable == null) return;

		if (updatable.UseUpdate && !updateList.Contains(updatable))
		{
			updateList.Add(updatable);
		}

		if (updatable.UseFixedUpdate && !fixedUpdateList.Contains(updatable))
		{
			fixedUpdateList.Add(updatable);
		}

		if (updatable.UseUnscaledTime && !unscaledUpdateList.Contains(updatable))
		{
			unscaledUpdateList.Add(updatable);
		}
	}
}