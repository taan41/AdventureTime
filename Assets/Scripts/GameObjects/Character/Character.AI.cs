using System.Collections.Generic;
using UnityEngine;

using static Character.AIState;
using static Character.TargetFindMode;

public partial class Character
{
	#region AI Variables
	// ---- Statics
	private static readonly Collider2D[] _overlapResults = new Collider2D[64];

	// ---- Settings
	private CharacterAISetting AISettings => Data.settings.ai;

	// ---- State
	public AIState State = Idle;
	public Vector3 AIDirection = Vector3.zero;
	private float aiStateTimer;

	// ---- Targeting
	public Character Target;
	private ContactFilter2D targetFilter = new() { useLayerMask = true, useTriggers = true };
	private bool IsTargetValid => Target != null && Target.Enabled;
	private Vector3 TargetVector => IsTargetValid ? Target.TransformCache.position - TransformCache.position : Vector3.zero;

	// ---- Regrouping
	public Character groupLeader;
	private bool IsLeaderValid => groupLeader != null && groupLeader.Enabled;
	private Vector3 LeaderVector => IsLeaderValid ? groupLeader.TransformCache.position - TransformCache.position : Vector3.zero;

	// ---- Distances
	private float TargetDistanceSqr => TargetVector.sqrMagnitude;
	private float LeaderDistanceSqr => LeaderVector.sqrMagnitude;
	private float regroupDistance;
	private float RegroupDistanceSqr => regroupDistance * regroupDistance;
	private float regroupIdleDistance;
	private float RegroupIdleDistanceSqr => regroupIdleDistance * regroupIdleDistance;
	private float regroupRestDistance;
	private float RegroupRestDistanceSqr => regroupRestDistance * regroupRestDistance;
	private float findDistance;
	private float FindDistanceSqr => findDistance * findDistance;
	private float chaseDistance;
	private float ChaseDistanceSqr => chaseDistance * chaseDistance;
	private float engageDistance;
	private float EngageDistanceSqr => engageDistance * engageDistance;
	private float fleeDistance;
	private float FleeDistanceSqr => fleeDistance * fleeDistance;

	// ---- Delays
	private int stateUpdateDelay;
	private int targetingDelay;
	private int boidsDelay;
	private int fleeCalculationDelay;

	// ---- Boids
	private Vector3 boidsDirection = Vector3.zero;

	// ---- Engaging
	private Skill lastUsedSkill;

	// ---- Fleeing
	private readonly List<Character> fleeTargets = new();
	private Vector3 fleeDirection = Vector3.zero;
	private bool dashing = false;
	#endregion

	#region Updates
	public void DoFixedUpdateAI(float fixedDeltaTime)
	{
		UpdateAICounters();

		UpdateAITarget();

		if (aiStateTimer > 0f)
		{
			aiStateTimer -= fixedDeltaTime;
		}
		else
		{
			UpdateAIState();
		}

		UpdateAIDirection();
		UpdateAIAction();
	}

	private void UpdateAICounters()
	{
		if (stateUpdateDelay > 0) stateUpdateDelay--;
		if (boidsDelay > 0) boidsDelay--;
		if (targetingDelay > 0) targetingDelay--;
		if (fleeCalculationDelay > 0) fleeCalculationDelay--;
	}

	private void UpdateAITarget()
	{
		if (targetingDelay > 0) return;

		FindTarget();

		if (IsTargetValid) targetingDelay = AISettings.findDelayOnSuccess;
		else targetingDelay = AISettings.findDelayOnFail;
	}

