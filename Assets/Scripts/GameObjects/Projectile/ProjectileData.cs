using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Data", menuName = "Data/Projectile/Physic")]
public class ProjectileData : CustomSO<ProjectileData>
{
	public Sprite defaultSprite;
	public ComponentData ComponentData;
	public List<Projectile.ProjectilePhaseData> PhaseDatas = new();

	public override void CopyFrom(ProjectileData other)
	{
		if (other == null) return;

		defaultSprite = other.defaultSprite;
		ComponentData.CopyFrom(other.ComponentData);

		PhaseDatas.Clear();
		foreach (var phase in other.PhaseDatas)
		{
			var newPhase = new Projectile.ProjectilePhaseData();
			newPhase.CopyFrom(phase);
			PhaseDatas.Add(newPhase);
		}

		SignalDataChange();
	}

	public override bool IsValid()
	{
		if (defaultSprite == null) return false;
		if (ComponentData == null) return false;

		return true;
	}
}