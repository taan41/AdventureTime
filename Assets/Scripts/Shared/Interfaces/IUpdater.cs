/// <summary>
/// Use UpdaterManager instead
/// </summary>
public interface IUpdater
{
	int Order { get; set; }
	void DoUpdate(float deltaTime);
	void DoFixedUpdate(float fixedDeltaTime);
	void DoUnscaledUpdate(float unscaledDeltaTime);
}