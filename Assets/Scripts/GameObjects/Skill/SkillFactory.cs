using System;

public static class SkillFactory
{
	public static Skill Create(Character owner, SkillData skillData)
	{
		if (skillData != null && !skillData.IsValid())
		{
			throw new ArgumentException("[SkillFactory.Create] Invalid Skill Data provided");
		}

		Skill skill = new(owner);
		skill.SetData(skillData);
		return skill;
	}
}