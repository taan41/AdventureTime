using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AudioManager.AudioChannel;

public partial class PauseMenu
{
	#region Settings Panels Base

	[Serializable]
	public class SettingsPanelBase
	{
		[SerializeField] protected GameObject panel;
		[SerializeField] protected RectTransform panelTransform;
		[SerializeField] protected float animationDuration = 0.25f;
		[SerializeField] protected Vector2 animationPositionOffset = new(-50f, 0f);

		protected Vector2 originalPanelPosition;

		public virtual void Initialize()
		{
			if (panelTransform == null) panelTransform = panel.GetComponent<RectTransform>();
			originalPanelPosition = panelTransform.anchoredPosition;
			panel.SetActive(false);
		}

		public virtual void SetActive(bool isActive)
		{
			panel.SetActive(isActive);
		}

		public virtual IEnumerator Toggle() => ActivateAnimation(!panel.activeSelf);
		public virtual IEnumerator ActivateAnimation(bool isActive)
		{
			float duration = 0.25f;
			float elapsed = 0f;

			Vector2 startPos = isActive ? originalPanelPosition + animationPositionOffset : originalPanelPosition;
			Vector2 targetPos = isActive ? originalPanelPosition : originalPanelPosition + animationPositionOffset;
			panelTransform.anchoredPosition = startPos;

			if (isActive) panel.SetActive(true);

			while (elapsed < duration)
			{
				elapsed += Time.unscaledDeltaTime;
				float t = Mathf.Clamp01(elapsed / duration);
				panelTransform.anchoredPosition = isActive
					? CMath.Ease.Back.Vector2Out(startPos, targetPos, t)
					: CMath.Ease.Back.Vector2In(startPos, targetPos, t);
				yield return null;
			}

			panelTransform.anchoredPosition = targetPos;

			if (!isActive) panel.SetActive(false);
		}
	}
	
	#endregion

	#region Audio Settings

	[Serializable]
	public class AudioSettings : SettingsPanelBase
	{
		[Serializable]
		private class VolumeControl
		{
			public CustomSlider volumeSlider;
			public Checkbox muteCheckbox;
		}

		[SerializeField] private VolumeControl masterVolume;
		[SerializeField] private VolumeControl bgmVolume;
		[SerializeField] private VolumeControl sfxVolume;
		private AudioManager AudioManager => AudioManager.Instance;

		private float delaySaveTimer = 0f;

