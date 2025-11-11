using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PickupNew : MonoBehaviour, IUpdatable
{
	public event Action<bool> OnEnabledChanged;

	public bool Enabled { get; private set; } = false;
	public bool UseUpdate { get; } = true;
	public bool UseFixedUpdate { get; } = false;
	public bool UseUnscaledTime { get; private set; } = false;

	public Transform transformCache;
	public CapsuleCollider2D capsuleCollider;
	public Rigidbody2D rb;

	public Transform iconTransform;
	public SpriteRenderer icon;
	public Updater<PickupNew> updater;

	public Item pickupItem = new();
	public FloatingText floatingText;
	public float goldAmount = 0f;

	public Vector3 originPosition;
	public float bobHeight = 0.25f;
	public float bobSpeed = 1f;

	private float elapsedTime = 0f;
	private float waitTime = 0f;

	void Awake()
	{
		gameObject.layer = LayerMask.NameToLayer("Pickup");

		transformCache = transform;
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		rb = GetComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Kinematic;
		rb.simulated = false;

		var iconObj = new GameObject("Sprite");
		iconObj.transform.SetParent(transformCache);
		iconTransform = iconObj.transform;
		icon = iconObj.AddComponent<SpriteRenderer>();
		icon.sortingLayerName = "Character";

		updater = UpdaterManager.Instance.GetUpdater<PickupNew>();
		updater.Register(this);
	}

	public void Enable(bool enabled)
	{
		Enabled = enabled;

		capsuleCollider.enabled = enabled;
		rb.simulated = enabled;
		icon.enabled = enabled;

		OnEnabledChanged?.Invoke(enabled);
	}

	[ContextMenu("Activate")]
	public void Activate()
	{
		SetItem(pickupItem.Data, pickupItem.Quantity, transformCache.position);
	}

	public void SetBob(float speed, float height)
	{
		bobSpeed = speed;
		bobHeight = height;
	}

	public void SetItem(ItemData data, int quantity, Vector3 position, float waitTime = 0f)
	{
		if (data == null) return;

		elapsedTime = 0f;
		this.waitTime = waitTime;

		pickupItem.SetData(data, quantity);
		goldAmount = 0f;

		icon.sprite = data.icon;
		icon.material = UIManager.Instance.GetTextureOutlineMaterial(data.icon.texture, data.rarity);

		originPosition = position;
		transformCache.position = position;
		iconTransform.localPosition = Vector3.zero;

		var boundsSize = data.icon.bounds.size;
		transformCache.localScale = new(1f / boundsSize.x, 1f / boundsSize.y, 1f);
		capsuleCollider.direction = boundsSize.y > boundsSize.x ? CapsuleDirection2D.Vertical : CapsuleDirection2D.Horizontal;
		capsuleCollider.size = boundsSize;

		floatingText = FloatingUIObjectManager.Instance.GetText(transformCache, data.itemName, data.rarity);

		Enable(true);
	}

	public void SetGold(float amount, Sprite goldIcon, Vector3 position, float waitTime = 0f)
	{
		if (amount <= 0f) return;

		elapsedTime = 0f;
		this.waitTime = waitTime;

		pickupItem.SetData(null, 0);
		goldAmount = amount;

		icon.sprite = goldIcon;
		icon.material = UIManager.Instance.GetTextureBaseMaterial();

		originPosition = position;
		transformCache.position = position;
		iconTransform.localPosition = Vector3.zero;

		var boundsSize = goldIcon.bounds.size;
		transformCache.localScale = new(1f / boundsSize.x, 1f / boundsSize.y, 1f);
		capsuleCollider.direction = boundsSize.y > boundsSize.x ? CapsuleDirection2D.Vertical : CapsuleDirection2D.Horizontal;
		capsuleCollider.size = boundsSize * 2f;

		Enable(true);
	}

	public void DoUpdate(float deltaTime)
	{
		elapsedTime += deltaTime;

		if (bobHeight > 0f && bobSpeed > 0f)
		{
			float offset = Mathf.Sin(elapsedTime * bobSpeed) * bobHeight;
			iconTransform.localPosition = new Vector3(0f, offset, 0f);
		}

		if (elapsedTime > 30f)
		{
			FloatingUIObjectManager.Instance.Return(floatingText);
			floatingText = null;
			Enable(false);
		}
	}

	public void DoUnscaledUpdate(float unscaledDeltaTime) { }

	public void DoFixedUpdate(float fixedDeltaTime) { }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (elapsedTime < waitTime) return;

		if (other.TryGetComponent<Character>(out var character))
		{
			character.InventoryHandler.Gold += goldAmount;
			goldAmount = 0f;
			character.InventoryHandler.inventory.Add(pickupItem.Data, pickupItem.Quantity, out ItemData returnedData, out int returnedQuantity);
			pickupItem.SetData(returnedData, returnedQuantity, out _, out _, out _);
			if (pickupItem.Data == null || pickupItem.Quantity <= 0)
			{
				FloatingUIObjectManager.Instance.Return(floatingText);
				floatingText = null;
				Enable(false);
			}
		}
	}
}