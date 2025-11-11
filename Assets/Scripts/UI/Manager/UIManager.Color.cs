using System.Collections.Generic;
using UnityEngine;

public partial class UIManager
{
	public Dictionary<Item.Rarity, RarityColor> RarityColors { get; private set; } = new();

	void AwakeColor()
	{
		foreach (var rarity in Item.AllItemRarities)
		{
			RarityColors[rarity] = new RarityColor
			{
				rarity = rarity,
				mainColor = Color.white,
				outlineColor = Color.black
			};
		}
	}

	void RefreshDataColor()
	{
		foreach (var rarityColor in data.rarityColors)
		{
			RarityColors[rarityColor.rarity] = rarityColor;
		}
	}
}