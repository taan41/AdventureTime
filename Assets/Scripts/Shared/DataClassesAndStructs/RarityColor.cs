using UnityEngine;

[System.Serializable]
public class RarityColor
{
	[HideInInspector] public string rarityName;
	[HideInInspector] public Item.Rarity rarity;
	public Color mainColor;
	public Color outlineColor;
}