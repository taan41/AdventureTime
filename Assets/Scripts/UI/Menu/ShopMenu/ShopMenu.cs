using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ShopMenu : MenuBase
{
	private enum ShopState
	{
		None,
		Shop,
		Inventory,
	}

	private class ShopInfoPerSession
	{
		public event Action OnGoldSpentChanged;

		private Character.CharacterTradingSetting tradingSetting;
		public Character.CharacterTradingSetting TradingSetting
		{
			get => tradingSetting;
			set
			{
				tradingSetting = value;
				GoldSpent = 0f;
				SellPercentage = tradingSetting.sellPricePercentage.basePercentage;
				BuyPercentage = tradingSetting.buyPricePercentage.basePercentage;
			}
		}

		private Character.CharacterTradingSetting.PricePercentage SellPercentageSetting => TradingSetting.sellPricePercentage;
		private Character.CharacterTradingSetting.PricePercentage BuyPercentageSetting => TradingSetting.buyPricePercentage;

		private float goldSpent = 0f;
		public float GoldSpent
		{
			get => goldSpent;
			set
			{
				goldSpent = value;

				SellPercentage = Mathf.Clamp(
					SellPercentageSetting.basePercentage + SellPercentageSetting.incrementPerProfit * goldSpent,
					SellPercentageSetting.minPercentage,
					SellPercentageSetting.maxPercentage);

				BuyPercentage = Mathf.Clamp(
					BuyPercentageSetting.basePercentage + BuyPercentageSetting.incrementPerProfit * goldSpent,
					BuyPercentageSetting.minPercentage,
					BuyPercentageSetting.maxPercentage);

				OnGoldSpentChanged?.Invoke();
			}
		}

		public float SellPercentage { get; private set; }
		public float BuyPercentage { get; private set; }
	}

	[SerializeField] private CursorOverlayShop shopCursorPrefab;
	private CursorOverlayShop shopCursor;

	private ShopState shopState = ShopState.None;

	private readonly Dictionary<CharacterData, ShopInfoPerSession> characterShopInfo = new();

	private Character Seller => shopUI.Seller;
	private Character Buyer => inventoryUI.DisplayedHero;

	private Item displayedItemRef = null;

	public override void Initialize()
	{
		characterShopInfo.Clear();

		shopUI.Initialize();
		shopUI.detectArea.OnPointerOverChanged += SetShopState;

		inventoryUI.Initialize();
		inventoryUI.detectArea.OnPointerOverChanged += SetInventoryState;

		shopCursor = Instantiate(shopCursorPrefab);

		InputManager.Instance.OnControlInputFlagChanged += ShowDisplayedItemInfo;
	}

	public override bool Open(Character focusedCharacter)
	{
		if (!base.Open(focusedCharacter)) return false;

		if (!characterShopInfo.ContainsKey(focusedCharacter.Data))
		{
			characterShopInfo[focusedCharacter.Data] = new()
			{
				TradingSetting = focusedCharacter.Data.settings.trading
			};
		}

		shopUI.SetSeller(focusedCharacter, characterShopInfo[focusedCharacter.Data]);

		return true;
	}

	public override bool Close()
	{
		if (!IsOpened) return false;

		shopUI.SetSeller(null, null);
		inventoryUI.Close();

		return base.Close();
	}

	private void SetShopState(bool set) => shopState = set ? ShopState.Shop : ShopState.None;
	private void SetInventoryState(bool set) => shopState = set ? ShopState.Inventory : ShopState.None;

	public override void OnItemDisplayClicked(ItemDisplay itemDisplay)
	{
		if (Seller == null || Buyer == null) return;

		var itemRef = itemDisplay.ItemRef;
		var itemDataRef = itemDisplay.ItemRef.Data;

		if (itemDataRef == null) return;

		var inputFlags = GetInputFlags();
		int tradeQuantity = (inputFlags & InputFlags.Shift) != 0 ? itemRef.Quantity : 1;
		float goldValue;
		switch (shopState)
		{
			case ShopState.Shop:
				goldValue = tradeQuantity * itemDataRef.basePrice * characterShopInfo[Seller.Data].SellPercentage / 100f;
				if (goldValue > Buyer.InventoryHandler.Gold) break;
				if (Buyer.InventoryHandler.inventory.Add(itemDataRef, tradeQuantity, out _, out _))
				{
					itemRef.ModifyQuantity(-tradeQuantity, out _);
					Buyer.InventoryHandler.Gold -= goldValue;
					characterShopInfo[Seller.Data].GoldSpent += goldValue;
				}
				break;

			case ShopState.Inventory:
				goldValue = tradeQuantity * itemDataRef.basePrice * characterShopInfo[Seller.Data].BuyPercentage / 100f;
				if (Seller.InventoryHandler.inventory.Add(itemDataRef, tradeQuantity, out _, out _))
				{
					itemRef.ModifyQuantity(-tradeQuantity, out _);
					Buyer.InventoryHandler.Gold += goldValue;
					characterShopInfo[Seller.Data].GoldSpent -= goldValue;
				}
				break;
		}

		inventoryUI.RefreshHeroGold();
		OnItemDisplayPointed(itemDisplay, true);
	}
	public override void OnItemDisplayDragged(ItemDisplay itemDisplay, bool startDragging = true) { }
	public override void OnItemDisplayDropped(ItemDisplay itemDisplay) { }
	public override void OnItemDisplayPointed(ItemDisplay itemDisplay, bool pointing = true)
	{
		if (Seller == null) return;

		if (pointing && itemDisplay.ItemRef.Data != null)
		{
			displayedItemRef = itemDisplay.ItemRef;
		}
		else
		{
			displayedItemRef = null;
		}

		ShowDisplayedItemInfo();
	}

	public override void OnBlankAreaClicked() { }

	public override void OnBlankAreaDropped() { }
	
	private void ShowDisplayedItemInfo()
	{
		if (!IsOpened) return;
		
		if (displayedItemRef == null || displayedItemRef.Data == null)
		{
			shopCursor.SetGoldText(0f, false);
			ItemTooltipManager.Instance.Show(null);
			return;
		}

		var inputFlags = GetInputFlags();
		int tradeQuantity = (inputFlags & InputFlags.Shift) != 0 ? displayedItemRef.Quantity : 1;
		switch (shopState)
		{
			case ShopState.Shop:
				shopCursor.SetGoldText(-tradeQuantity * displayedItemRef.Data.basePrice * characterShopInfo[Seller.Data].SellPercentage / 100f, true);
				break;

			case ShopState.Inventory:
				shopCursor.SetGoldText(tradeQuantity * displayedItemRef.Data.basePrice * characterShopInfo[Seller.Data].BuyPercentage / 100f, true);
				break;
		}
		
		ItemTooltipManager.Instance.Show(displayedItemRef.Data);
	}
}