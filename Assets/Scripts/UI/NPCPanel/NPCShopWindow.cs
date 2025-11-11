// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// using static Character.Action;
// using static EightWayDirection;

// public class NPCShopWindow : IngameWindow
// {
// 	[Header("Decorations")]
// 	public Image backgroundImage;
// 	public Image npcImage;
// 	public Image heroImage;

// 	[Header("Inventory")]
// 	public GridLayoutGroup inventoryGridLayout;
// 	public RectTransform inventoryTransform;

// 	[Header("Infos")]
// 	public TextMeshProUGUI goldText;
// 	public Button buyButton;
// 	public Button sellButton;
// 	public Button prevHeroButton;
// 	public Button nextHeroButton;

// 	private Character CurrentHero => HeroParty.Instance.ControlledHero;
// 	private AnimationData<Character.Action> HeroAnimData => CurrentHero.Data.animationData;
// 	private ItemContainer HeroInventory => CurrentHero.InventoryHandler.inventory;

// 	private NPC npc;
// 	private ItemContainer npcInventory;
// 	private ShopItemSlot shopItemSlotPrefab;
// 	private float buyMultiplier;
// 	private float sellMultiplier;

// 	private readonly List<ShopItemSlot> inventorySlots = new();

// 	private AnimationModuleLite npcAnimModule;
// 	private AnimationModuleLite heroAnimModule;

// 	protected override void Awake()
// 	{
// 		base.Awake();

// 		npcAnimModule = new(npcImage);
// 		heroAnimModule = new(heroImage);

// 		buyButton.onClick.AddListener(SetNPCInventory);
// 		sellButton.onClick.AddListener(SetHeroInventory);

// 		prevHeroButton.onClick.AddListener(HeroParty.Instance.SetPreviousHero);
// 		nextHeroButton.onClick.AddListener(HeroParty.Instance.SetNextHero);

// 		HeroParty.Instance.OnControlledHeroChanged += RefreshHero;
// 		HeroParty.Instance.OnPartyGoldChanged += RefreshInfos;
// 	}

// 	public void Setup(NPC npc, ItemContainer npcInventory, ShopItemSlot shopItemSlotPrefab, Sprite background, float buyMultiplier, float sellMultiplier)
// 	{
// 		this.npc = npc;
// 		this.npcInventory = npcInventory;
// 		this.shopItemSlotPrefab = shopItemSlotPrefab;
// 		this.buyMultiplier = buyMultiplier;
// 		this.sellMultiplier = sellMultiplier;

// 		backgroundImage.sprite = background;

// 		RefreshNPC();
// 		RefreshHero();
// 		RefreshInfos();
// 	}

// 	void Update()
// 	{
// 		var timeDelta = Time.unscaledDeltaTime;
// 		npcAnimModule.DoUpdate(timeDelta);
// 		heroAnimModule.DoUpdate(timeDelta);
// 	}

// 	private void RefreshInfos()
// 	{
// 		goldText.text = HeroParty.Instance.PartyGold.ToString("F0");
// 	}

// 	private void RefreshNPC()
// 	{
// 		if (npc == null) return;

// 		var npcAnimData = npc.Data.animationData;
// 		npcAnimModule.Enable(true);
// 		if (npcAnimData.HasClip(Idle, RightDown))
// 		{
// 			npcAnimModule.SetClip(npcAnimData.Clips[(Idle, RightDown)]);
// 		}
// 		else if (npcAnimData.HasClip(Idle, Down))
// 		{
// 			npcAnimModule.SetClip(npcAnimData.Clips[(Idle, Down)]);
// 		}
// 		else if (npcAnimData.DefaultClip != null)
// 		{
// 			npcAnimModule.SetClip(npcAnimData.DefaultClip);
// 		}
// 		else
// 		{
// 			npcAnimModule.Enable(false);
// 			npcImage.sprite = npcAnimData.DefaultSprite;
// 		}
// 	}

// 	private void RefreshHero()
// 	{
// 		if (CurrentHero == null) return;

// 		var animData = HeroAnimData;
// 		heroAnimModule.Enable(true);
// 		if (animData.HasClip(Idle, LeftDown))
// 		{
// 			heroAnimModule.SetClip(animData.Clips[(Idle, LeftDown)]);
// 		}
// 		else if (animData.HasClip(Idle, Down))
// 		{
// 			heroAnimModule.SetClip(animData.Clips[(Idle, Down)]);
// 		}
// 		else if (animData.DefaultClip != null)
// 		{
// 			heroAnimModule.SetClip(animData.DefaultClip);
// 		}
// 		else
// 		{
// 			heroAnimModule.Enable(false);
// 			heroImage.sprite = animData.DefaultSprite;
// 		}
// 	}

// 	private void SetNPCInventory()
// 	{
// 		var npcInv = npcInventory;
// 		for (int i = 0; i < npcInv.ItemCount; i++)
// 		{
// 			if (i >= inventorySlots.Count)
// 			{
// 				var newSlot = Instantiate(shopItemSlotPrefab, inventoryTransform);
// 				inventorySlots.Add(newSlot);
// 			}

// 			var slot = inventorySlots[i];
// 			slot.SetItemRef(npcInv[i]);
// 			slot.Setup(buyMultiplier, false);
// 			slot.CleanUpOverlay();
// 			slot.gameObject.SetActive(true);
// 		}

// 		if (inventorySlots.Count > npcInv.ItemCount)
// 		{
// 			for (int i = npcInv.ItemCount; i < inventorySlots.Count; i++)
// 			{
// 				inventorySlots[i].SetItemRef(null);
// 			}
// 		}

// 		int rows = Mathf.CeilToInt((float)npcInv.ItemCount / inventoryGridLayout.constraintCount);
// 		float height = inventoryGridLayout.padding.top + inventoryGridLayout.padding.bottom +
// 			(inventoryGridLayout.cellSize.y * rows) +
// 			(inventoryGridLayout.spacing.y * (rows - 1));
// 		inventoryTransform.sizeDelta = new Vector2(inventoryTransform.sizeDelta.x, height);
// 	}

// 	private void SetHeroInventory()
// 	{
// 		var heroInv = HeroInventory;
// 		for (int i = 0; i < heroInv.ItemCount; i++)
// 		{
// 			if (i >= inventorySlots.Count)
// 			{
// 				var newSlot = Instantiate(shopItemSlotPrefab, inventoryTransform);
// 				inventorySlots.Add(newSlot);
// 			}

// 			var slot = inventorySlots[i];
// 			slot.SetItemRef(heroInv[i]);
// 			slot.Setup(sellMultiplier, true);
// 			slot.CleanUpOverlay();
// 			slot.gameObject.SetActive(true);
// 		}

// 		if (inventorySlots.Count > heroInv.ItemCount)
// 		{
// 			for (int i = heroInv.ItemCount; i < inventorySlots.Count; i++)
// 			{
// 				inventorySlots[i].SetItemRef(null);
// 			}
// 		}

// 		int rows = Mathf.CeilToInt((float)heroInv.ItemCount / inventoryGridLayout.constraintCount);
// 		float height = inventoryGridLayout.padding.top + inventoryGridLayout.padding.bottom +
// 			(inventoryGridLayout.cellSize.y * rows) +
// 			(inventoryGridLayout.spacing.y * (rows - 1));
// 		inventoryTransform.sizeDelta = new Vector2(inventoryTransform.sizeDelta.x, height);
// 	}
// }