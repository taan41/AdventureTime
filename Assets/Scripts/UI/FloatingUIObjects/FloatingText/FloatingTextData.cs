using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatingTextData", menuName = "ScriptableObjects/UI/FloatingTextData")]
public class FloatingTextData : CustomSO<FloatingTextData>
{
	// [Serializable]
	// public struct RarityColor
	// {
	// 	[HideInInspector] public string rarityName;
	// 	[HideInInspector] public Item.Rarity rarity;
	// 	public Color color;
	// 	public Color outlineColor;

	// 	public FloatingText.FloatingTextColor ToTextColor()
	// 	{
	// 		return new FloatingText.FloatingTextColor
	// 		{
	// 			textColor = color,
	// 			outlineColor = outlineColor
	// 		};
	// 	}
	// }

	[Header("General Settings")]
	public int poolSize = 50;
	public bool fixedPoolSize = true;

	[Header("Text Settings")]
	public TMP_FontAsset tmpFont;
	public Font legacyFont;
	public int fontSize = 10;
	public Vector3 positionOffset = new(0f, 0f);

	// [Header("Color Settings")]
	// public RarityColor[] textRarityColorData = new RarityColor[Item.AllItemRarities.Length];

	private void OnEnable()
	{
		// if (textRarityColorData.Length != Item.AllItemRarities.Length)
		// {
		// 	Array.Resize(ref textRarityColorData, Item.AllItemRarities.Length);
		// }

		// for (int i = 0; i < textRarityColorData.Length; i++)
		// {
		// 	if (i < Item.AllItemRarities.Length)
		// 	{
		// 		textRarityColorData[i].rarity = Item.AllItemRarities[i];
		// 		textRarityColorData[i].rarityName = textRarityColorData[i].rarity.ToString();
		// 	}
		// }
	}

	protected override void OnValidate()
	{
		// if (textRarityColorData.Length != Item.AllItemRarities.Length)
		// {
		// 	Array.Resize(ref textRarityColorData, Item.AllItemRarities.Length);

		// 	for (int i = 0; i < textRarityColorData.Length; i++)
		// 	{
		// 		if (i < Item.AllItemRarities.Length)
		// 		{
		// 			textRarityColorData[i].rarity = Item.AllItemRarities[i];
		// 			textRarityColorData[i].rarityName = textRarityColorData[i].rarity.ToString();
		// 		}
		// 	}
		// }

		base.OnValidate();
	}

	public override void CopyFrom(FloatingTextData other)
	{
		if (other == null) return;

		poolSize = other.poolSize;
		fixedPoolSize = other.fixedPoolSize;
		legacyFont = other.legacyFont;
		fontSize = other.fontSize;

		// textRarityColorData = data.textRarityColorData;
	}
}