using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	public enum AudioChannel
	{
		Master,
		BGM,
		SFX,
	}

	public static AudioManager Instance { get; private set; }

	private static Unity.Mathematics.Random rng = new((uint)System.DateTime.Now.Ticks);

	[SerializeField] private AudioSource bgmSource;
	[SerializeField] private AudioSource sfxSource;
	[SerializeField] private AudioMixer audioMixer;
	
	public AudioClip[] menuBGM;
	public AudioClip[] ingameBGM;

	private int menuBgmIndex = -1;
	private int ingameBgmIndex = -1;

	private void Awake()
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
	}

	public void PlayBGM(AudioClip clip)
	{
		bgmSource.Stop();
		bgmSource.clip = clip;
		bgmSource.loop = true;
		bgmSource.Play();
	}

	public void PlayMenuBGM(bool random = true)
	{
		if (menuBGM.Length == 0) return;

		if (random)
		{
			menuBgmIndex = rng.NextInt(0, menuBGM.Length);
		}
		else
		{
			menuBgmIndex = (menuBgmIndex + 1) % menuBGM.Length;
		}
		PlayBGM(menuBGM[menuBgmIndex]);
	}

	public void PlayIngameBGM()
	{
		if (ingameBGM.Length == 0) return;

		ingameBgmIndex = (ingameBgmIndex + 1) % ingameBGM.Length;
		PlayBGM(ingameBGM[ingameBgmIndex]);
	}

	public void PlaySFX(AudioClip clip)
	{
		sfxSource.PlayOneShot(clip);
	}

	public float PlayRandomSFX(List<AudioClip> clips, float chance = 1f)
	{
		if (clips == null || clips.Count == 0) return 0f;

		if (rng.NextFloat() > chance) return 0f;

		int index = rng.NextInt(0, clips.Count);
		sfxSource.PlayOneShot(clips[index]);
		return clips[index].length;
	}

	public float GetVolume(AudioChannel channel)
	{
		float volume;
		switch (channel)
		{
			case AudioChannel.Master:
				audioMixer.GetFloat("MasterVolume", out volume);
				break;
			case AudioChannel.BGM:
				audioMixer.GetFloat("BGMVolume", out volume);
				break;
			case AudioChannel.SFX:
				audioMixer.GetFloat("SFXVolume", out volume);
				break;
			default:
				volume = 0f;
				break;
		}
		volume = Mathf.Pow(10f, volume / 20f);
		if (volume < 0.0001f) volume = 0f;
		return volume;
	}

	public void SetVolume(AudioChannel channel, float volume)
	{
		volume = Mathf.Clamp(Mathf.Log10(volume) * 20, -80f, 0f);

		switch (channel)
		{
			case AudioChannel.Master:
				audioMixer.SetFloat("MasterVolume", volume);
				break;
			case AudioChannel.BGM:
				audioMixer.SetFloat("BGMVolume", volume);
				break;
			case AudioChannel.SFX:
				audioMixer.SetFloat("SFXVolume", volume);
				break;
		}
	}

	public void SetMasterVolume(float volume) => SetVolume(AudioChannel.Master, volume);

	public void SetBGMVolume(float volume) => SetVolume(AudioChannel.BGM, volume);

	public void SetSFXVolume(float volume) => SetVolume(AudioChannel.SFX, volume);
}