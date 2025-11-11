using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "ScriptableObjects/CombatEffects/DamageEffect")]
public class DamageEffect : CombatEffect
{
	[Serializable]
	public class DamageParameters : Parameters
	{
		public float damage = 0f;

		public DamageParameters() : base(typeof(DamageEffect)) { }
	}

	public override void ApplyEffect(ActivationData data, Parameters parameters)
	{
		if (EffectRNG.NextFloat() > parameters.chance) return;

		if (data.Target != null && parameters is DamageParameters dmgParams)
		{
			data.Target.TakeDamage(new(dmgParams.damage));
		}
	}

	public override Parameters CreateParametersInstance()
	{
		return new DamageParameters();
	}

	public override EffectWrapper CreateEntry(EffectCondition condition)
	{
		return CreateEntry(condition, new DamageParameters());
	}
}