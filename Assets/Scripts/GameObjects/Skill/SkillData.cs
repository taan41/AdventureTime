using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Data", menuName = "ScriptableObjects/SkillData")]
public class SkillData : CustomSO<SkillData>
{
	[Serializable]
	public class ProjectileDataEntry
	{
		public ProjectileData physicData;
		public Projectile.ProjectileDataPerSkill specificData;

		public bool IsValid => physicData != null && specificData != null;
	}

	public Sprite icon = null;
	public string skillName = "New Skill";
	public string description = "Skill Description";
	public Skill.SkillType type = Skill.SkillType.Weapon;
	public Skill.SkillRangeType rangeType = Skill.SkillRangeType.Generic;
	public Skill.SkillDirection directionType = Skill.SkillDirection.Forward;
	public OneTypeContainer<Skill.SkillStat> baseStats = new();
	public Skill.SkillRules rules = new();
	public List<Skill.SkillPhaseData> phaseData = new();
	public List<ProjectileDataEntry> projectileDatas;

	protected override void OnValidate()
	{
		baseStats.ValidateList();

		base.OnValidate();
	}

	public override void CopyFrom(SkillData other)
	{
		if (other == null) return;

		projectileDatas = other.projectileDatas;
		SignalDataChange();
	}
}