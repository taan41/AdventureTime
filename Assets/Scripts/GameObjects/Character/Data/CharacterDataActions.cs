using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterAction", menuName = "Data/Character/Action")]
public class CharacterDataActions : ScriptableObject
{
	[Serializable]
	public class ActionData
	{
		public int priority;
		public AnimationModule.AnimationClipData animation = new();
		public int lockedActionFrame;
		public float sfxChance = 0.6f;
		public List<AudioClip> sfxClips = new();

		public virtual void CopyFrom(ActionData other)
		{
			if (other == null) return;

			priority = other.priority;
			lockedActionFrame = other.lockedActionFrame;

			sfxClips.Clear();
			sfxClips.AddRange(other.sfxClips);

			animation.CopyFrom(other.animation);
		}
	}

	[Serializable]
	public class SkillActionData : ActionData
	{
		public SkillData skillData;
		public int skillDelayFrame;
		public bool scaleFrameRateWithAttackSpeed;

		public override void CopyFrom(ActionData other)
		{
			base.CopyFrom(other);

			if (other is not SkillActionData otherSkill) return;

			skillData = otherSkill.skillData;
			skillDelayFrame = otherSkill.skillDelayFrame;
			scaleFrameRateWithAttackSpeed = otherSkill.scaleFrameRateWithAttackSpeed;
		}
	}

	public ActionData idle = new();
	public ActionData move = new();
	public ActionData dash = new();
	public ActionData cast = new();
	public ActionData hit = new();
	public ActionData death = new();
	public SkillActionData genericSkill = new();
	public SkillActionData meleeSkill = new();
	public SkillActionData rangedSkill = new();
	public List<SkillActionData> primarySkillActions = new();
	public List<SkillActionData> secondarySkillActions = new();

	public ActionData this[Character.Action action, int skillIndex = 0]
	{
		get => action switch
		{
			Character.Action.Idle => idle,
			Character.Action.Move => move,
			Character.Action.Dash => dash,
			Character.Action.Cast => cast,
			Character.Action.Hit => hit,
			Character.Action.Death => death,
			Character.Action.GenericSkill => genericSkill,
			Character.Action.MeleeSkill => meleeSkill,
			Character.Action.RangedSkill => rangedSkill,
			Character.Action.PrimarySkill => skillIndex >= 0 && skillIndex < primarySkillActions.Count ? primarySkillActions[skillIndex] : primarySkillActions[skillIndex % primarySkillActions.Count],
			Character.Action.SecondarySkill => skillIndex >= 0 && skillIndex < secondarySkillActions.Count ? secondarySkillActions[skillIndex] : secondarySkillActions[skillIndex % secondarySkillActions.Count],
			_ => idle,
		};
	}

	public SkillActionData GetSkillActionData(Character.Action action, int attackIndex)
	{
		return action switch
		{
			Character.Action.GenericSkill => genericSkill,
			Character.Action.MeleeSkill => meleeSkill,
			Character.Action.RangedSkill => rangedSkill,
			Character.Action.PrimarySkill => primarySkillActions[attackIndex % primarySkillActions.Count],
			Character.Action.SecondarySkill => secondarySkillActions[attackIndex % secondarySkillActions.Count],
			_ => null,
		};
	}

	public List<SkillData> GetSkillDatas()
	{
		List<SkillData> skillDatas = new();

		foreach (var skillAction in primarySkillActions)
		{
			if (skillAction.skillData != null)
			{
				skillDatas.Add(skillAction.skillData);
			}
		}

		foreach (var skillAction in secondarySkillActions)
		{
			if (skillAction.skillData != null)
			{
				skillDatas.Add(skillAction.skillData);
			}
		}

		return skillDatas;
	}

	[ContextMenu("Clear Animations")]
	public void Clear()
	{
		idle.animation = null;
		move.animation = null;
		dash.animation = null;
		cast.animation = null;
		hit.animation = null;
		death.animation = null;
		genericSkill.animation = null;
		meleeSkill.animation = null;
		rangedSkill.animation = null;

		foreach (var skillAction in primarySkillActions)
		{
			skillAction.animation = null;
		}

		foreach (var skillAction in secondarySkillActions)
		{
			skillAction.animation = null;
		}
	}

	[ContextMenu("Copy from Generic Skill")]
	public void CopyGenericSkill()
	{
		meleeSkill.CopyFrom(genericSkill);
		rangedSkill.CopyFrom(genericSkill);
	}
}