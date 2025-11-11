using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer
{
	[Serializable]
	public struct ItemRequirementCount
	{
		public Item.ItemRequirement requirement;
		public int count;

		public ItemRequirementCount(Item.ItemRequirement requirement, int count)
		{
			this.requirement = requirement;
			this.count = count;
		}
	}

	public event Action OnItemCountChanged;
	// public event Action<ItemData, int> OnItemAdded;
	// public event Action<ItemData, int> OnItemRemoved;

	public readonly List<Item> items = new();
	private Item.ItemRequirement defaultRequirement = default;
	public bool fixedCapacity = false;
	public bool swapFirstOnFull = false;
	public Character owner = null;
	private Item.ItemSlotType defaultSlotType = Item.ItemSlotType.Inventory;

	public int ItemCount => items.Count;

	public ItemContainer(Character owner)
	{
		this.owner = owner;
	}

	public Item this[int index]
	{
		get => Get(index);
	}

	public Item this[Item.ItemRequirement requirement]
	{
		get
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].requirement == requirement)
				{
					return items[i];
				}
			}
			return null;
		}
	}

	public void SetInfo(List<ItemRequirementCount> requirementCounts, bool swapFirstOnFull = false, Item.ItemSlotType slotType = Item.ItemSlotType.None)
	{
		this.swapFirstOnFull = swapFirstOnFull;
		defaultSlotType = slotType;

		if (requirementCounts != null && requirementCounts.Count > 0)
		{
			defaultRequirement = requirementCounts[0].requirement;
			int itemIndex = 0;
			for (int i = 0; i < requirementCounts.Count; i++)
			{
				for (int j = 0; j < requirementCounts[i].count; j++)
				{
					if (itemIndex < items.Count)
					{
						items[itemIndex].SetData(null, 0, out _, out _, out _);
						items[itemIndex].requirement = requirementCounts[i].requirement;
						items[itemIndex].slotType = slotType;
					}
					else
					{
						AddNewItem(null, 0, requirementCounts[i].requirement, slotType);
					}
					itemIndex++;
				}
			}
			if (items.Count > itemIndex)
			{
				for (int i = items.Count - 1; i >= itemIndex; i--)
				{
					items.RemoveAt(i);
				}
			}
		}
		else
		{
			defaultRequirement = new();
			Clear();
		}

		items.Sort((a, b) => a.requirement.CompareTo(b.requirement));
		fixedCapacity = items.Count > 0;

		OnItemCountChanged?.Invoke();
	}

	public void SetInfo(Item.ItemRequirement requirement, int capacity = 0, bool swapFirstOnFull = false, Item.ItemSlotType slotType = Item.ItemSlotType.None)
	{
		this.swapFirstOnFull = swapFirstOnFull;
		defaultRequirement = requirement;
		defaultSlotType = slotType;

		if (capacity > 0)
		{
			int itemIndex = 0;
			for (int i = 0; i < capacity; i++)
			{
				if (itemIndex < items.Count)
				{
					items[itemIndex].SetData(null, 0, out _, out _, out _);
					items[itemIndex].requirement = requirement;
				}
				else
				{
					AddNewItem(null, 0, requirement, slotType);
				}
				itemIndex++;
			}
			if (items.Count > itemIndex)
			{
				for (int i = items.Count - 1; i >= itemIndex; i--)
				{
					items.RemoveAt(i);
				}
			}
		}
		else
		{
			Clear();
		}

		fixedCapacity = capacity > 0;

		OnItemCountChanged?.Invoke();
	}

	public bool Add(ItemData data, int quantity, out ItemData returnedData, out int returnedQuantity)
	{
		returnedData = null;
		returnedQuantity = 0;

		if (data == null || quantity <= 0) return false;

		bool addedNewItem = false;
		int originalQuantity = quantity;
		int leftoverQuantity = 0;
		int itemIndex = 0;
		while (quantity > 0)
		{
			if (itemIndex < items.Count)
			{
				if (items[itemIndex].Data == null)
				{
					if (items[itemIndex].SetData(data, quantity, out leftoverQuantity, out _, out _))
					{
						quantity = leftoverQuantity;
					}
				}
				else if (items[itemIndex].Data == data)
				{
					if (items[itemIndex].ModifyQuantity(quantity, out leftoverQuantity))
					{
						quantity = leftoverQuantity;
					}
				}
			}
			else
			{
				if (fixedCapacity)
				{
					if (!swapFirstOnFull)
					{
						returnedData = data;
						returnedQuantity = quantity;
						return originalQuantity != quantity;
					}

					return this[new Item.ItemRequirement(data.itemType, data.equipmentSlot)]?.SetData(data, quantity, out _, out returnedData, out returnedQuantity) ?? false;
				}
				else
				{
					quantity = AddNewItem(data, quantity, defaultRequirement, defaultSlotType);
					addedNewItem = true;
				}
			}

			if (quantity <= 0) break;

			itemIndex++;
		}

		if (addedNewItem) OnItemCountChanged?.Invoke();

		return originalQuantity != quantity;
	}
	
	public void AddRange(List<Item> addingItems, bool clear = false)
	{
		if (addingItems == null || addingItems.Count == 0) return;

		if (clear) Clear();

		for (int i = 0; i < addingItems.Count; i++)
		{
			Add(addingItems[i].Data, addingItems[i].Quantity, out _, out _);
		}
	}

	public Item Get(int index)
	{
		if (index < 0 || index >= items.Count) return null;
		return items[index];
	}

	public void Clear()
	{
		for (int i = 0; i < items.Count; i++)
		{
			items[i].SetData(null, 0);
		}
	}

	public void Sort(bool reverse = false)
	{
		items.Sort((a, b) => a.CompareTo(b, reverse));
	}

	private int AddNewItem(ItemData data, int quantity, Item.ItemRequirement requirement, Item.ItemSlotType slotType)
	{
		Item newItem = new()
		{
			owner = owner,
			requirement = requirement,
			slotType = slotType
		};
		newItem.SetData(data, quantity, out quantity, out _, out _);
		items.Add(newItem);
		return quantity;
	}
}
