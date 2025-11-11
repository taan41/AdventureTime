using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

#region Stats
[Serializable]
public class Stat
{
	[Serializable]
	public struct Multiplier
	{
		public enum MultiplierType
		{
			Additive,
			Multiplicative,
		}

		public MultiplierType type;
		public float value;

		public Multiplier(float value = 1f, MultiplierType type = MultiplierType.Multiplicative)
		{
			this.type = type;
			this.value = value;
		}

		public void CopyFrom(Multiplier other)
		{
			value = other.value;
			type = other.type;
		}
	}

	public event Action OnStatChanged;

	[HideInInspector] public string StatName = "Stat";
	[SerializeField] private float @base = 0f;
	[SerializeField] private float bonus = 0f;
	[SerializeField] private float flat = 0f;
	[SerializeField] private List<Multiplier> addMultipliers = new();
	[SerializeField] private List<Multiplier> multMultipliers = new();
	private float cachedAddMultiplier = 1f;
	private float cachedMultMultiplier = 1f;

	public float Base
	{
		get => @base;
		set
		{
			@base = value;
			CalculateFinal();
		}
	}
	
	public float Bonus
	{
		get => bonus;
		set
		{
			bonus = value;
			CalculateFinal();
		}
	}

	public float Flat
	{
		get => flat;
		set
		{
			flat = value;
			CalculateFinal();
		}
	}

	public float Final { get; private set; } = 0f;

	public Stat(string debugName = "Stat")
	{
		StatName = debugName;
	}

	public Stat(Stat other)
	{
		if (other == null) return;

		@base = other.@base;
		bonus = other.bonus;
		flat = other.flat;
		SetMultipliers(other.addMultipliers, other.multMultipliers);
		StatName = other.StatName;

		CalculateFinal(false);
	}

	public void CopyFrom(Stat other)
	{
		if (other == null) return;

		@base = other.@base;
		bonus = other.bonus;
		flat = other.flat;
		SetMultipliers(other.addMultipliers, other.multMultipliers);

		CalculateFinal();
	}

	public void CopyFrom(OneTypeStat other)
	{
		if (other == null) return;

		switch (other.Type)
		{
			case OneTypeStat.StatType.Base:
				@base = other.Value;
				break;
			case OneTypeStat.StatType.Bonus:
				bonus = other.Value;
				break;
		}

		CalculateFinal();
	}

	public void AddMultiplier(Multiplier multiplier)
	{
		switch (multiplier.type)
		{
			case Multiplier.MultiplierType.Additive:
				addMultipliers.Add(multiplier);
				cachedAddMultiplier += multiplier.value - 1f;
				break;
			case Multiplier.MultiplierType.Multiplicative:
				multMultipliers.Add(multiplier);
				cachedMultMultiplier *= multiplier.value;
				break;
		}
		CalculateFinal();
	}

	public void RemoveMultiplier(Multiplier multiplier)
	{
		switch (multiplier.type)
		{
			case Multiplier.MultiplierType.Additive:
				if (addMultipliers.Remove(multiplier))
				{
					if (addMultipliers.Count == 0) cachedAddMultiplier = 1f;
					else cachedAddMultiplier -= multiplier.value - 1f;
				}
				break;
			case Multiplier.MultiplierType.Multiplicative:
				if (multMultipliers.Remove(multiplier))
				{
					if (multMultipliers.Count == 0) cachedMultMultiplier = 1f;
					else cachedMultMultiplier /= multiplier.value;
				}
				break;
		}
		CalculateFinal();
	}

	private void SetMultipliers(List<Multiplier> otherAddMultipliers, List<Multiplier> otherMultMultipliers)
	{
		if (otherAddMultipliers == null) return;

		addMultipliers.Clear();
		cachedAddMultiplier = 1f;
		for (int i = 0; i < otherAddMultipliers.Count; i++)
		{
			addMultipliers.Add(new Multiplier(otherAddMultipliers[i].value));
			cachedAddMultiplier += otherAddMultipliers[i].value - 1f;
		}

		multMultipliers.Clear();
		cachedMultMultiplier = 1f;
		for (int i = 0; i < otherMultMultipliers.Count; i++)
		{
			multMultipliers.Add(new Multiplier(otherMultMultipliers[i].value));
			cachedMultMultiplier *= otherMultMultipliers[i].value;
		}

		CalculateFinal();
	}

