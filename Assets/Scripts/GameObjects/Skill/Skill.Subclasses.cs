using System;
using System.Collections.Generic;

public partial class Skill
{
	public enum SkillStat
	{
		Damage,
		Recovery,
		Cooldown,
		StaminaCost,
		ManaCost,
		CastTime,
		ActivateRange,
		ProjectileSpeedBonus,
		ProjectileRangeBonus,
		ProjectileDurationBonus,
		ProjectileSizeBonus,
		ProjectileHitIntervalBonus,
		ProjectileKnockbackBonus,
		ProjectileKnockbackDurationBonus,
		ProjectileSpreadAngleBonus,
		ProjectileCountBonus,
	}

	public static readonly SkillStat[] AllSkillStats = (SkillStat[])System.Enum.GetValues(typeof(SkillStat));

	public enum SkillPhase
	{
		Startup,
		Active,
		Completion,
	}

	public enum SkillType
	{
		Weapon,
		Primary,
		Secondary,
		Special,
		Passive,
	}

	public enum SkillRangeType
	{
		Generic,
		Melee,
		Ranged,
	}

	public enum SkillDirection
	{
		None,
		Forward,
		Target,
		Aimed,
	}

	[Serializable]
	public class SkillProjectileData : IGameObjectData
	{
		public ProjectileData projectileData;
		public OneTypeContainer<Projectile.ProjectileStatType> stats = new();

		public void CopyFrom(IGameObjectData other)
		{
			if (other is not SkillProjectileData otherData) return;

			projectileData = otherData.projectileData;

			stats.CopyFrom(otherData.stats);
		}
	}

	[Serializable]
	public class SkillPhaseData : IGameObjectData
	{
		[Serializable]
		public struct ProjectileCount
		{
			public ProjectileData projectileData;
			public int count;
		}

		[Serializable]
		public struct IndividualPhaseData
		{
			public SkillPhase phase;
			public float duration;
			public List<ProjectileCount> projectiles;
		}

		public SkillPhase cooldownStartPhase = SkillPhase.Completion;
		public List<IndividualPhaseData> phases = new();

		public void CopyFrom(IGameObjectData other)
		{
			if (other is not SkillPhaseData otherData) return;

			cooldownStartPhase = otherData.cooldownStartPhase;

			phases.Clear();
			phases.AddRange(otherData.phases);
		}
	}

	[Serializable]
	public class SkillRules : IGameObjectData
	{
		public bool autoCast = false;
		public bool cooldownAfterCast = false;

		public void CopyFrom(IGameObjectData other)
		{
			if (other == null) return;
			if (other is not SkillRules otherRules) return;

			autoCast = otherRules.autoCast;
			cooldownAfterCast = otherRules.cooldownAfterCast;
		}
	}
}