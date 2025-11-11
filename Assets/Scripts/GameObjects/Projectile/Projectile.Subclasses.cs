using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Projectile
{
	public enum ProjectileType
	{
		Melee,
		Ranged,
	}

	public enum ProjectileStatType
	{
		SkillDamageFactor,
		Speed,
		Range,
		Duration,
		Size,
		HitInterval,
		Count,
		SpawnInterval,
		SpreadAngle,
		Knockback,
		KnockbackDuration,
	}

	[Serializable]
	public class ProjectileStat
	{
		public float damageFactor = 1f;
		public float healFactor = 0f;
		public float staminaRecoveryFactor = 0f;
		public float manaRecoveryFactor = 0f;
		public float size = 1f;
		public float speed = 0f;
		public float speedOverTimeFactor = 1f;
		public float range = 0f;
		public float hitInterval = 0f;
		public float knockback = 0f;
		public float knockbackDuration = 0f;
		public float spreadAngle = 0f;

		// public void CopyFrom(ProjectileStat other)
		// {
		// 	if (other == null) return;

		// 	damageFactor = other.damageFactor;
		// 	size = other.size;
		// 	speed = other.speed;
		// 	range = other.range;
		// 	duration = other.duration;
		// 	hitInterval = other.hitInterval;
		// 	knockback = other.knockback;
		// 	knockbackDuration = other.knockbackDuration;
		// 	spreadAngle = other.spreadAngle;
		// }
	}
	
	[Serializable]
	public class ProjectilePhaseData
	{
		public enum PhasePosition
		{
			Default,
			OwnerPosition,
			TargetPosition,
		}

		public PhasePosition phasePosition = PhasePosition.Default;
		public Vector2 positionOffset = Vector2.zero;
		public float directionOffset = 0f;
		public bool enableCollider = false;
		public bool findNewTarget = false;
		public float duration = 0f;
		public bool useDurationFactor = false;
		public bool loopAnimation = false;
		public AnimationModule.AnimationClipData animation = new();
		public float sfxChance = 0.6f;
		public List<AudioClip> sfxClips = new();

		public void CopyFrom(ProjectilePhaseData other)
		{
			if (other == null) return;

			phasePosition = other.phasePosition;
			positionOffset = other.positionOffset;
			directionOffset = other.directionOffset;
			enableCollider = other.enableCollider;
			duration = other.duration;
			useDurationFactor = other.useDurationFactor;
			loopAnimation = other.loopAnimation;
			animation.CopyFrom(other.animation);

			sfxChance = other.sfxChance;
			sfxClips = new List<AudioClip>(other.sfxClips);
		}
	}

	[Serializable]
	public class ProjectileSetting
	{
		[Serializable]
		public struct DisableCondition
		{
			public bool OnOwnerDeath;
			public bool OnMaxRange;
			public bool OnSpeedZero;
		}

		[Header("Gameplay Settings")]
		public DisableCondition additionalDisableCondition = new();
		public bool changePhaseOnAdditionalDisable = false;
		public int phaseLoop = 0;
		public bool ownerSizeAffectsSize = false;
		public bool homing = false;
		public bool followingMouse = false;
		public bool orbital = false;
		public bool randomSpreadAngle = false;
		public int targetingRange = 5;
		public int maxTargetingCall = 1;
		public bool overrideDirection = false;

		[Header("Transform Settings")]
		public Vector2 positionOffset = Vector2.zero;
		public float directionOffsetFactor = 0f;
		public bool sizeAffectsOffset = true;
		public bool rotateToDirection = true;
		public float rotateAngleOffset = 0f;
		public float rotateSpeed = 0f;

		[Header("Size Settings")]
		public bool shouldScaleX = true;
		public bool shouldScaleY = true;
		public bool useRendererSize = false;
		public bool useRendererX = false;
		public bool useRendererY = false;
		public float minRendererX = 1f;
		public float minRendererY = 1f;

		// public void CopyFrom(ProjectileSetting other)
		// {
		// 	if (other == null) return;

		// 	sizeAffectsOffset = other.sizeAffectsOffset;

		// 	rotateToDirection = other.rotateToDirection;
		// 	rotateAngleOffset = other.rotateAngleOffset;
		// 	rotateSpeed = other.rotateSpeed;

		// 	orbital = other.orbital;

		// 	shouldScaleX = other.shouldScaleX;
		// 	shouldScaleY = other.shouldScaleY;
		// 	useRendererSize = other.useRendererSize;
		// 	useRendererX = other.useRendererX;
		// 	useRendererY = other.useRendererY;
		// 	minRendererX = other.minRendererX;
		// 	minRendererY = other.minRendererY;
		// }
	}

	[Serializable]
	public class ProjectileDataPerSkill
	{
		public string projectileName = "Projectile";
		public int count = 1;
		public float spawnDelay = 0f;
		public float spawnGap = 0f;
		public ProjectileStat stats = new();
		public ProjectileSetting settings = new();
		public CombatEffectData combatEffectData = new();

		public void CopyFrom(ProjectileDataPerSkill other)
		{
			if (other == null) return;
		}
	}
}