	private void CalculateFinal(bool signalChange = true)
	{
		float oldFinal = Final;

		Final = (@base + bonus) * cachedAddMultiplier * cachedMultMultiplier + flat;

		if (signalChange && Final != oldFinal) SignalChange();
	}

	public void SignalChange() => OnStatChanged?.Invoke();
}

[Serializable]
public class OneTypeStat
{
	public enum StatType
	{
		Base,
		Bonus,
	}

	public event Action OnStatChanged;
	public void SignalChange() => OnStatChanged?.Invoke();

	[HideInInspector] public string StatName = "Stat";
	[HideInInspector] public StatType Type = StatType.Base;
	[SerializeField] private float value = 0f;

	public float Value
	{
		get => value;
		set
		{
			this.value = value;
			SignalChange();
		}
	}

	public OneTypeStat() { }

	public OneTypeStat(string statName, float value = 0f, StatType type = StatType.Base)
	{
		StatName = statName;
		Type = type;
		this.value = value;
	}

	public void CopyFrom(OneTypeStat other)
	{
		if (other == null) return;

		value = other.value;
	}

	public void CopyFrom(Stat other)
	{
		if (other == null) return;

		value = other.Base;
	}
}
#endregion

#region Stat Containers
[Serializable]
public class StatContainerNew<TEnum> where TEnum : struct, Enum
{
	private static readonly Dictionary<Type, List<TEnum>> enumLists = new();
	private static readonly Dictionary<Type, Dictionary<string, TEnum>> enumNameDictionaries = new();

	public event Action OnStatChanged;

	private readonly List<TEnum> enumList;
	private readonly Dictionary<string, TEnum> enumNames = new();

	public List<Stat> Stats = new();

	public StatContainerNew()
	{
		if (!enumLists.ContainsKey(typeof(TEnum)))
		{
			enumLists[typeof(TEnum)] = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
		}
		enumList = enumLists[typeof(TEnum)];

		if (!enumNameDictionaries.ContainsKey(typeof(TEnum)))
		{
			enumNameDictionaries[typeof(TEnum)] = new();
			enumNames = enumNameDictionaries[typeof(TEnum)];
			for (int i = 0; i < enumList.Count; i++)
			{
				enumNames[enumList[i].ToString()] = enumList[i];
			}
		}
		else enumNames = enumNameDictionaries[typeof(TEnum)];

		for (int i = 0; i < enumList.Count; i++)
		{
			Stat stat = new(enumList[i].ToString());
			stat.OnStatChanged += SignalChange;
			Stats.Add(stat);
		}
	}

	public Stat this[TEnum enumType]
	{
		get => Stats[(int)(object)enumType];
	}

	public void Clear()
	{
		for (int i = 0; i < Stats.Count; i++)
		{
			Stats[i].Base = 0f;
			Stats[i].Bonus = 0f;
			Stats[i].Flat = 0f;
		}
	}

	public void CopyFrom(StatContainerNew<TEnum> other = null)
	{
		if (other == null) return;

		for (int i = 0; i < enumList.Count; i++)
		{
			Stats[i].CopyFrom(other.Stats[i]);
		}
		OnStatChanged?.Invoke();
	}

	public void CopyFrom(OneTypeContainer<TEnum> other = null)
	{
		if (other == null) return;

		for (int i = 0; i < enumList.Count; i++)
		{
			Stats[i].CopyFrom(other.Stats[i]);
		}
		OnStatChanged?.Invoke();
	}

	public void ValidateList()
	{
		bool invalidList = false;

		if (Stats.Count > enumList.Count)
		{
			Stats.RemoveRange(enumList.Count, Stats.Count - enumList.Count);
		}

		if (Stats.Count < enumList.Count)
		{
			invalidList = true;
		}
		else
		{
			for (int i = 0; i < Stats.Count; i++)
			{
				if (Stats[i] == null || !enumList[i].ToString().Equals(Stats[i].StatName))
				{
					invalidList = true;
					break;
				}
			}
		}

		if (!invalidList)
		{
			SignalChangeAll();
			return;
		}

		Dictionary<TEnum, Stat> statDict = new();

		for (int i = Stats.Count - 1; i >= 0; i--)
		{
			if (Stats[i] != null && enumNames.ContainsKey(Stats[i].StatName))
			{
				statDict[enumNames[Stats[i].StatName]] = Stats[i];
			}
			else
			{
				Stats.RemoveAt(i);
			}
		}

		Stats.Clear();
		for (int i = 0; i < enumList.Count; i++)
		{
			if (statDict.ContainsKey(enumList[i]))
				Stats.Add(statDict[enumList[i]]);
			else
				Stats.Add(new(enumList[i].ToString()));
			Stats[i].SignalChange();
		}

		OnStatChanged?.Invoke();
	}

