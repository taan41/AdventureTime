using System;
using UnityEngine;

[Serializable]
public partial class Item
{
	[Serializable]
	public struct ItemRequirement
	{
		public Type typeRequirement;
		public EquipmentType equipRequirement;

		public ItemRequirement(Type typeRequirement = Type.None, EquipmentType equipRequirement = EquipmentType.None)
		{
			this.typeRequirement = typeRequirement;
			this.equipRequirement = equipRequirement;
		}

		public ItemRequirement(ItemRequirement other)
		{
			typeRequirement = other.typeRequirement;
			equipRequirement = other.equipRequirement;
		}

		public readonly bool Matches(Type typeRequirement, EquipmentType equipRequirement)
		{
			if (this.typeRequirement != Type.None && this.typeRequirement != typeRequirement) return false;
			if (this.equipRequirement != EquipmentType.None && this.equipRequirement != equipRequirement) return false;
			return true;
		}

		public readonly bool Matches(ItemData data)
		{
			if (data == null) return true;
			return Matches(data.itemType, data.equipmentSlot);
		}

		public static bool operator ==(ItemRequirement a, ItemRequirement b) => a.typeRequirement == b.typeRequirement && a.equipRequirement == b.equipRequirement;

		public static bool operator !=(ItemRequirement a, ItemRequirement b) => !(a == b);

		public override readonly bool Equals(object obj)
		{
			if (obj is ItemRequirement other)
			{
				return this == other;
			}
			return false;
		}

		public override readonly int GetHashCode()
		{
			return HashCode.Combine(typeRequirement, equipRequirement);
		}

		public readonly int CompareTo(ItemRequirement other)
		{
			if (typeRequirement != other.typeRequirement)
			{
				return typeRequirement.CompareTo(other.typeRequirement);
			}
			return equipRequirement.CompareTo(other.equipRequirement);
		}
	}

	public event Action OnDataChanged;
	public event Action OnQuantityChanged;

	[SerializeField] private ItemData data;
	public ItemData Data
	{
		get => data;
		private set
		{
			data = value;
			OnDataChanged?.Invoke();
		}
	}

	[SerializeField] private int quantity = 0;
	public int Quantity
	{
		get => quantity;
		private set
		{
			quantity = value;
			OnQuantityChanged?.Invoke();
		}
	}

	public int MaxStack => Data == null ? 0 : Data.stackable && Data.maxStack > 0 ? Data.maxStack : 1;
	[HideInInspector] public ItemRequirement requirement = default;
	[HideInInspector] public Character owner = null;
	[HideInInspector] public ItemSlotType slotType = ItemSlotType.None;

	public Item() { }

	public Item(Item other)
	{
		if (other == null) return;

		owner = other.owner;
		Data = other.Data;
		Quantity = other.Quantity;
		requirement = new(other.requirement);
	}

	public bool SetData(ItemData newData, int newQuantity)
	{
		if (!requirement.Matches(newData)) return false;
		if (slotType == ItemSlotType.Equipment) ApplyItemEffects(owner, false);

		Data = newData;
		SetQuantity(newQuantity, out _);
		if (slotType == ItemSlotType.Equipment) ApplyItemEffects(owner, true);
		return true;
	}

	public bool SetData(ItemData newData, int newQuantity, out int leftoverQuantity, out ItemData oldData, out int oldQuantity)
	{
		oldData = null;
		oldQuantity = 0;
		leftoverQuantity = 0;

		if (!requirement.Matches(newData)) return false;

		oldData = Data;
		oldQuantity = Quantity;
		if (slotType == ItemSlotType.Equipment) ApplyItemEffects(owner, false);

		Data = newData;
		SetQuantity(newQuantity, out leftoverQuantity);
		if (slotType == ItemSlotType.Equipment) ApplyItemEffects(owner, true);
		return true;
	}

