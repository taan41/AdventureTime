using UnityEngine;

public static class CMath
{
	#region Constants
	#endregion

	#region Methods
	public static float Lerp(float minValue, float maxValue, float timeFactor)
	{
		return minValue + (maxValue - minValue) * timeFactor;
	}

	public static float LerpClamped(float minValue, float maxValue, float timeFactor)
	{
		return minValue + (maxValue - minValue) * Mathf.Clamp01(timeFactor);
	}
	#endregion

	#region Easing
	public static class Ease
	{
		public static class Back
		{
			public static float In(float timeFactor, float overshoot = 1.70158f)
				=> timeFactor * timeFactor * ((overshoot + 1) * timeFactor - overshoot);

			public static float Out(float timeFactor, float overshoot = 1.70158f)
				=> --timeFactor * timeFactor * ((overshoot + 1) * timeFactor + overshoot) + 1;

			public static float InOut(float timeFactor, float overshoot = 1.70158f, float adjustment = 1.525f)
				=> (timeFactor *= 2) < 1
					? 0.5f * (timeFactor * timeFactor * ((overshoot *= adjustment + 1) * timeFactor - overshoot))
					: 0.5f * ((timeFactor -= 2) * timeFactor * ((overshoot *= adjustment + 1) * timeFactor + overshoot) + 2);

			public static Vector2 Vector2In(Vector2 start, Vector2 end, float timeFactor, float overshoot = 1.70158f)
				=> new(
					In(timeFactor, overshoot) * (end.x - start.x) + start.x,
					In(timeFactor, overshoot) * (end.y - start.y) + start.y
				);

			public static Vector2 Vector2Out(Vector2 start, Vector2 end, float timeFactor, float overshoot = 1.70158f)
				=> new(
					Out(timeFactor, overshoot) * (end.x - start.x) + start.x,
					Out(timeFactor, overshoot) * (end.y - start.y) + start.y
				);
				
			public static Vector2 Vector2InOut(Vector2 start, Vector2 end, float timeFactor, float overshoot = 1.70158f, float adjustment = 1.525f)
				=> new(
					InOut(timeFactor, overshoot, adjustment) * (end.x - start.x) + start.x,
					InOut(timeFactor, overshoot, adjustment) * (end.y - start.y) + start.y
				);
		}

		public static class Expo
		{
			public static float In(float timeFactor)
				=> timeFactor == 0 ? 0 : Mathf.Pow(2, 10 * (timeFactor - 1));

			public static float Out(float timeFactor)
				=> timeFactor == 1 ? 1 : 1 - Mathf.Pow(2, -10 * timeFactor);

			public static float InOut(float timeFactor)
			{
				if (timeFactor == 0) return 0;
				if (timeFactor == 1) return 1;
				if ((timeFactor *= 2) < 1) return 0.5f * Mathf.Pow(2, 10 * (timeFactor - 1));
				return 0.5f * (2 - Mathf.Pow(2, -10 * --timeFactor));
			}
		}
	}
	#endregion
}