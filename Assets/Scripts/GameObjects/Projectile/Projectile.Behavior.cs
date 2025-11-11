using System.Collections.Generic;
using UnityEngine;

public partial class Projectile
{
	private float intervalTimer = 0f;
	private readonly HashSet<Character> hitAllies = new();
	private readonly HashSet<Character> hitEnemies = new();
	// private readonly HashSet<Character> removeQueue = new();
	private readonly HashSet<Character> browseSet = new();

	private void UpdateBehavior(float deltaTime)
	{
		if (intervalTimer > 0f)
		{
			intervalTimer -= deltaTime;
			if (intervalTimer <= 0f)
			{
				if (hitAllies.Count == 0 && hitEnemies.Count == 0) return;

				intervalTimer = hitInterval;

				browseSet.UnionWith(hitAllies);

				foreach (var ally in browseSet)
				{
					Recovery(ally);
				}

				browseSet.Clear();
				browseSet.UnionWith(hitEnemies);

				foreach (var enemy in browseSet)
				{
					if (!DealDamage(enemy))
					{
						hitEnemies.Remove(enemy);
					}
				}

				browseSet.Clear();
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Character character))
		{
			if (owner.Tag.IsAlly(character.Tag))
			{
				if (hitInterval <= 0f)
				{
					Recovery(character);
				}
				else
				{
					hitAllies.Add(character);
				}
			}
			else
			{
				if (hitInterval <= 0f)
				{
					DealDamage(character);
				}
				else
				{
					hitEnemies.Add(character);
				}
			}

			if (intervalTimer <= 0f)
			{
				intervalTimer = 0.01f;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Character character))
		{
			hitAllies.Remove(character);
			hitEnemies.Remove(character);
		}
	}

	private bool DealDamage(Character character)
	{
		if (character.TakeDamage(damage, direction, knockback, knockbackDuration))
		{
			combatEffectHandler.ActivateEffects(CombatEffect.EffectCondition.OnHit, new(owner, character, skill, character.TransformCache.position));
			return true;
		}
		return false;
	}
	
	private void Recovery(Character character)
	{
		character.Recover(Character.StatType.Health, heal);
		character.Recover(Character.StatType.Stamina, staminaRecovery);
		character.Recover(Character.StatType.Mana, manaRecovery);
		combatEffectHandler.ActivateEffects(CombatEffect.EffectCondition.OnHeal, new(owner, character, skill, character.TransformCache.position));
	}
}