using System.Collections.Generic;
using UnityEngine;

public partial class UIManager
{
	private readonly Dictionary<(Vector2 textureSize, Color outlineColor), Material> textureMaterials = new();
	private readonly Dictionary<(Color faceColor, Color outlineColor), Material> textMaterials = new();

	public Color GetRarityColor(Item.Rarity rarity)
	{
		return RarityColors[rarity].mainColor;
	}

	public Material GetTextMaterial(Color faceColor, Color outlineColor)
	{
		if (textMaterials.ContainsKey((faceColor, outlineColor)))
		{
			return textMaterials[(faceColor, outlineColor)];
		}
		else
		{
			Material newMat = new(data.textMaterial);
			newMat.SetColor("_FaceColor", faceColor);
			newMat.SetColor("_OutlineColor", outlineColor);

			textMaterials[(faceColor, outlineColor)] = newMat;
			return newMat;
		}
	}

	public Material GetTextMaterial(Item.Rarity rarity)
	{
		return GetTextMaterial(RarityColors[rarity].mainColor, RarityColors[rarity].outlineColor);
	}

	public Material GetTextureBaseMaterial()
	{
		return data.textureBaseMaterial;
	}

	public Material GetTextureOutlineMaterial(Texture2D mainTexture, Color outlineColor)
	{
		Vector2 textureSize = new(mainTexture.width, mainTexture.height);
		if (textureMaterials.ContainsKey((textureSize, outlineColor)))
		{
			return textureMaterials[(textureSize, outlineColor)];
		}
		else
		{
			Material newMat = new(data.textureOutlineMaterial)
			{
				mainTexture = mainTexture
			};
			newMat.SetVector("_TextureSize", new Vector4(textureSize.x, textureSize.y, 0, 0));
			newMat.SetColor("_OutlineColor", outlineColor);

			textureMaterials[(textureSize, outlineColor)] = newMat;
			return newMat;
		}
	}

	public Material GetTextureOutlineMaterial(Texture2D mainTexture, Item.Rarity rarity)
	{
		return GetTextureOutlineMaterial(mainTexture, RarityColors[rarity].outlineColor);
	}
}