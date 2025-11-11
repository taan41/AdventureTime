using System;
using UnityEngine;

public interface IInteractable
{
	public enum FocusedCharacterType
	{
		Self,
		Other
	}

	[Serializable]
	public class InteractableRule
	{
		public bool isInteractable = false;
		public float interactionRange = 2f;
		public float offcamDespawnDelay = 10f;
		public Sprite indicatorIcon = null;
		public Vector2 indicatorOffset = Vector2.zero;
		public MenuManager.MenuType menuType = MenuManager.MenuType.None;
		public FocusedCharacterType focusedCharacterType = FocusedCharacterType.Self;
	}

	[RequireComponent(typeof(CircleCollider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class HeroDetector : MonoBehaviour
	{
		private IInteractable owner;
		private CircleCollider2D circleCollider = null;
		private Rigidbody2D rigidBody = null;

		private GameObject indicatorObj = null;
		private SpriteRenderer indicatorRenderer = null;

		private void Awake()
		{
			name = "HeroDetector";
			gameObject.layer = LayerMask.NameToLayer("Interactable");

			circleCollider = GetComponent<CircleCollider2D>();
			circleCollider.isTrigger = true;
			circleCollider.enabled = false;

			rigidBody = GetComponent<Rigidbody2D>();
			rigidBody.bodyType = RigidbodyType2D.Kinematic;
			rigidBody.simulated = false;

			indicatorObj = new GameObject("InteractableIndicator");
			indicatorObj.transform.SetParent(transform);

			indicatorRenderer = indicatorObj.AddComponent<SpriteRenderer>();
			indicatorRenderer.sortingLayerName = "Foreground";
			indicatorRenderer.enabled = false;
		}

		public void SetRules(InteractableRule rules)
		{
			Enable(rules.isInteractable);
			SetRadius(rules.interactionRange);
			SetIndicator(rules.indicatorIcon, rules.indicatorOffset);
		}

		public void Enable(bool enabled)
		{
			circleCollider.enabled = enabled;
			rigidBody.simulated = enabled;
		}

		public void SetOwner(IInteractable interactable)
		{
			owner = interactable;
		}

		public void SetRadius(float detectionRadius)
		{
			circleCollider.radius = detectionRadius;
		}

		public void SetIndicator(Sprite icon, Vector2 offset)
		{
			indicatorRenderer.sprite = icon;
			indicatorObj.transform.localPosition = offset;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.TryGetComponent(out Character character) && (object)character != owner)
			{
				character.NearbyInteractables.Add(owner);
				indicatorRenderer.enabled = true;
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.TryGetComponent(out Character character) && (object)character != owner)
			{
				character.NearbyInteractables.Remove(owner);
				indicatorRenderer.enabled = false;
			}
		}
	}

	public Transform InteractableTransform { get; }
	public InteractableRule InteractableRules { get; }
	public HeroDetector InteractableHeroDetector { get; }
	public void Interact(Character character);

	public static HeroDetector CreateHeroDetector(IInteractable owner)
	{
		var detectorObj = new GameObject("Hero Detector");
		var detector = detectorObj.AddComponent<HeroDetector>();
		detector.SetOwner(owner);
		if (owner is MonoBehaviour mb)
		{
			detector.transform.SetParent(mb.transform, false);
		}
		return detector;
	}
}