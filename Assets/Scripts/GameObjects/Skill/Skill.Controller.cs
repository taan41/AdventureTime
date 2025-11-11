using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Skill.SkillStat;

public partial class Skill
{
	public const float MIN_COOLDOWN = 0.05f;

	public event Action OnCastTimeStart;
	public event Action OnCooldownStart;

	private float cooldownTimer = 0.2f;
	private float castTimer = 0f;
	private Vector3 fireDir = Vector3.zero;

	public float CooldownTimer => cooldownTimer;
	public float CastTimer => castTimer;

	private Dictionary<float, WaitForSeconds> waitCache = new();

	public void UpdateController(float deltaTime)
	{
		if (cooldownTimer > 0f)
		{
			cooldownTimer -= deltaTime;
			if (cooldownTimer <= 0f)
			{
				cooldownTimer = 0f;
				if (Rules.autoCast) Activate();
			}
		}

		if (castTimer > 0f)
		{
			castTimer -= deltaTime;
			if (castTimer <= 0f)
			{
				castTimer = 0f;
				if (Rules.cooldownAfterCast) StartCooldown();
			}
		}
	}

	public (bool activated, float castTime) Activate()
	{
		if (!ValidData) return (false, 0f);
		if (projectileDatas.Count == 0) return (false, 0f);

		if (cooldownTimer > 0f || castTimer > 0f) return (false, 0f);
		if (!owner.StaminaModule.HasAtLeast(Stats[StaminaCost].Final)) return (false, 0f);
		if (!owner.ManaModule.HasAtLeast(Stats[ManaCost].Final)) return (false, 0f);

		owner.StaminaModule.Use(Stats[StaminaCost].Final);
		owner.ManaModule.Use(Stats[ManaCost].Final);

		for (int i = 0; i < projectileDatas.Count; i++)
		{
			var projData = projectileDatas[i];
			owner.StartCoroutine(SpawnProjectiles(projData, GetSkillDirection()));
		}

		castTimer = Stats[CastTime].Final;

		if (castTimer > 0f && Rules.cooldownAfterCast)
		{
			OnCastTimeStart?.Invoke();
			return (true, castTimer);
		}
		
		StartCooldown();

		return (true, 0f);
	}

	private IEnumerator SpawnProjectiles(SkillData.ProjectileDataEntry projData, Vector3 direction)
	{
		int count = projData.specificData.count + (int)Stats[ProjectileCountBonus].Final;
		Vector3 spawnPosition = owner.TransformCache.position;
		Vector3 gapDir;

		if (!waitCache.TryGetValue(projData.specificData.spawnDelay, out WaitForSeconds wait))
		{
			wait = new WaitForSeconds(projData.specificData.spawnDelay);
			waitCache[projData.specificData.spawnDelay] = wait;
		}

		for (int j = 0; j < count; j++)
		{
			gapDir = j * projData.specificData.spawnGap * direction;
			var proj = projectilePools[projData].Get();
			proj.Activate(direction, spawnPosition + gapDir, owner.Target, j, count);

			if (projData.specificData.spawnDelay > 0f)
			{
				yield return wait;
			}
		}
	}

	private void StartCooldown()
	{
		cooldownTimer = Stats[Cooldown].Final;
		if (cooldownTimer < MIN_COOLDOWN) cooldownTimer = MIN_COOLDOWN;
		OnCooldownStart?.Invoke();
	}

	public Vector3 GetSkillDirection()
	{
		if (!ValidData) return Vector3.down;
		return Data.directionType switch
		{
			SkillDirection.Forward => owner.nonZeroDirection,
			SkillDirection.Target => owner.Target != null ? (owner.Target.TransformCache.position - owner.TransformCache.position).normalized : owner.lookDirection,
			SkillDirection.Aimed => owner.lookDirection,
			SkillDirection.None or _ => Vector3.down,
		};
	}
}