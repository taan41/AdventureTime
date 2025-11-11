using System;
using System.Collections.Generic;
using UnityEngine;

using static Character.Action;
using static Character.StatType;
using static EightWayDirection;

public partial class Character
{
	#region Enums & Nested Classes
	public enum Action
	{
		Idle,
		Move,
		Dash,
		Cast,
		GenericSkill,
		MeleeSkill,
		RangedSkill,
		PrimarySkill,
		SecondarySkill,
		Hit,
		Death,
	}
	#endregion

	#region Fields & Properties
	private CharacterRule RulesRef => Data.settings.rules;
	private CharacterConfiguration SettingsRef => Data.settings.configs;
	private CharacterDataActions ActionDataRef => Data.actions;

	public Action currentAction = Idle;
	public Vector3 direction = Vector3.zero;
	public Vector3 nonZeroDirection = Vector3.down;
	public Vector3 lookDirection = Vector3.down;

	private int currentPriority;
	private float lockedActionTimer;

	private Action lastAnimAction = Idle;
	private EightWayDirection lastAnimDirection8 = Down;

	private bool isInvulnerable = false;
	private float invulTimer;

	private Skill skillToActivate;
	private float skillDelayTimer;
	private float skillCastTimer;

	private int primarySkillActionIndex;
	private float primarySkillIndexResetTimer;

	private float sfxTimer;
	#endregion

	#region Update
	public void DoUpdateController(float deltaTime)
	{
		if (direction != Vector3.zero)
		{
			nonZeroDirection = direction;
		}

		if (ControlledByPlayer)
		{
			lookDirection = (MouseManager.Instance.MouseWorldPosition - (Vector2)TransformCache.position).normalized;
		}
		else
		{
			lookDirection = nonZeroDirection;
		}

		if (lockedActionTimer > 0f)
		{
			lockedActionTimer -= deltaTime;
			if (lockedActionTimer <= 0f)
			{
				lockedActionTimer = 0f;

				if (currentAction == Death)
				{
					Dying();
					return;
				}

				currentAction = Idle;
				currentPriority = ActionDataRef[Idle].priority;
				PlayMedia(Idle, nonZeroDirection, true, true);
			}
			else
			{
				if (currentAction == Cast)
				{
					PlayMedia(Cast, lookDirection, false, true);
				}
			}
		}

		if (skillDelayTimer > 0f)
		{
			skillDelayTimer -= deltaTime;
			if (skillDelayTimer <= 0f && skillToActivate != null)
			{
				skillDelayTimer = 0f;
				skillCastTimer = skillToActivate.Activate().castTime;
				skillToActivate = null;

				if (skillCastTimer > 0f)
				{
					currentAction = Cast;
					currentPriority = ActionDataRef[Cast].priority;
					PlayMedia(Cast, direction, true, true);
					lockedActionTimer = skillCastTimer;
				}
			}
		}

		if (skillCastTimer > 0f)
		{
			skillCastTimer -= deltaTime;
			if (skillCastTimer <= 0f)
			{
				skillCastTimer = 0f;
			}
		}

		if (primarySkillIndexResetTimer > 0f)
		{
			primarySkillIndexResetTimer -= deltaTime;
			if (primarySkillIndexResetTimer <= 0f)
			{
				primarySkillIndexResetTimer = 0f;
				primarySkillActionIndex = 0;
			}
		}

		if (invulTimer > 0f)
		{
			invulTimer -= deltaTime;
			isInvulnerable = true;
			if (invulTimer <= 0f)
			{
				invulTimer = 0f;
				isInvulnerable = false;
			}
		}

		if (sfxTimer > 0f)
		{
			sfxTimer -= deltaTime;
			if (sfxTimer < 0f) sfxTimer = 0f;
		}
	}
	#endregion

