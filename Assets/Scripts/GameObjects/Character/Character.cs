using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public partial class Character : MonoBehaviour, IUpdatable, IInteractable
{
	#region Initialization
	protected virtual void Awake()
	{
		TransformCache = transform;

		spriteRenderer = GetComponent<SpriteRenderer>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		rigidBody = GetComponent<Rigidbody2D>();

		InteractableHeroDetector = IInteractable.CreateHeroDetector(this);
	}

	protected virtual void Start()
	{
		if (initialData != null)
		{
			SetData(initialData);
			Enable(true);
		}
	}

	private void Initialize()
	{
		if (initialized) return;
		initialized = true;

		InventoryHandler = new(this);
		
		HealthModule = new(true);
		StaminaModule = new(false);
		ManaModule = new(false);

		MovementModule = new(TransformCache);
		AnimationModule = new(spriteRenderer);

		Stats = new();
		InitializeStats();

		SkillHandler = new(this);

		ResourceBars = FloatingUIObjectManager.Instance.GetResourceBars();

		UpdaterManager.Instance.GetUpdater<Character>().Register(this);

		HealthModule.OnResourceDepleted += Dying;
	}

	public virtual bool SetData(CharacterData newData, bool refresh = true)
	{
		Initialize();

		if (newData == Data) return false;

		if (ValidData)
		{
			Data.OnDataChanged -= RefreshData;
			Data.componentData.OnDataChanged -= RefreshDataComponents;
		}

		Data = newData;

		if (!ValidData)
		{
			Data = null;
			return false;
		}

		name = Data.characterName;

		if (refresh) RefreshData();

		Data.OnDataChanged += RefreshData;
		Data.componentData.OnDataChanged += RefreshDataComponents;

		return true;
	}

	public void Enable(bool enabled)
	{
		Initialize();
		
		Enabled = enabled;

		SetPlayerControl(ControlledByPlayer);

		HealthModule.Enable(enabled);
		StaminaModule.Enable(enabled);
		ManaModule.Enable(enabled);
		AnimationModule.Enable(enabled);
		MovementModule.Enable(enabled);

		SkillHandler.Enable(enabled);

		spriteRenderer.enabled = enabled;
		capsuleCollider.enabled = enabled;
		rigidBody.simulated = enabled;

		if (enabled)
		{
			RefreshData();
			
			ActiveCharacters[Tag].Add(this);
		}
		else
		{
			ActiveCharacters[Tag].Remove(this);
		}

		OnEnabledChanged?.Invoke(enabled);
	}

	public void Enable(bool enabled, Vector3? position)
	{
		if (position.HasValue) TransformCache.position = position.Value;

		Enable(enabled);
	}
	#endregion

	public void SetPlayerControl(bool controlledByPlayer)
	{
		ControlledByPlayer = controlledByPlayer;

		PlayerInputRef ??= InputManager.Instance.PlayerInput;

		if (controlledByPlayer)
		{
			PlayerInputRef.DashEvent += DashingVoid;
			PlayerInputRef.PrimarySkillEvent += UsingPrimarySkill;
			// PlayerInputRef.SecondarySkillEvent += UsingSecondarySkill;
			PlayerInputRef.WeaponSkillEvent += UsingWeaponSkill;
			PlayerInputRef.InteractEvent += InteractWithNearbyInteractable;
		}
		else
		{
			PlayerInputRef.DashEvent -= DashingVoid;
			PlayerInputRef.PrimarySkillEvent -= UsingPrimarySkill;
			// PlayerInputRef.SecondarySkillEvent -= UsingSecondarySkill;
			PlayerInputRef.WeaponSkillEvent -= UsingWeaponSkill;
			PlayerInputRef.InteractEvent -= InteractWithNearbyInteractable;
		}
	}

	#region Update
	public void DoUpdate(float deltaTime)
	{
		if (ControlledByPlayer)
		{
			direction = PlayerInputRef.InputDirection;
			Moving();
		}

		DoUpdateController(deltaTime);

		if (offcamDespawnTimer > 0f)
		{
			offcamDespawnTimer -= deltaTime;
			if (offcamDespawnTimer <= 0f)
			{
				Enable(false);
				offcamDespawnTimer = 0f;
				InfoOverlayManager.Instance.SetDebugText("", Color.white);
			}
		}
	}

	public void DoFixedUpdate(float fixedDeltaTime)
	{
		CurrentCell = SpatialGrid.Instance.WorldToCell(TransformCache.position);
		if (CurrentCell != lastCell)
		{
			SpatialGrid.Instance.Remove(this, lastCell);
			SpatialGrid.Instance.Add(this, CurrentCell);
			lastCell = CurrentCell;
		}

		if (!ControlledByPlayer && RulesRef.useAI) DoFixedUpdateAI(fixedDeltaTime);
	}

	public void DoUnscaledUpdate(float unscaledDeltaTime) { }
	#endregion

	#region Data Refreshers
	private void RefreshData()
	{
		name = Data.characterName;
		Tag = Data.tag;
		gameObject.layer = LayerMask.NameToLayer(Tag.ToString());

		RefreshDataComponents();
		RefreshDataStats();
		RefreshDataActions();
		RefreshDataAI();

		InteractableHeroDetector.SetRules(Data.settings.interactable);

		SkillHandler.AddSkillDatas(Data.properties.initialSkills, true);
		SkillHandler.AddSkillDatas(Data.actions.GetSkillDatas(), false);

		InventoryHandler.inventory.SetInfo(Data.properties.inventoryRequirements, false, Item.ItemSlotType.Inventory);
		InventoryHandler.inventory.AddRange(Data.properties.inventory, true);

		InventoryHandler.equipment.SetInfo(Data.properties.equipmentRequirements, true, Item.ItemSlotType.Equipment);
		InventoryHandler.equipment.AddRange(Data.properties.equipments, true);

		ResourceBars.SetTarget(this, SettingsRef.showStaminaAndManaBars);
	}

	private void RefreshDataComponents()
	{
		Data.componentData.SetComponents(Data.defaultSprite, spriteRenderer, capsuleCollider, rigidBody, out spriteSize);
		colliderSize = capsuleCollider.size;
		MovementModule.SetWallOffset(0f, spriteSize.y / 2f, spriteSize.x / 2f, spriteSize.x / 2f);
	}
	#endregion

	#region IInteractable
	void OnBecameInvisible()
	{
		offcamDespawnTimer = InteractableRules.offcamDespawnDelay;
	}

	void OnBecameVisible()
	{
		offcamDespawnTimer = 0f;
	}
	#endregion
}