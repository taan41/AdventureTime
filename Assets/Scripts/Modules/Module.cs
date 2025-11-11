public abstract class ModuleBase : IUpdatable
{
	public event System.Action<bool> OnEnabledChanged;

	public bool Enabled { get; private set; } = false;
	public bool UseUpdate { get; private set; } = false;
	public bool UseFixedUpdate { get; private set; } = false;
	public bool UseUnscaledTime { get; private set; } = false;

	private bool isRegistered = false;

	protected ModuleBase(bool useUpdate = false, bool useFixedUpdate = false, bool useUnscaledTime = false)
	{
		UseUpdate = useUpdate;
		UseFixedUpdate = useFixedUpdate;
		UseUnscaledTime = useUnscaledTime;
	}

	public virtual void Enable(bool enabled = true)
	{
		Enabled = enabled;
		OnEnabledChanged?.Invoke(enabled);

		if (!isRegistered && enabled)
		{
			UpdaterManager.Instance.GetUpdater<ModuleBase>().Register(this);
			isRegistered = true;
		}
	}

	public virtual void DoUpdate(float deltaTime) { }

	public virtual void DoFixedUpdate(float fixedDeltaTime) { }

	public virtual void DoUnscaledUpdate(float unscaledDeltaTime) { }
}