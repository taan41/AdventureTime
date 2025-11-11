using System.Collections;
using UnityEngine;

public partial class MainMenu : MonoBehaviour
{
	[SerializeField] private CustomButton playButton;
	[SerializeField] private CustomButton quitButton;

	[Header("Settings Panel")]
	[SerializeField] private CustomButton settingsButton;
	[SerializeField] private GameObject settingsPanel;
	[SerializeField] private RectTransform settingsPanelTransform;
	[SerializeField] private float animationDuration = 0.25f;
	[SerializeField] private Vector2 animationPositionOffset = new(50f, 0f);

	[SerializeField] private CustomButton audioButton;
	[SerializeField] private CustomButton videoButton;
	[SerializeField] private CustomButton controlsButton;
	[SerializeField] private CustomButton backButton;

	[SerializeField] private PauseMenu.AudioSettings audioSettings;
	[SerializeField] private PauseMenu.VideoSettings videoSettings;
	[SerializeField] private PauseMenu.ControlSettings controlSettings;

	private PauseMenu.SettingsPanelBase lastPanel = null;
	private Vector2 originalPanelPosition;

	private void Awake()
	{
		originalPanelPosition = settingsPanelTransform.anchoredPosition;
	}

	private void Start()
	{
		GameStateManager.Instance.SetState(GameStateManager.GameState.MainMenu);

		playButton.OnClicked += PlayGame;
		quitButton.OnClicked += QuitGame;

		settingsButton.OnClicked += ToggleSettingsPanel;
		audioButton.OnClicked += ToggleAudioSettings;
		videoButton.OnClicked += ToggleVideoSettings;
		controlsButton.OnClicked += ToggleControlSettings;
		backButton.OnClicked += ToggleSettingsPanel;

		audioSettings.Initialize();
		videoSettings.Initialize();
		controlSettings.Initialize();

		settingsPanel.SetActive(false);
	}

	private void Update()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;

		audioSettings.TickTimer(unscaledDeltaTime);
		// videoSettings.TickTimer(unscaledDeltaTime);
	}

	private void PlayGame()
	{
		StopAllCoroutines();
		StartCoroutine(AnimateSettingsPanel(false));

		GameStateManager.Instance.LoadScene("GameScene1");
	}

	private void QuitGame()
	{
		Application.Quit();
	}

	private void ToggleAudioSettings() => TogglePanel(audioSettings);

	private void ToggleVideoSettings() => TogglePanel(videoSettings);

	private void ToggleControlSettings() => TogglePanel(controlSettings);

	private void TogglePanel(PauseMenu.SettingsPanelBase panel)
	{
		if (lastPanel != null)
		{
			StopCoroutine(lastPanel.Toggle());
			if (lastPanel != panel) lastPanel.SetActive(false);
		}

		StartCoroutine(panel.Toggle());
		lastPanel = panel;
	}

	private void CloseSettingsPanels()
	{
		audioSettings.SetActive(false);
		videoSettings.SetActive(false);
		controlSettings.SetActive(false);

		audioSettings.Save(true);
		videoSettings.ApplyChanges();

		lastPanel = null;
	}

	private void ToggleSettingsPanel()
	{
		StopAllCoroutines();
		StartCoroutine(AnimateSettingsPanel(!settingsPanel.activeSelf));
	}

	private IEnumerator AnimateSettingsPanel(bool isOpening)
	{
		if (isOpening) settingsPanel.SetActive(true);

		float elapsed = 0f;
		Vector2 startPos = isOpening ? originalPanelPosition + animationPositionOffset : originalPanelPosition;
		Vector2 targetPos = isOpening ? originalPanelPosition : originalPanelPosition + animationPositionOffset;
		settingsPanelTransform.anchoredPosition = startPos;

		while (elapsed < animationDuration)
		{
			elapsed += Time.unscaledDeltaTime;
			float t = Mathf.Clamp01(elapsed / animationDuration);
			settingsPanelTransform.anchoredPosition = isOpening
				? CMath.Ease.Back.Vector2Out(startPos, targetPos, t)
				: CMath.Ease.Back.Vector2In(startPos, targetPos, t);
			yield return null;
		}

		settingsPanelTransform.anchoredPosition = targetPos;

		if (!isOpening)
		{
			CloseSettingsPanels();
			settingsPanel.SetActive(false);
		}
	}
}