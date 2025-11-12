using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestObjectManager : MonoBehaviour
{
	public Text fpsText;
	public Text lowestFpsText;
	public Text objCountText;
	public Text modeText;
	public Text horizontalInput;
	public Text verticalInput;
	public Text boidsRadiusText;
	public Text boidsCapText;
	public Text separationStrengthText;
	public Text alignmentStrengthText;
	public Button spawnButton;
	public Button applyPhysics;
	public Toggle physicsToggle;
	public Toggle boidsToggle;
	public TestCharacter testObj;
	public Rect spawnArea = new(-10, -10, 20, 20);

	public List<TestCharacter> InitialObjects = new();
	private readonly List<TestCharacter> objectList = new();

	private Vector3 spawnPosStart;
	private int frameCount = 0;
	private float elapsedTime = 0f;
	private float highestUpdateTime = 0f;
	// private float spawnTimer = 0f;
	private int lastSpawnedCount = 0;

	private void Awake()
	{
		modeText.text = "All objects follow player\n No physics / boids";

		spawnButton.onClick.AddListener(SpawnFromInput);
		applyPhysics.onClick.AddListener(ApplyPhysics);

		spawnPosStart = new(spawnArea.xMin, spawnArea.yMin, 0);

		objectList.Clear();
		objectList.AddRange(InitialObjects);
		
		boidsRadiusText.text = testObj.boidsRadius.ToString();
		boidsCapText.text = testObj.boidsCap.ToString();
		separationStrengthText.text = testObj.seperationStrength.ToString();
		alignmentStrengthText.text = testObj.alignmentStrength.ToString();

		physicsToggle.isOn = testObj.usingPhysics;
		boidsToggle.isOn = testObj.usingBoids;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		highestUpdateTime = Mathf.Max(highestUpdateTime, deltaTime);

		frameCount++;
		elapsedTime += deltaTime;

		if (elapsedTime >= 1f)
		{
			fpsText.text = $"FPS: {frameCount}";
			lowestFpsText.text = $"Highest time: {highestUpdateTime:F4}s ({1f / highestUpdateTime:F1} FPS)";

			highestUpdateTime = 0f;
			frameCount = 0;
			elapsedTime -= 1f;
		}

		// SpawnFromInput();
	}

	private void FixedUpdate()
	{
		float deltaTime = Time.fixedDeltaTime;
		for (int i = 0; i < objectList.Count; i++)
		{
			objectList[i].DoUpdate(deltaTime);
		}
	}

	public void SpawnObjects(int horizontal, int vertical)
	{
		Vector3 spawnPos;
		Vector3 spawnOffset = new(spawnArea.width / horizontal, spawnArea.height / vertical, 0);
		int spawnedCount = 0;
		for (int i = 0; i < horizontal; i++)
		{
			for (int j = 0; j < vertical; j++)
			{
				spawnPos = spawnPosStart + new Vector3(i * spawnOffset.x, j * spawnOffset.y, 0);
				if (spawnedCount < objectList.Count)
				{
					objectList[spawnedCount].transformCache.position = spawnPos;
					objectList[spawnedCount].gameObjectCache.SetActive(true);
					spawnedCount++;
					continue;
				}
				TestCharacter obj = Instantiate(testObj, spawnPos, Quaternion.identity).GetComponent<TestCharacter>();
				objectList.Add(obj);
				spawnedCount++;
			}
		}

		objCountText.text = $"Objects: {horizontal * vertical}";
	}

	public void ClearObjects()
	{
		for (int i = 0; i < objectList.Count; i++)
		{
			// Destroy(updateList[i].gameObject);
			objectList[i].gameObjectCache.SetActive(false);
		}
		// objectList.Clear();
	}

	public void SpawnFromInput()
	{
		if (int.TryParse(horizontalInput.text, out int horizontal) && int.TryParse(verticalInput.text, out int vertical))
		{
			if (horizontal * vertical == lastSpawnedCount) return;
			lastSpawnedCount = horizontal * vertical;
			ClearObjects();
			SpawnObjects(horizontal, vertical);
		}
	}

	private void ApplyPhysics()
	{
		for (int i = 0; i < objectList.Count; i++)
		{
			if (int.TryParse(boidsRadiusText.text, out int boidsRadius) &&
				int.TryParse(boidsCapText.text, out int boidsCap) &&
				float.TryParse(separationStrengthText.text, out float separationStrength) &&
				float.TryParse(alignmentStrengthText.text, out float alignmentStrength))
			{
				objectList[i].SetBoids(boidsToggle.isOn, boidsRadius, boidsCap, separationStrength, alignmentStrength);
			}
			objectList[i].SetPhysics(physicsToggle.isOn);
		}
	}
}