	public bool SetDataOrModifyQuantity(ItemData data, int quanity, out int leftoverQuantity)
	{
		leftoverQuantity = 0;

		if (Data != null && Data == data)
		{
			return ModifyQuantity(quanity, out leftoverQuantity);
		}
		else if (Data == null)
		{
			if (!requirement.Matches(data)) return false;

			Data = data;
			Quantity = 0;
			ModifyQuantity(quanity, out leftoverQuantity);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Modifies the quantity of the item. If the quantity exceeds max stack, the leftover quantity is returned. If the quantity drops to 0 or below, the item data is cleared, and the taken quantity is returned.
	/// </summary>
	public bool ModifyQuantity(int modifyQuantity, out int returnedQuantity)
	{
		returnedQuantity = 0;

		if (Data == null) return false;
		if (modifyQuantity == 0) return true;

		returnedQuantity = Quantity;
		Quantity += modifyQuantity;

		if (Quantity > MaxStack)
		{
			returnedQuantity = Quantity - MaxStack;
			Quantity = MaxStack;
		}
		else if (Quantity <= 0)
		{
			Quantity = 0;
			Data = null;
		}
		else
		{
			returnedQuantity = 0;
		}
		return true;
	}

	public void SetQuantity(int newQuantity, out int leftoverQuantity)
	{
		leftoverQuantity = 0;

		if (newQuantity < 0) Quantity = 0;
		else if (Data != null && newQuantity > MaxStack)
		{
			Quantity = MaxStack;
			leftoverQuantity = newQuantity - MaxStack;
		}
		else Quantity = newQuantity;

		if (Quantity == 0) Data = null;
	}

	public void Use(Character character = null)
	{
		if (Data == null) return;
		if (character == null) character = owner;

		switch (Data.itemType)
		{
			case Type.Consumable:
				ApplyItemEffects(character);
				ModifyQuantity(-1, out _);
				break;

			case Type.Equipment:
				switch (slotType)
				{
					case ItemSlotType.Equipment:
						if (character.InventoryHandler.inventory.Add(Data, Quantity, out ItemData returnedData, out int returnedQuantity))
						{
							SetData(returnedData, returnedQuantity);
						}
						break;

					case ItemSlotType.Inventory:
						if (character.InventoryHandler.equipment.Add(Data, Quantity, out returnedData, out returnedQuantity))
						{
							SetData(returnedData, returnedQuantity);
						}
						break;
				}
				break;
		}
	}

	void ApplyItemEffects(Character character, bool apply = true)
	{
		if (Data == null || character == null) return;

		bool doRecover = Data.itemType == Type.Consumable && Data.recoverConsumable;

		ItemData.StatBuff buff;
		for (int i = 0; i < Data.statBuffs.Count; i++)
		{
			buff = Data.statBuffs[i];

			if (doRecover)
				character.Recover(buff.statType, buff.amount);
			else
				character.Stats[buff.statType].Bonus += buff.amount * (apply ? 1 : -1);
		}

		ItemData.StatMultiplier multiplier;
		for (int i = 0; i < Data.statMultipliers.Count; i++)
		{
			multiplier = Data.statMultipliers[i];

			if (apply)
			{
				if (doRecover)
					character.RecoverRatio(multiplier.statType, multiplier.multiplier.value);
				else
					character.Stats[multiplier.statType].AddMultiplier(multiplier.multiplier);
			}
			else if (!doRecover)
			{
				character.Stats[multiplier.statType].RemoveMultiplier(multiplier.multiplier);
			}
		}

		if (apply)
			character.SkillHandler.AddSkillData(Data.grantedSkill);
		else
			character.SkillHandler.RemoveSkillData(Data.grantedSkill);
	}

	public void Drop(Vector3 position, int dropQuantity = 0)
	{
		PickupManager.Instance.ItemPickup(Data, dropQuantity > 0 ? dropQuantity : Quantity, position, 1.5f);
		ModifyQuantity(- (dropQuantity > 0 ? dropQuantity : Quantity), out _);
	}
	
	public int CompareTo(Item other, bool reverse = false)
	{
		if (other == null) return -1;
		if (Data == null && other.Data == null) return requirement.CompareTo(other.requirement);
		if (Data == null) return 1;
		if (other.Data == null) return -1;
		return Data.CompareTo(other.Data) * (reverse ? -1 : 1);
	}
}