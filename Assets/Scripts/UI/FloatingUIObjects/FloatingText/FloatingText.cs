using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(TextMeshPro))]
public class FloatingText : FloatingUIObject
{
	private FloatingTextData data;
	private RectTransform rectTransform;
	private MeshRenderer meshRenderer;
	private TextMeshPro text;
	// private Text text;
	// private Outline outline;

	private Transform targetTransform;

	public void Initialize(FloatingTextData data)
	{
		if (rectTransform != null) return;

		rectTransform = GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(1, 1);
		rectTransform.localScale = Vector3.one;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;

		// text = GetComponent<Text>();
		// text.alignment = TextAnchor.MiddleCenter;
		// text.verticalOverflow = VerticalWrapMode.Overflow;
		// text.horizontalOverflow = HorizontalWrapMode.Overflow;
		text = GetComponent<TextMeshPro>();
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        text.sortingLayerID = SortingLayer.NameToID("Foreground");
		text.enabled = false;

		// outline = GetComponent<Outline>();
		// outline.effectColor = Color.black;
		// outline.useGraphicAlpha = true;

		this.data = data;
		data.OnDataChanged += SetupData;
		SetupData();

		Enable(false);
	}

	private void SetupData()
	{
		// text.font = data.legacyFont;
		text.font = data.tmpFont;
		text.fontSize = data.fontSize;

		// outline.effectDistance = new Vector2(data.fontSize / 20f, -data.fontSize / 20f);
	}

	public override void Enable(bool enabled)
	{
		meshRenderer.enabled = enabled;
		text.enabled = enabled;
		// outline.enabled = enabled;

		base.Enable(enabled);
	}

	public void Activate(Transform target, string text, Material material)
	{
		targetTransform = target;
		rectTransform.position = targetTransform.position + data.positionOffset;

		this.text.text = text;

		this.text.fontMaterial = material;

		Enable(true);
	}

	public void Deactivate()
	{
		targetTransform = null;
		Enable(false);
		FloatingUIObjectManager.Instance.Return(this);
	}

	public override void DoUpdate(float deltaTime)
	{
		if (!Enabled) return;

		rectTransform.position = targetTransform.position + data.positionOffset;
	}

	public static FloatingText Create(Transform parent, FloatingTextData data)
	{
		GameObject obj = new("FloatingText");
		obj.transform.SetParent(parent, false);
		var text = obj.AddComponent<FloatingText>();
		text.Initialize(data);
		return text;
	}
}