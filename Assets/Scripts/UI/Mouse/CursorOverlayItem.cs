using UnityEngine;
using UnityEngine.UI;

public class CursorOverlayItem : CursorOverlay
{
	[SerializeField] private Image image;
	[SerializeField] private Text quantityText;
	[SerializeField] private Text quantityShadowText;

	public void SetItemIcon(Sprite newSprite, int quantity = 0, bool showQuantity = false)
	{
		if (newSprite != null)
		{
			image.sprite = newSprite;
			image.preserveAspect = true;
			image.SetNativeSize();
			quantityText.text = showQuantity ? quantity.ToString() : "";
			quantityShadowText.text = showQuantity ? quantity.ToString() : "";
			Enable(true);
		}
		else
		{
			Enable(false);
		}
	}
}