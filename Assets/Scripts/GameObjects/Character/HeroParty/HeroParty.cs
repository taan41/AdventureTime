using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroParty : MonoBehaviour
{
	public static HeroParty Instance { get; private set; }

	public event Action OnControlledHeroChanged;
	public event Action OnActiveHeroesChanged;
	public event Action OnPartyGoldChanged;

	public HeroPartyData partyData;

	public List<Character> ActiveHeroes { get; private set; } = new();

	public Character Leader { get; private set; }
	public Character ControlledHero { get; private set; }
	public int ControlledHeroIndex { get; private set; } = 0;

	private float partyGold = 0f;
	public float PartyGold
	{
		get => partyGold;
		set
		{
			partyGold = value;
			OnPartyGoldChanged?.Invoke();
		}
	}

	private bool dataSet = false;

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

		if (!dataSet && partyData != null)
		{
			SetData(partyData);
		}

		if (ActiveHeroes.Count > 0)
		{
			SetControlledHero(0);
		}

		SetPlayerActions();
	}

	private void SetPlayerActions()
	{
		InputManager.Instance.PlayerActions.SwitchHero.performed += ctx => SetNextHero();
		
	}

	public void SetData(HeroPartyData newData)
	{
		if (ActiveHeroes.Count > 0)
		{
			foreach (var hero in ActiveHeroes)
			{
				Destroy(hero.gameObject);
			}
		}
		ActiveHeroes.Clear();
		partyData = newData;
		dataSet = true;

		for (int i = 0; i < partyData.heroDatas.Count; i++)
		{
			var hero = CharacterFactory.Create(partyData.heroDatas[i]);
			hero.OnDeath += RemoveActiveHero;
			hero.partyRef = this;
			ActiveHeroes.Add(hero);
			hero.Enable(true, GetSpawnPosition(i));
		}

		if (ActiveHeroes.Count > 0)
		{
			Leader = ActiveHeroes[0];
			SetControlledHero(0);
		}
	}

	public void SetControlledHero(int index)
	{
		if (ControlledHero != null)
			ControlledHero.SetPlayerControl(false);

		if (index < 0 || index >= ActiveHeroes.Count)
			index = (index % ActiveHeroes.Count + ActiveHeroes.Count) % ActiveHeroes.Count;

		if (index >= ActiveHeroes.Count) return;

		ControlledHero = ActiveHeroes[index];
		ControlledHero.SetPlayerControl(true);
		ControlledHeroIndex = index;

		for (int i = 0; i < ActiveHeroes.Count; i++)
		{
			if (i == index) continue;
			ActiveHeroes[i].groupLeader = ControlledHero;
		}

		CameraManager.Instance.SetTrackingTarget(ControlledHero.TransformCache);

		OnControlledHeroChanged?.Invoke();
	}

	public void SetNextHero()
	{
		if (ActiveHeroes.Count <= 1) return;
		SetControlledHero(ControlledHeroIndex + 1);
	}

	public void SetPreviousHero()
	{
		if (ActiveHeroes.Count <= 1) return;
		SetControlledHero(ControlledHeroIndex - 1);
	}

	public void AddActiveHero(Character hero)
	{
		if (hero == null || ActiveHeroes.Contains(hero)) return;

		ActiveHeroes.Add(hero);
		hero.Enable(true);

		OnActiveHeroesChanged?.Invoke();

		if (ControlledHero == null)
		{
			SetControlledHero(0);
		}
	}

	public void RemoveActiveHero(Character hero)
	{
		if (hero == null || !ActiveHeroes.Contains(hero)) return;

		ActiveHeroes.Remove(hero);
		hero.Enable(false);

		OnActiveHeroesChanged?.Invoke();

		if (ControlledHero == hero)
		{
			if (ActiveHeroes.Count > 0)
			{
				SetControlledHero(0);
			}
			else
			{
				ControlledHero = null;
				ControlledHeroIndex = -1;

				ResultScreen.Instance.ShowDefeat();
			}
		}
	}

	private Vector3 GetSpawnPosition(int index)
	{
		if (index > 0)
		{
			var pos = Quaternion.Euler(0f, 0f, 360f / (ActiveHeroes.Count - 1) * (index - 1)) * Vector3.right * 1.5f;
			return pos;
		}
		return Vector3.zero;
	}
}