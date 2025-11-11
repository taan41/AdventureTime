using UnityEngine;

public class PersistenceRootObj : MonoBehaviour
{
	public static PersistenceRootObj Instance { get; private set; }

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}
}