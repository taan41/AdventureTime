using UnityEngine;

[CreateAssetMenu(fileName = "CreateProjectileEffect", menuName = "ScriptableObjects/CombatEffects/CreateProjectileEffect")]
public class CreateProjectileEffect : CombatEffect
{
	[System.Serializable]
	class ProjectileParameter : Parameters
	{
		public ProjectileData projectilePhysicData = null;
		public Projectile.ProjectileDataPerSkill projectileSpecificData = null;

		public ProjectileParameter() : base(typeof(CreateProjectileEffect)) { }
	}

	public override void ApplyEffect(ActivationData activation, Parameters parameters)
	{
		if (EffectRNG.NextFloat() > parameters.chance) return;

		if (activation.Character != null && parameters is ProjectileParameter projParams)
		{
			var proj = ProjectileFactory.Create(activation.Character, activation.Skill, projParams.projectilePhysicData, projParams.projectileSpecificData);
			proj.Activate(default, activation.Position, activation.Target);
		}
	}

	public override Parameters CreateParametersInstance()
	{
		return new ProjectileParameter();
	}

	public override EffectWrapper CreateEntry(EffectCondition condition)
	{
		return CreateEntry(condition, new ProjectileParameter());
	}
}