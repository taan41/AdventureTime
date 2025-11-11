using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
// [RequireComponent(typeof(Rigidbody2D))]
public partial class Projectile : MonoBehaviour, IUpdatable
{
	private static Unity.Mathematics.Random projectileRNG = new((uint)DateTime.Now.Ticks);

	public event Action<bool> OnEnabledChanged;

	private SpriteRenderer spriteRenderer;
	private CapsuleCollider2D capsuleCollider;
	// private Rigidbody2D rigidBody;

	public Character owner;
	private Transform ownerTransform;

	private Skill skill;

	private ProjectileData physicData;
	private ProjectileDataPerSkill specificData;
	private ComponentData compData;
	// private AnimationData<ProjectilePhase> animData;

	public Transform TransformCache { get; private set; }

	public MovementModule MovementModule { get; private set; }
	public AnimationModule AnimationPlayer { get; private set; }

	private readonly CombatEffectHandler combatEffectHandler = new();

	private Updater<Projectile> updater;
	private bool isRegistered = false;

	public bool Enabled { get; private set; } = false;
	public bool UseUpdate { get; private set; } = true;
	public bool UseFixedUpdate { get; private set; } = true;
	public bool UseUnscaledTime { get; private set; } = false;

	private bool initialized = false;

	private Vector2 baseRendererSize = Vector2.one;
	private Vector2 colliderBaseSize = Vector2.one;
	private Vector2 colliderBaseOffset = Vector2.zero;

	public void Initialize(Character owner, Skill sourceSkill)
	{
		if (initialized) return;
		initialized = true;

		gameObject.layer = LayerMask.NameToLayer("Projectile");

		this.owner = owner;
		ownerTransform = owner.TransformCache;
		owner.OnDeath += OnOwnerDeath;

		skill = sourceSkill;

		spriteRenderer = GetComponent<SpriteRenderer>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		// rigidBody = GetComponent<Rigidbody2D>();

		TransformCache = transform;
		TransformCache.SetParent(ownerTransform);

		MovementModule = new(TransformCache);
		AnimationPlayer = new(spriteRenderer);

		updater = UpdaterManager.Instance.GetUpdater<Projectile>();
	}

	public void SetData(ProjectileData newPhysicData, ProjectileDataPerSkill newSpecificData, bool force = false)
	{
		if (!initialized) return;

		if (!force && physicData == newPhysicData) return;

		if (physicData != null && physicData.IsValid())
		{
			physicData.OnDataChanged -= RefreshPhysicData;
			physicData.ComponentData.OnDataChanged -= RefreshComponents;
		}

		physicData = newPhysicData;
		specificData = newSpecificData;

		if (specificData != null) RefreshSpecificData();

		if (physicData != null && physicData.IsValid())
		{
			compData = physicData.ComponentData;

			RefreshPhysicData();
			RefreshComponents();

			physicData.OnDataChanged += RefreshPhysicData;
			physicData.ComponentData.OnDataChanged += RefreshComponents;
		}
	}

	public void Enable(bool enabled)
	{
		Enabled = enabled;

		MovementModule.Enable(enabled);
		AnimationPlayer.Enable(enabled);

		spriteRenderer.enabled = enabled;
		capsuleCollider.enabled = enabled;
		// rigidBody.simulated = enabled;

		if (enabled)
		{
			if (!isRegistered)
			{
				updater.Register(this);
				isRegistered = true;
			}
		}
		else
		{
			TransformCache.SetParent(ownerTransform);
		}

		OnEnabledChanged?.Invoke(enabled);
	}

	public void DoUpdate(float deltaTime)
	{
		if (!Enabled) return;

		UpdateBehavior(deltaTime);
	}

	public void DoFixedUpdate(float fixedDeltaTime)
	{
		if (!Enabled) return;

		FixedUpdateController(fixedDeltaTime);
	}

	public void DoUnscaledUpdate(float unscaledDeltaTime) { }

	private void RefreshPhysicData()
	{
		if (physicData == null || !physicData.IsValid()) return;
	}

	private void RefreshSpecificData()
	{
		if (specificData == null) return;

		name = specificData.projectileName;

		combatEffectHandler.SetData(specificData.combatEffectData);
	}

	private void RefreshComponents()
	{
		if (compData == null) return;

		spriteRenderer.sprite = physicData.defaultSprite;

		compData.SetComponents(physicData.defaultSprite, spriteRenderer, capsuleCollider, null, out baseRendererSize);

		// baseRendererSize = spriteRenderer.size;

		colliderBaseSize = capsuleCollider.size;
		colliderBaseOffset = capsuleCollider.offset;
	}
}