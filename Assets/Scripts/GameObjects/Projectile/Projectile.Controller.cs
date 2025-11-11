using System.Collections.Generic;
using UnityEngine;

public partial class Projectile
{
	private Vector3 MousePosition => MouseManager.Instance.MouseWorldPosition;

	[SerializeField] private Character target;

	private ProjectileStat Stats => specificData.stats;
	private ProjectileSetting Settings => specificData.settings;
	private List<ProjectilePhaseData> Phases => physicData.PhaseDatas;

	private Vector3 targetDirection;
	private Vector3 direction;
	private Vector3 skillPositionOffset;
	private Vector3 phasePositionOffset;
	private float skillDirectionFactor;
	private float phaseDirectionFactor;

	// Snapshot stats when activated
	private Damage damage;
	private float heal;
	private float staminaRecovery;
	private float manaRecovery;
	private float speed;
	private float range;
	private float size;
	private float hitInterval;
	private float knockback;
	private float knockbackDuration;
	private float spreadAngle;

	private float traveledDistance = 0f;
	private float activeDurationTimer = 0f;
	private int phaseIndex = 0;
	private int phaseLoopCount = 0;

	private void FixedUpdateController(float fixedDeltaTime)
	{
		if (Settings.followingMouse)
		{
			if (owner.ControlledByPlayer)
			{
				targetDirection = Settings.orbital
					? (MousePosition - ownerTransform.position).normalized
					: (MousePosition - TransformCache.position).normalized;
			}
			else if (Settings.orbital)
			{
				targetDirection = owner.lookDirection;
			}
		}
		else if (Settings.homing && target != null && target.Enabled)
		{
			targetDirection = (target.TransformCache.position - TransformCache.position).normalized;
		}

		targetDirection.z = 0f;

		if (direction != targetDirection && targetDirection != Vector3.zero)
		{
			if (Settings.rotateSpeed > 0f || Settings.orbital)
			{
				if (Vector3.Dot(direction, targetDirection) < -0.99f)
				{
					direction = Quaternion.Euler(0f, 0f, 5f) * direction;
				}
				direction = Vector3.RotateTowards(direction, targetDirection, (Settings.orbital ? speed : Settings.rotateSpeed) * fixedDeltaTime, 0f).normalized;

			}
			else direction = targetDirection;

			direction.z = 0f;

			if (!Settings.orbital)
			{
				MovementModule.Direction = direction;
			}

			if (Settings.rotateToDirection)
			{
				TransformCache.rotation = Quaternion.LookRotation(Vector3.forward, direction) * Quaternion.Euler(0f, 0f, Settings.rotateAngleOffset);
			}
		}

		if (Settings.orbital)
		{
			TransformCache.position = ownerTransform.position + skillPositionOffset + skillDirectionFactor * direction;
		}

		if (Stats.speedOverTimeFactor != 1f)
		{
			speed *= Stats.speedOverTimeFactor;
			if (speed < 0.01f) speed = 0f;
			if (!Settings.orbital) MovementModule.Speed = speed;
		}

		traveledDistance += speed * fixedDeltaTime;

		activeDurationTimer -= fixedDeltaTime;
		if (activeDurationTimer <= 0f)
		{
			StartNextPhase(TransformCache.position, direction);
			return;
		}

		if (Settings.additionalDisableCondition.OnMaxRange && traveledDistance >= range)
		{
			if (Settings.changePhaseOnAdditionalDisable)
			{
				traveledDistance = 0f;
				StartNextPhase(TransformCache.position, direction);
			}
			else
			{
				Enable(false);
			}
			return;
		}

		if (Settings.additionalDisableCondition.OnSpeedZero && speed <= 0f)
		{
			if (Settings.changePhaseOnAdditionalDisable)
			{
				StartNextPhase(TransformCache.position, direction);
			}
			else
			{
				Enable(false);
			}
			return;
		}
	}

