using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CursorOverlay : MonoBehaviour, IUpdatable
{
	public event System.Action<bool> OnEnabledChanged;

	public bool Enabled { get; private set; } = false;
	public bool UseUpdate { get; private set; } = false;
	public bool UseFixedUpdate { get; private set; } = false;
	public bool UseUnscaledTime { get; private set; } = true;

	public RectTransform CursorTransform { get; private set; }

	public Vector2 offset = Vector2.zero;

	private RectTransform parentRect;
	private Camera uiCamera;

	private void Awake()
	{
		if (CursorTransform == null) CursorTransform = GetComponent<RectTransform>();

		UpdaterManager.StaticRegisterLastUpdater(this);

		Initialize();
	}

	public virtual void Initialize()
	{
		parentRect = UIReferences.Instance.menuOverlayCanvas.transform as RectTransform;
		uiCamera = CameraManager.Instance.uiCamera;

		CursorTransform.SetParent(parentRect, false);
		CursorTransform.localScale = Vector3.one;
		CursorTransform.localPosition = Vector3.zero;
		Enable(false);
	}

	public void Enable(bool enabled)
	{
		Enabled = enabled;
		Cursor.visible = !enabled;
		gameObject.SetActive(enabled);
		OnEnabledChanged?.Invoke(enabled);
	}

	public void DoUpdate(float deltaTime) { }
	public void DoFixedUpdate(float fixedDeltaTime) { }
	public void DoUnscaledUpdate(float unscaledDeltaTime)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			parentRect,
			MouseManager.Instance.MouseScreenPosition,
			uiCamera,
			out Vector2 mousePosition);

		CursorTransform.anchoredPosition = mousePosition + offset;
	}
}