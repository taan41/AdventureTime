using System;

[Serializable]
public struct CoreStat
{
	public enum CoreType
	{
		Strength,
		Vitality,
		Intelligence,
		Agility,
	}

	public static CoreType[] AllTypes = (CoreType[])Enum.GetValues(typeof(CoreType));

	public CoreType Type;
	public float Value;
}

[Serializable]
public struct CombatStat
{
	public enum CombatType
	{
		Health,
		Speed,
		Cooldown,
		JumpHeight,
		JumpCount,
		DashSpeed,
		DashDuration,
	}
	public static CombatType[] AllTypes = (CombatType[])Enum.GetValues(typeof(CombatType));

	public CombatType Type;
	public float Value;
}