		private VolumeControl this[AudioManager.AudioChannel channel]
		{
			get
			{
				return channel switch
				{
					Master => masterVolume,
					BGM => bgmVolume,
					SFX => sfxVolume,
					_ => null,
				};
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			SetVolume(Master, PlayerPrefs.GetFloat("AudioSettings/MasterVolume", 1f), true);
			SetVolume(BGM, PlayerPrefs.GetFloat("AudioSettings/BGMVolume", 1f), true);
			SetVolume(SFX, PlayerPrefs.GetFloat("AudioSettings/SFXVolume", 1f), true);

			ToggleMuteMaster(PlayerPrefs.GetInt("AudioSettings/MasterMuted", 0) == 1);
			ToggleMuteBGM(PlayerPrefs.GetInt("AudioSettings/BGMMuted", 0) == 1);
			ToggleMuteSFX(PlayerPrefs.GetInt("AudioSettings/SFXMuted", 0) == 1);

			masterVolume.volumeSlider.OnValueChanged += SetMasterVolume;
			bgmVolume.volumeSlider.OnValueChanged += SetBGMVolume;
			sfxVolume.volumeSlider.OnValueChanged += SetSFXVolume;

			masterVolume.muteCheckbox.OnChecked += ToggleMuteMaster;
			bgmVolume.muteCheckbox.OnChecked += ToggleMuteBGM;
			sfxVolume.muteCheckbox.OnChecked += ToggleMuteSFX;
		}

		private void SetMasterVolume(float volume) => SetVolume(Master, volume);
		private void SetBGMVolume(float volume) => SetVolume(BGM, volume);
		private void SetSFXVolume(float volume) => SetVolume(SFX, volume);
		private void SetVolume(AudioManager.AudioChannel channel, float volume, bool setSlider = false)
		{
			AudioManager.SetVolume(channel, volume);
			if (setSlider) this[channel].volumeSlider.SetValue(volume, true);
			this[channel].muteCheckbox.SetChecked(volume <= 0f);
			Save();
		}

		private void ToggleMuteMaster(bool isMuted) => ToggleMute(Master, isMuted);
		private void ToggleMuteBGM(bool isMuted) => ToggleMute(BGM, isMuted);
		private void ToggleMuteSFX(bool isMuted) => ToggleMute(SFX, isMuted);
		private void ToggleMute(AudioManager.AudioChannel channel, bool isMuted)
		{
			if (isMuted)
			{
				SetVolume(channel, 0f);
			}
			else
			{
				if (this[channel].volumeSlider.Value <= 0f)
				{
					this[channel].volumeSlider.SetValue(0.1f, true);
				}

				SetVolume(channel, this[channel].volumeSlider.Value);
			}

			Save();
		}

		public void TickTimer(float unscaledDeltaTime)
		{
			if (delaySaveTimer > 0f)
			{
				delaySaveTimer -= unscaledDeltaTime;
				if (delaySaveTimer <= 0f)
				{
					Save(true);
				}
			}
		}

		public void Save(bool immediate = false)
		{
			if (immediate)
			{
				PlayerPrefs.SetFloat("AudioSettings/MasterVolume", AudioManager.GetVolume(Master));
				PlayerPrefs.SetFloat("AudioSettings/BGMVolume", AudioManager.GetVolume(BGM));
				PlayerPrefs.SetFloat("AudioSettings/SFXVolume", AudioManager.GetVolume(SFX));

				PlayerPrefs.SetInt("AudioSettings/MasterMuted", AudioManager.GetVolume(Master) <= 0f ? 1 : 0);
				PlayerPrefs.SetInt("AudioSettings/BGMMuted", AudioManager.GetVolume(BGM) <= 0f ? 1 : 0);
				PlayerPrefs.SetInt("AudioSettings/SFXMuted", AudioManager.GetVolume(SFX) <= 0f ? 1 : 0);

				PlayerPrefs.Save();
				delaySaveTimer = 0f;
			}
			else
			{
				delaySaveTimer = 2f;
			}
		}
	}

	#endregion

	#region Video Settings

	[Serializable]
	public class VideoSettings : SettingsPanelBase
	{
		[Serializable]
		public struct FullscreenOption
		{
			public string label;
			public FullScreenMode mode;
		}

		[SerializeField] private Dropdown resolutionDropdown;
		[SerializeField] private Dropdown fullscreenDropdown;
		[SerializeField] private Dropdown refreshRateDropdown;
		[SerializeField] private GameObject applyButtonContainer;
		[SerializeField] private CustomButton applyButton;
		[SerializeField] private CustomButton cancelButton;

		private readonly FullscreenOption[] fullscreenModes = new FullscreenOption[]
		{
			new() { label = "Windowed", mode = FullScreenMode.Windowed },
			new() { label = "Maximized Window", mode = FullScreenMode.MaximizedWindow },
			new() { label = "Fullscreen Window", mode = FullScreenMode.FullScreenWindow },
			new() { label = "Exclusive Fullscreen", mode = FullScreenMode.ExclusiveFullScreen },
		};
		private readonly List<Resolution> resolutions = new();
		private readonly List<RefreshRate> refreshRates = new();

		private int resolutionIndex = 0;
		private int fullscreenIndex = 0;
		private int refreshRateIndex = 0;

