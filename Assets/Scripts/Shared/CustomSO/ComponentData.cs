using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComponentData", menuName = "ScriptableObjects/Generic/ComponentData")]
public class ComponentData : CustomSO<ComponentData>
{
	public enum GameObjectSortingLayer
	{
		Background,
		UnderCharacter,
		Character,
		AboveCharacter,
		Foreground,
	}

	[Header("Sprite Renderer")]
	public SpriteDrawMode drawMode = SpriteDrawMode.Simple;
	public Vector2 rendererSize = Vector2.one;
	public SpriteTileMode spriteTileMode = SpriteTileMode.Continuous;
	public SpriteSortPoint spriteSortPoint = SpriteSortPoint.Pivot;
	public GameObjectSortingLayer sortingLayer;
	public int orderInLayer = 0;

	[Header("Collider")]
	public bool isTrigger = true;
	public bool autoSize = false;
	public float autoSizeMultiplierX = 1f;
	public float autoSizeMultiplierY = 1f;
	public float autoSizeOffsetMultiplierX = 0f;
	public float autoSizeOffsetMultiplierY = 0f;
	public CapsuleDirection2D manualDirection = CapsuleDirection2D.Vertical;
	public Vector2 manualSize = Vector2.one;
	public Vector2 manualOffset = Vector2.zero;

	[Header("Rigidbody2D")]
	public RigidbodyType2D bodyType = RigidbodyType2D.Kinematic;
	public CollisionDetectionMode2D collisionDetection = CollisionDetectionMode2D.Discrete;
	public RigidbodySleepMode2D sleepMode = RigidbodySleepMode2D.StartAwake;
	public RigidbodyInterpolation2D interpolation = RigidbodyInterpolation2D.None;
	public RigidbodyConstraints2D constraints = RigidbodyConstraints2D.FreezeRotation;

	private readonly List<Vector2> spritePoints = new();
	private Vector2 minSpritePoint = Vector2.zero;
	private Vector2 maxSpritePoint = Vector2.zero;

	public override void CopyFrom(ComponentData other)
	{
		if (other == null) return;

		drawMode = other.drawMode;
		rendererSize = other.rendererSize;
		spriteTileMode = other.spriteTileMode;
		spriteSortPoint = other.spriteSortPoint;
		sortingLayer = other.sortingLayer;
		orderInLayer = other.orderInLayer;

		isTrigger = other.isTrigger;
		autoSize = other.autoSize;
		autoSizeMultiplierX = other.autoSizeMultiplierX;
		autoSizeMultiplierY = other.autoSizeMultiplierY;
		autoSizeOffsetMultiplierX = other.autoSizeOffsetMultiplierX;
		autoSizeOffsetMultiplierY = other.autoSizeOffsetMultiplierY;
		manualDirection = other.manualDirection;
		manualSize = other.manualSize;
		manualOffset = other.manualOffset;

		bodyType = other.bodyType;
		collisionDetection = other.collisionDetection;
		sleepMode = other.sleepMode;
		interpolation = other.interpolation;
		constraints = other.constraints;

		SignalDataChange();
	}

	public void SetComponents(Sprite refSprite, SpriteRenderer renderer, CapsuleCollider2D collider, Rigidbody2D rigidbody, out Vector2 spriteSize)
	{
		if (renderer != null)
		{
			renderer.drawMode = drawMode;
			renderer.size = rendererSize;
			renderer.tileMode = spriteTileMode;
			renderer.sortingOrder = orderInLayer;
			renderer.sortingLayerName = sortingLayer.ToString();
			renderer.spriteSortPoint = spriteSortPoint;
		}

		if (refSprite == null)
		{
			spriteSize = Vector2.one;
		}
		else
		{
			spritePoints.Clear();
			try
			{
				refSprite.GetPhysicsShape(0, spritePoints);
			}
			catch
			{
				spritePoints.Add(Vector2.zero);
				spritePoints.Add(new Vector2(refSprite.rect.width, refSprite.rect.height));
			}
			minSpritePoint = spritePoints[0];
			maxSpritePoint = spritePoints[0];
			foreach (var point in spritePoints)
			{
				if (point.x < minSpritePoint.x) minSpritePoint.x = point.x;
				if (point.y < minSpritePoint.y) minSpritePoint.y = point.y;
				if (point.x > maxSpritePoint.x) maxSpritePoint.x = point.x;
				if (point.y > maxSpritePoint.y) maxSpritePoint.y = point.y;
			}
			spriteSize = maxSpritePoint - minSpritePoint;
		}

		if (collider != null)
		{
			collider.isTrigger = isTrigger;

			if (autoSize && refSprite != null)
			{
				Vector2 finalSize = new(spriteSize.x * autoSizeMultiplierX, spriteSize.y * autoSizeMultiplierY);

				collider.direction = finalSize.x > finalSize.y ? CapsuleDirection2D.Horizontal : CapsuleDirection2D.Vertical;
				collider.size = finalSize;
				collider.offset = new Vector2(
					(minSpritePoint.x + maxSpritePoint.x) / 2f + autoSizeOffsetMultiplierX * spriteSize.x,
					(minSpritePoint.y + maxSpritePoint.y) / 2f + autoSizeOffsetMultiplierY * spriteSize.y
				);
			}
			else
			{
				collider.direction = manualDirection;
				collider.size = manualSize;
				collider.offset = manualOffset;
			}
		}

		if (rigidbody != null)
		{
			rigidbody.bodyType = bodyType;
			rigidbody.collisionDetectionMode = collisionDetection;
			rigidbody.sleepMode = sleepMode;
			rigidbody.interpolation = interpolation;
			rigidbody.constraints = constraints;
		}
	}
}