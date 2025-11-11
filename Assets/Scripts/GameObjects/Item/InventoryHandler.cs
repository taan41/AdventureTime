using System;
using UnityEngine;

[System.Serializable]
public class InventoryHandler
{
	public event Action OnGoldChanged;

	public Character owner = null;
	public ItemContainer inventory;
	public ItemContainer equipment;
	private float gold = 0f;
	public float Gold
	{
		get => gold;
		set
		{
			if (owner.partyRef != null && owner != owner.partyRef.Leader)
			{
				owner.InventoryHandler.gold = value;
				owner.partyRef.Leader.InventoryHandler.OnGoldChanged?.Invoke();
			}
			else
			{
				gold = value;
				OnGoldChanged?.Invoke();
			}
		}
	}

	public InventoryHandler(Character owner)
	{
		this.owner = owner;
		inventory = new ItemContainer(owner);
		equipment = new ItemContainer(owner);
	}
}