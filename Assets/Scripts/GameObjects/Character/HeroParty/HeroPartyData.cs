using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroPartyData", menuName = "Scriptable Objects/Character Data/Hero Party")]
public class HeroPartyData : ScriptableObject
{
	public List<CharacterData> heroDatas = new();
}