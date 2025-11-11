using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerMenu
{
	[System.Serializable]
	public class ItemsPage
	{
		public RectTransform itemsPageTransform;
		public RectTransform equipmentContainer;
		public RectTransform inventoryContainer;
		public RectTransform equipmentIndicatorContainer;
		public RectTransform inventoryIndicatorContainer;
		public PageIndicator pageIndicatorPrefab;
		public RectTransform equipmentPageButtonContainer;
		public RectTransform inventoryPageButtonContainer;
		public Button prevEquipButton;
		public Button nextEquipButton;
		public Button prevInventoryButton;
		public Button nextInventoryButton;
		public Button sortInventoryButton;
	}

	private class DisplayContainerManager
	{
		private readonly List<ItemDisplay> itemDisplays = new();
		private readonly List<PageIndicator> pageIndicators = new();
		private readonly RectTransform displayContainer;
		private readonly RectTransform indicatorContainer;
		private readonly PageIndicator pageIndicatorPrefab;
		private readonly RectTransform pageButtonContainer;
		private readonly int displayPerPage;

		private int pageCount = 0;
		private int currentPage = 0;
		private PageIndicator activeIndicator;

		private ItemContainer itemContainerRef;

		public DisplayContainerManager(RectTransform displayContainer, RectTransform indicatorContainer, PageIndicator pageIndicatorPrefab, Button prevButton, Button nextButton, RectTransform pageButtonContainer, Button sortButton = null)
		{
			this.displayContainer = displayContainer;
			this.indicatorContainer = indicatorContainer;
			this.pageIndicatorPrefab = pageIndicatorPrefab;
			this.pageButtonContainer = pageButtonContainer;

			// Initialize item displays
			for (int i = 0; i < this.displayContainer.childCount; i++)
			{
				if (this.displayContainer.GetChild(i).TryGetComponent(out ItemDisplay itemDisplay))
				{
					itemDisplays.Add(itemDisplay);
				}
			}
			displayPerPage = itemDisplays.Count;

			// Initialize page indicators
			for (int i = 0; i < indicatorContainer.childCount; i++)
			{
				if (indicatorContainer.GetChild(i).TryGetComponent(out PageIndicator pageIndicator))
				{
					pageIndicators.Add(pageIndicator);
				}
			}

			// Button listeners
			prevButton.onClick.AddListener(PreviousPage);
			nextButton.onClick.AddListener(NextPage);
			if (sortButton != null) sortButton.onClick.AddListener(SortItems);
		}

		public void SetItemContainer(ItemContainer itemContainer)
		{
			itemContainerRef = itemContainer;

			pageCount = Mathf.CeilToInt((float)itemContainerRef.ItemCount / displayPerPage);


			for (int i = 0; i < pageCount; i++)
			{
				if (i >= pageIndicators.Count)
				{
					var newIndicator = Instantiate(pageIndicatorPrefab, indicatorContainer);
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
			pageButtonContainer.gameObject.SetActive(pageCount > 1);

			RefreshDisplay();
		}

		public void NextPage()
		{
			if (pageCount <= 1) return;

			activeIndicator.image.sprite = activeIndicator.inactiveSprite;
			currentPage = (currentPage + 1) % pageCount;
			activeIndicator = pageIndicators[currentPage];
			activeIndicator.image.sprite = activeIndicator.activeSprite;

			RefreshDisplay();
		}

		public void PreviousPage()
		{
			if (pageCount <= 1) return;

			activeIndicator.image.sprite = activeIndicator.inactiveSprite;
			currentPage = (currentPage - 1 + pageCount) % pageCount;
			activeIndicator = pageIndicators[currentPage];
			activeIndicator.image.sprite = activeIndicator.activeSprite;

			RefreshDisplay();
		}

		public void SortItems()
		{
			if (itemContainerRef == null) return;

			itemContainerRef.Sort();

			currentPage = 0;
			activeIndicator.image.sprite = activeIndicator.inactiveSprite;
			activeIndicator = pageIndicators[0];
			activeIndicator.image.sprite = activeIndicator.activeSprite;

			RefreshDisplay();
		}

		public void RefreshDisplay()
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

	public ItemsPage items;

	private DisplayContainerManager equipmentDisplayManager;
	private DisplayContainerManager inventoryDisplayManager;

	private void InitializeItems()
	{
		equipmentDisplayManager = new DisplayContainerManager(
			items.equipmentContainer,
			items.equipmentIndicatorContainer,
			items.pageIndicatorPrefab,
			items.prevEquipButton,
			items.nextEquipButton,
			items.equipmentPageButtonContainer
		);

		inventoryDisplayManager = new DisplayContainerManager(
			items.inventoryContainer,
			items.inventoryIndicatorContainer,
			items.pageIndicatorPrefab,
			items.prevInventoryButton,
			items.nextInventoryButton,
			items.inventoryPageButtonContainer,
			items.sortInventoryButton
		);
	}

	private void RefreshItems()
	{
		equipmentDisplayManager.SetItemContainer(displayedHero.InventoryHandler.equipment);
		inventoryDisplayManager.SetItemContainer(displayedHero.InventoryHandler.inventory);
	}
}