using System;
using System.Collections;
using UnityEngine;

public partial class PauseMenu : MenuBase, IUpdatable
{
	public event Action<bool> OnEnabledChanged;
	
	[Header("Animation Settings")]
	[SerializeField] private RectTransform panelTransform;
	[SerializeField] private float animationDuration = 0.25f;
	[SerializeField] private Vector2 animationPositionOffset = new(-50f, 0f);

	[Header("Option Buttons")]
	[SerializeField] private GameObject optionButtonsContainer;
	[SerializeField] private CustomButton resumeButton;
	[SerializeField] private CustomButton settingsButton;
	[SerializeField] private CustomButton mainMenuButton;

	[Header("Settings Buttons")]
	[SerializeField] private GameObject settingsButtonsContainer;
	[SerializeField] private CustomButton audioButton;
	[SerializeField] private CustomButton videoButton;
	[SerializeField] private CustomButton controlsButton;
	[SerializeField] private CustomButton backButton;

	private Vector2 originalPanelPosition;

	public bool Enabled { get; private set; } = false;

	public bool UseUpdate { get; private set; } = false;

	public bool UseFixedUpdate { get; private set; } = false;

	public bool UseUnscaledTime { get; private set; } = true;

	public override void Initialize()
	{
		originalPanelPosition = panelTransform.anchoredPosition;

		resumeButton.OnClicked += Resume;
		settingsButton.OnClicked += OpenSettings;
		mainMenuButton.OnClicked += ToMainMenu;

		backButton.OnClicked += BackToOptions;

		resumeButton.ListenToMenuClose(this);
		settingsButton.ListenToMenuClose(this);
		mainMenuButton.ListenToMenuClose(this);

		audioButton.ListenToMenuClose(this);
		videoButton.ListenToMenuClose(this);
		controlsButton.ListenToMenuClose(this);
		backButton.ListenToMenuClose(this);

		InitializeSettingsMenu();

		UpdaterManager.StaticRegisterLastUpdater(this);
		Enable(true);
		menuPanel.SetActive(false);
	}

	public void Enable(bool enabled)
	{
		Enabled = enabled;
		OnEnabledChanged?.Invoke(Enabled);
	}

	public override bool Open(Character focusedCharacter)
	{
		if (!base.Open(focusedCharacter)) return false;

		optionButtonsContainer.SetActive(true);
		settingsButtonsContainer.SetActive(false);

		StopAllCoroutines();
		StartCoroutine(ActivateAnimation(true));

		return true;
	}

	public override bool Close()
	{
		if (!IsOpened) return false;

		CloseSettingsPanels();
		ClearButtonsVisuals();

		StopAllCoroutines();
		StartCoroutine(ActivateAnimation(false));

		return true;
	}

	private IEnumerator ActivateAnimation(bool isOpening)
	{
		float elapsed = 0f;

		Vector2 startPos = isOpening ? originalPanelPosition + animationPositionOffset : originalPanelPosition;
		Vector2 targetPos = isOpening ? originalPanelPosition : originalPanelPosition + animationPositionOffset;
		panelTransform.anchoredPosition = startPos;

		while (elapsed < animationDuration)
		{
			elapsed += Time.unscaledDeltaTime;
			float t = Mathf.Clamp01(elapsed / animationDuration);
			panelTransform.anchoredPosition = isOpening
				? CMath.Ease.Back.Vector2Out(startPos, targetPos, t)
				: CMath.Ease.Back.Vector2In(startPos, targetPos, t);
			yield return null;
		}

		panelTransform.anchoredPosition = targetPos;

		if (!isOpening)
		{
			menuPanel.SetActive(false);
			InvokeOnClosed();
			GameStateManager.Instance.RemoveState(GameStateManager.GameState.GameMenu);
		}
	}

	private void Resume()
	{
		Close();
	}

	private void OpenSettings()
	{
		CloseSettingsPanels();
		ClearButtonsVisuals();

		optionButtonsContainer.SetActive(false);
		settingsButtonsContainer.SetActive(true);
	}

	private void BackToOptions()
	{
		CloseSettingsPanels();
		ClearButtonsVisuals();

		optionButtonsContainer.SetActive(true);
		settingsButtonsContainer.SetActive(false);
	}

	private void ToMainMenu()
	{
		GameStateManager.Instance.LoadScene("MainMenu");
	}

	private void ClearButtonsVisuals()
	{
		resumeButton.ClearVisuals();
		settingsButton.ClearVisuals();
		mainMenuButton.ClearVisuals();

		audioButton.ClearVisuals();
		videoButton.ClearVisuals();
		controlsButton.ClearVisuals();
		backButton.ClearVisuals();
	}
	public void DoUpdate(float deltaTime) { }

	public void DoFixedUpdate(float fixedDeltaTime) { }

	public void DoUnscaledUpdate(float unscaledDeltaTime)
	{
		UpdateSettings(unscaledDeltaTime);
	}
}