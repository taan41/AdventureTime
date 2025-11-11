using UnityEngine;

public class GameStateManager : MonoBehaviour
{
	public static GameStateManager Instance { get; private set; }

	public enum GameState : byte
	{
		InGame = 0,
		GameMenu = 1 << 0,
		MainMenu = 1 << 1,
	}

	public GameState CurrentState { get; private set; }

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
	}

	public void AddState(GameState newState)
	{
		CurrentState |= newState;
		StateEffects(newState);
	}

	public void RemoveState(GameState state)
	{
		CurrentState &= ~state;
		StateEffects(CurrentState);
	}

	public void SetState(GameState newState)
	{
		CurrentState = newState;
		StateEffects(newState);
	}

	public bool CheckState(GameState state, bool inclusive = true)
	{
		return inclusive ? CurrentState <= state : CurrentState < state;
	}

	public bool IsOnlyState(GameState state)
	{
		return CurrentState == state;
	}

	private void StateEffects(GameState state)
	{
		Time.timeScale = state switch
		{
			GameState.GameMenu => 0f,
			_ => 1f,
		};
	}

	public void LoadScene(string sceneName)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

		InputManager.Instance.ClearSignals();

		if (sceneName == "MainMenu")
		{
			SetState(GameState.MainMenu);
			AudioManager.Instance.PlayMenuBGM();
		}
		else
		{
			SetState(GameState.InGame);
			AudioManager.Instance.PlayIngameBGM();
		}
	}
}