using System;
using UnityEngine;

public static class ProjectileFactory
{
	public static Projectile Create(Character owner, Skill sourceSkill, ProjectileData physicData = null, Projectile.ProjectileDataPerSkill specificData = null)
	{
		if (physicData != null && !physicData.IsValid())
		{
			throw new ArgumentException("[ProjectileFactory.Create] Invalid Projectile Data provided");
		}

		var obj = new GameObject("Projectile");
		var projectile = obj.AddComponent<Projectile>();
		projectile.Initialize(owner, sourceSkill);
		projectile.SetData(physicData, specificData, true);
		projectile.Enable(false);
		return projectile;
	}
}