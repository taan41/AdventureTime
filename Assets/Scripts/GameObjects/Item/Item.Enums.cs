public partial class Item
{
	public enum Type
	{
		None,
		Equipment,
		Consumable,
		Material,
		Miscellaneous,
	}

	public static Type[] AllItemTypes = (Type[])System.Enum.GetValues(typeof(Type));

	public enum Rarity
	{
		Legendary,
		Epic,
		Rare,
		Uncommon,
		Common,
	}

	public static readonly Rarity[] AllItemRarities = (Rarity[])System.Enum.GetValues(typeof(Rarity));
	
	public enum EquipmentType
	{
		None,
		Weapon,
		Accessory,
		Helmet,
		Chestplate,
		Leggings,
		Boots,
	}

	public static EquipmentType[] AllEquipmentSlots = (EquipmentType[])System.Enum.GetValues(typeof(EquipmentType));

	public enum ItemSlotType
	{
		None,
		Inventory,
		Equipment,
		QuickAccess,
		Storage,
		Shop
	}
}