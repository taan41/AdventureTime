using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatEffect : ScriptableObject
{
	protected static Unity.Mathematics.Random EffectRNG = new((uint)DateTime.Now.Ticks);

	public enum EffectCondition
	{
		OnHit,
		OnCrit,
		OnBlock,
		OnDodge,
		OnParry,
		OnTakeDamage,
		OnHeal,
	}

	public static readonly EffectCondition[] AllConditions = (EffectCondition[])System.Enum.GetValues(typeof(EffectCondition));

	[Serializable]
	public class EffectWrapper
	{
		public EffectCondition condition;
		public CombatEffect effect;
		[SerializeReference] public Parameters parameters;
	}

	[Serializable]
	public class Parameters
	{
		public Type effectClassType;
		[Range(0f, 1f)] public float chance = 1f;

		public Parameters(Type type) { effectClassType = type; }
	}

	public struct ActivationData
	{
		public Character Character;
		public Character Target;
		public Skill Skill;
		public Vector3 Position;

		public ActivationData(Character source, Character target, Skill skill, Vector3 position = default)
		{
			Character = source;
			Target = target;
			Skill = skill;
			Position = position;
		}
	}

	public abstract void ApplyEffect(ActivationData activation, Parameters parameters);

	public abstract Parameters CreateParametersInstance();

	public abstract EffectWrapper CreateEntry(EffectCondition condition);

	protected EffectWrapper CreateEntry(EffectCondition condition, Parameters parameters)
	{
		return new()
		{
			condition = condition,
			effect = this,
			parameters = parameters,
		};
	}
}

[Serializable]
public class CombatEffectData
{
	public List<CombatEffect.EffectWrapper> effects = new();

	public void OnValidate()
	{
		for (int i = effects.Count - 1; i >= 0; i--)
		{
			if (effects[i].effect == null)
			{
				effects[i].parameters = null;
			}
			else if (effects[i].parameters == null || effects[i].parameters.effectClassType != effects[i].effect.GetType())
			{
				effects[i].parameters = effects[i].effect.CreateParametersInstance();
			}
		}
	}
}