	public void SignalChangeAll()
	{
		foreach (var stat in Stats)
		{
			stat.SignalChange();
		}
		OnStatChanged?.Invoke();
	}

	private void SignalChange() => OnStatChanged?.Invoke();
}

[Serializable]
public class OneTypeContainer<TEnum> where TEnum : struct, Enum
{
	private static readonly Dictionary<Type, List<TEnum>> enumLists = new();
	private static readonly Dictionary<Type, Dictionary<string, TEnum>> enumNameDictionaries = new();
	private readonly List<TEnum> enumList;
	private readonly Dictionary<string, TEnum> enumNames;
	private readonly float defaultValue = 0f;

	public List<OneTypeStat> Stats = new();

	public OneTypeContainer(float defaultValue = 0f, OneTypeStat.StatType type = OneTypeStat.StatType.Base)
	{
		this.defaultValue = defaultValue;

		if (!enumLists.ContainsKey(typeof(TEnum)))
		{
			enumLists[typeof(TEnum)] = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
		}
		enumList = enumLists[typeof(TEnum)];

		if (!enumNameDictionaries.ContainsKey(typeof(TEnum)))
		{
			enumNameDictionaries[typeof(TEnum)] = new();
			enumNames = enumNameDictionaries[typeof(TEnum)];
			for (int i = 0; i < enumList.Count; i++)
			{
				enumNames[enumList[i].ToString()] = enumList[i];
			}
		}
		else enumNames = enumNameDictionaries[typeof(TEnum)];

		for (int i = 0; i < enumList.Count; i++)
		{
			Stats.Add(new(enumList[i].ToString(), defaultValue, type));
		}
	}

	public OneTypeStat this[int index]
	{
		get => Stats[index];
		private set => Stats[index] = value;
	}

	public OneTypeStat this[TEnum enumType]
	{
		get => Stats[(int)(object)enumType];
		private set => Stats[(int)(object)enumType] = value;
	}

	public void CopyFrom(OneTypeContainer<TEnum> other = null)
	{
		if (other == null) return;

		for (int i = 0; i < enumList.Count; i++)
		{
			Stats[i].CopyFrom(other.Stats[i]);
		}
	}

	public void CopyFrom(StatContainerNew<TEnum> other = null)
	{
		if (other == null) return;

		for (int i = 0; i < enumList.Count; i++)
		{
			Stats[i].CopyFrom(other.Stats[i]);
		}
	}

	public void ValidateList()
	{
		bool invalidList = false;

		if (Stats.Count > enumList.Count)
		{
			Stats.RemoveRange(enumList.Count, Stats.Count - enumList.Count);
		}

		if (Stats.Count < enumList.Count)
		{
			invalidList = true;
		}
		else
		{
			for (int i = 0; i < Stats.Count; i++)
			{
				if (Stats[i] == null || !enumList[i].ToString().Equals(Stats[i].StatName))
				{
					invalidList = true;
					break;
				}
			}
		}

		if (!invalidList)
		{
			SignalChangeAll();
			return;
		}

		Dictionary<TEnum, OneTypeStat> statDict = new();

		for (int i = Stats.Count - 1; i >= 0; i--)
		{
			if (Stats[i] != null && enumNames.ContainsKey(Stats[i].StatName))
			{
				statDict[enumNames[Stats[i].StatName]] = Stats[i];
			}
			else
			{
				Stats.RemoveAt(i);
			}
		}

		Stats.Clear();
		for (int i = 0; i < enumList.Count; i++)
		{
			if (statDict.ContainsKey(enumList[i]))
				Stats.Add(statDict[enumList[i]]);
			else
				Stats.Add(new(enumList[i].ToString()));
			Stats[i].SignalChange();
		}
	}

	public void Clear()
	{
		for (int i = 0; i < Stats.Count; i++)
		{
			Stats[i].Value = defaultValue;
		}
	}

	public void SignalChangeAll()
	{
		foreach (var stat in Stats)
		{
			stat.SignalChange();
		}
	}
}
#endregion