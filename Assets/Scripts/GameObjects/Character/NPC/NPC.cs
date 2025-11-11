// using UnityEngine;

// public partial class NPC : Character
// {

// 	private NPCData NPCData => Data as NPCData;

// 	private HeroDetector detector;
// 	private GameObject detectionIconObj;
// 	private SpriteRenderer detectionIconRenderer;

// 	protected override void Awake()
// 	{
// 		base.Awake();

// 		detector = new GameObject("Detector").AddComponent<HeroDetector>();
// 		detector.transform.SetParent(transform);
// 		detector.transform.localPosition = Vector3.zero;

// 		detectionIconObj = new GameObject("Detection Icon");
// 		detectionIconObj.transform.SetParent(TransformCache);
// 		detectionIconObj.SetActive(false);

// 		detectionIconRenderer = detectionIconObj.AddComponent<SpriteRenderer>();
// 		detectionIconRenderer.sortingLayerName = "Foreground";
// 	}

// 	protected override void Start()
// 	{
// 		base.Start();

// 		// if (NPCData != null)
// 		// {
// 		// 	detector.Setup(this, NPCData.detectionRadius);
// 		// }
// 	}

// 	public override bool SetData(CharacterDataNew newData, bool refresh = true)
// 	{
// 		if (!base.SetData(newData, refresh)) return false;

// 		detector.SetRadius(this, NPCData.detectionRadius);

// 		detectionIconRenderer.sprite = NPCData.detectionIcon;
// 		detectionIconObj.transform.localPosition = NPCData.detectionIconOffset;

// 		return true;
// 	}

// 	private void OnDetectorEnter(Collider2D collision)
// 	{
// 		if (collision.TryGetComponent(out Character character) && character != this)
// 		{
// 			if (NPCData.interactableTags.Contains(character.Tag))
// 			{
// 				detectionIconObj.SetActive(true);
// 				character.NearbyInteractables.Add(this);
// 			}
// 		}
// 	}

// 	private void OnDetectorExit(Collider2D collision)
// 	{
// 		if (collision.TryGetComponent(out Character character) && character != this)
// 		{
// 			if (NPCData.interactableTags.Contains(character.Tag))
// 			{
// 				detectionIconObj.SetActive(false);
// 				character.NearbyInteractables.Remove(this);
// 			}
// 		}
// 	}

// 	public void Interact(Character character)
// 	{
// 		if (NPCData == null || NPCData.function == null) return;

// 		NPCData.function.Activate(this, character);
// 	}

// 	public void StopInteract(Character character)
// 	{
// 		if (NPCData == null || NPCData.function == null) return;

// 		NPCData.function.Deactivate(this, character);
// 	}
// }