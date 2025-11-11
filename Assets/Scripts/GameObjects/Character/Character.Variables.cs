using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Character
{
	#region Static
	public static Dictionary<CharacterTag, HashSet<Character>> ActiveCharacters = new();
	protected static Unity.Mathematics.Random CharacterRNG = new((uint)DateTime.Now.Ticks);

	static Character()
	{
		foreach (CharacterTag tag in Enum.GetValues(typeof(CharacterTag)))
		{
			ActiveCharacters[tag] = new();
		}
	}
	#endregion

	#region Events
	public event Action<bool> OnEnabledChanged;
	public event Action<Character> OnDeath;
	#endregion

	#region Basic
	// ---- IUpdatable
	public bool Enabled { get; private set; } = false;
	public bool UseUpdate { get; private set; } = true;
	public bool UseFixedUpdate { get; private set; } = true;
	public bool UseUnscaledTime { get; private set; } = false;

	// ---- Character Data
	[SerializeField] private CharacterData initialData = null;
	public CharacterData Data { get; private set; } = null;
	public bool ValidData => Data != null && Data.IsValid();

	// ---- Initialization
	private bool initialized = false;

	// ---- Unity Components
	public Transform TransformCache { get; private set; }
	private SpriteRenderer spriteRenderer;
	private CapsuleCollider2D capsuleCollider;
	private Rigidbody2D rigidBody;

	// ---- Modules
	public ResourceModule HealthModule { get; private set; }
	public ResourceModule StaminaModule { get; private set; }
	public ResourceModule ManaModule { get; private set; }
	public MovementModule MovementModule { get; private set; }
	public AnimationModule AnimationModule { get; private set; }

	// ---- UI
	public FloatingResourceBars ResourceBars { get; private set; }

	// ---- Other References
	private InputManager.PlayerInputHandler PlayerInputRef;
	private Updater<Character> updater;

	// ---- Identification
	public CharacterTag Tag = CharacterTag.Hero;
	public bool ControlledByPlayer;

	public Vector2Int CurrentCell { get; private set; } = Vector2Int.zero;
	private Vector2Int lastCell = Vector2Int.zero;

	// ---- Handlers
	public InventoryHandler InventoryHandler { get; private set; }
	public SkillHandler SkillHandler { get; private set; }

	// ---- Interactable
	public Transform InteractableTransform => TransformCache;
	public IInteractable.InteractableRule InteractableRules => Data.settings.interactable;
	public IInteractable.HeroDetector InteractableHeroDetector { get; private set; }
	public readonly List<IInteractable> NearbyInteractables = new();


	// ---- Other Variables
	public CharacterTradingSetting TradingSetting => Data.settings.trading;
	public StatContainerNew<StatType> Stats;

	private Vector2 spriteSize = Vector2.zero;
	private Vector2 colliderSize = Vector2.zero;

	public HeroParty partyRef = null;

	private float offcamDespawnTimer = 0f;
	#endregion
}