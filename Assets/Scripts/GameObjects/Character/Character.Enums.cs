using System;

using static Character.CharacterTag;

public partial class Character
{

	public enum CharacterTag
	{
		Hero,
		Monster,
		NPC
	}

	public enum StatType
	{
		STR,
		INT,
		AGI,
		VIT,

		Health,
		Stamina,
		Mana,

		HealthRegen,
		HealthRegenInterval,
		HealthRegenDelay,

		StaminaRegen,
		StaminaRegenInterval,
		StaminaRegenDelay,

		ManaRegen,
		ManaRegenInterval,
		ManaRegenDelay,

		DamageBuff,
		CritChance,
		CritDamage,
		AttackSpeed,

		Armor,
		MagicResist,
		DamageReduction,

		MoveSpeed,

		DashSpeedMultiplier,
		DashOvertimeFactor,
		DashDuration,
		DashInvulDurationFactor,
		DashStaminaCost,

		Size,

		KnockbackResistance,
		KnockbackOvertimeFactor,

		ImmunityDuration,
	}

	public static readonly StatType[] AllStatTypes = (StatType[])Enum.GetValues(typeof(StatType));

	public enum AIState
	{
		Idle,
		Chasing,
		Engaging,
		Fleeing,
		Regrouping,
	}

	public enum TargetFindMode
	{
		GridMode,
		ListMode,
	}
}

public static class CharacterTagExtensions
{
	public static bool IsAlly(this Character.CharacterTag tag, Character.CharacterTag otherTag)
	{
		// Define ally relationships
		if (tag == Hero && otherTag == NPC) return true;
		if (tag == NPC && otherTag == Hero) return true;

		return tag == otherTag;
	}

	public static Character.CharacterTag GetEnemy(this Character.CharacterTag tag)
	{
		return tag switch
		{
			Hero or NPC => Monster,
			Monster => Hero,
			_ => tag,
		};
	}
}