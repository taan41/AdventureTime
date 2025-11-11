using UnityEngine;

public partial class PlayerMenu : MenuBase
{
	[SerializeField] private CursorOverlayItem itemCursorPrefab;
	[SerializeField] private BookmarkManager bookmarkManager;

	private CursorOverlayItem itemCursor;

	private Character displayedHero = null;

	private Item itemHolder = null;

	private ItemData pointedItemDataRef = null;
	private ItemData comparedItemDataRef = null;

	private Item draggingItemRef = null;
	private Item stackSourceRef = null;

	private void ClearRefs()
	{
		pointedItemDataRef = null;
		comparedItemDataRef = null;
		draggingItemRef = null;
		stackSourceRef = null;
	}

	public override void Initialize()
	{
		InitializeProfile();
		InitializeItems();

		itemCursor = Instantiate(itemCursorPrefab);

		itemHolder = new()
		{
			owner = null,
			requirement = new(),
			slotType = Item.ItemSlotType.Inventory
		};
		itemHolder.OnDataChanged += RefreshItemHolder;
		itemHolder.OnQuantityChanged += RefreshItemHolder;

		HeroParty.Instance.OnControlledHeroChanged += RefreshDisplayingHero;
		RefreshDisplayingHero();
	}

	public override bool Open(Character _)
	{
		if (!base.Open(_)) return false;

		RefreshDisplayingHero();
		
		return true;
	}

	public override bool Close()
	{
		if (!base.Close()) return false;

		if (itemHolder.Data != null)
		{
			itemHolder.Drop(displayedHero.TransformCache.position + displayedHero.MovementModule.NonZeroDirection * 1.5f);
		}

		menuPanel.SetActive(false);
		ItemTooltipManager.Instance.Show(null);

		ClearRefs();
		CloseProfile();

		bookmarkManager.OnMenuClose();

		return true;
	}

	public override void OnItemDisplayClicked(ItemDisplay itemDisplay)
	{
		if (draggingItemRef != null) return;

		InputFlags inputFlag = GetInputFlags();

		switch (inputFlag)
		{
			case InputFlags.LeftClick:
				ExchangeItemHolder(itemDisplay.ItemRef);
				break;

			case InputFlags.LeftClick | InputFlags.Shift:
				itemDisplay.ItemRef.Use();
				break;

			case InputFlags.RightClick:
				ExchangeItemHolder(itemDisplay.ItemRef, 0f);
				break;

			case InputFlags.RightClick | InputFlags.Shift:
				ExchangeItemHolder(itemDisplay.ItemRef, 0.5f);
				break;
		}

		SetPointedItemRef(itemDisplay.ItemRef);
	}

	public override void OnItemDisplayDragged(ItemDisplay itemDisplay, bool startDragging = true)
	{
		if (startDragging)
		{
			if (draggingItemRef != null) return;

			draggingItemRef = itemDisplay.ItemRef;
			ExchangeItemHolder(draggingItemRef);
		}
		else
		{
			if (draggingItemRef != itemDisplay.ItemRef) return;

			ExchangeItemHolder(draggingItemRef);
			draggingItemRef = null;
		}

		RefreshItemTooltip();
	}

	public override void OnItemDisplayDropped(ItemDisplay itemDisplay)
	{
		if (draggingItemRef == null || itemDisplay.ItemRef == draggingItemRef) return;

		ExchangeItemHolder(itemDisplay.ItemRef);

		draggingItemRef = null;

		RefreshItemTooltip();
	}

	public override void OnItemDisplayPointed(ItemDisplay itemDisplay, bool pointing = true)
	{
		if (pointing && itemDisplay != null)
		{
			SetPointedItemRef(itemDisplay.ItemRef);
		}
		else
		{
			SetPointedItemRef();
		}
	}

	public override void OnBlankAreaClicked()
	{
		if (itemHolder.Data == null)
		{
			MenuManager.Instance.CloseActiveMenu();
			return;
		}

		itemHolder.Drop(displayedHero.TransformCache.position + displayedHero.MovementModule.NonZeroDirection * 1.5f);

		draggingItemRef = null;
		stackSourceRef = null;
	}

	public override void OnBlankAreaDropped()
	{
		itemHolder.Drop(displayedHero.TransformCache.position + displayedHero.MovementModule.NonZeroDirection * 1.5f);

		draggingItemRef = null;
		stackSourceRef = null;
	}

