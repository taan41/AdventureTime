using System.Collections.Generic;
using System;

using static StatContainer.ContainerType;
using static CoreStat;
using static CombatStat;
public class StatContainer
{
	public enum ContainerType
	{
		Base,
		Bonus,
		Multiplier,
		Final
	}

	public static ContainerType[] AllTypes = (ContainerType[])Enum.GetValues(typeof(ContainerType));

	public event Action<CoreType, float> OnCoreStatChanged;
	public event Action<CombatType, float> OnCombatStatChanged;

	public Dictionary<ContainerType, Dictionary<CoreType, float>> CoreStats { get; private set; } = new();
	public Dictionary<ContainerType, Dictionary<CombatType, float>> CombatStats { get; private set; } = new();

	public StatContainer()
	{
		foreach (var containerType in AllTypes)
		{
			CoreStats[containerType] = new();
			foreach (var coreType in CoreStat.AllTypes)
			{
				CoreStats[containerType][coreType] = 0f;
			}

			CombatStats[containerType] = new();
			foreach (var combatType in CombatStat.AllTypes)
			{
				CombatStats[containerType][combatType] = 0f;
			}
		}
	}

	public float GetCoreStat(CoreType coreType, ContainerType containerType = Final)
		=> CoreStats[containerType][coreType];

	public float GetCombatStat(CombatType combatType, ContainerType containerType = Final)
		=> CombatStats[containerType][combatType];

	public void ModifyCoreStat(ContainerType containerType, CoreType coreType, float value)
	{
		if (containerType == Final) return;

		CoreStats[containerType][coreType] += value;
		CoreStats[Final][coreType] = (CoreStats[Base][coreType] + CoreStats[Bonus][coreType]) * (1 + CoreStats[Multiplier][coreType]);

		OnCoreStatChanged?.Invoke(coreType, CoreStats[Final][coreType]);
	}

	public void ModifyCombatStat(ContainerType containerType, CombatType combatType, float value)
	{
		if (containerType == Final) return;

		CombatStats[containerType][combatType] += value;
		CombatStats[Final][combatType] = (CombatStats[Base][combatType] + CombatStats[Bonus][combatType]) * (1 + CombatStats[Multiplier][combatType]);

		OnCombatStatChanged?.Invoke(combatType, CombatStats[Final][combatType]);
	}
}