using System;
using UnityEngine;

public partial class Character
{
	[Serializable]
	public class CharacterRule : IGameObjectData
	{
		public bool useAI = true;
		public bool invincible = false;

		public void CopyFrom(IGameObjectData other)
		{
			if (other == null) return;
			if (other is not CharacterRule otherRule) return;
		}
	}

	[Serializable]
	public class CharacterConfiguration
	{
		public bool sizeAsWorldUnit = false;
		public bool showStaminaAndManaBars = true;
		public Vector2 healthBarOffset = new(0f, 1.9f);
		public Vector2 staminaManaBarOffset = new(1f, 0.7f);
	}

	[Serializable]
	public class CharacterTradingSetting
	{
		[Serializable]
		public struct PricePercentage
		{
			public float basePercentage;
			public float minPercentage;
			public float maxPercentage;
			public float incrementPerProfit;
		}

		public PricePercentage sellPricePercentage = new()
		{
			basePercentage = 100f,
			minPercentage = 50f,
			maxPercentage = 150f,
			incrementPerProfit = -0.1f
		};

		public PricePercentage buyPricePercentage = new()
		{
			basePercentage = 50f,
			minPercentage = 10f,
			maxPercentage = 100f,
			incrementPerProfit = 0.1f
		};
	}

	[Serializable]
	public class CharacterAISetting
	{
		[Header("General")]
		public int stateUpdateDelay = 10;

		[Header("Regrouping")]
		public bool fleeWhileRegrouping = false;
		public float fleeWhileRegroupingFactor = 0.7f;
		public float regroupDuration = 0.5f;
		public float regroupDistance = 8f;
		public float regroupIdleDistance = 4f;
		public float regroupRestDistance = 3f;

		[Header("Finding Target")]
		public CharacterTag targetTag = CharacterTag.Hero;
		public TargetFindMode findMode = TargetFindMode.GridMode;
		public bool useDistanceOnList = false;
		public float findDistance = 8f;
		public int findDelayOnSuccess = 10;
		public int findDelayOnFail = 20;
		public int maxFindTargetCount = 5;

		[Header("Boids")]
		public bool useBoids = false;
		public int boidsUpdateDelay = 10;
		public int boidsGridRange = 2;
		public int boidsNeighborCount = 8;
		public float boidsSeparationForce = 0.3f;
		public float boidsAlignmentForce = 0.1f;

		[Header("Fleeing")]
		public bool onlyFleeFromTarget = false;
		public bool dashWhileFleeing = true;
		public int fleeCalculationDelay = 10;
		public int fleeTargetCount = 8;
		public float fleeDuration = 0.5f;
		public float fleeDistance = 0f;

		[Header("Engaging")]
		public bool fleeWhenNotEngaging = false;
		public float engageDuration = 0.2f;
		public float engageDistance = 0f;

		[Header("Chasing")]
		public bool alwaysChase = false;
		public bool dashWhileChasing = false;
		public float chaseDuration = 0.2f;
		public float chaseDistance = 0f;

		[Header("Skill Usage")]
		public float offCdFindingDistanceFactor = 2f;
		public float offCdChaseDistanceFactor = 1.5f;
		public float offCdEngageDistanceFactor = 1f;
		public float offCdFleeDistanceFactor = 0.4f;
		public float onCdFindingDistanceFactor = 1.5f;
		public float onCdChaseDistanceFactor = 0f;
		public float onCdEngageDistanceFactor = 0f;
		public float onCdFleeDistanceFactor = 0.9f;
	}
}