		private int lastAppliedResolutionIndex = 0;
		private int lastAppliedFullscreenModeIndex = 0;
		private int lastAppliedRefreshRateIndex = 0;

		private Coroutine revertCoroutine = null;

		public override void Initialize()
		{
			base.Initialize();

			applyButton.OnClicked += ApplyChanges;
			cancelButton.OnClicked += CancelChanges;

			List<string> fullscreenOptions = new();
			for (int i = 0; i < fullscreenModes.Length; i++)
			{
				fullscreenOptions.Add(fullscreenModes[i].label);

				if (fullscreenModes[i].mode == Screen.fullScreenMode)
				{
					fullscreenIndex = i;
				}
			}

			lastAppliedFullscreenModeIndex = PlayerPrefs.GetInt("VideoSettings/FullscreenModeIndex", fullscreenIndex);
			if (lastAppliedFullscreenModeIndex != fullscreenIndex)
			{
				fullscreenIndex = lastAppliedFullscreenModeIndex;
				Screen.fullScreenMode = fullscreenModes[fullscreenIndex].mode;
			}

			fullscreenDropdown.ClearOptions();
			fullscreenDropdown.AddOptions(fullscreenOptions);
			fullscreenDropdown.value = fullscreenIndex;
			fullscreenDropdown.RefreshShownValue();
			fullscreenDropdown.onValueChanged.AddListener(SetFullscreen);

			resolutions.Clear();
			List<string> resolutionOptions = new();
			resolutionIndex = 0;

			refreshRates.Clear();
			List<string> refreshRateOptions = new();
			refreshRateIndex = 0;

			Resolution[] availableResolutions = Screen.resolutions;
			Resolution currentResolution = Screen.currentResolution;
			RefreshRate currentRefreshRate = currentResolution.refreshRateRatio;

			bool perfectRatio = currentResolution.width % 16 == 0 && currentResolution.height % 9 == 0;
			if (perfectRatio)
			{
				resolutions.Add(new Resolution
				{
					width = 640,
					height = 360,
					refreshRateRatio = new RefreshRate { numerator = currentRefreshRate.numerator, denominator = currentRefreshRate.denominator }
				});
				resolutionOptions.Add("640x360");
			}

			Resolution resolution;
			int width, height;
			uint numerator, denominator;
			float refreshRate;

			for (int i = 0; i < availableResolutions.Length; i++)
			{
				resolution = availableResolutions[i];
				width = resolution.width;
				height = resolution.height;
				numerator = resolution.refreshRateRatio.numerator;
				denominator = resolution.refreshRateRatio.denominator;
				refreshRate = (float)numerator / denominator;

				if (width > 640 && height > 360)
				{
					if (!perfectRatio || (perfectRatio && width % 640 == 0 && height % 360 == 0))
					{
						resolutions.Add(resolution);
						resolutionOptions.Add($"{width}x{height}");

						if (currentResolution.width == width && currentResolution.height == height)
						{
							resolutionIndex = resolutions.Count - 1;
							lastAppliedResolutionIndex = resolutionIndex;
						}
					}
				}

				if (!refreshRates.Exists(rr => rr.numerator == numerator && rr.denominator == denominator))
				{
					refreshRates.Add(new RefreshRate { numerator = numerator, denominator = denominator });
					refreshRateOptions.Add($"{refreshRate:F0} Hz");

					if (currentRefreshRate.numerator == numerator && currentRefreshRate.denominator == denominator)
					{
						refreshRateIndex = refreshRates.Count - 1;
					}
				}
			}

			int savedWidth = PlayerPrefs.GetInt("VideoSettings/ResolutionWidth", currentResolution.width);
			int savedHeight = PlayerPrefs.GetInt("VideoSettings/ResolutionHeight", currentResolution.height);
			for (int i = 0; i < resolutions.Count; i++)
			{
				resolution = resolutions[i];
				if (resolution.width == savedWidth && resolution.height == savedHeight)
				{
					lastAppliedResolutionIndex = i;
					break;
				}
			}

			lastAppliedRefreshRateIndex = PlayerPrefs.GetInt("VideoSettings/RefreshRateIndex", refreshRateIndex);

			if (lastAppliedResolutionIndex != resolutionIndex || lastAppliedRefreshRateIndex != refreshRateIndex)
			{
				resolutionIndex = lastAppliedResolutionIndex;
				refreshRateIndex = lastAppliedRefreshRateIndex;
				Resolution res = resolutions[resolutionIndex];
				Screen.SetResolution(res.width, res.height, fullscreenModes[fullscreenIndex].mode, refreshRates[refreshRateIndex]);
			}

			resolutionDropdown.ClearOptions();
			resolutionDropdown.AddOptions(resolutionOptions);
			resolutionDropdown.value = resolutionIndex;
			resolutionDropdown.RefreshShownValue();
			resolutionDropdown.onValueChanged.AddListener(SetResolution);

			refreshRateDropdown.ClearOptions();
			refreshRateDropdown.AddOptions(refreshRateOptions);
			refreshRateDropdown.value = refreshRateIndex;
			refreshRateDropdown.RefreshShownValue();
			refreshRateDropdown.onValueChanged.AddListener(SetRefreshRate);
		}

