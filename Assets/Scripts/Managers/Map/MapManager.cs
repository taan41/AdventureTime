using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
	[System.Serializable]
	class GroundTileInfo
	{
		public TileBase[] tiles;
		public float weight;
	}

	private static Unity.Mathematics.Random rng = new((uint)System.DateTime.Now.Ticks);

	public static MapManager Instance { get; private set; }

	public Tilemap groundTilemap;
	[SerializeField] private GroundTileInfo[] groundTileInfos;
	public int tileSize = 1;

	private readonly Dictionary<Vector3Int, TileBase> groundTileCaches = new();
	private readonly HashSet<Vector3Int> loadedTiles = new();
	private readonly HashSet<Vector3Int> tilesToUnload = new();

	private float groundTilesTotalWeight;
	private Camera cameraRef;
	private Vector3 minView = Vector3.zero;
	private Vector3 maxView = new(1f, 1f, 0f);
	private Vector3 minPos, maxPos;
	private Vector2Int minTile = new(), maxTile = new();
	private Vector2Int lastMinTile = new(), lastMaxTile = new();

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

		float prevWeight = 0f;
		for (int i = 0; i < groundTileInfos.Length; i++)
		{
			var info = groundTileInfos[i];
			groundTilesTotalWeight += info.weight;
			info.weight += prevWeight;
			prevWeight = info.weight;
		}

		cameraRef = CameraManager.Instance.mainCamera;
	}

	private void Update()
	{
		minPos = cameraRef.ViewportToWorldPoint(minView);
		maxPos = cameraRef.ViewportToWorldPoint(maxView);

		minTile.x = Mathf.FloorToInt(minPos.x / tileSize) - 1;
		minTile.y = Mathf.FloorToInt(minPos.y / tileSize) - 1;

		maxTile.x = Mathf.CeilToInt(maxPos.x / tileSize) + 1;
		maxTile.y = Mathf.CeilToInt(maxPos.y / tileSize) + 1;

		if (minTile != lastMinTile || maxTile != lastMaxTile)
		{
			lastMinTile = minTile;
			lastMaxTile = maxTile;

			FillTilemap(minTile, maxTile);
		}
	}

	private void FillTilemap(Vector2Int min, Vector2Int max)
	{
		tilesToUnload.UnionWith(loadedTiles);

		float random;
		Vector3Int tilePos = new();
		for (int x = min.x; x <= max.x; x++)
		{
			for (int y = min.y; y <= max.y; y++)
			{
				tilePos.x = x;
				tilePos.y = y;

				if (!groundTileCaches.ContainsKey(tilePos))
				{
					// groundTileCaches[tilePos] = groundTiles[rng.NextInt(0, groundTiles.Length)];
					random = rng.NextFloat(0f, groundTilesTotalWeight);
					for (int i = 0; i < groundTileInfos.Length; i++)
					{
						if (random <= groundTileInfos[i].weight)
						{
							groundTileCaches[tilePos] = groundTileInfos[i].tiles[rng.NextInt(0, groundTileInfos[i].tiles.Length)];
							break;
						}
					}
				}

				if (loadedTiles.Add(tilePos))
				{
					groundTilemap.SetTile(tilePos, groundTileCaches[tilePos]);
				}

				tilesToUnload.Remove(tilePos);
			}
		}

		foreach (var tileToUnload in tilesToUnload)
		{
			groundTilemap.SetTile(tileToUnload, null);
		}
		loadedTiles.ExceptWith(tilesToUnload);
		tilesToUnload.Clear();
	}
}