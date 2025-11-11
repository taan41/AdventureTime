using UnityEngine;
using UnityEngine.UI;

public class CursorOverlayShop : CursorOverlay
{
	[SerializeField] private Image iconImage;
	[SerializeField] private Text goldText;
	[SerializeField] private Text goldShadowText;

	public override void Initialize()
	{
		base.Initialize();

		iconImage.SetNativeSize();
	}

	public void SetGoldText(float delta, bool show)
	{
		if (show)
		{
			goldText.text = $"{(delta > 0 ? "+" : "")}{delta:F0}";
			goldShadowText.text = goldText.text;
		}
		Enable(show);
	}
}