using System;
using System.Collections.Generic;
using UnityEngine;

using static Character;

public class SpatialGrid
{
	public static SpatialGrid Instance { get; private set; } = new();

	public float CellSize { get; private set; } = 1f;
	private readonly Dictionary<(CharacterTag, Vector2Int), HashSet<Character>> grid = new();

	public SpatialGrid(float cellSize = 1f)
	{
		CellSize = cellSize > 0f ? cellSize : 1f;
	}

	public Vector2Int WorldToCell(Vector2 position)
		=> Vector2Int.FloorToInt(position / CellSize);

	public void Add(Character character, Vector2Int cell)
	{
		var key = (character.Tag, cell);
		if (!grid.ContainsKey(key))
		{
			grid[key] = new();
		}
		grid[key].Add(character);
	}

	public void Remove(Character character, Vector2Int cell)
	{
		var key = (character.Tag, cell);
		if (grid.TryGetValue(key, out var characters))
		{
			characters.Remove(character);
		}
	}

	public void QueryFromCenter(CharacterTag tag, Vector3 position, int range, Action<Character> actiom, int maxCall)
		=> QueryFromCenter(tag, WorldToCell(position), range, actiom, maxCall);
		
	public void QueryFromCenter(CharacterTag tag, Vector2Int centerCell, int range, Action<Character> actiom, int maxCall)
	{
		int callCount = 0;
		Vector2Int cell;
		for (int r = 0; r <= range; r++)
		{
			for (int x = -r; x <= r; x++)
			{
				for (int y = -r; y <= r; y++)
				{
					if (Mathf.Abs(x) == r || Mathf.Abs(y) == r)
					{
						cell = new(centerCell.x + x, centerCell.y + y);
						if (grid.TryGetValue((tag, cell), out var characters))
						{
							foreach (var character in characters)
							{
								actiom?.Invoke(character);
								callCount++;
								if (callCount >= maxCall) return;
							}
						}
					}
				}
			}
		}
	}

	public void Clear()
	{
		grid.Clear();
	}
}