	private void ExchangeItemHolder(Item itemRef, float quantityRatio = 1f)
	{
		if (itemRef == null) return;

		bool nullHolderData = itemHolder.Data == null;
		bool nullRefData = itemRef.Data == null;

		if (nullRefData && nullHolderData) return;

		if (nullHolderData)
		{
			stackSourceRef = itemRef;
		}

		if (itemRef == stackSourceRef)
		{
			if (!nullHolderData && itemHolder.Data != itemRef.Data) return;

			if (quantityRatio > 1f) quantityRatio = 1f;
			int quantity = quantityRatio > 0 ? Mathf.CeilToInt(itemRef.Quantity * quantityRatio) : 1;

			if (itemHolder.SetDataOrModifyQuantity(itemRef.Data, quantity, out var leftoverQuantity))
			{
				itemHolder.owner = itemRef.owner;
				itemRef.ModifyQuantity(-quantity + leftoverQuantity, out _);
				if (itemRef.Data == null || itemRef.Quantity <= 0)
				{
					stackSourceRef = null;
				}
			}
		}
		else
		{
			if (!nullRefData && itemRef.Data != itemHolder.Data)
			{
				if (itemRef.SetData(itemHolder.Data, itemHolder.Quantity, out _, out var oldData, out var oldQuantity))
				{
					itemHolder.SetData(oldData, oldQuantity);
					itemHolder.owner = itemRef.owner;

					if (itemHolder.Data == null || itemHolder.Quantity == 0)
					{
						itemHolder.owner = null;
						stackSourceRef = null;
					}
				}
				return;
			}

			int quantity = quantityRatio > 0 ? Mathf.CeilToInt(itemHolder.Quantity * quantityRatio) : 1;

			if (itemRef.SetDataOrModifyQuantity(itemHolder.Data, quantity, out var leftoverQuantity))
			{
				itemHolder.ModifyQuantity(-quantity + leftoverQuantity, out _);

				if (itemHolder.Data == null || itemHolder.Quantity == 0)
				{
					itemHolder.owner = null;
					stackSourceRef = null;
				}
			}
		}
	}

	private void RefreshItemHolder()
	{
		if (itemHolder.Data != null && itemHolder.Quantity > 0)
		{
			itemCursor.SetItemIcon(itemHolder.Data.icon, itemHolder.Quantity, itemHolder.Data.stackable);
		}
		else
		{
			itemCursor.SetItemIcon(null);
		}

		RefreshItemTooltip();
	}

	private void SetPointedItemRef(Item pointedItemRef = null)
	{
		pointedItemDataRef = pointedItemRef?.Data;

		if (pointedItemDataRef != null && pointedItemRef.owner != null
			&& pointedItemRef.slotType == Item.ItemSlotType.Inventory
			&& pointedItemDataRef.equipmentSlot != Item.EquipmentType.None
		)
		{
			Item.ItemRequirement equipType = new(Item.Type.Equipment, pointedItemDataRef.equipmentSlot);
			comparedItemDataRef = pointedItemRef.owner.InventoryHandler.equipment[equipType].Data;
		}
		else
		{
			comparedItemDataRef = null;
		}
		
		RefreshItemTooltip();
	}
	
	private void RefreshItemTooltip()
	{
		if (itemHolder.Data != null)
			ItemTooltipManager.Instance.Show(itemHolder.Data, pointedItemDataRef);
		else
			ItemTooltipManager.Instance.Show(pointedItemDataRef, comparedItemDataRef, true);
	}

	private void RefreshDisplayingHero()
	{
		if (HeroParty.Instance.ControlledHero == displayedHero)
		{
			if (displayedHero != null)
			{
				RefreshProfile();
				RefreshItems();
			}
			return;
		}

		if (displayedHero != null)
		{
			displayedHero.HealthModule.OnResourceChanged -= RefreshResourceDisplays;
			displayedHero.ManaModule.OnResourceChanged -= RefreshResourceDisplays;
			displayedHero.StaminaModule.OnResourceChanged -= RefreshResourceDisplays;
			displayedHero.Stats.OnStatChanged -= RefreshStatDisplays;
		}

		displayedHero = HeroParty.Instance.ControlledHero;

		if (displayedHero != null)
		{
			RefreshProfile();
			RefreshItems();

			displayedHero.HealthModule.OnResourceChanged += RefreshResourceDisplays;
			displayedHero.ManaModule.OnResourceChanged += RefreshResourceDisplays;
			displayedHero.StaminaModule.OnResourceChanged += RefreshResourceDisplays;
			displayedHero.Stats.OnStatChanged += RefreshStatDisplays;
		}
	}
}