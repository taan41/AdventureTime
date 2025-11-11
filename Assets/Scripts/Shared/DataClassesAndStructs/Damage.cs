using UnityEngine;

public struct Damage
{
	public float value;
	public float critChance;
	public float critMultiplier;
	public bool isCrit;
	public bool isFinal;

	public Damage(float value, float critChance = 0f, float critMultiplier = 1.5f, bool isCrit = false)
	{
		this.value = value;
		this.critChance = critChance;
		this.critMultiplier = critMultiplier;
		this.isCrit = isCrit;
		isFinal = false;
	}
}

public static class DamageExtensions
{
	public static Damage CalcFinalDamage(this Damage damage, float armor = 0f, float reduction = 0f, float bonusCritChance = 0f, float bonusCritMultiplier = 0f)
	{
		if (damage.isFinal) return damage;
		damage.isFinal = true;

		if (!damage.isCrit && damage.critChance + bonusCritChance > Random.value)
		{
			damage.value *= damage.critMultiplier + bonusCritMultiplier;
			damage.isCrit = true;
		}

		damage.value = damage.value * (1f - reduction) * (50f / (50f + armor));

		return damage;
	}
}