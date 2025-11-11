using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObjects/Item/ItemData")]
public class ItemData : CustomSO<ItemData>
{
	[System.Serializable]
	public class StatBuff
	{
		public Character.StatType statType = Character.StatType.STR;
		public float amount = 0f;
	}

	[System.Serializable]
	public class StatMultiplier
	{
		public Character.StatType statType = Character.StatType.STR;
		public Stat.Multiplier multiplier = new(1f);
	}

	public Sprite icon;
	public string itemName;
	public string description;

	public Item.Type itemType = Item.Type.None;
	public Item.Rarity rarity = Item.Rarity.Common;
	public Item.EquipmentType equipmentSlot = Item.EquipmentType.None;
	public List<StatBuff> statBuffs = new();
	public List<StatMultiplier> statMultipliers = new();
	public SkillData grantedSkill = null;
	public bool recoverConsumable = false;
	public bool stackable = true;
	public int maxStack = 99;
	public float basePrice = 0f;

	public bool IsEquipment => itemType == Item.Type.Equipment && equipmentSlot != Item.EquipmentType.None;

	protected override void OnValidate()
	{
		if (!stackable || maxStack < 1) maxStack = 1;
		if (itemType != Item.Type.Consumable)
		{
			recoverConsumable = false;
		}

		base.OnValidate();
	}

	public override void CopyFrom(ItemData other)
	{
		if (other == null) return;

		icon = other.icon;
		itemName = other.itemName;
		description = other.description;

		itemType = other.itemType;
		rarity = other.rarity;
		equipmentSlot = other.equipmentSlot;
		recoverConsumable = other.recoverConsumable;
		stackable = other.stackable;
		maxStack = other.maxStack;

		statBuffs = new(other.statBuffs);
		statMultipliers = new(other.statMultipliers);
		grantedSkill = other.grantedSkill;

		SignalDataChange();
	}

	public int CompareTo(ItemData other)
	{
		if (other == null) return -1;
		if (itemType != other.itemType) return itemType.CompareTo(other.itemType);
		if (rarity != other.rarity) return rarity.CompareTo(other.rarity);
		if (equipmentSlot != other.equipmentSlot) return equipmentSlot.CompareTo(other.equipmentSlot);
		return itemName.CompareTo(other.itemName);
	}
}