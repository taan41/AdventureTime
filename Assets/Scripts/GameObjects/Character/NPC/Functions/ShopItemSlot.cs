// using UnityEngine.EventSystems;

// public class ShopItemSlot : ItemSlotNew
// {
// 	private float priceMultiplier = 1f;
// 	private bool isHeroSlot = true;

// 	public void Setup(float newPriceMultiplier, bool isHeroSlot)
// 	{
// 		priceMultiplier = newPriceMultiplier;
// 		this.isHeroSlot = isHeroSlot;
// 	}

// 	public override void OnPointerClick(PointerEventData eventData)
// 	{
// 		if (ItemData != null && HeroParty.Instance.PartyGold < -ItemData.basePrice * priceMultiplier)
// 		{
// 			return;
// 		}

// 		base.OnPointerClick(eventData);
// 	}

// 	protected override void ShowTooltip()
// 	{
// 		if (ItemData != null)
// 		{
// 			MouseManager.Instance.SetShopMouse(ItemData.basePrice * priceMultiplier, true);
// 		}
// 		ItemTooltipManager.Instance.SetItemDataRef(ItemData);
// 	}

// 	protected override void HideTooltip()
// 	{
// 		MouseManager.Instance.SetShopMouse(0, false);
// 		ItemTooltipManager.Instance.SetItemDataRef(null);
// 	}
// }