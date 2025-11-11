using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillHandler
{
	public event Action OnSkillsChange;

	public Character Owner { get; private set; }
	public List<Skill> Skills { get; private set; } = new();

	private readonly List<Skill> weaponSkills = new();
	private readonly List<Skill> primarySkills = new();

	private bool enabled = true;

	public SkillHandler(Character owner)
	{
		Owner = owner;
	}

	public void Enable(bool enabled = true)
	{
		this.enabled = enabled;

		for (int i = 0; i < Skills.Count; i++)
		{
			Skills[i].Enable(enabled);
		}
	}

	public void AddSkillData(SkillData skillData, bool sortAfter = true, bool signal = true)
	{
		if (skillData == null || !skillData.IsValid()) return;

		var newSkill = SkillFactory.Create(Owner, skillData);
		Skills.Add(newSkill);
		newSkill.Enable(enabled);

		if (sortAfter)
		{
			Skills.Sort((a, b) => a.Data.type.CompareTo(b.Data.type));
		}

		if (skillData.type == Skill.SkillType.Weapon)
		{
			weaponSkills.Add(newSkill);
		}
		else if (skillData.type == Skill.SkillType.Primary)
		{
			primarySkills.Add(newSkill);
		}

		if (signal) OnSkillsChange?.Invoke();
	}

	public void AddSkillDatas(List<SkillData> skillDatas, bool clearExisting = false)
	{
		if (skillDatas == null || skillDatas.Count == 0) return;

		if (clearExisting)
		{
			Clear(false);
		}

		for (int i = 0; i < skillDatas.Count; i++)
		{
			AddSkillData(skillDatas[i], false, false);
		}

		Skills.Sort((a, b) => a.Data.type.CompareTo(b.Data.type));

		OnSkillsChange?.Invoke();
	}

	public void RemoveSkillData(SkillData skillData, bool signal = true)
	{
		if (skillData == null || !skillData.IsValid()) return;

		for (int i = Skills.Count - 1; i >= 0; i--)
		{
			if (Skills[i].ValidData && Skills[i].Data == skillData)
			{
				if (Skills[i].Data.type == Skill.SkillType.Weapon)
				{
					weaponSkills.Remove(Skills[i]);
				}
				else if (Skills[i].Data.type == Skill.SkillType.Primary)
				{
					primarySkills.Remove(Skills[i]);
				}

				Skills[i].Enable(false);
				Skills[i].SetData(null);
				Skills.RemoveAt(i);
				break;
			}
		}

		if (signal) OnSkillsChange?.Invoke();
	}

	public void RemoveSkillDatas(List<SkillData> skillDatas)
	{
		if (skillDatas == null || skillDatas.Count == 0) return;

		for (int i = skillDatas.Count - 1; i >= 0; i--)
		{
			RemoveSkillData(skillDatas[i], false);
		}

		OnSkillsChange?.Invoke();
	}

	public void Clear(bool signal = true)
	{
		for (int i = Skills.Count - 1; i >= 0; i--)
		{
			Skills[i].Enable(false);
			Skills[i].SetData(null);
			Skills.RemoveAt(i);
		}

		weaponSkills.Clear();
		primarySkills.Clear();

		if (signal) OnSkillsChange?.Invoke();
	}

	public bool HasOffCooldown(Skill.SkillType type)
	{
		switch (type)
		{
			case Skill.SkillType.Weapon:
				if (weaponSkills.Count > 0)
				{
					for (int i = 0; i < weaponSkills.Count; i++)
					{
						if (weaponSkills[i].CooldownTimer <= 0f)
						{
							return true;
						}
					}
				}
				break;

			case Skill.SkillType.Primary:
				if (primarySkills.Count > 0)
				{
					for (int i = 0; i < primarySkills.Count; i++)
					{
						if (primarySkills[i].CooldownTimer <= 0f)
						{
							return true;
						}
					}
				}
				break;
		}
		return false;
	}

	public Skill GetOffCooldownSkill(Skill.SkillType type)
	{
		switch (type)
		{
			case Skill.SkillType.Weapon:
				for (int i = 0; i < weaponSkills.Count; i++)
				{
					if (weaponSkills[i].CooldownTimer <= 0f)
					{
						return weaponSkills[i];
					}
				}
				break;

			case Skill.SkillType.Primary:
				for (int i = 0; i < primarySkills.Count; i++)
				{
					if (primarySkills[i].CooldownTimer <= 0f)
					{
						return primarySkills[i];
					}
				}
				break;
		}
		return null;
	}

	public int GetSkillIndex(Skill skill)
	{
		return skill.Data.type switch
		{
			Skill.SkillType.Weapon => weaponSkills.IndexOf(skill),
			Skill.SkillType.Primary => primarySkills.IndexOf(skill),
			_ => -1,
		};
	}

	// public (bool activated, float castTime, float channelTime) ActivateAttackSkill(Vector3 direction)
	// {
	// 	if (weaponSkills.Count == 0) return (false, 0f, 0f);

	// 	for (int i = 0; i < weaponSkills.Count; i++)
	// 	{
	// 		var (activated, castTime, channelTime) = weaponSkills[i].Activate(direction);
	// 		if (activated) return (true, castTime, channelTime);
	// 	}
	// 	return (false, 0f, 0f);
	// }

	// public bool ActivateWeaponSkill(Vector3 direction)
	// {
	// 	if (weaponSkills.Count == 0) return false;

	// 	for (int i = 0; i < weaponSkills.Count; i++)
	// 	{
	// 		if (weaponSkills[i].Activate(direction).activated) return true;
	// 	}
	// 	return false;
	// }

	// public bool ActivatePrimarySkill(Vector3 direction, int index = 0)
	// {
	// 	if (primarySkills.Count == 0) return false;

	// 	if (primarySkills[index].Activate(direction).activated) return true;
	// 	return false;
	// }
}