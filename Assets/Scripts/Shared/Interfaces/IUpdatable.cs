/// <summary>
/// Remember to use UpdaterManager
/// </summary>
public interface IUpdatable
{
	public event System.Action<bool> OnEnabledChanged;

	public bool Enabled { get; }
	public bool UseUpdate { get; }
	public bool UseFixedUpdate { get; }
	public bool UseUnscaledTime { get; }

	public void Enable(bool updating);
	public void DoUpdate(float deltaTime);
	public void DoFixedUpdate(float fixedDeltaTime);
	public void DoUnscaledUpdate(float unscaledDeltaTime);
}
