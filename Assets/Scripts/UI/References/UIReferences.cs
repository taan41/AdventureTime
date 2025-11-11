using UnityEngine;

public class UIReferences : MonoBehaviour
{
	public static UIReferences Instance { get; private set; }

	public Canvas battleOverlayCanvas;
	public Canvas menuCanvas;
	public Canvas menuOverlayCanvas;
	public Canvas pauseCanvas;

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
	}
}