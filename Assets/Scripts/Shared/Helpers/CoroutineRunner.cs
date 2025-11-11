using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
	public static CoroutineRunner Instance { get; private set; }

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public static Coroutine Start(IEnumerator routine)
	{
		if (Instance == null) return null;
		return Instance.StartCoroutine(routine);
	}

	public static void Stop(Coroutine coroutine)
	{
		if (Instance == null || coroutine == null) return;
		Instance.StopCoroutine(coroutine);
	}
}