	public void Activate(Vector3 activateDirection, Vector3 position, Character target, int projIndex = 0, int projCount = 1)
	{
		TransformCache.SetParent(null, false);

		SnapshotStats();

		this.target = FindTarget();

		if (Settings.overrideDirection && target != null)
		{
			direction = (target.TransformCache.position - position).normalized;
		}
		else
		{
			direction = activateDirection != Vector3.zero ? activateDirection.normalized : owner.direction;
		}
		
		direction.z = 0f;

		if (spreadAngle != 0f && projCount > 1)
		{
			float currentAngle = Settings.randomSpreadAngle
				? projectileRNG.NextFloat(-spreadAngle / 2f, spreadAngle / 2f)
				: spreadAngle * (projIndex - (projCount - 1) / 2f);
			direction = Quaternion.Euler(0f, 0f, currentAngle) * direction;
		}

		if (Settings.rotateToDirection && direction != Vector3.zero)
		{
			TransformCache.rotation = Quaternion.LookRotation(Vector3.forward, direction) * Quaternion.Euler(0f, 0f, Settings.rotateAngleOffset);
		}

		skillPositionOffset = Settings.positionOffset * (Settings.sizeAffectsOffset ? size : 1f);
		skillDirectionFactor = (Settings.sizeAffectsOffset ? size : 1f) * Settings.directionOffsetFactor;

		phaseIndex = -1;
		StartNextPhase(position, direction);

		traveledDistance = 0f;

		if (!Settings.orbital)
		{
			MovementModule.Enable(true);
			MovementModule.Direction = direction;
			MovementModule.Speed = speed;
		}
		else
		{
			MovementModule.Enable(false);
			MovementModule.Direction = Vector3.zero;
			MovementModule.Speed = 0f;
		}

		hitEnemies.Clear();
		hitAllies.Clear();
		intervalTimer = 0f;

		Enable(true);
	}

	private void StartNextPhase(Vector3 position, Vector3 direction)
	{
		if (Phases.Count == 0)
		{
			Enable(false);
			return;
		}

		phaseIndex++;
		if (phaseIndex >= Phases.Count)
		{
			if (phaseLoopCount < Settings.phaseLoop)
			{
				phaseLoopCount++;
			}
			else
			{
				phaseLoopCount = 0;
				Enable(false);
				return;
			}

			Activate(direction, position, target);
			return;
		}

		if (Phases[phaseIndex].findNewTarget)
		{
			target = FindTarget();
		}

		var phase = Phases[phaseIndex];
		Vector3 startPosition = phase.phasePosition switch
		{
			ProjectilePhaseData.PhasePosition.OwnerPosition => ownerTransform.position,
			ProjectilePhaseData.PhasePosition.TargetPosition => target != null ? target.TransformCache.position : position,
			ProjectilePhaseData.PhasePosition.Default or _ => position,
		};

		phasePositionOffset = (Vector3)phase.positionOffset * (Settings.sizeAffectsOffset ? size : 1f);
		phaseDirectionFactor = (Settings.sizeAffectsOffset ? size : 1f) * phase.directionOffset;

		TransformCache.position = startPosition + skillPositionOffset + phasePositionOffset + (skillDirectionFactor + phaseDirectionFactor) * direction;

		capsuleCollider.enabled = phase.enableCollider;

		activeDurationTimer = phase.duration > 0f ? phase.duration : phase.animation.GetDuration(EightWayDirection.Down);

		if (phase.useDurationFactor) activeDurationTimer *= skill.Stats[Skill.SkillStat.ProjectileDurationBonus].Final + 1f;

		AnimationPlayer.SetFlipX(phase.animation.IsFlipped(direction));
		AnimationPlayer.Play(phase.animation[direction], phase.animation.frameRate, true, phase.loopAnimation);

		AudioManager.Instance.PlayRandomSFX(phase.sfxClips, phase.sfxChance);
	}

