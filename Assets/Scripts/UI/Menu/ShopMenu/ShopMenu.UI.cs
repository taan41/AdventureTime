using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class ShopMenu
{
	#region Shop UI
	[Serializable]
	private class ShopUI
	{
		public RectTransform shopTransform;
		public DetectArea detectArea;
		public Text name;
		public Image avatar;
		public Text goldSpent;
		public Text sellPricePercentage;
		public Text buyPricePercentage;
		public RectTransform inventoryContainer;
		public RectTransform inventoryNavigateButtonContainer;
		public Button prevInventoryButton;
		public Button nextInventoryButton;
		public Button sortInventoryButton;
		public RectTransform indicatorContainer;
		public PageIndicator indicatorPrefab;

		private readonly List<ItemDisplay> itemDisplays = new();
		private readonly List<PageIndicator> pageIndicators = new();

		private int pageCount = 0;
		private int currentPage = 0;
		private int displayPerPage;
		private PageIndicator activeIndicator;

		private ItemContainer itemContainerRef;
		private ShopInfoPerSession shopInfoRef;

		public Character Seller { get; private set; }

		public void Initialize()
		{
			itemDisplays.Clear();
			pageIndicators.Clear();

			for (int i = 0; i < inventoryContainer.childCount; i++)
			{
				if (inventoryContainer.GetChild(i).TryGetComponent(out ItemDisplay itemDisplay))
				{
					itemDisplays.Add(itemDisplay);
				}
			}
			displayPerPage = itemDisplays.Count;

			for (int i = 0; i < indicatorContainer.childCount; i++)
			{
				if (indicatorContainer.GetChild(i).TryGetComponent(out PageIndicator pageIndicator))
				{
					pageIndicators.Add(pageIndicator);
				}
			}

			prevInventoryButton.onClick.AddListener(PreviousPage);
			nextInventoryButton.onClick.AddListener(NextPage);
			sortInventoryButton.onClick.AddListener(SortItems);
		}

		public void SetSeller(Character seller, ShopInfoPerSession shopInfo)
		{
			Seller = seller;

			if (Seller == null)
			{
				name.text = "";
				avatar.sprite = null;
				SetItemContainer(null);

				if (shopInfoRef != null)
				{
					shopInfoRef.OnGoldSpentChanged -= RefreshShopInfo;
				}

				shopInfoRef = null;
				return;
			}

			name.text = Seller.Data.characterName;
			avatar.sprite = Seller.Data.defaultSprite;
			avatar.SetNativeSize();

			SetItemContainer(Seller.InventoryHandler.inventory);

			if (shopInfoRef != null)
			{
				shopInfoRef.OnGoldSpentChanged -= RefreshShopInfo;
			}

			shopInfoRef = shopInfo;

			if (shopInfoRef != null)
			{
				RefreshShopInfo();
				shopInfoRef.OnGoldSpentChanged += RefreshShopInfo;
			}
		}

		private void RefreshShopInfo()
		{
			goldSpent.text = shopInfoRef.GoldSpent.ToString("F0");
			sellPricePercentage.text = shopInfoRef.SellPercentage.ToString("F1") + "%";
			buyPricePercentage.text = shopInfoRef.BuyPercentage.ToString("F1") + "%";
		}

		public void SetItemContainer(ItemContainer itemContainer)
		{
			itemContainerRef = itemContainer;

			if (itemContainer == null) return;

			pageCount = Mathf.CeilToInt((float)itemContainerRef.ItemCount / displayPerPage);

			for (int i = 0; i < pageCount; i++)
			{
				if (i >= pageIndicators.Count)
				{
					var newIndicator = Instantiate(indicatorPrefab, indicatorContainer);
					pageIndicators.Add(newIndicator);
				}
				pageIndicators[i].gameObject.SetActive(true);
				pageIndicators[i].image.sprite = pageIndicators[i].inactiveSprite;
			}

			for (int i = pageCount; i < pageIndicators.Count; i++)
			{
				pageIndicators[i].gameObject.SetActive(false);
			}

			currentPage = 0;
			activeIndicator = pageIndicators[0];
			activeIndicator.image.sprite = activeIndicator.activeSprite;
			inventoryNavigateButtonContainer.gameObject.SetActive(pageCount > 1);

			RefreshInventory();
		}

		public void NextPage()
		{
			if (pageCount <= 1) return;

			activeIndicator.image.sprite = activeIndicator.inactiveSprite;
			currentPage = (currentPage + 1) % pageCount;
			activeIndicator = pageIndicators[currentPage];
			activeIndicator.image.sprite = activeIndicator.activeSprite;

			RefreshInventory();
		}

		public void PreviousPage()
		{
			if (pageCount <= 1) return;

			activeIndicator.image.sprite = activeIndicator.inactiveSprite;
			currentPage = (currentPage - 1 + pageCount) % pageCount;
			activeIndicator = pageIndicators[currentPage];
			activeIndicator.image.sprite = activeIndicator.activeSprite;

			RefreshInventory();
		}

		public void SortItems()
		{
			if (itemContainerRef == null) return;

			itemContainerRef.Sort();

			currentPage = 0;
			activeIndicator.image.sprite = activeIndicator.inactiveSprite;
			activeIndicator = pageIndicators[0];
			activeIndicator.image.sprite = activeIndicator.activeSprite;

			RefreshInventory();
		}

		public void RefreshInventory()
		{
			if (itemContainerRef == null) return;

			ItemDisplay itemDisplay;
			int itemIndex;
			for (int i = 0; i < itemDisplays.Count; i++)
			{
				itemDisplay = itemDisplays[i];
				itemIndex = currentPage * displayPerPage + i;

				if (itemIndex < itemContainerRef.ItemCount)
				{
					itemDisplay.gameObject.SetActive(true);
					itemDisplay.SetItemRef(itemContainerRef.items[itemIndex]);
				}
				else
				{
					itemDisplay.gameObject.SetActive(false);
				}
			}
		}
	}
	#endregion

	#region Inventory UI
	[Serializable]
	private class InventoryUI
	{
		public RectTransform inventoryTransform;
		public DetectArea detectArea;
		public Text name;
		public Image avatar;
		public Text goldText;
		public Button prevHeroButton;
		public Button nextHeroButton;
		public RectTransform inventoryContainer;
		public RectTransform inventoryNavigateButtonContainer;
		public Button prevInventoryButton;
		public Button nextInventoryButton;
		public Button sortInventoryButton;
		public RectTransform indicatorContainer;
		public PageIndicator indicatorPrefab;

		private readonly List<ItemDisplay> itemDisplays = new();
		private readonly List<PageIndicator> pageIndicators = new();

		private int pageCount = 0;
		private int currentPage = 0;
		private int displayPerPage;
		private PageIndicator activeIndicator;

		private ItemContainer itemContainerRef;

		public Character DisplayedHero { get; private set; }

		public void Initialize()
		{
			itemDisplays.Clear();
			pageIndicators.Clear();

			for (int i = 0; i < inventoryContainer.childCount; i++)
			{
				if (inventoryContainer.GetChild(i).TryGetComponent(out ItemDisplay itemDisplay))
				{
					itemDisplays.Add(itemDisplay);
				}
			}
			displayPerPage = itemDisplays.Count;

			for (int i = 0; i < indicatorContainer.childCount; i++)
			{
				if (indicatorContainer.GetChild(i).TryGetComponent(out PageIndicator pageIndicator))
				{
					pageIndicators.Add(pageIndicator);
				}
			}

			prevInventoryButton.onClick.AddListener(PreviousPage);
			nextInventoryButton.onClick.AddListener(NextPage);
			sortInventoryButton.onClick.AddListener(SortItems);

			prevHeroButton.onClick.AddListener(HeroParty.Instance.SetPreviousHero);
			nextHeroButton.onClick.AddListener(HeroParty.Instance.SetNextHero);

			HeroParty.Instance.OnControlledHeroChanged += OnControlldHeroChanged;
			OnControlldHeroChanged();
		}

		public void Close()
		{
			if (DisplayedHero != null)
			{
				DisplayedHero.InventoryHandler.OnGoldChanged -= RefreshHeroGold;
			}
		}

		private void OnControlldHeroChanged()
		{
			if (DisplayedHero == HeroParty.Instance.ControlledHero) return;

			if (DisplayedHero != null)
			{
				DisplayedHero.InventoryHandler.OnGoldChanged -= RefreshHeroGold;
			}

			DisplayedHero = HeroParty.Instance.ControlledHero;

			if (DisplayedHero != null)
			{
				name.text = DisplayedHero.Data.characterName;
				avatar.sprite = DisplayedHero.Data.defaultSprite;
				avatar.SetNativeSize();

				SetItemContainer(DisplayedHero.InventoryHandler.inventory);

				DisplayedHero.InventoryHandler.OnGoldChanged += RefreshHeroGold;
				RefreshHeroGold();
			}
		}

		public void RefreshHeroGold()
		{
			goldText.text = DisplayedHero.InventoryHandler.Gold.ToString("F0");
		}

		public void SetItemContainer(ItemContainer itemContainer)
		{
			itemContainerRef = itemContainer;

			pageCount = Mathf.CeilToInt((float)itemContainerRef.ItemCount / displayPerPage);


			for (int i = 0; i < pageCount; i++)
			{
				if (i >= pageIndicators.Count)
				{
					var newIndicator = Instantiate(indicatorPrefab, indicatorContainer);
					pageIndicators.Add(newIndicator);
				}
				pageIndicators[i].gameObject.SetActive(true);
				pageIndicators[i].image.sprite = pageIndicators[i].inactiveSprite;
			}

			for (int i = pageCount; i < pageIndicators.Count; i++)
			{
				pageIndicators[i].gameObject.SetActive(false);
			}

			currentPage = 0;
			activeIndicator = pageIndicators[0];
			activeIndicator.image.sprite = activeIndicator.activeSprite;
			inventoryNavigateButtonContainer.gameObject.SetActive(pageCount > 1);

			RefreshInventory();
		}

		public void NextPage()
		{
			if (pageCount <= 1) return;

			activeIndicator.image.sprite = activeIndicator.inactiveSprite;
			currentPage = (currentPage + 1) % pageCount;
			activeIndicator = pageIndicators[currentPage];
			activeIndicator.image.sprite = activeIndicator.activeSprite;

			RefreshInventory();
		}

		public void PreviousPage()
		{
			if (pageCount <= 1) return;

			activeIndicator.image.sprite = activeIndicator.inactiveSprite;
			currentPage = (currentPage - 1 + pageCount) % pageCount;
			activeIndicator = pageIndicators[currentPage];
			activeIndicator.image.sprite = activeIndicator.activeSprite;

			RefreshInventory();
		}

		public void SortItems()
		{
			if (itemContainerRef == null) return;

			itemContainerRef.Sort();

			currentPage = 0;
			activeIndicator.image.sprite = activeIndicator.inactiveSprite;
			activeIndicator = pageIndicators[0];
			activeIndicator.image.sprite = activeIndicator.activeSprite;

			RefreshInventory();
		}

		public void RefreshInventory()
		{
			if (itemContainerRef == null) return;

			ItemDisplay itemDisplay;
			int itemIndex;
			for (int i = 0; i < itemDisplays.Count; i++)
			{
				itemDisplay = itemDisplays[i];
				itemIndex = currentPage * displayPerPage + i;

				if (itemIndex < itemContainerRef.ItemCount)
				{
					itemDisplay.gameObject.SetActive(true);
					itemDisplay.SetItemRef(itemContainerRef.items[itemIndex]);
				}
				else
				{
					itemDisplay.gameObject.SetActive(false);
				}
			}
		}
	}
	#endregion

	[SerializeField] private ShopUI shopUI = new();
	[SerializeField] private InventoryUI inventoryUI = new();
}