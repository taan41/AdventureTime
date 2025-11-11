using System.Collections.Generic;
using UnityEngine;

public class MovementModule : ModuleBase
{
	struct WallWithOffset
	{
		public Rect wallRect;
		public Rect offsetRect;

		public WallWithOffset(Rect rect, WallOffset offset)
		{
			wallRect = rect;
			offsetRect = new Rect(
				rect.xMin - offset.leftOffset,
				rect.yMin - offset.downOffset,
				rect.width + offset.rightOffset + offset.leftOffset,
				rect.height + offset.upOffset + offset.downOffset
			);
		}

		public void SetOffset(WallOffset offset)
		{
			offsetRect = new Rect(
				wallRect.xMin - offset.leftOffset,
				wallRect.yMin - offset.downOffset,
				wallRect.width + offset.rightOffset + offset.leftOffset,
				wallRect.height + offset.upOffset + offset.downOffset
			);
		}
	}

	struct WallOffset
	{
		public float upOffset;
		public float downOffset;
		public float leftOffset;
		public float rightOffset;

		public static WallOffset operator*(WallOffset offset, float size)
		{
			return new WallOffset
			{
				upOffset = offset.upOffset * size,
				downOffset = offset.downOffset * size,
				leftOffset = offset.leftOffset * size,
				rightOffset = offset.rightOffset * size
			};
		}
	}

	private readonly Transform transform;

	#region Basic Movement
	private Vector3 direction = Vector3.zero;
	public Vector3 Direction
	{
		get => direction;
		set
		{
			direction = value;
			if (direction != Vector3.zero)
			{
				NonZeroDirection = direction;
			}
		}
	}
	public Vector3 NonZeroDirection { get; private set; } = Vector3.down;
	public float Speed = 5f;
	#endregion

	#region Temporary Movement
	private float tempSpeed;
	private float tempSpeedFactor;
	private float tempDuration;
	private Vector3 tempDirection = Vector3.zero;
	private bool tempEndOnZeroSpeed = false;
	private bool forcedMovement = false;
	#endregion

	#region Wall Collision
	private readonly List<WallWithOffset> walls = new();
	private WallOffset wallOffset = new();
	private float offsetSize;
	public float OffsetSize
	{
		get => offsetSize;
		set
		{
			offsetSize = value;
			for (int i = 0; i < walls.Count; i++)
			{
				walls[i].SetOffset(wallOffset * offsetSize);
			}
		}
	}
	#endregion

	public MovementModule(Transform transform) : base(false, true)
	{
		this.transform = transform;
	}

	public void SetWallOffset(float up, float down, float left, float right)
	{
		wallOffset.upOffset = up;
		wallOffset.downOffset = down;
		wallOffset.leftOffset = left;
		wallOffset.rightOffset = right;
	}

	public override void DoFixedUpdate(float fixedDeltaTime)
	{
		if (!Enabled) return;

		if (tempDuration > 0f)
		{
			tempDuration -= fixedDeltaTime;
			if (tempDuration <= 0f)
			{
				tempDuration = 0f;
				forcedMovement = false;
			}

			tempSpeed *= tempSpeedFactor;
			if (tempSpeed <= 0f)
			{
				tempSpeed = 0f;
				if (tempEndOnZeroSpeed)
				{
					tempDuration = 0f;
					forcedMovement = false;
				}
			}

			transform.position += fixedDeltaTime * tempSpeed * tempDirection;
		}
		else
		{
			transform.position += fixedDeltaTime * Speed * direction;
		}

		transform.position = HandleWallCollision(transform.position);
	}

	public bool TempMovement(float speed, float speedFactor, float duration, Vector3 direction = default, bool endOnZeroSpeed = false, bool force = false)
	{
		if (forcedMovement && !force) return false;
		if (duration <= 0f) return true;

		tempSpeed = speed;
		tempSpeedFactor = speedFactor;
		tempDuration = duration;
		tempDirection = direction != Vector3.zero ? direction : NonZeroDirection;
		tempEndOnZeroSpeed = endOnZeroSpeed;
		forcedMovement = force;
		return true;
	}

	public void SetWalls(Rect wallRect, bool add)
	{
		if (add)
		{
			walls.Add(new WallWithOffset(wallRect, wallOffset * OffsetSize));
		}
		else
		{
			walls.RemoveAll(wall => wall.wallRect == wallRect);
		}
	}

	Vector3 HandleWallCollision(Vector3 position)
	{
		if (walls.Count == 0) return position;
		for (int i = 0; i < walls.Count; i++)
		{
			Rect rect = walls[i].offsetRect;
			if (rect.Contains(new Vector2(position.x, position.y)))
			{
				float leftDist = Mathf.Abs(position.x - rect.xMin);
				float rightDist = Mathf.Abs(position.x - rect.xMax);
				float topDist = Mathf.Abs(position.y - rect.yMax);
				float bottomDist = Mathf.Abs(position.y - rect.yMin);

				float minDist = Mathf.Min(leftDist, rightDist, topDist, bottomDist);

				if (minDist == leftDist)
				{
					position.x = rect.xMin;
				}
				else if (minDist == rightDist)
				{
					position.x = rect.xMax;
				}
				else if (minDist == topDist)
				{
					position.y = rect.yMax;
				}
				else if (minDist == bottomDist)
				{
					position.y = rect.yMin;
				}
			}
		}
		return position;
	}
}