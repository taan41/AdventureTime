using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultScreen : MonoBehaviour
{
	public static ResultScreen Instance { get; private set; }
	
	[SerializeField] private GameObject victoryPanel;
	[SerializeField] private GameObject defeatPanel;
	[SerializeField] private CustomButton playAgainButton;
	[SerializeField] private CustomButton mainMenuButton;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		gameObject.SetActive(false);

		playAgainButton.OnClicked += OnPlayAgainClicked;
		mainMenuButton.OnClicked += OnMainMenuClicked;
	}

	public void ShowVictory()
	{
		GameStateManager.Instance.SetState(GameStateManager.GameState.GameMenu);

		victoryPanel.SetActive(true);
		defeatPanel.SetActive(false);
		gameObject.SetActive(true);
	}

	public void ShowDefeat()
	{
		GameStateManager.Instance.SetState(GameStateManager.GameState.GameMenu);
		
		victoryPanel.SetActive(false);
		defeatPanel.SetActive(true);
		gameObject.SetActive(true);
	}

	private void OnPlayAgainClicked()
	{
		GameStateManager.Instance.LoadScene("GameScene1");
	}

	private void OnMainMenuClicked()
	{
		GameStateManager.Instance.LoadScene("MainMenu");
	}
}