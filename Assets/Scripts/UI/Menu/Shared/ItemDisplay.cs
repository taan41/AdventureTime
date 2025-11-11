using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour, IUpdatable, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	protected enum MouseInputFlags : byte
	{
		None = 0,
		LeftClick = 1 << 0,
		RightClick = 1 << 1,
		MiddleClick = 1 << 2,
		AnyClick = LeftClick | RightClick | MiddleClick,
		Ctrl = 1 << 3,
		Shift = 1 << 4,
		Alt = 1 << 5,
		DoubleClick = 1 << 6,
	}

	[Serializable]
	public struct DefaultEquipIcon
	{
		public Item.EquipmentType equipSlot;
		public Sprite icon;
	}

	public event Action<bool> OnEnabledChanged;

	public bool Enabled { get; private set; } = true;
	public bool UseUpdate { get; private set; } = false;
	public bool UseFixedUpdate { get; private set; } = false;
	public bool UseUnscaledTime { get; private set; } = true;

	[SerializeField] private RectTransform rectTransform;
	[SerializeField] private Image iconImage;
	[SerializeField] private RectTransform iconContainer;
	[SerializeField] private Sprite defaultIcon;
	[SerializeField] private Text quantityText;
	[SerializeField] private Text quantityShadowText;
	[SerializeField] private Image borderImage;
	[SerializeField] private List<Sprite> borderSprites = new();
	[SerializeField] private float borderInterval = 0.5f;
	[SerializeField] private List<DefaultEquipIcon> defaultEquipIcons = new();

	private bool isHighlighted = false;
	private float borderTimer = 0f;
	private int borderIndex = 0;

	private readonly Dictionary<Item.EquipmentType, Sprite> defaultEquipIconDict = new();

	public RectTransform IconContainerTransform => iconContainer;
	public Item ItemRef { get; private set; } = null;

	private Transform OverlayCanvasTransform => UIReferences.Instance.menuOverlayCanvas.transform;
	protected Character ControlledHero => HeroParty.Instance.ControlledHero;
	protected Item MouseItemRef => MouseManager.Instance.MouseItem;

	private void Awake()
	{
		for (int i = 0; i < defaultEquipIcons.Count; i++)
		{
			defaultEquipIconDict[defaultEquipIcons[i].equipSlot] = defaultEquipIcons[i].icon;
		}

		UpdaterManager.StaticRegisterLastUpdater(this);
	}

	public void Enable(bool enabled)
	{
		Enabled = enabled;

		OnEnabledChanged?.Invoke(Enabled);
	}

	public void SetItemRef(Item newItemRef)
	{
		if (newItemRef == ItemRef) return;
		if (ItemRef != null)
		{
			ItemRef.OnDataChanged -= RefreshDisplay;
			ItemRef.OnQuantityChanged -= RefreshDisplay;
		}

		ItemRef = newItemRef;

		if (ItemRef != null)
		{
			if (ItemRef.slotType == Item.ItemSlotType.Equipment)
			{
				if (defaultEquipIconDict.TryGetValue(ItemRef.requirement.equipRequirement, out var equipIcon))
				{
					defaultIcon = equipIcon;
				}
				else
				{
					defaultIcon = null;
				}
			}

			ItemRef.OnDataChanged += RefreshDisplay;
			ItemRef.OnQuantityChanged += RefreshDisplay;
		}

		RefreshDisplay();
	}

	private void RefreshDisplay()
	{
		if (ItemRef == null || ItemRef.Data == null || ItemRef.Quantity <= 0)
		{
			ClearDisplay();
			return;
		}

		SetIcon(ItemRef.Data.icon);

		if (ItemRef.Data.stackable && ItemRef.Quantity > 0)
		{
			quantityText.text = ItemRef.Quantity.ToString();
			quantityShadowText.text = ItemRef.Quantity.ToString();
		}
		else
		{
			quantityText.text = "";
			quantityShadowText.text = "";
		}

	}

	private void ClearDisplay()
	{
		SetIcon(defaultIcon);

		quantityText.text = "";
		quantityShadowText.text = "";

		// borderImage.enabled = false;
		// isHighlighted = false;
	}

	private void SetIcon(Sprite icon)
	{
		icon = icon != null ? icon : defaultIcon;

		if (icon != null)
		{
			iconImage.sprite = icon;
			iconImage.preserveAspect = true;
			iconImage.SetNativeSize();
			iconImage.enabled = true;
		}
		else
		{
			iconImage.sprite = null;
			iconImage.enabled = false;
		}
	}

	public void DoUpdate(float deltaTime) { }

	public void DoFixedUpdate(float fixedDeltaTime) { }

	public void DoUnscaledUpdate(float unscaledDeltaTime)
	{
		if (isHighlighted && borderSprites.Count > 0)
		{
			borderTimer += Time.unscaledDeltaTime;

			if (borderTimer >= borderInterval)
			{
				borderTimer -= borderInterval;
				borderIndex = (borderIndex + 1) % borderSprites.Count;
				borderImage.sprite = borderSprites[borderIndex];
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		MenuManager.Instance.ActiveMenu.OnItemDisplayClicked(this);
	}
	
	public void OnPointerEnter(PointerEventData eventData)
	{
		isHighlighted = true;
		borderImage.enabled = true;
		borderTimer = 0f;
		borderIndex = 0;
		borderImage.sprite = borderSprites[borderIndex];

		MenuManager.Instance.ActiveMenu.OnItemDisplayPointed(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHighlighted = false;
		borderImage.enabled = false;

		MenuManager.Instance.ActiveMenu.OnItemDisplayPointed(this, false);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		MenuManager.Instance.ActiveMenu.OnItemDisplayDragged(this);
	}

	public void OnDrag(PointerEventData eventData)
	{
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		MenuManager.Instance.ActiveMenu.OnItemDisplayDragged(this, false);
	}

	public void OnDrop(PointerEventData eventData)
	{
		MenuManager.Instance.ActiveMenu.OnItemDisplayDropped(this);
	}
}