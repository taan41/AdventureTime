using UnityEngine;

public class Wall : MonoBehaviour
{
	private Transform transformCache;
	public Rect WallRect { get; private set; }

	private void Awake()
	{
		transformCache = transform;

		if (TryGetComponent(out BoxCollider2D boxCollider))
		{
			Vector2 colliderSize = boxCollider.size;
			WallRect = new Rect(
				transformCache.position.x + boxCollider.offset.x - colliderSize.x * 0.5f,
				transformCache.position.y + boxCollider.offset.y - colliderSize.y * 0.5f,
				colliderSize.x,
				colliderSize.y
			);
			boxCollider.size = new(colliderSize.x + 2f, colliderSize.y + 2f);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Character character))
		{
			character.MovementModule.SetWalls(WallRect, true);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out Character character))
		{
			character.MovementModule.SetWalls(WallRect, false);
		}
	}
}