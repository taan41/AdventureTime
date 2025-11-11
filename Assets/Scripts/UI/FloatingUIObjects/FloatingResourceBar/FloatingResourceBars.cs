using UnityEngine;
using UnityEngine.UI;

public class FloatingResourceBars : FloatingUIObject
{
	[SerializeField] private Transform transformCache;
	[SerializeField] private Vector2 healthBarPosition;
	[SerializeField] private Vector2 staminaAndManaBarPosition;
	[SerializeField] private float disableTime = 2f;
	[SerializeField] private GameObject healthBar;
	[SerializeField] private RectTransform healthBarRect;
	[SerializeField] private Canvas healthBarCanvas;
	[SerializeField] private Image healthFill;
	[SerializeField] private GameObject staminaAndManaBar;
	[SerializeField] private RectTransform staminaAndManaBarRect;
	[SerializeField] private Canvas staminaAndManaBarCanvas;
	[SerializeField] private Image staminaFill;
	[SerializeField] private Image manaFill;

	// [SerializeField] private Camera uiCamera;
	[SerializeField] private Character target;

	private Transform targetTransform;
	private ResourceModule healthModule;
	private ResourceModule staminaModule;
	private ResourceModule manaModule;

	private Vector3 lastTargetPosition;
	private float healthDisableTimer = 0f;
	private float staminaAndManaDisableTimer = 0f;

	private bool initialized = false;

	public void Initialize()
	{
		if (initialized) return;
		initialized = true;

		// transformCache.SetParent(UIReferences.Instance.floatingResourceBarCanvas.transform, false);
		// uiCamera = CameraManager.Instance.mainCamera;

		healthBarCanvas.worldCamera = CameraManager.Instance.uiCamera;
		staminaAndManaBarCanvas.worldCamera = CameraManager.Instance.uiCamera;

		UpdaterManager.Instance.GetUpdater<FloatingResourceBars>().Register(this);
		Enable(false);
	}

	public override void Enable(bool enabled)
	{
		gameObject.SetActive(enabled);

		Enabled = enabled;
		InvokeOnUpdatingChanged(enabled);
	}

	public void SetTarget(Character targetCharacter, bool showStaminaAndMana = true)
	{
		if (target != null)
		{
			transformCache.SetParent(null);
			target.OnEnabledChanged -= Enable;
			healthModule.OnResourceChanged -= RefreshHealthBar;
			staminaModule.OnResourceChanged -= RefreshStaminaAndManaBar;
			manaModule.OnResourceChanged -= RefreshStaminaAndManaBar;
		}

		target = targetCharacter;

		if (target == null)
		{
			transformCache.SetParent(null);
			Enable(false);
			return;
		}

		targetTransform = target.TransformCache;
		healthModule = target.HealthModule;

		transformCache.SetParent(targetTransform, false);
		transformCache.localPosition = Vector3.zero;

		healthFill.fillAmount = healthModule.Normalized;
		healthBarRect.anchoredPosition = targetCharacter.Data.settings.configs.healthBarOffset;
		healthBar.SetActive(false);

		target.OnEnabledChanged += Enable;
		healthModule.OnResourceChanged += RefreshHealthBar;

		if (showStaminaAndMana)
		{
			staminaAndManaBarRect.anchoredPosition = targetCharacter.Data.settings.configs.staminaManaBarOffset;

			staminaModule = target.StaminaModule;
			manaModule = target.ManaModule;

			staminaFill.fillAmount = staminaModule.Normalized;
			manaFill.fillAmount = manaModule.Normalized;

			staminaModule.OnResourceChanged += RefreshStaminaAndManaBar;
			manaModule.OnResourceChanged += RefreshStaminaAndManaBar;
		}
		else
		{
			staminaModule = null;
			manaModule = null;
		}

		staminaAndManaBar.SetActive(false);

		Enable(true);
	}

	public override void DoUpdate(float deltaTime)
	{
		if (healthDisableTimer > 0f)
		{
			healthDisableTimer -= deltaTime;
			if (healthDisableTimer <= 0f)
			{
				healthBar.SetActive(false);
			}
		}

		if (staminaAndManaDisableTimer > 0f)
		{
			staminaAndManaDisableTimer -= deltaTime;
			if (staminaAndManaDisableTimer <= 0f)
			{
				staminaAndManaBar.SetActive(false);
			}
		}
	}

	private void RefreshHealthBar()
	{
		healthFill.fillAmount = healthModule.Normalized;

		healthBar.SetActive(true);

		healthDisableTimer = healthFill.fillAmount == 1f ? disableTime : 0f;
	}

	private void RefreshStaminaAndManaBar()
	{
		staminaFill.fillAmount = staminaModule.Normalized;
		manaFill.fillAmount = manaModule.Normalized;

		staminaAndManaBar.SetActive(true);

		staminaAndManaDisableTimer = (staminaFill.fillAmount == 1f && manaFill.fillAmount == 1f) ? disableTime : 0f;
	}
}