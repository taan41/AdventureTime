using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterProperties", menuName = "Data/Character/Properties")]
public class CharacterDataProperties : ScriptableObject
{
	[Serializable]
	public class LootTable
	{
		[Serializable]
		public class ItemLoot
		{
			public ItemData itemData;
			public int minQuantity = 1;
			public int quantityVariance = 0;
			public float dropChance = 0f;
		}

		public float goldMin = 0f;
		public float goldVariance = 0f;
		public float goldChance = 0f;
		public List<ItemLoot> items = new();
	}
	
	[Header("Stats & Skills")]
	public OneTypeContainer<Character.StatType> baseStats = new();
	public List<SkillData> initialSkills = new();

	[Header("Inventory & Equipments")]
	public List<ItemContainer.ItemRequirementCount> inventoryRequirements = new();
	public List<ItemContainer.ItemRequirementCount> equipmentRequirements = new();
	public List<Item> inventory = new();
	public List<Item> equipments = new();

	[Header("Loot Table")]
	public LootTable lootTable = new();

	private void OnValidate()
	{
		baseStats.ValidateList();

		for (int i = equipments.Count - 1; i >= 0; i--)
		{
			equipments[i].SetQuantity(1, out _);
		}

		for (int i = inventory.Count - 1; i >= 0; i--)
		{
			var itemData = inventory[i].Data;
			if (itemData == null)
			{
				inventory[i].SetQuantity(1, out _);
			}
			else
			{
				int qty = inventory[i].Quantity;

				if (qty <= 0) inventory.RemoveAt(i);
				else if (qty > itemData.maxStack)
				{
					inventory[i].SetQuantity(itemData.maxStack, out _);
				}
			}
		}
	}
}