	private void UpdateAIState()
	{
		if (stateUpdateDelay > 0) return;
		stateUpdateDelay = AISettings.stateUpdateDelay;

		if (IsLeaderValid)
		{
			var leaderDistanceSqr = LeaderDistanceSqr;
			if ((State == Idle && leaderDistanceSqr > RegroupIdleDistanceSqr) || leaderDistanceSqr > RegroupDistanceSqr)
			{
				State = Regrouping;
				aiStateTimer = AISettings.regroupDuration;
				dashing = true;
				return;
			}
		}

		if (!IsTargetValid) return;

		dashing = false;
		float targetDistanceSqr = TargetDistanceSqr;
		bool fleeing = false;

		var skill = SkillHandler.GetOffCooldownSkill(Skill.SkillType.Primary);
		skill ??= SkillHandler.GetOffCooldownSkill(Skill.SkillType.Weapon);

		if (skill != null)
		{
			lastUsedSkill = skill;

			float skillRange = skill.Stats[Skill.SkillStat.ActivateRange].Final;
			findDistance = skillRange * AISettings.offCdFindingDistanceFactor;
			chaseDistance = skillRange * AISettings.offCdChaseDistanceFactor;
			engageDistance = skillRange * AISettings.offCdEngageDistanceFactor;
			fleeDistance = skillRange * AISettings.offCdFleeDistanceFactor;
		}
		else
		{
			if (lastUsedSkill != null)
			{
				if (AISettings.fleeWhenNotEngaging)
				{
					float skillRange = lastUsedSkill.Stats[Skill.SkillStat.ActivateRange].Final;
					findDistance = skillRange * AISettings.onCdFindingDistanceFactor;
					chaseDistance = skillRange * AISettings.onCdChaseDistanceFactor;
					engageDistance = skillRange * AISettings.onCdEngageDistanceFactor;
					fleeDistance = skillRange * AISettings.onCdFleeDistanceFactor;
				}
			}
			else
			{
				findDistance = AISettings.findDistance;
				chaseDistance = AISettings.chaseDistance;
				engageDistance = AISettings.engageDistance;
				fleeDistance = AISettings.fleeDistance;
			}

			fleeing = CalculateFleeDirection();
		}

		if (fleeing)
		{
			State = Fleeing;
			aiStateTimer = AISettings.fleeDuration;
			dashing = AISettings.dashWhileFleeing && targetDistanceSqr < (FleeDistanceSqr * 0.01f);
		}
		else if (targetDistanceSqr < EngageDistanceSqr)
		{
			State = Engaging;
			direction = TargetVector.normalized;
			aiStateTimer = UsingSkill(skill).duration;
		}
		else if (AISettings.alwaysChase || targetDistanceSqr < ChaseDistanceSqr)
		{
			State = Chasing;
			aiStateTimer = AISettings.chaseDuration;
			dashing = AISettings.dashWhileChasing && targetDistanceSqr > (ChaseDistanceSqr * 0.64f);
		}
		else
		{
			State = Idle;
			aiStateTimer = 0f;
		}
	}

	private void UpdateAIDirection()
	{
		bool fleeWhileRegrouping = AISettings.fleeWhileRegrouping && State == Regrouping;

		if (State == Fleeing) CalculateFleeDirection();
		else if (fleeWhileRegrouping) CalculateFleeDirection(AISettings.fleeWhileRegroupingFactor);

		AIDirection = State switch
		{
			Engaging => TargetVector.normalized,
			Chasing => TargetVector.normalized,
			Fleeing => fleeDirection,
			Regrouping => LeaderVector.normalized,
			_ => Vector3.zero,
		};

		if (fleeWhileRegrouping)
		{
			AIDirection += fleeDirection;
			AIDirection.Normalize();
		}

		if (AISettings.useBoids && State == Chasing)
		{
			if (boidsDelay <= 0)
			{
				boidsDelay = AISettings.boidsUpdateDelay;
				CalculateBoidsDirection();
			}

			if (boidsDirection != Vector3.zero)
			{
				AIDirection += boidsDirection;
				AIDirection.Normalize();
			}
		}

		direction = AIDirection;
	}

	private void UpdateAIAction()
	{
		switch (State)
		{
			case Idle:
				Idling();
				break;

			case Regrouping:
				var leaderDistanceSqr = LeaderDistanceSqr;

				if (leaderDistanceSqr <= RegroupRestDistanceSqr)
				{
					State = Idle;
					aiStateTimer = 0f;
					Idling();
					break;
				}

				if (dashing && leaderDistanceSqr >= RegroupDistanceSqr * 0.5f && Dashing(AIDirection))
				{
					dashing = false;
					break;
				}

				Moving();
				break;

			case Engaging:
				break;

			case Fleeing:
			case Chasing:
				if (dashing && Dashing(AIDirection))
				{
					dashing = false;
					break;
				}
				Moving();
				break;
		}
	}
	#endregion

