using UnityEngine;

public class MenuManager : MonoBehaviour
{
	public enum MenuType
	{
		None,
		Pause,
		Player,
		Storage,
		Shop,
	}

	public static MenuManager Instance { get; private set; }

	[SerializeField] private PauseMenu pauseMenuPrefab;
	[SerializeField] private PlayerMenu playerMenuPrefab;
	[SerializeField] private ShopMenu shopMenuPrefab;

	private PauseMenu pauseMenu;
	private PlayerMenu playerMenu;
	private ShopMenu shopMenu;

	public MenuBase ActiveMenu { get; private set; } = null;

	private void Awake()
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

		pauseMenu = Instantiate(pauseMenuPrefab, UIReferences.Instance.pauseCanvas.transform);
		pauseMenu.Initialize();
		pauseMenu.gameObject.SetActive(false);
		pauseMenu.OnClosed += () => ActiveMenu = null;

		playerMenu = Instantiate(playerMenuPrefab, UIReferences.Instance.menuCanvas.transform);
		playerMenu.Initialize();
		playerMenu.gameObject.SetActive(false);
		playerMenu.OnClosed += () => ActiveMenu = null;

		shopMenu = Instantiate(shopMenuPrefab, UIReferences.Instance.menuCanvas.transform);
		shopMenu.Initialize();
		shopMenu.gameObject.SetActive(false);
		shopMenu.OnClosed += () => ActiveMenu = null;

		InputManager.Instance.PlayerActions.PlayerMenu.performed += ctx => TogglePlayerMenu();
		InputManager.Instance.UIActions.Cancel.performed += ctx => TogglePauseMenu();
	}

	private void TogglePlayerMenu()
	{
		if (ActiveMenu == null) ToggleMenu(MenuType.Player, null);
		else CloseActiveMenu();
	}

	private void TogglePauseMenu()
	{
		if (ActiveMenu == null) ToggleMenu(MenuType.Pause, null);
		else CloseActiveMenu();
	}

	public void ToggleMenu(MenuType menuType, Character focusedCharacter)
	{
		MenuBase requestedMenu = menuType switch
		{
			MenuType.Pause => pauseMenu,
			MenuType.Player => playerMenu,
			MenuType.Shop => shopMenu,
			_ => null,
		};

		if (ActiveMenu != null)
		{
			if (ActiveMenu == requestedMenu)
			{
				ActiveMenu.Close();
				ActiveMenu = null;
				return;
			}

			ActiveMenu.Close();
		}

		ActiveMenu = requestedMenu;

		if (ActiveMenu == null) return;

		ActiveMenu.Open(focusedCharacter);
	}

	public void CloseActiveMenu()
	{
		if (ActiveMenu != null)
		{
			ActiveMenu.Close();
			ActiveMenu = null;
		}
	}
}