using UnityEngine;
using static Character.StatType;

public partial class Character
{
	private void InitializeStats()
	{
		Stats[Size].OnStatChanged += SetSize;

		Stats[Health].OnStatChanged += () => HealthModule.SetMaxValue(Stats[Health].Final, true);
		Stats[HealthRegen].OnStatChanged += () => HealthModule.RegenAmount = Stats[HealthRegen].Final;
		Stats[HealthRegenInterval].OnStatChanged += () => HealthModule.RegenInterval = Stats[HealthRegenInterval].Final;
		Stats[HealthRegenDelay].OnStatChanged += () => HealthModule.RegenDelay = Stats[HealthRegenDelay].Final;

		Stats[Stamina].OnStatChanged += () => StaminaModule.SetMaxValue(Stats[Stamina].Final, true);
		Stats[StaminaRegen].OnStatChanged += () => StaminaModule.RegenAmount = Stats[StaminaRegen].Final;
		Stats[StaminaRegenInterval].OnStatChanged += () => StaminaModule.RegenInterval = Stats[StaminaRegenInterval].Final;
		Stats[StaminaRegenDelay].OnStatChanged += () => StaminaModule.RegenDelay = Stats[StaminaRegenDelay].Final;

		Stats[Mana].OnStatChanged += () => ManaModule.SetMaxValue(Stats[Mana].Final, true);
		Stats[ManaRegen].OnStatChanged += () => ManaModule.RegenAmount = Stats[ManaRegen].Final;
		Stats[ManaRegenInterval].OnStatChanged += () => ManaModule.RegenInterval = Stats[ManaRegenInterval].Final;
		Stats[ManaRegenDelay].OnStatChanged += () => ManaModule.RegenDelay = Stats[ManaRegenDelay].Final;

		Stats[MoveSpeed].OnStatChanged += () => MovementModule.Speed = Stats[MoveSpeed].Final;
		Stats[Size].OnStatChanged += () => MovementModule.OffsetSize = Stats[Size].Final;
	}

	private void RefreshDataStats()
	{
		Stats.Clear();
		Stats.CopyFrom(Data.properties.baseStats);

		SetSize();

		HealthModule.SetMaxValue(Stats[Health].Final, true, true);
		HealthModule.RegenAmount = Stats[HealthRegen].Final;
		HealthModule.RegenInterval = Stats[HealthRegenInterval].Final;
		HealthModule.RegenDelay = Stats[HealthRegenDelay].Final;

		StaminaModule.SetMaxValue(Stats[Stamina].Final, true, true);
		StaminaModule.RegenAmount = Stats[StaminaRegen].Final;
		StaminaModule.RegenInterval = Stats[StaminaRegenInterval].Final;
		StaminaModule.RegenDelay = Stats[StaminaRegenDelay].Final;

		ManaModule.SetMaxValue(Stats[Mana].Final, true, true);
		ManaModule.RegenAmount = Stats[ManaRegen].Final;
		ManaModule.RegenInterval = Stats[ManaRegenInterval].Final;
		ManaModule.RegenDelay = Stats[ManaRegenDelay].Final;

		MovementModule.Speed = Stats[MoveSpeed].Final;
		MovementModule.OffsetSize = Stats[Size].Final;
	}

	private void SetSize()
	{
		TransformCache.localScale = Stats[Size].Final * Vector3.one;
	}
}