	private Character FindTarget()
	{
		float closestDistanceSqr = float.MaxValue;
		Character target = null;
		Vector3 selfPosition = TransformCache.position;
		
		void CheckNextTarget(Character nextTarget)
		{
			if (nextTarget == null || !nextTarget.Enabled) return;

			float distanceSqr = (nextTarget.TransformCache.position - selfPosition).sqrMagnitude;
			if (distanceSqr < closestDistanceSqr)
			{
				closestDistanceSqr = distanceSqr;
				target = nextTarget;
			}
		}

		SpatialGrid.Instance.QueryFromCenter(
			owner.Tag.GetEnemy(),
			selfPosition,
			Settings.targetingRange,
			CheckNextTarget,
			Settings.maxTargetingCall
		);

		return target;
	}
	
	private void SnapshotStats()
	{
		damage = new(
			skill.Stats[Skill.SkillStat.Damage].Final * Stats.damageFactor,
			owner.Stats[Character.StatType.CritChance].Final,
			owner.Stats[Character.StatType.CritDamage].Final
		);

		heal = Stats.healFactor * skill.Stats[Skill.SkillStat.Recovery].Final;
		staminaRecovery = Stats.staminaRecoveryFactor * skill.Stats[Skill.SkillStat.Recovery].Final;
		manaRecovery = Stats.manaRecoveryFactor * skill.Stats[Skill.SkillStat.Recovery].Final;

		speed = Stats.speed * (skill.Stats[Skill.SkillStat.ProjectileSpeedBonus].Final + 1f);
		range = Stats.range * (skill.Stats[Skill.SkillStat.ProjectileRangeBonus].Final + 1f);
		hitInterval = Stats.hitInterval * (skill.Stats[Skill.SkillStat.ProjectileHitIntervalBonus].Final + 1f);
		
		knockback = Stats.knockback * (skill.Stats[Skill.SkillStat.ProjectileKnockbackBonus].Final + 1f);
		knockbackDuration = Stats.knockbackDuration * (skill.Stats[Skill.SkillStat.ProjectileKnockbackDurationBonus].Final + 1f);
		spreadAngle = Stats.spreadAngle * (skill.Stats[Skill.SkillStat.ProjectileSpreadAngleBonus].Final + 1f);

		SnapshotSize();
	}

	private void SnapshotSize()
	{
		float newSize = Stats.size * (skill.Stats[Skill.SkillStat.ProjectileSizeBonus].Final + 1f);
		if (Settings.ownerSizeAffectsSize) newSize *= owner.Stats[Character.StatType.Size].Final;

		if (size == newSize) return;

		size = newSize;

		if (compData != null && compData.drawMode != SpriteDrawMode.Simple && Settings.useRendererSize)
		{
			var rendererSize = new Vector2(
				Mathf.Max(baseRendererSize.x, Settings.minRendererX),
				Mathf.Max(baseRendererSize.y, Settings.minRendererY)
			);

			var scale = Vector3.one;

			if (Settings.useRendererX)
			{
				rendererSize.x *= size;
			}
			else if (Settings.shouldScaleX)
			{
				scale.x = size;
			}

			if (Settings.useRendererY)
			{
				rendererSize.y *= size;
			}
			else if (Settings.shouldScaleY)
			{
				scale.y = size;
			}

			spriteRenderer.size = rendererSize;
			TransformCache.localScale = scale;

			capsuleCollider.size = colliderBaseSize * rendererSize / baseRendererSize;
			capsuleCollider.offset = colliderBaseOffset * rendererSize / baseRendererSize;
		}
		else
		{
			TransformCache.localScale = new Vector3(Settings.shouldScaleX ? size : 1f, Settings.shouldScaleY ? size : 1f, 1f);
		}
	}

	private void OnOwnerDeath(Character _)
	{
		if (Settings.additionalDisableCondition.OnOwnerDeath)
		{
			Enable(false);
		}
	}
}