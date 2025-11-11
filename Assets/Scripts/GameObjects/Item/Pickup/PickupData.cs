using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPickupData", menuName = "ScriptableObjects/Item/PickupData")]
public class PickupData : CustomSO<PickupData>
{
	[System.Serializable]
	public class GoldIconEntry
	{
		[HideInInspector] public string entryName = "New Entry";
		public float amountThreshold = 0f;
		public Sprite icon = null;
	}

	[Header("Bob Settings")]
	public float bobSpeed = 1f;
	public float bobHeight = 0.25f;

	[Header("Gold Icons")]
	public List<GoldIconEntry> goldIcons = new();

	[ContextMenu("Sort Gold Icons")]
	public void SortGoldIcons()
	{
		goldIcons.Sort((a, b) => a.amountThreshold.CompareTo(b.amountThreshold));
	}

	protected override void OnValidate()
	{
		for (int i = 0; i < goldIcons.Count; i++)
		{
			goldIcons[i].entryName = goldIcons[i].amountThreshold.ToString("F0");
		}

		base.OnValidate();
	}

	public override void CopyFrom(PickupData other)
	{
		if (other == null) return;

		bobSpeed = other.bobSpeed;
		bobHeight = other.bobHeight;

		goldIcons.Clear();
		foreach (var entry in other.goldIcons)
		{
			var newEntry = new GoldIconEntry()
			{
				amountThreshold = entry.amountThreshold,
				icon = entry.icon
			};
			goldIcons.Add(newEntry);
		}
	}
}