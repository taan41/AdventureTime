using UnityEngine;
using UnityEngine.UI;

public class CustomButton : CustomUIComponent
{
	[SerializeField] private Text buttonText;
	[SerializeField] private Image buttonImage;
	[SerializeField] private RectTransform buttonDecorator;
	[SerializeField] private Vector2 textOnClickOffset;

	public Text Text => buttonText;
	public Image Image => buttonImage;

	private Vector2 originalTextPosition;
	private Vector2 originalImagePosition;
	private Vector2 originalDecoratorPosition;

	protected override void Awake()
	{
		if (buttonText != null) originalTextPosition = buttonText.rectTransform.anchoredPosition;
		if (buttonImage != null) originalImagePosition = buttonImage.rectTransform.anchoredPosition;
		if (buttonDecorator != null) originalDecoratorPosition = buttonDecorator.anchoredPosition;

		base.Awake();
	}

	public override void RefreshVisuals()
	{
		if (buttonText != null) buttonText.rectTransform.anchoredPosition = originalTextPosition + (isPressed ? textOnClickOffset : Vector2.zero);
		if (buttonImage != null) buttonImage.rectTransform.anchoredPosition = originalImagePosition + (isPressed ? textOnClickOffset : Vector2.zero);
		if (buttonDecorator != null) buttonDecorator.anchoredPosition = originalDecoratorPosition + (isPressed ? textOnClickOffset : Vector2.zero);

		base.RefreshVisuals();
	}

	public override void ClearVisuals()
	{
		if (buttonText != null) buttonText.rectTransform.anchoredPosition = originalTextPosition;
		if (buttonImage != null) buttonImage.rectTransform.anchoredPosition = originalImagePosition;
		if (buttonDecorator != null) buttonDecorator.anchoredPosition = originalDecoratorPosition;

		base.ClearVisuals();
	}
}