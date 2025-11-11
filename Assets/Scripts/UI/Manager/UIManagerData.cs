using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIManagerData", menuName = "Data/UI/UIManagerData")]
public class UIManagerData : CustomSO<UIManagerData>
{
	[Header("Color")]
	public Color positiveStatColor = Color.green;
	public Color negativeStatColor = Color.red;
	public List<RarityColor> rarityColors = new();

	[Header("Materials")]
	public Material textMaterial;
	public Material textureBaseMaterial;
	public Material textureOutlineMaterial;

	// [Header("Mouse")]
	// public CursorOverlayItem mouseItemImagePrefab;

	[Header("Overlays")]
	public Vector2 itemTooltipOffset = new(50f, 0f);
	public ItemTooltip itemTooltipPrefab;

	protected override void OnValidate()
	{
		ValidateList(rarityColors, Item.AllItemRarities.Length, (rarityColor, index) =>
		{
			rarityColor.rarity = Item.AllItemRarities[index];
			rarityColor.rarityName = rarityColor.rarity.ToString();
		});

		base.OnValidate();
	}

	public override void CopyFrom(UIManagerData other)
	{
		if (other == null) return;

		rarityColors.Clear();
		rarityColors.AddRange(other.rarityColors);
	}

	void ValidateList<T>(List<T> list, int desiredCount, Action<T, int> initializer) where T : new()
	{
		if (list.Count > desiredCount)
		{
			list.RemoveRange(desiredCount, list.Count - desiredCount);
		}

		for (int i = 0; i < desiredCount; i++)
		{
			if (i >= list.Count)
			{
				var newItem = new T();
				initializer?.Invoke(newItem, i);
				list.Add(newItem);
			}
		}
	}
}