		private void SetResolution(int resolutionIndex)
		{
			if (resolutionIndex < 0 || resolutionIndex >= resolutions.Count) return;
			if (resolutionIndex == this.resolutionIndex) return;

			this.resolutionIndex = resolutionIndex;
			Resolution res = resolutions[resolutionIndex];
			Screen.SetResolution(res.width, res.height, fullscreenModes[fullscreenIndex].mode, refreshRates[refreshRateIndex]);

			StartRevertCountdown();
		}

		private void SetFullscreen(int fullscreenIndex)
		{
			if (fullscreenIndex < 0 || fullscreenIndex >= fullscreenModes.Length) return;
			if (fullscreenIndex == this.fullscreenIndex) return;

			this.fullscreenIndex = fullscreenIndex;
			Screen.fullScreenMode = fullscreenModes[fullscreenIndex].mode;

			StartRevertCountdown();
		}

		private void SetRefreshRate(int refreshRateIndex)
		{
			if (refreshRateIndex < 0 || refreshRateIndex >= refreshRates.Count) return;
			if (refreshRateIndex == this.refreshRateIndex) return;

			this.refreshRateIndex = refreshRateIndex;
			Resolution res = resolutions[resolutionIndex];
			Screen.SetResolution(res.width, res.height, fullscreenModes[fullscreenIndex].mode, refreshRates[refreshRateIndex]);

			StartRevertCountdown();
		}

		public void ApplyChanges()
		{
			lastAppliedResolutionIndex = resolutionIndex;
			lastAppliedFullscreenModeIndex = fullscreenIndex;
			lastAppliedRefreshRateIndex = refreshRateIndex;

			applyButtonContainer.SetActive(false);

			PlayerPrefs.SetInt("VideoSettings/ResolutionWidth", resolutions[resolutionIndex].width);
			PlayerPrefs.SetInt("VideoSettings/ResolutionHeight", resolutions[resolutionIndex].height);
			PlayerPrefs.SetInt("VideoSettings/FullscreenModeIndex", fullscreenIndex);
			PlayerPrefs.SetInt("VideoSettings/RefreshRateIndex", refreshRateIndex);
			PlayerPrefs.Save();
			
			if (revertCoroutine != null)
			{
				CoroutineRunner.Stop(revertCoroutine);
				revertCoroutine = null;
			}
		}

