using UnityEngine;

public class PickupManager : MonoBehaviour
{
	public static PickupManager Instance { get; private set; }

	public PickupData data;

	private Transform transformCache;
	private ObjectPool<PickupNew> pickupPool;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		transformCache = transform;
		pickupPool = new(CreatePickup, 20);
	}

	public void SetData(PickupData newData)
	{
		if (newData == data) return;

		data = newData;

		if (data == null) return;

		data.SortGoldIcons();
	}

	public void ItemPickup(ItemData itemData, int quantity, Vector3 position, float waitTime = 0f)
	{
		var pickup = pickupPool.Get();
		pickup.SetBob(data.bobSpeed, data.bobHeight);
		pickup.SetItem(itemData, quantity, position, waitTime);
	}

	public void GoldPickup(float amount, Vector3 position, float waitTime = 0f)
	{
		var pickup = pickupPool.Get();
		Sprite goldIcon = null;
		for (int i = 0; i < data.goldIcons.Count; i++)
		{
			if (amount >= data.goldIcons[i].amountThreshold)
			{
				goldIcon = data.goldIcons[i].icon;
			}
			else
			{
				break;
			}
		}
		pickup.SetBob(0f, 0f);
		pickup.SetGold(amount, goldIcon, position, waitTime);
	}

	PickupNew CreatePickup()
	{
		var pickup = new GameObject("Pickup").AddComponent<PickupNew>();
		pickup.transform.SetParent(transformCache);
		pickup.OnEnabledChanged += (enabled) =>
		{
			if (!enabled)
			{
				ReturnPickup(pickup);
			}
		};
		return pickup;
	}

	void ReturnPickup(PickupNew pickup)
	{
		if (pickup == null) return;
		pickupPool.Return(pickup);
	}
}