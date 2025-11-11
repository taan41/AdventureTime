using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatingNumberData", menuName = "ScriptableObjects/UI/FloatingNumberData")]
public class FloatingNumberData : CustomSO<FloatingNumberData>
{
	[Header("General Settings")]
	public int poolSize = 50;
	public bool fixedPoolSize = true;
	public TMP_FontAsset tmpFont;
	public Font legacyFont;
	public int fontSize = 10;

	[Header("Color Settings")]
	public FloatingNumber.FloatingNumberColor defaultColor;
	public FloatingNumber.FloatingNumberColor damageColor;
	public FloatingNumber.FloatingNumberColor healColor;
	public FloatingNumber.FloatingNumberColor staminaColor;
	public FloatingNumber.FloatingNumberColor manaColor;
	public float colorVarianceFactor = 0.5f;

	[Header("Attribute Settings")]
	public Vector2 positionOffset = new(0f, 0f);
	public Vector2 positionOffsetVariance = new(0.5f, 0.5f);
	public float rotationVariance = 10f;

	public float speed = 5f;
	public float minSpeed = 0.5f;
	public float speedVariance = 1f;
	public float acceleration = -8f;
	// public float accelerationVariance = 0f;

	public float duration = 1f;
	public float durationVariance = 0.3f;

	public float minScale = 1f;
	public float maxScale = 1.5f;
	// public float scaleVarianceFactor = 0.3f;

	public float maxOnNumber = 5000f;
	public float minRandomFactor = -1f;
	public float maxRandomFactor = 1f;

	public override void CopyFrom(FloatingNumberData other)
	{
		if (other == null) return;

		poolSize = other.poolSize;
		fixedPoolSize = other.fixedPoolSize;
		// tmp_font = data.tmp_font;
		legacyFont = other.legacyFont;
		fontSize = other.fontSize;

		defaultColor = other.defaultColor;
		damageColor = other.damageColor;
		healColor = other.healColor;
		colorVarianceFactor = other.colorVarianceFactor;

		positionOffset = other.positionOffset;
		positionOffsetVariance = other.positionOffsetVariance;
		rotationVariance = other.rotationVariance;

		speed = other.speed;
		minSpeed = other.minSpeed;
		speedVariance = other.speedVariance;
		acceleration = other.acceleration;
		// accelerationVariance = data.accelerationVariance;

		duration = other.duration;
		durationVariance = other.durationVariance;

		minScale = other.minScale;
		maxScale = other.maxScale;
		// scaleVarianceFactor = data.scaleVarianceFactor;

		maxOnNumber = other.maxOnNumber;
		minRandomFactor = other.minRandomFactor;
		maxRandomFactor = other.maxRandomFactor;
	}
}