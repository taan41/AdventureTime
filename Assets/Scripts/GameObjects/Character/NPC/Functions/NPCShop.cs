// using System;
// using System.Collections.Generic;
// using UnityEngine;

// [CreateAssetMenu(fileName = "NPC Shop", menuName = "Data/Character/NPC Functions/Shop")]
// public class NPCShop : NPCFunction
// {
// 	public NPCShopWindow shopWindowPrefab;
// 	public ShopItemSlot shopItemSlotPrefab;
// 	public Sprite shopBackground;
// 	public List<ItemStack> shopItems = new();
// 	public float buyMultiplier = 1.0f;
// 	public float sellMultiplier = 0.5f;
// 	public int shopSlotCount = 25;

// 	[NonSerialized] private NPCShopWindow shopWindowInstance;
// 	[NonSerialized] private ItemContainer shopInventory;
// 	[NonSerialized] private bool buying = true;
// 	[NonSerialized] private NPC currentNPC;
// 	[NonSerialized] private Character prevHero;

// 	public override void Activate(NPC npc, Character character)
// 	{
// 		HeroParty.Instance.OnControlledHeroChanged += AutoSetCurrentHero;
// 		ItemTooltipManager.Instance.SetShopCursor(true);

// 		currentNPC = npc;
// 		prevHero = character;

// 		if (shopWindowInstance == null)
// 		{
// 			shopWindowInstance = Instantiate(shopWindowPrefab);

// 			shopWindowInstance.OnToggle += OnWindowToggle;
// 		}

// 		shopInventory = new ItemContainer(null);
// 		shopInventory.SetInfo(default, shopSlotCount, false);
// 		shopInventory.AddRange(shopItems);

// 		var inventory = character.InventoryHandler.inventory;
// 		inventory.SetSwapContainer(shopInventory, true);
// 		inventory.OnItemAdded += OnBuyItem;
// 		inventory.OnItemRemoved += SetSelling;

// 		shopInventory.SetSwapContainer(inventory, true);
// 		shopInventory.OnItemAdded += OnSellItem;
// 		shopInventory.OnItemRemoved += SetBuying;

// 		shopWindowInstance.Setup(currentNPC, shopInventory, shopItemSlotPrefab, shopBackground, -buyMultiplier, sellMultiplier);
// 		shopWindowInstance.Toggle(true);
// 	}

// 	public override void Deactivate(NPC npc, Character character)
// 	{
// 		HeroParty.Instance.OnControlledHeroChanged -= AutoSetCurrentHero;
// 		ItemTooltipManager.Instance.SetShopCursor(false);

// 		if (shopInventory == null) return;
		
// 		currentNPC = null;

// 		var inventory = character.InventoryHandler.inventory;
// 		inventory.SetSwapContainer(character.InventoryHandler.inventory);
// 		inventory.OnItemAdded -= OnBuyItem;
// 		inventory.OnItemRemoved -= SetSelling;

// 		shopInventory.SetSwapContainer(null);
// 		shopInventory.OnItemAdded -= OnSellItem;
// 		shopInventory.OnItemRemoved -= SetBuying;
// 	}

// 	private void OnWindowToggle(bool enabled)
// 	{
// 		if (!enabled)
// 		{
// 			Deactivate(currentNPC, prevHero);
// 		}
// 	}

// 	private void AutoSetCurrentHero()
// 	{
// 		var currentHero = HeroParty.Instance.ControlledHero;
// 		if (prevHero != null && prevHero != currentHero)
// 		{
// 			var inventory = prevHero.InventoryHandler.inventory;
// 			inventory.SetSwapContainer(prevHero.InventoryHandler.inventory);
// 			inventory.OnItemAdded -= OnBuyItem;
// 			inventory.OnItemRemoved -= SetSelling;

// 			inventory = currentHero.InventoryHandler.inventory;
// 			inventory.SetSwapContainer(shopInventory, true);
// 			inventory.OnItemAdded += OnBuyItem;
// 			inventory.OnItemRemoved += SetSelling;

// 			prevHero = currentHero;
// 		}
// 	}

// 	private void OnBuyItem(ItemData itemData, int quantity)
// 	{
// 		if (!buying) return;

// 		HeroParty.Instance.PartyGold -= itemData.basePrice * buyMultiplier * quantity;
// 	}

// 	private void OnSellItem(ItemData itemData, int quantity)
// 	{
// 		if (buying) return;

// 		HeroParty.Instance.PartyGold += itemData.basePrice * sellMultiplier * quantity;
// 	}

// 	private void SetBuying(ItemData _, int __)
// 	{
// 		buying = true;
// 	}

// 	private void SetSelling(ItemData _, int __)
// 	{
// 		buying = false;
// 	}
// }