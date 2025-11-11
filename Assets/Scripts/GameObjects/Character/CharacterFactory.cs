using System;
using UnityEngine;

public static class CharacterFactory
{
	public static Character Create(CharacterData data = null)
	{
		if (data != null && !data.IsValid())
		{
			throw new ArgumentException("[CharacterFactory.Create] Invalid Character Data provided");
		}

		var obj = new GameObject("Character");
		var character = obj.AddComponent<Character>();
		character.SetData(data, false);
		character.Enable(false);
		return character;
	}
}