	#region Public Actions
	public bool TakeDamage(Damage baseDamage, Vector3 knockbackDirection = default, float force = 0f, float duration = 0f)
	{
		if (isInvulnerable || RulesRef.invincible) return false;

		if (!HealthModule.HasResource) return false;

		float resistanceFactor = 1f - Stats[KnockbackResistance].Final;
		if (resistanceFactor <= 0f) resistanceFactor = 0f;

		force *= resistanceFactor;

		float knockbackDuration = Mathf.Clamp(duration * resistanceFactor, 0f, 1.5f);

		MovementModule.TempMovement(force, Stats[KnockbackOvertimeFactor].Final, duration, knockbackDirection, false, true);

		var damage = baseDamage.CalcFinalDamage(Stats[Armor].Final, Stats[DamageReduction].Final);
		FloatingUIObjectManager.Instance.ActivateDamageNumber(damage.value, TransformCache.position, damage.isCrit);
		HealthModule.Use(damage.value);

		invulTimer = Stats[ImmunityDuration].Final;
		isInvulnerable = invulTimer > 0f;

		GettingHit(knockbackDirection, knockbackDuration);

		return true;
	}

	public void Recover(StatType statType, float amount)
	{
		float recoverAmount;
		switch (statType)
		{
			case Health:
				recoverAmount = HealthModule.Recover(amount);
				if (recoverAmount > 0)
					FloatingUIObjectManager.Instance.ActivateHealNumber(recoverAmount, TransformCache.position);
				break;

			case Stamina:
				recoverAmount = StaminaModule.Recover(amount);
				if (recoverAmount > 0)
					FloatingUIObjectManager.Instance.ActivateStaminaNumber(recoverAmount, TransformCache.position);
				break;

			case Mana:
				recoverAmount = ManaModule.Recover(amount);
				if (recoverAmount > 0)
					FloatingUIObjectManager.Instance.ActivateManaNumber(recoverAmount, TransformCache.position);
				break;
		}
	}
	
	public void RecoverRatio(StatType statType, float ratio)
	{
		float recoverAmount;
		switch (statType)
		{
			case Health:
				recoverAmount = HealthModule.Recover(ratio * HealthModule.Max);
				if (recoverAmount > 0)
					FloatingUIObjectManager.Instance.ActivateHealNumber(recoverAmount, TransformCache.position);
				break;

			case Stamina:
				recoverAmount = StaminaModule.Recover(ratio * StaminaModule.Max);
				if (recoverAmount > 0)
					FloatingUIObjectManager.Instance.ActivateStaminaNumber(recoverAmount, TransformCache.position);
				break;

			case Mana:
				recoverAmount = ManaModule.Recover(ratio * ManaModule.Max);
				if (recoverAmount > 0)
					FloatingUIObjectManager.Instance.ActivateManaNumber(recoverAmount, TransformCache.position);
				break;
		}
	}

	public void DropLoot(float goldMod = 1f, float itemMod = 1f, float chanceMod = 1f)
	{
		var lootTable = Data.properties.lootTable;

		Vector3 dropPosition;
		bool dropGold = CharacterRNG.NextFloat() < lootTable.goldChance * chanceMod;
		if (dropGold)
		{
			dropPosition = TransformCache.position + new Vector3(
				CharacterRNG.NextFloat(-1f, 1f),
				CharacterRNG.NextFloat(-1f, 1f),
				0f
			);
			float goldAmount = CharacterRNG.NextFloat(lootTable.goldMin, lootTable.goldMin + lootTable.goldVariance) * goldMod;
			PickupManager.Instance.GoldPickup(goldAmount, dropPosition);
		}

		foreach (var itemLoot in lootTable.items)
		{
			bool dropItem = CharacterRNG.NextFloat() < itemLoot.dropChance * chanceMod;
			if (dropItem)
			{
				dropPosition = TransformCache.position + new Vector3(
					CharacterRNG.NextFloat(-1f, 1f),
					CharacterRNG.NextFloat(-1f, 1f),
					0f
				);
				int quantity = (int)(CharacterRNG.NextInt(itemLoot.minQuantity, itemLoot.minQuantity + itemLoot.quantityVariance) * itemMod);
				PickupManager.Instance.ItemPickup(itemLoot.itemData, quantity, dropPosition);
			}
		}
	}

	public void Interact(Character character)
	{
		if (InteractableRules.menuType != MenuManager.MenuType.None)
		{
			Character focused = InteractableRules.focusedCharacterType switch
			{
				IInteractable.FocusedCharacterType.Self => this,
				IInteractable.FocusedCharacterType.Other => character,
				_ => character
			};
			MenuManager.Instance.ToggleMenu(InteractableRules.menuType, focused);
		}
	}

