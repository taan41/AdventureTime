using UnityEngine;

using static EightWayDirection;

public enum EightWayDirection
{
	Up,
	Down,
	Left,
	Right,
	LeftUp,
	LeftDown,
	RightUp,
	RightDown,
}

public static class EightWayDirectionExtension
{
	public static Vector3 LeftUpVector = new Vector3(-1, 1).normalized;
	public static Vector3 LeftDownVector = new Vector3(-1, -1).normalized;
	public static Vector3 RightUpVector = new Vector3(1, 1).normalized;
	public static Vector3 RightDownVector = new Vector3(1, -1).normalized;

	public static readonly EightWayDirection[] AllDirections = (EightWayDirection[])System.Enum.GetValues(typeof(EightWayDirection));

	public static bool IsUp(this EightWayDirection dir) => dir == Up || dir == LeftUp || dir == RightUp;
	public static bool IsDown(this EightWayDirection dir) => dir == Down || dir == LeftDown || dir == RightDown;
	public static bool IsLeft(this EightWayDirection dir) => dir == Left || dir == LeftUp || dir == LeftDown;
	public static bool IsRight(this EightWayDirection dir) => dir == Right || dir == RightUp || dir == RightDown;
	public static bool IsUpDown(this EightWayDirection dir) => IsUp(dir) || IsDown(dir);
	public static bool IsLeftRight(this EightWayDirection dir) => IsLeft(dir) || IsRight(dir);
	public static bool IsDiagonal(this EightWayDirection dir) => dir == LeftUp || dir == LeftDown || dir == RightUp || dir == RightDown;
	public static bool IsCardinal(this EightWayDirection dir) => !dir.IsDiagonal();
	public static bool IsHorizontal(this EightWayDirection dir) => dir == Left || dir == Right;
	public static bool IsVertical(this EightWayDirection dir) => dir == Up || dir == Down;

	public static EightWayDirection Flip(this EightWayDirection dir)
	{
		return dir switch
		{
			Up => Down,
			Down => Up,
			Left => Right,
			Right => Left,
			LeftUp => RightDown,
			LeftDown => RightUp,
			RightUp => LeftDown,
			RightDown => LeftUp,
			_ => dir
		};
	}

	public static EightWayDirection FlipX(this EightWayDirection dir)
	{
		return dir switch
		{
			Left => Right,
			Right => Left,
			LeftUp => RightUp,
			LeftDown => RightDown,
			RightUp => LeftUp,
			RightDown => LeftDown,
			_ => dir
		};
	}

	public static EightWayDirection FlipY(this EightWayDirection dir)
	{
		return dir switch
		{
			Up => Down,
			Down => Up,
			LeftUp => LeftDown,
			LeftDown => LeftUp,
			RightUp => RightDown,
			RightDown => RightUp,
			_ => dir
		};
	}

	public static EightWayDirection ToEightWay(this Vector3 vector)
	{
		if (vector == Vector3.zero) return Down;

		float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
		if (angle < 0) angle += 360f;

		if (angle >= 337.5f || angle < 22.5f) return Right;
		if (angle >= 22.5f && angle < 67.5f) return RightUp;
		if (angle >= 67.5f && angle < 112.5f) return Up;
		if (angle >= 112.5f && angle < 157.5f) return LeftUp;
		if (angle >= 157.5f && angle < 202.5f) return Left;
		if (angle >= 202.5f && angle < 247.5f) return LeftDown;
		if (angle >= 247.5f && angle < 292.5f) return Down;
		return RightDown;
	}

	public static EightWayDirection ToFourWay(this EightWayDirection dir)
	{
		return dir switch
		{
			LeftUp => Up,
			RightUp => Up,
			LeftDown => Down,
			RightDown => Down,
			_ => dir
		};
	}

	public static EightWayDirection ToFourWay(this Vector3 vector)
	{
		return vector.ToEightWay().ToFourWay();
	}

	public static Vector3 ToVector(this EightWayDirection dir)
	{
		return dir switch
		{
			Up => Vector3.up,
			Down => Vector3.down,
			Left => Vector3.left,
			Right => Vector3.right,
			LeftUp => LeftUpVector,
			LeftDown => LeftDownVector,
			RightUp => RightUpVector,
			RightDown => RightDownVector,
			_ => Vector3.zero,
		};
	}

	public static Vector3 ToEightWayVector(this Vector3 vector)
	{
		return vector.ToEightWay().ToVector();
	}
}