		public void CancelChanges()
		{
			if (resolutionIndex != lastAppliedResolutionIndex)
			{
				resolutionIndex = lastAppliedResolutionIndex;
				resolutionDropdown.value = resolutionIndex;
				resolutionDropdown.RefreshShownValue();
			}

			if (fullscreenIndex != lastAppliedFullscreenModeIndex)
			{
				fullscreenIndex = lastAppliedFullscreenModeIndex;
				fullscreenDropdown.value = fullscreenIndex;
				fullscreenDropdown.RefreshShownValue();
			}

			if (refreshRateIndex != lastAppliedRefreshRateIndex)
			{
				refreshRateIndex = lastAppliedRefreshRateIndex;
				refreshRateDropdown.value = refreshRateIndex;
				refreshRateDropdown.RefreshShownValue();
			}

			Resolution res = resolutions[resolutionIndex];
			Screen.SetResolution(res.width, res.height, fullscreenModes[fullscreenIndex].mode, refreshRates[refreshRateIndex]);

			applyButtonContainer.SetActive(false);
			
			if (revertCoroutine != null)
			{
				CoroutineRunner.Stop(revertCoroutine);
				revertCoroutine = null;
			}
		}

		private void StartRevertCountdown()
		{
			applyButton.ClearVisuals();
			cancelButton.ClearVisuals();
			applyButtonContainer.SetActive(true);

			if (CoroutineRunner.Instance != null)
			{
				if (revertCoroutine != null)
				{
					CoroutineRunner.Stop(revertCoroutine);
				}
				revertCoroutine = CoroutineRunner.Start(RevertCountdown());
			}
		}

		public IEnumerator RevertCountdown()
		{
			float revertTimer = 10f;
			int revertTimeDisplay = 10;

			while (revertTimer > 0f)
			{
				revertTimer -= Time.unscaledDeltaTime;
				if ((int)revertTimer < revertTimeDisplay)
				{
					revertTimeDisplay = (int)revertTimer;
					cancelButton.Text.text = $"Cancel ({revertTimeDisplay + 1}s)";
				}

				if (revertTimer <= 0f)
				{
					CancelChanges();
					cancelButton.Text.text = "Cancel";
				}
				yield return null;
			}

			CancelChanges();
		}
	}

	#endregion

	#region Control Settings

	[Serializable]
	public class ControlSettings : SettingsPanelBase
	{
		[SerializeField] private GameObject rebindsContainer;
		[SerializeField] private RebindUI rebindPrefab;

		public override void Initialize()
		{
			base.Initialize();

			var map = InputManager.Instance.InputActionAsset.FindActionMap("Player");
			foreach (var action in map.actions)
			{
				for (int i = 0; i < action.bindings.Count; i++)
				{
					if (action.bindings[i].isComposite) continue;

					RebindUI rebindUI = Instantiate(rebindPrefab, rebindsContainer.transform);
					rebindUI.SetupRebind(action, i);
					rebindUI.gameObject.SetActive(true);
				}
			}
		}
	}

	#endregion
	
	#region PauseMenu Settings

	[Header("Settings Panels")]
	[SerializeField] private AudioSettings audioSettings;
	[SerializeField] private VideoSettings videoSettings;
	[SerializeField] private ControlSettings controlSettings;

	private SettingsPanelBase lastPanel = null;

	private void InitializeSettingsMenu()
	{
		audioButton.OnClicked += ToggleAudioSettings;
		videoButton.OnClicked += ToggleVideoSettings;
		controlsButton.OnClicked += ToggleControlSettings;

		audioSettings.Initialize();
		videoSettings.Initialize();
		controlSettings.Initialize();
	}

	private void UpdateSettings(float unscaledDeltaTime)
	{
		audioSettings.TickTimer(unscaledDeltaTime);
		// videoSettings.TickTimer(unscaledDeltaTime);
	}

	private void ToggleAudioSettings() => TogglePanel(audioSettings);

	private void ToggleVideoSettings() => TogglePanel(videoSettings);

	private void ToggleControlSettings() => TogglePanel(controlSettings);

	private void TogglePanel(SettingsPanelBase panel)
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

	#endregion
}