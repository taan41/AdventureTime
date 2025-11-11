using UnityEngine;

public partial class UIManager : MonoBehaviour, IUpdatable
{
	public static UIManager Instance { get; private set; }

	public event System.Action<bool> OnEnabledChanged;

	public bool Enabled { get; private set; } = true;
	public bool UseUpdate { get; private set; } = false;
	public bool UseFixedUpdate { get; private set; } = false;
	public bool UseUnscaledTime { get; private set; } = true;

	[SerializeField] private UIManagerData data;
	public UIManagerData Data { get => data; }

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		AwakeColor();
		// AwakeMouse();
		// AwakeOverlay();

		SetData(data, true);

		UpdaterManager.StaticRegisterFirstUpdater(this);
	}

	public void SetData(UIManagerData newData = null, bool force = false)
	{
		if (newData == data && !force) return;
		if (data != null) data.OnDataChanged -= RefreshData;

		data = newData;

		if (data == null) return;

		RefreshData();

		data.OnDataChanged += RefreshData;
	}

	void RefreshData()
	{
		RefreshDataColor();
		// RefreshDataMouse();
		// RefreshDataOverlay();
	}

	public void Enable(bool enabled)
	{
		Enabled = enabled;
		OnEnabledChanged?.Invoke(enabled);
	}

	public void DoUnscaledUpdate(float unscaledDeltaTime)
	{
		// UpdateMouse(unscaledDeltaTime);
	}

	public void DoUpdate(float deltaTime) { }
	public void DoFixedUpdate(float fixedDeltaTime) { }
}