	public void InteractWithNearbyInteractable()
	{
		if (NearbyInteractables.Count == 0) return;

		float closestDistanceSqr = float.MaxValue;
		IInteractable closest = null;
		for (int i = 0; i < NearbyInteractables.Count; i++)
		{
			var npc = NearbyInteractables[i];
			float distanceSqr = (npc.InteractableTransform.position - TransformCache.position).sqrMagnitude;
			if (distanceSqr < closestDistanceSqr)
			{
				closestDistanceSqr = distanceSqr;
				closest = npc;
			}
		}
		closest?.Interact(this);
	}
	#endregion

	#region Base Actions
	private void Idling()
	{
		if (currentAction == Idle) return;
		if (!GameStateManager.Instance.CheckState(GameStateManager.GameState.InGame)) return;
		if (!CheckPriority(Idle)) return;

		direction = Vector3.zero;
		MovementModule.Direction = direction;

		PlayMedia(Idle, direction, true, true);

		currentAction = Idle;
		currentPriority = ActionDataRef[Idle].priority;
	}

	private void Moving()
	{
		MovementModule.Direction = direction;
		if (direction == Vector3.zero)
		{
			Idling();
			return;
		}
		if (currentAction == Move)
		{
			PlayMedia(Move, direction, false, true);
			return;
		}
		if (!GameStateManager.Instance.CheckState(GameStateManager.GameState.InGame)) return;
		if (!CheckPriority(Move)) return;

		MovementModule.Direction = direction;

		PlayMedia(Move, direction, true, true);

		currentAction = Move;
		currentPriority = ActionDataRef[Move].priority;
	}

	private void DashingVoid() => Dashing();

	private bool Dashing(Vector3 dashDirection = default)
	{
		if (currentAction == Dash) return false;
		if (!GameStateManager.Instance.CheckState(GameStateManager.GameState.InGame)) return false;
		if (!CheckPriority(Dash)) return false;

		if (!StaminaModule.HasAtLeast(Stats[DashStaminaCost].Final)) return false;

		if (dashDirection == Vector3.zero) dashDirection = nonZeroDirection;
		float dashDuration = Stats[DashDuration].Final;

		if (!MovementModule.TempMovement(
			Stats[MoveSpeed].Final * Stats[DashSpeedMultiplier].Final,
			Stats[DashOvertimeFactor].Final,
			dashDuration,
			dashDirection)) return false;

		invulTimer = Mathf.Max(dashDuration * Stats[DashInvulDurationFactor].Final, invulTimer);
		StaminaModule.Use(Stats[DashStaminaCost].Final);

		PlayMedia(Dash, dashDirection, true, false);

		currentAction = Dash;
		currentPriority = ActionDataRef[Dash].priority;
		lockedActionTimer = dashDuration;

		return true;
	}

	private void UsingWeaponSkill() => UsingSkill(Skill.SkillType.Weapon);
	
	private void UsingPrimarySkill() => UsingSkill(Skill.SkillType.Primary);

	private (bool success, float duration) UsingSkill(Skill.SkillType skillType)
	{
		return UsingSkill(SkillHandler.GetOffCooldownSkill(skillType));
	}

	private (bool success, float duration) UsingSkill(Skill skill)
	{
		if (!GameStateManager.Instance.CheckState(GameStateManager.GameState.InGame)) return (false, 0f);

		if (skill == null) return (false, 0f);

		Action skillAction;
		if (skill.Data.type == Skill.SkillType.Primary)
		{
			skillAction = PrimarySkill;
		}
		else
		{
			skillAction = skill.Data.rangeType switch
			{
				Skill.SkillRangeType.Melee => MeleeSkill,
				Skill.SkillRangeType.Ranged => RangedSkill,
				Skill.SkillRangeType.Generic or _ => GenericSkill,
			};
		}

		if (!CheckPriority(skillAction)) return (false, 0f);

		var skillActionData = ActionDataRef.GetSkillActionData(skillAction, 0);

		float frameRate = skillActionData.scaleFrameRateWithAttackSpeed
			? skillActionData.animation.frameRate * Stats[AttackSpeed].Final
			: skillActionData.animation.frameRate;

		if (frameRate <= 0f) frameRate = 4f;

		if (skillAction == PrimarySkill)
		{
			primarySkillIndexResetTimer = 2f;
			primarySkillActionIndex = SkillHandler.GetSkillIndex(skill);
		}

		direction = skill.GetSkillDirection();

		PlayMedia(skillAction, direction, true, false);

		lockedActionTimer = skillActionData.lockedActionFrame / frameRate;
		MovementModule.TempMovement(0f, 0f, lockedActionTimer, Vector3.zero, false, true);

		skillToActivate = skill;
		skillDelayTimer = skillActionData.skillDelayFrame / frameRate;

		currentAction = skillAction;
		currentPriority = ActionDataRef[skillAction].priority;

		return (true, lockedActionTimer);
		
	}

