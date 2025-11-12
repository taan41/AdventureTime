using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class TestCharacter : MonoBehaviour
{
	private static Dictionary<Vector2Int, HashSet<TestCharacter>> cellDict = new();

	public SpriteRenderer spriteRenderer;
	public float moveSpeed = 2f;
	public bool isControlled = false;
	public bool usingPhysics = false;
	public bool usingBoids = false;
	public int boidsRadius = 2;
	public int boidsCap = 10;
	public float seperationStrength = 0.1f;
	public float alignmentStrength = 0.15f;

	public Transform transformCache;
	public GameObject gameObjectCache;
	public Rigidbody2D rigidbodyCache;
	public CapsuleCollider2D capsuleColliderCache;

	public Transform targetTransform;

	private Vector2Int lastCell = new(-9999, -9999);
	private Vector2Int currentCell = new(-9999, -9999);

	private Vector3 direction;

	private void Awake()
	{
		if (transformCache == null) transformCache = transform;
		if (gameObjectCache == null) gameObjectCache = gameObject;
		if (rigidbodyCache == null) rigidbodyCache = GetComponent<Rigidbody2D>();
		if (capsuleColliderCache == null) capsuleColliderCache = GetComponent<CapsuleCollider2D>();

		SetPhysics(usingPhysics);
	}

	public void DoUpdate(float deltaTime)
	{
		currentCell = Vector2Int.FloorToInt(transformCache.position);
		if (currentCell != lastCell)
		{
			if (cellDict.ContainsKey(lastCell))
			{
				cellDict[lastCell].Remove(this);
			}
			if (!cellDict.ContainsKey(currentCell))
			{
				cellDict[currentCell] = new HashSet<TestCharacter>();
			}
			cellDict[currentCell].Add(this);
			lastCell = currentCell;
		}

		if (isControlled)
		{
			direction = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
			direction.Normalize();
		}
		else
		{
			direction = (targetTransform.position - transformCache.position).normalized;
		}

		if (usingBoids)
		{
			UpdateBoids();
		}

		if (usingPhysics)
		{
			rigidbodyCache.linearVelocity = moveSpeed * direction;
		}
		else
		{
			transformCache.position += moveSpeed * deltaTime * direction;
		}
	}

	public void SetPhysics(bool usePhysics)
	{
		if (isControlled) usePhysics = false;

		usingPhysics = usePhysics;
		rigidbodyCache.simulated = usePhysics;
		capsuleColliderCache.enabled = usePhysics;
	}

	public void SetBoids(bool useBoids, int boidsRadius, int boidsCap, float seperationStrength, float alignmentStrength)
	{
		if (isControlled) useBoids = false;

		usingBoids = useBoids;
		this.boidsRadius = boidsRadius;
		this.boidsCap = boidsCap;
		this.seperationStrength = seperationStrength;
		this.alignmentStrength = alignmentStrength;
	}

	private void UpdateBoids()
	{
		Vector3 neighborVector;
		int neighborMet = 0;

		for (int x = -boidsRadius; x <= boidsRadius; x++)
		{
			for (int y = -boidsRadius; y <= boidsRadius; y++)
			{
				Vector2Int neighborCell = currentCell + new Vector2Int(x, y);
				if (cellDict.ContainsKey(neighborCell))
				{
					foreach (TestCharacter neighbor in cellDict[neighborCell])
					{
						if (neighbor == this || neighbor.isControlled) continue;
						if (neighborMet >= boidsCap) break;
						neighborMet++;

						neighborVector = transformCache.position - neighbor.transformCache.position;
						direction += seperationStrength / neighborVector.sqrMagnitude * neighborVector;
						direction += alignmentStrength * neighbor.direction;
					}
				}
			}
		}

		direction.Normalize();
	}
}