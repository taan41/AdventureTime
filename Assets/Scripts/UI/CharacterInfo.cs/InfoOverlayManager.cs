using System;
using System.Collections.Generic;
using UnityEngine;

public partial class InfoOverlayManager : MonoBehaviour, IUpdatable
{
	public static InfoOverlayManager Instance { get; private set; }

	public event Action<bool> OnEnabledChanged;

	public bool Enabled { get; private set; } = false;

	public bool UseUpdate { get; private set; } = false;

	public bool UseFixedUpdate { get; private set; } = true;

	public bool UseUnscaledTime { get; private set; } = true;

	[SerializeField] private InfoOverlay prefab;
	private InfoOverlay overlay;

	private Character displayingHero = null;
	private readonly List<SkillIconDisplay> skillIcons = new();

	private int frameCounter = 0;
	private float elapsedTime = 0f;
	private float highestDeltaTime = 0f;

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

		UpdaterManager.StaticRegisterLastUpdater(this);

		overlay = Instantiate(prefab, UIReferences.Instance.battleOverlayCanvas.transform);

		HeroParty.Instance.OnControlledHeroChanged += RefreshDisplayingHero;

		RefreshDisplayingHero();

		Enable(true);
	}

	public void Enable(bool enabled)
	{
		Enabled = enabled;
		overlay.panel.SetActive(enabled);
		OnEnabledChanged?.Invoke(enabled);
	}

	public void DoUpdate(float deltaTime) { }

	public void DoFixedUpdate(float fixedDeltaTime)
	{
		if (displayingHero != null)
			overlay.positionText.text = displayingHero.TransformCache.position.ToString("F2");
	}

	public void DoUnscaledUpdate(float unscaledDeltaTime)
	{
		frameCounter++;
		elapsedTime += unscaledDeltaTime;
		highestDeltaTime = Mathf.Max(highestDeltaTime, unscaledDeltaTime);

		if (elapsedTime >= 1f)
		{
			float fps = frameCounter / elapsedTime;
			overlay.fpsText.text = $"FPS: {fps:F1} (Lowest: {1f / highestDeltaTime:F1})";

			frameCounter = 0;
			highestDeltaTime = 0f;
			elapsedTime -= 1f;
		}
	}

	public void SetDebugText(string text, Color color)
	{
		overlay.debugText.color = color;
		overlay.debugText.text = text;
	}

	private void RefreshDisplayingHero()
	{
		var currentHero = HeroParty.Instance.ControlledHero;

		if (displayingHero == currentHero) return;
		if (displayingHero != null)
		{
			displayingHero.HealthModule.OnResourceChanged -= RefreshResources;
			displayingHero.ManaModule.OnResourceChanged -= RefreshResources;
			displayingHero.StaminaModule.OnResourceChanged -= RefreshResources;
			displayingHero.InventoryHandler.OnGoldChanged -= RefreshGold;
			displayingHero.SkillHandler.OnSkillsChange -= RefreshSkills;
		}

		displayingHero = currentHero;

		if (displayingHero == null) return;

		RefreshBasicInfo();
		RefreshResources();
		RefreshGold();
		RefreshSkills();

		displayingHero.HealthModule.OnResourceChanged += RefreshResources;
		displayingHero.ManaModule.OnResourceChanged += RefreshResources;
		displayingHero.StaminaModule.OnResourceChanged += RefreshResources;
		displayingHero.InventoryHandler.OnGoldChanged += RefreshGold;
		displayingHero.SkillHandler.OnSkillsChange += RefreshSkills;
	}

	private void RefreshBasicInfo()
	{
		overlay.nameText.text = displayingHero.Data.characterName;
		overlay.icon.sprite = displayingHero.Data.icon;
	}

	private void RefreshResources()
	{
		overlay.healthFillImage.fillAmount = displayingHero.HealthModule.Normalized;
		overlay.healthText.text = displayingHero.HealthModule.Current.ToString("F0");
		overlay.manaFillImage.fillAmount = displayingHero.ManaModule.Normalized;
		overlay.staminaFillImage.fillAmount = displayingHero.StaminaModule.Normalized;
	}

	private void RefreshGold()
	{
		overlay.goldText.text = displayingHero.InventoryHandler.Gold.ToString("F0");
	}
	
	private void RefreshSkills()
	{
		for (int i = 0; i < displayingHero.SkillHandler.Skills.Count; i++)
		{
			if (i < skillIcons.Count)
			{
				skillIcons[i].SetSkill(displayingHero.SkillHandler.Skills[i]);
			}
			else
			{
				var newIcon = Instantiate(overlay.skillIconPrefab, overlay.skillsContainer);
				newIcon.SetSkill(displayingHero.SkillHandler.Skills[i]);
				skillIcons.Add(newIcon);
			}
		}

		for (int i = displayingHero.SkillHandler.Skills.Count; i < skillIcons.Count; i++)
		{
			skillIcons[i].SetSkill(null);
		}
	}
}