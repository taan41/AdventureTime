using System.Collections.Generic;

using static CombatEffect;

public class CombatEffectHandler
{
	private readonly Dictionary<EffectCondition, List<EffectWrapper>> effects = new();
	private readonly List<CombatEffectHandler> linkedHandlers = new();

	public CombatEffectHandler()
	{
		foreach (var condition in AllConditions)
		{
			effects[condition] = new();
		}
	}

	public void SetData(CombatEffectData data)
	{
		if (data == null) return;

		foreach (var condition in AllConditions)
		{
			effects[condition].Clear();
		}
		
		foreach (var entry in data.effects)
		{
			if (entry.effect != null)
			{
				effects[entry.condition].Add(entry);
			}
		}
	}

	public void AddEffect(EffectWrapper wrapper)
	{
		effects[wrapper.condition].Add(wrapper);
	}

	public void RemoveEffect(EffectWrapper wrapper)
	{
		effects[wrapper.condition].Remove(wrapper);
	}

	public void LinkHandler(CombatEffectHandler handler)
	{
		if (handler != null && !linkedHandlers.Contains(handler))
		{
			linkedHandlers.Add(handler);
		}
	}

	public void UnlinkHandler(CombatEffectHandler handler)
	{
		if (handler != null)
		{
			linkedHandlers.Remove(handler);
		}
	}

	public void ActivateEffects(EffectCondition condition, ActivationData data)
	{
		EffectWrapper wrapper;
		for (int i = 0; i < effects[condition].Count; i++)
		{
			wrapper = effects[condition][i];
			wrapper.effect.ApplyEffect(data, wrapper.parameters);
		}

		for (int i = 0; i < linkedHandlers.Count; i++)
		{
			linkedHandlers[i].ActivateEffects(condition, data);
		}
	}
}