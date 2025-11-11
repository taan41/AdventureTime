using UnityEngine;
using UnityEngine.UI;

public class InfoOverlay : MonoBehaviour
{
	public GameObject panel;

	[Header("Extra Info")]
	public Text fpsText;
	public Text positionText;
	public Text debugText;

	[Header("Name")]
	public Text nameText;

	[Header("Icon")]
	public Image icon;

	[Header("Resources")]
	public Image healthFillImage;
	public Text healthText;
	public Image manaFillImage;
	public Image staminaFillImage;

	[Header("Gold")]
	public Text goldText;

	[Header("Skills")]
	public RectTransform skillsContainer;
	public SkillIconDisplay skillIconPrefab;
}