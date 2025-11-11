using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static CameraManager Instance { get; private set; }

	public Camera mainCamera;
	public Camera uiCamera;
	public CinemachineCamera cinemachineCamera;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}

		if (cinemachineCamera == null)
		{
			cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
			if (cinemachineCamera == null)
			{
				Debug.LogError("Cinemachine Camera not found in the scene.");
			}
		}
	}

	public void SetTrackingTarget(Transform target)
	{
		if (cinemachineCamera != null)
		{
			cinemachineCamera.Follow = target;
		}
	}
}