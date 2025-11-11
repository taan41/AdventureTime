using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerMenu
{
	[System.Serializable]
	public class ProfilePage
	{
		public Text heroName;
		public Image heroAvatar;
		public Image healthBar;
		public Text healthText;
		public Image manaBar;
		public Text manaText;
		public Image staminaBar;
		public Text staminaText;
		public Button prevCharButton;
		public Button nextCharButton;
		public RectTransform statsContainer;
		public RectTransform heroIndicatorContainer;
		public HeroIndicator indicatorPrefab;
	}

	public ProfilePage profile;

	private readonly List<StatDisplayNew> statDisplays = new();
	private readonly List<HeroIndicator> heroIndicators = new();

	private AnimationModule heroAnimRef;
	private AnimationModuleLite animModule;
	
	private ResourceModule tempResourceModule;

	private void InitializeProfile()
	{
		animModule = new(profile.heroAvatar);

		var statsContainer = profile.statsContainer;
		for (int i = 0; i < statsContainer.childCount; i++)
		{
			if (statsContainer.GetChild(i).TryGetComponent(out StatDisplayNew statDisplay))
			{
				statDisplays.Add(statDisplay);
			}
		}

		var heroIndicatorContainer = profile.heroIndicatorContainer;
		for (int i = 0; i < heroIndicatorContainer.childCount; i++)
		{
			if (heroIndicatorContainer.GetChild(i).TryGetComponent(out HeroIndicator heroIndicator))
			{
				heroIndicators.Add(heroIndicator);
			}
		}

		profile.prevCharButton.onClick.AddListener(HeroParty.Instance.SetPreviousHero);
		profile.nextCharButton.onClick.AddListener(HeroParty.Instance.SetNextHero);
	}

	private void RefreshProfile()
	{
		// Name
		profile.heroName.text = displayedHero.Data.characterName;

		// Avatar
		heroAnimRef = displayedHero.AnimationModule;
		if (heroAnimRef.sprites != null)
		{
			profile.heroAvatar.sprite = heroAnimRef.sprites[0];
			profile.heroAvatar.SetNativeSize();
			animModule.Enable(true);
			animModule.Play(heroAnimRef.sprites, 1f / heroAnimRef.frameDuration);
		}
		else
		{
			animModule.Enable(false);
			profile.heroAvatar.sprite = displayedHero.Data.defaultSprite;
		}

		HeroIndicator heroIndicator;
		int heroIndex = HeroParty.Instance.ControlledHeroIndex;
		for (int i = 0; i < HeroParty.Instance.ActiveHeroes.Count; i++)
		{
			if (i >= heroIndicators.Count)
			{
				heroIndicator = Instantiate(profile.indicatorPrefab, profile.heroIndicatorContainer);
				heroIndicators.Add(heroIndicator);
			}
			else
			{
				heroIndicator = heroIndicators[i];
				if (!heroIndicator.gameObject.activeSelf)
				{
					heroIndicator.gameObject.SetActive(true);
				}
			}

			if (i == heroIndex)
			{
				heroIndicator.indicatorImage.sprite = displayedHero.Data.icon;
			}
			else
			{
				heroIndicator.indicatorImage.sprite = heroIndicator.defaultSprite;
				heroIndicator.indicatorImage.type = UnityEngine.UI.Image.Type.Simple;
			}
			heroIndicator.indicatorImage.SetNativeSize();
		}

		for (int i = HeroParty.Instance.ActiveHeroes.Count; i < heroIndicators.Count; i++)
		{
			heroIndicators[i].gameObject.SetActive(false);
		}

		RefreshResourceDisplays();
		RefreshStatDisplays();
	}

	private void RefreshResourceDisplays()
	{
		tempResourceModule = displayedHero.HealthModule;
		profile.healthBar.fillAmount = tempResourceModule.Normalized;
		profile.healthText.text = $"{tempResourceModule.Current}/{tempResourceModule.Max}";

		tempResourceModule = displayedHero.ManaModule;
		profile.manaBar.fillAmount = tempResourceModule.Normalized;
		profile.manaText.text = $"{tempResourceModule.Current}/{tempResourceModule.Max}";

		tempResourceModule = displayedHero.StaminaModule;
		profile.staminaBar.fillAmount = tempResourceModule.Normalized;
		profile.staminaText.text = $"{tempResourceModule.Current}/{tempResourceModule.Max}";
	}

	private void RefreshStatDisplays()
	{
		StatDisplayNew statDisplay;
		for (int i = 0; i < statDisplays.Count; i++)
		{
			statDisplay = statDisplays[i];
			if (statDisplay.percentage)
			{
				statDisplay.statValue.text = $"{displayedHero.Stats[statDisplay.statType].Final * 100:F0}%";
			}
			else
			{
				statDisplay.statValue.text = displayedHero.Stats[statDisplay.statType].Final.ToString("F0");
			}
		}
	}
	
	private void CloseProfile()
	{
		animModule.Enable(false);
	}
}