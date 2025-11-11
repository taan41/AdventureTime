using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerData", menuName = "ScriptableObjects/Characters/SpawnerData")]
public class SpawnerDataNew : ScriptableObject
{
	public List<SpawnerNew.SpawnGroup> spawnGroups = new();
	public float offcamSpawnPointXMultiplier = 1.2f;
	public float offcamSpawnPointYMultiplier = 1.2f;
	public float timeStartAtSecond = 0f;

	public void SortGroups()
	{
		spawnGroups.Sort((a, b) => a.startTime.CompareTo(b.startTime));
	}
}