	#region Private Methods
	private void RefreshDataAI()
	{
		regroupIdleDistance = AISettings.regroupIdleDistance;
		regroupDistance = AISettings.regroupDistance;
		regroupRestDistance = AISettings.regroupRestDistance;

		findDistance = AISettings.findDistance;

		fleeDistance = AISettings.fleeDistance;
		engageDistance = AISettings.engageDistance;
		chaseDistance = AISettings.chaseDistance;

		targetFilter.layerMask = LayerMask.GetMask(AISettings.targetTag.ToString());

		State = Idle;
		aiStateTimer = 0f;
		stateUpdateDelay = CharacterRNG.NextInt(0, 5);
		boidsDelay = CharacterRNG.NextInt(0, 5);
		targetingDelay = CharacterRNG.NextInt(0, 5);
		Target = null;
		fleeTargets.Clear();
		AIDirection = Vector3.zero;
	}

	private void FindTarget()
	{
		Vector3 selfPos = TransformCache.position;
		float closestDistanceSqr = float.MaxValue;
		Character newTarget = null;

		switch (AISettings.findMode)
		{
			case ListMode:

				foreach (Character character in ActiveCharacters[AISettings.targetTag])
				{
					if (character == null || character == this) continue;

					float distanceSqr = (character.TransformCache.position - selfPos).sqrMagnitude;

					if (AISettings.useDistanceOnList && distanceSqr > FindDistanceSqr) continue;
					if (distanceSqr >= closestDistanceSqr) continue;

					newTarget = character;
					closestDistanceSqr = distanceSqr;
				}
				break;

			case GridMode:
				SpatialGrid.Instance.QueryFromCenter(AISettings.targetTag, CurrentCell, (int)findDistance, EvaluateTarget, AISettings.maxFindTargetCount);
				break;
		}

		void EvaluateTarget(Character possibleTarget)
		{
			if (possibleTarget == null || !possibleTarget.Enabled) return;

			float distanceSqr = (possibleTarget.TransformCache.position - selfPos).sqrMagnitude;

			if (distanceSqr >= closestDistanceSqr) return;

			newTarget = possibleTarget;
			closestDistanceSqr = distanceSqr;
		}
		
		Target = newTarget;
	}
	

	private bool CalculateFleeDirection(float finalFactor = 1f)
	{
		if (fleeCalculationDelay > 0) return fleeDirection != Vector3.zero;
		fleeCalculationDelay = AISettings.fleeCalculationDelay;

		if (AISettings.onlyFleeFromTarget)
		{
			if (!IsTargetValid) return false;
			if (TargetDistanceSqr > FleeDistanceSqr) return false;

			fleeDirection = -TargetVector.normalized;
			fleeDirection *= finalFactor;
			return true;
		}

		fleeDirection = Vector3.zero;
		SpatialGrid.Instance.QueryFromCenter(AISettings.targetTag, CurrentCell, Mathf.CeilToInt(fleeDistance / SpatialGrid.Instance.CellSize), SetFleeDirection, AISettings.fleeTargetCount);

		if (fleeDirection == Vector3.zero) return false;

		fleeDirection.Normalize();
		fleeDirection *= finalFactor;
		return true;
	}

	private void SetFleeDirection(Character neighbor)
	{
		if (neighbor == null || !neighbor.Enabled) return;
		Vector3 neighborVector = TransformCache.position - neighbor.TransformCache.position;
		fleeDirection += neighborVector / neighborVector.sqrMagnitude;
	}

	private void CalculateBoidsDirection()
	{
		boidsDirection = Vector3.zero;
		SpatialGrid.Instance.QueryFromCenter(Tag, CurrentCell, AISettings.boidsGridRange, SetBoidsDirection, AISettings.boidsNeighborCount);
		boidsDirection.Normalize();
	}

	private void SetBoidsDirection(Character neighbor)
	{
		if (neighbor == null || neighbor == this || !neighbor.Enabled) return;
		if (neighbor.State == Fleeing) return;

		Vector3 neighborVector = TransformCache.position - neighbor.TransformCache.position;
		boidsDirection += AISettings.boidsSeparationForce / neighborVector.sqrMagnitude * neighborVector;
		boidsDirection += AISettings.boidsAlignmentForce * neighbor.AIDirection;
	}
	#endregion

	#region Public Methods
	#endregion
}