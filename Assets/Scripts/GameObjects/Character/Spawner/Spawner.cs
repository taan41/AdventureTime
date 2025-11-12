using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerNew : MonoBehaviour
{
	public enum SpawnPositionType
	{
		OffCam,
		WorldPosition,
	}

	[System.Serializable]
	public class SpawnGroup
	{
		public CharacterData characterData;
		public SpawnPositionType spawnPositionType;
		public Vector3 worldPosition;
		public float offcamPositionOffset = 0f;
		public int spawnPerWave = 1;
		public int waveCount = 1;
		public float waveInterval = 1f;
		public float startTime = 0f;
		public bool notify = false;
		public Color notifyColor = Color.green;
	}

	private static Unity.Mathematics.Random spawnerRNG = new((uint)System.DateTime.Now.Ticks);

	private Transform offcamTopRightSpawnPoint;
	private Transform offcamBottomLeftSpawnPoint;

	public SpawnerDataNew data;

	public float elapsedTime = 0f;
	public float nextGroupTime = 0f;
	public int groupIndex = 0;
	public int totalSpawned = 0;

	private readonly Dictionary<CharacterData, ObjectPool<Character>> characterPools = new();

	private void Awake()
	{
		if (data == null)
		{
			Debug.LogError("Spawner: SpawnerData asset is not assigned.");
			enabled = false;
			return;
		}

		float cameraSize = CameraManager.Instance.cinemachineCamera.Lens.OrthographicSize;

		offcamTopRightSpawnPoint = new GameObject("TopRightSpawnPoint").transform;
		offcamTopRightSpawnPoint.SetParent(CameraManager.Instance.cinemachineCamera.transform);
		offcamTopRightSpawnPoint.localPosition = cameraSize * new Vector3(1920 / 1080f * data.offcamSpawnPointXMultiplier, data.offcamSpawnPointYMultiplier, 0);

		offcamBottomLeftSpawnPoint = new GameObject("BottomLeftSpawnPoint").transform;
		offcamBottomLeftSpawnPoint.SetParent(CameraManager.Instance.cinemachineCamera.transform);
		offcamBottomLeftSpawnPoint.localPosition = new Vector3(-offcamTopRightSpawnPoint.localPosition.x, -offcamTopRightSpawnPoint.localPosition.y, 0);

		data.SortGroups();
		// Move index to the active group based on start time
		elapsedTime = data.timeStartAtSecond;
		for (int i = 0; i < data.spawnGroups.Count; i++)
		{
			if (data.spawnGroups[i].startTime + data.spawnGroups[i].waveCount >= elapsedTime)
			{
				groupIndex = i;
				nextGroupTime = data.spawnGroups[i].startTime;
				break;
			}
		}

		for (int i = 0; i < data.spawnGroups.Count; i++)
		{
			CreatePoolForGroup(data.spawnGroups[i]);
		}
	}

	void Update()
	{
		elapsedTime += Time.deltaTime;

		if (groupIndex >= data.spawnGroups.Count)
		{
			if (Character.ActiveCharacters[Character.CharacterTag.Monster].Count == 0)
			{
				StartCoroutine(EndStage());
				enabled = false;
			}
			return;
		}

		while (nextGroupTime <= elapsedTime)
		{
			StartCoroutine(SpawningGroup(data.spawnGroups[groupIndex]));

			groupIndex++;
			if (groupIndex < data.spawnGroups.Count)
			{
				nextGroupTime = data.spawnGroups[groupIndex].startTime;
			}
			else break;
		}
	}

	void CreatePoolForGroup(SpawnGroup group)
	{
		if (group.characterData == null) return;

		int poolSize = group.spawnPerWave * group.waveCount;
		if (poolSize <= 1) poolSize = 1;

		if (!characterPools.ContainsKey(group.characterData))
		{
			characterPools[group.characterData] = new(() => CharacterFactory.Create(group.characterData), poolSize);
		}
		else if (characterPools[group.characterData].Count < poolSize)
		{
			characterPools[group.characterData].Preload(poolSize - characterPools[group.characterData].Count);
		}
	}

	IEnumerator SpawningGroup(SpawnGroup group)
	{
		var characterData = group.characterData;
		if (characterData == null) yield break;
		if (!characterPools.ContainsKey(characterData)) CreatePoolForGroup(group);

		float interval = group.waveInterval > 0 ? group.waveInterval : 1f;
		int waveIndex = (int)((elapsedTime - group.startTime) / interval);

		var intervalWait = new WaitForSeconds(interval);
		Vector3 spawnPosition = Vector3.zero;

		while (waveIndex < group.waveCount)
		{
			for (int i = 0; i < group.spawnPerWave; i++)
			{
				switch (group.spawnPositionType)
				{
					case SpawnPositionType.OffCam:
						spawnPosition = GetOffcamSpawnPoint(group.offcamPositionOffset);
						characterPools[characterData].Get().Enable(true, spawnPosition);
						break;
					case SpawnPositionType.WorldPosition:
						spawnPosition = group.worldPosition;
						characterPools[characterData].Get().Enable(true, spawnPosition);
						break;
				}
			}
			waveIndex++;
			totalSpawned += group.spawnPerWave;
			yield return intervalWait;
		}

		if (group.notify) InfoOverlayManager.Instance.SetDebugText($"{group.characterData.characterName} has spawned at {spawnPosition}", group.notifyColor);
	}

	Vector3 GetOffcamSpawnPoint(float offset)
	{
		if (spawnerRNG.NextFloat() > 0.5f)
		{
			return new Vector3(
				spawnerRNG.NextFloat(offcamBottomLeftSpawnPoint.position.x - offset, offcamTopRightSpawnPoint.position.x + offset),
				spawnerRNG.NextFloat() > 0.5f ? offcamBottomLeftSpawnPoint.position.y - offset : offcamTopRightSpawnPoint.position.y + offset,
				0);
		}
		else
		{
			return new Vector3(
				spawnerRNG.NextFloat() > 0.5f ? offcamBottomLeftSpawnPoint.position.x - offset : offcamTopRightSpawnPoint.position.x + offset,
				spawnerRNG.NextFloat(offcamBottomLeftSpawnPoint.position.y - offset, offcamTopRightSpawnPoint.position.y + offset),
				0);
		}
	}
	
	IEnumerator EndStage()
	{
		yield return new WaitForSeconds(2f);
		ResultScreen.Instance.ShowVictory();
	}
}