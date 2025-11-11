using UnityEngine;
using UnityEngine.UI;

public class SkillIconDisplay : MonoBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private Image cooldownOverlay;

	public Skill Skill { get; private set; } = null;
	// private float totalCastTime;
	private float totalCooldown;

	private bool Enabled = false;

	public void SetSkill(Skill newSkill)
	{
		if (Skill != null)
		{
			// Skill.OnCastTimeStart -= RefreshCastTime;
			Skill.OnCooldownStart -= RefreshCooldown;
		}

		Skill = newSkill;

		if (!CheckSkillData()) return;

		icon.sprite = Skill.Data.icon;
		icon.SetNativeSize();
		cooldownOverlay.fillAmount = 0f;

		// totalCastTime = 0f;
		totalCooldown = 0f;

		// Skill.OnCastTimeStart += RefreshCastTime;
		Skill.OnCooldownStart += RefreshCooldown;

		Enabled = true;
		gameObject.SetActive(true);
	}

	public bool CheckSkillData()
	{
		if (Skill != null && Skill.Data != null && Skill.Data.IsValid()) return true;

		Skill = null;
		Enabled = false;
		gameObject.SetActive(false);
		return false;
	}

	// public void RefreshCastTime()
	// {
	// 	totalCastTime = Skill.CastTimer;
	// 	totalCooldown = 0f;
	// }

	public void RefreshCooldown()
	{
		totalCooldown = Skill.CooldownTimer;
	}

	void Update()
	{
		if (!Enabled) return;

		if (totalCooldown > 0f)
		{
			var fillAmount = Skill.CooldownTimer / totalCooldown;
			cooldownOverlay.fillAmount = fillAmount;

			if (fillAmount <= 0f)
			{
				totalCooldown = 0f;
			}
		}
	}
}