	private bool GettingHit(Vector3 knockbackDirection, float duration)
	{
		if (!GameStateManager.Instance.CheckState(GameStateManager.GameState.InGame)) return false;
		if (currentPriority > ActionDataRef[Hit].priority) return false;

		if (duration <= 0f)
		{
			PlaySoundEffect(Hit);
			return true;
		}

		PlayMedia(Hit, -knockbackDirection, true, false);

		currentAction = Hit;
		currentPriority = ActionDataRef[Hit].priority;
		lockedActionTimer = duration;

		return true;
	}
	
	private void Dying()
	{
		if (currentAction == Death)
		{
			DropLoot();
			OnDeath?.Invoke(this);
			Enable(false);
			return;
		}
		if (!GameStateManager.Instance.CheckState(GameStateManager.GameState.InGame)) return;
		if (!CheckPriority(Death)) return;

		ActiveCharacters[Tag].Remove(this);
		
		float animDuration = PlayMedia(Death, nonZeroDirection, true, false);

		MovementModule.TempMovement(0f, 0f, animDuration, Vector3.zero, false, true);

		currentAction = Death;
		currentPriority = ActionDataRef[Death].priority;
		lockedActionTimer = animDuration;
	}
	#endregion

	#region Helpers
	private bool CheckPriority(Action action)
	{
		int newPriority = ActionDataRef[action].priority;
		if (currentPriority > newPriority) return false;
		if (currentPriority == newPriority && lockedActionTimer > 0f) return false;
		return true;
	}

	private float PlayMedia(Action action, Vector3 animDirection, bool resetFrame, bool loop)
	{
		var animDirection8 = (animDirection != Vector3.zero ? animDirection : nonZeroDirection).ToEightWay();
		if (action == lastAnimAction && animDirection8 == lastAnimDirection8 && !resetFrame) return 0f;

		lastAnimAction = action;
		lastAnimDirection8 = animDirection8;

		var actionData = ActionDataRef[action];
		var animClip = actionData.animation;
		float frameRate = animClip.frameRate;

		if (action >= GenericSkill && action <= SecondarySkill && ActionDataRef.GetSkillActionData(action, primarySkillActionIndex).scaleFrameRateWithAttackSpeed)
		{
			frameRate *= Stats[AttackSpeed].Final;
		}

		if (frameRate <= 0f) frameRate = 4f;

		AnimationModule.SetFlipX(animClip.IsFlipped(animDirection8));
		AnimationModule.Play(animClip[animDirection8], frameRate, resetFrame, loop);

		PlaySoundEffect(action);

		return animClip[animDirection8].Length / frameRate;
	}

	private void PlaySoundEffect(Action action)
	{
		if (sfxTimer > 0f) return;

		var actionData = ActionDataRef[action];
		sfxTimer = AudioManager.Instance.PlayRandomSFX(actionData.sfxClips, actionData.sfxChance);
	}

	private void RefreshDataActions()
	{
		AnimationModule.SetDefaultSprite(Data.defaultSprite);

		direction = Vector3.zero;
		nonZeroDirection = Vector3.down;
		lookDirection = Vector3.down;

		currentAction = Idle;
		
		currentPriority = ActionDataRef[Idle].priority;

		lockedActionTimer = 0f;

		PlayMedia(Idle, nonZeroDirection, true, true);
	}
	#endregion
}