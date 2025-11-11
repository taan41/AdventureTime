using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour, IUpdatable
{
	[Serializable]
	class ItemTooltipPanel
	{
		[SerializeField] public GameObject container;

		[SerializeField] public Image icon;
		[SerializeField] public Text nameText;
		[SerializeField] public Text typeText;

		[SerializeField] public GameObject descriptionContainer;
		[SerializeField] public Text descriptionText;

		[SerializeField] public GameObject statsContainer;
		[SerializeField] public Text statsText;

		[SerializeField] public GameObject skillContainer;
		[SerializeField] public Image skillIcon;
		[SerializeField] public Text skillName;
		[SerializeField] public Text skillType;
	}

	[Serializable]
	class SkillTooltipPanel
	{
		[SerializeField] public GameObject container;

		[SerializeField] public Image icon;
		[SerializeField] public Text nameText;
		[SerializeField] public Text typeText;

		[SerializeField] public GameObject descriptionContainer;
		[SerializeField] public Text descriptionText;

		[SerializeField] public GameObject statsContainer;
		[SerializeField] public Text statsText;
	}

	public event Action<bool> OnEnabledChanged;

	public bool Enabled { get; private set; } = false;
	public bool UseUpdate { get; private set; } = false;
	public bool UseFixedUpdate { get; private set; } = false;
	public bool UseUnscaledTime { get; private set; } = true;

	[SerializeField] private RectTransform rectTransform;
	[SerializeField] private ItemTooltipPanel mainPanel;
	[SerializeField] private ItemTooltipPanel comparePanel;
	[SerializeField] private SkillTooltipPanel skillPanel;

	private RectTransform parentTransform;
	private Camera uiCamera;

	private readonly OneTypeContainer<Character.StatType> displayedStatBuffs = new();
	private readonly OneTypeContainer<Character.StatType> displayedStatMultipliers = new(1f);
	private readonly OneTypeContainer<Character.StatType> compareStatBuffs = new();
	private readonly OneTypeContainer<Character.StatType> compareStatMultipliers = new(1f);
	private readonly StringBuilder stringBuilder = new();

	private ItemData lastItemData, lastCompareData;

	private string positiveColor;
	private string negativeColor;
	private Vector3 offset;

	void Awake()
	{
		UpdaterManager.StaticRegisterLastUpdater(this);

		InputManager.Instance.PlayerActions.PlayerMenu.performed += ctx => Enable(false);
		InputManager.Instance.PlayerActions.Interact.performed += ctx => Enable(false);

		parentTransform = UIReferences.Instance.menuOverlayCanvas.transform as RectTransform;
		uiCamera = CameraManager.Instance.uiCamera;

		rectTransform.SetParent(parentTransform, false);
		Enable(false);
	}

	public void RefreshData()
	{
		positiveColor = ColorUtility.ToHtmlStringRGB(UIManager.Instance.Data.positiveStatColor);
		negativeColor = ColorUtility.ToHtmlStringRGB(UIManager.Instance.Data.negativeStatColor);
		offset = UIManager.Instance.Data.itemTooltipOffset;
	}

	public void Setup(Vector3 offset, Color positiveColor, Color negativeColor)
	{
		this.offset = offset;
		this.positiveColor = ColorUtility.ToHtmlStringRGB(positiveColor);
		this.negativeColor = ColorUtility.ToHtmlStringRGB(negativeColor);
	}

	public void Enable(bool enabled)
	{
		Enabled = enabled;
		gameObject.SetActive(enabled);
		OnEnabledChanged?.Invoke(enabled);
	}

	public void DoUpdate(float deltaTime) { }
	public void DoFixedUpdate(float fixedDeltaTime) { }

	public void DoUnscaledUpdate(float unscaledDeltaTime)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, MouseManager.Instance.MouseScreenPosition, uiCamera, out var localPoint);

		rectTransform.pivot = new Vector2(localPoint.x > 0 ? 1f : 0f, localPoint.y > 0 ? 1f : 0f);

		localPoint.x += localPoint.x > 0 ? -offset.x : offset.x;
		localPoint.y += localPoint.y > 0 ? -offset.y : offset.y;

		localPoint.x = Mathf.Clamp(localPoint.x, -parentTransform.rect.width / 2, parentTransform.rect.width / 2);
		localPoint.y = Mathf.Clamp(localPoint.y, -parentTransform.rect.height / 2, parentTransform.rect.height / 2);

		rectTransform.localPosition = localPoint;
	}

	public void Show(ItemData itemData, ItemData compareData = null)
	{
		if (itemData == null)
		{
			lastItemData = null;
			lastCompareData = null;

			Enable(false);
			return;
		}

		if (itemData == lastItemData && compareData == lastCompareData)
		{
			return;
		}


		lastItemData = itemData;
		lastCompareData = compareData;

		if (compareData != null)
		{
			SetItemPanel(comparePanel, compareData);

			comparePanel.container.SetActive(true);
			skillPanel.container.SetActive(false);
		}
		else
		{
			comparePanel.container.SetActive(false);
		}

		SetItemPanel(mainPanel, itemData, compareData);
		mainPanel.container.SetActive(true);

		if (itemData.grantedSkill != null && compareData == null)
		{
			SetSkillPanel(skillPanel, itemData.grantedSkill);
			skillPanel.container.SetActive(true);
		}
		else
		{
			skillPanel.container.SetActive(false);
		}

		Enable(true);
	}

	private void SetItemPanel(ItemTooltipPanel panel, ItemData data, ItemData compareData = null)
	{
		if (data.icon != null)
		{
			panel.icon.sprite = data.icon;
			panel.icon.SetNativeSize();
			panel.icon.enabled = true;
		}
		else
		{
			panel.icon.enabled = false;
		}

		panel.nameText.text = data.itemName;
		panel.nameText.color = UIManager.Instance.GetRarityColor(data.rarity);
		panel.typeText.text = data.itemType.ToString();

		panel.descriptionText.text = data.description;
		panel.descriptionContainer.SetActive(!string.IsNullOrEmpty(panel.descriptionText.text));

		SetItemStatsText(panel.statsText, data, compareData);
		panel.statsContainer.SetActive(!string.IsNullOrEmpty(panel.statsText.text));

		if (data.grantedSkill != null)
		{
			if (data.grantedSkill.icon != null)
			{
				panel.skillIcon.sprite = data.grantedSkill.icon;
				panel.skillIcon.SetNativeSize();
				panel.skillIcon.enabled = true;
			}
			else
			{
				panel.skillIcon.enabled = false;
			}
			
			panel.skillName.text = data.grantedSkill.skillName;
			panel.skillType.text = data.grantedSkill.type.ToString() + " Skill";

			panel.skillContainer.SetActive(true);
		}
		else
		{
			panel.skillContainer.SetActive(false);
		}
	}

	private void SetSkillPanel(SkillTooltipPanel panel, SkillData data)
	{
		if (data.icon != null)
		{
			panel.icon.sprite = data.icon;
			panel.icon.SetNativeSize();
			panel.icon.enabled = true;
		}
		else
		{
			panel.icon.enabled = false;
		}

		panel.nameText.text = data.skillName;
		panel.typeText.text = data.type.ToString() + " Skill";

		panel.descriptionText.text = data.description;
		panel.descriptionContainer.SetActive(!string.IsNullOrEmpty(panel.descriptionText.text));

		SetSkillStatsText(panel.statsText, data);
		panel.statsContainer.SetActive(!string.IsNullOrEmpty(panel.statsText.text));
	}

	private void SetItemStatsText(Text text, ItemData data1, ItemData data2 = null)
	{
		bool comparing = data2 != null && data1.itemType == Item.Type.Equipment && data1.itemType == data2.itemType && data1.equipmentSlot == data2.equipmentSlot;

		displayedStatBuffs.Clear();
		displayedStatMultipliers.Clear();

		for (int i = 0; i < data1.statBuffs.Count; i++)
		{
			var statBuff = data1.statBuffs[i];
			displayedStatBuffs[statBuff.statType].Value = statBuff.amount;
		}

		for (int i = 0; i < data1.statMultipliers.Count; i++)
		{
			var statMultiplier = data1.statMultipliers[i];
			displayedStatMultipliers[statMultiplier.statType].Value = statMultiplier.multiplier.value;
		}

		if (comparing)
		{
			compareStatBuffs.Clear();
			compareStatMultipliers.Clear();

			for (int i = 0; i < data2.statBuffs.Count; i++)
			{
				var compareStat = data2.statBuffs[i];
				compareStatBuffs[compareStat.statType].Value = compareStat.amount;
			}

			for (int i = 0; i < data2.statMultipliers.Count; i++)
			{
				var compareMultiplier = data2.statMultipliers[i];
				compareStatMultipliers[compareMultiplier.statType].Value = compareMultiplier.multiplier.value;
			}
		}

		stringBuilder.Clear();
		foreach (var statType in Character.AllStatTypes)
		{
			if (displayedStatBuffs[statType].Value != 0f || (comparing && compareStatBuffs[statType].Value != 0f))
			{
				stringBuilder.Append(displayedStatBuffs[statType].Value);
				if (comparing)
				{
					float diff = displayedStatBuffs[statType].Value - compareStatBuffs[statType].Value;
					if (diff != 0f)
					{
						stringBuilder.Append(" <color=#");
						stringBuilder.Append(diff > 0 ? positiveColor : negativeColor);
						stringBuilder.Append(">(");
						stringBuilder.Append(diff > 0 ? "+" : "");
						stringBuilder.Append(diff);
						stringBuilder.Append(")</color>");
					}
				}
				stringBuilder.Append(" ");
				stringBuilder.Append(statType);
				stringBuilder.AppendLine();
			}

			if (displayedStatMultipliers[statType].Value != 1f || (comparing && compareStatMultipliers[statType].Value != 1f))
			{
				float multiplierValue = displayedStatMultipliers[statType].Value - 1f;

				stringBuilder.Append(multiplierValue > 0 ? "+" : "");
				stringBuilder.Append((multiplierValue * 100f).ToString("F1"));
				stringBuilder.Append("% ");
				if (comparing)
				{
					float diff = displayedStatMultipliers[statType].Value - compareStatMultipliers[statType].Value;
					if (diff != 0f)
					{
						stringBuilder.Append("<color=#");
						stringBuilder.Append(diff > 0 ? positiveColor : negativeColor);
						stringBuilder.Append(">(");
						stringBuilder.Append(diff > 0 ? "+" : "");
						stringBuilder.Append((diff * 100f).ToString("F1"));
						stringBuilder.Append("%)</color> ");
					}
				}
				stringBuilder.Append(statType);
				stringBuilder.AppendLine();
			}
		}

		if (stringBuilder.Length > 0)
		{
			stringBuilder.Length -= 1; // Remove last newline
			text.text = stringBuilder.ToString();
		}
		else
		{
			text.text = "";
		}
		stringBuilder.Clear();
	}
	
	private void SetSkillStatsText(Text text, SkillData data)
	{
		stringBuilder.Clear();
		foreach (var statType in Skill.AllSkillStats)
		{
			var stat = data.baseStats[statType];
			if (stat.Value != 0f)
			{
				stringBuilder.Append(statType);
				stringBuilder.Append(": ");
				stringBuilder.Append(stat.Value);
				stringBuilder.AppendLine();
			}
		}

		if (stringBuilder.Length > 0)
		{
			stringBuilder.Length -= 1; // Remove last newline
			text.text = stringBuilder.ToString();
		}
		else
		{
			text.text = "";
		}
		stringBuilder.Clear();
	}
}