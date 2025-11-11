using TMPro;
using UnityEngine;

public class FloatingUIObjectManager : MonoBehaviour
{
	public static FloatingUIObjectManager Instance { get; private set; }

	[SerializeField] private FloatingResourceBars floatingResourceBarsPrefab;
	[SerializeField] private int resourceBarsPoolSize = 20;
	[SerializeField] private bool fixedResourceBarsPoolSize = false;

	public FloatingNumberData floatingNumberData;
	public FloatingTextData floatingTextData;
	public Transform worldSpaceCanvas;

	private Transform transformCache;
	private Material damageNumberMaterial; 
	private Material healNumberMaterial;
	private Material staminaNumberMaterial;
	private Material manaNumberMaterial;

	private Updater<FloatingUIObject> updater;

	private ObjectPool<FloatingNumber> numberPool;
	// private ObjectPool<FloatingResourceBar> energyBarPool;
	private ObjectPool<FloatingText> textPool;

	private ObjectPool<FloatingResourceBars> resourceBarsPool;

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

		if (floatingNumberData == null || floatingTextData == null)
		{
			Debug.LogError("FloatingUIObjectManager: One or more FloatingUIObjectData assets are not assigned.");
			enabled = false;
			return;
		}

		transformCache = transform;

		updater = UpdaterManager.Instance.GetUpdater<FloatingUIObject>();

		// energyBarPool = new(() => FloatingResourceBar.Create(worldSpaceCanvas, floatingEnergyBarData), floatingEnergyBarData.poolSize);
		numberPool = new(() => FloatingNumber.Create(transformCache, floatingNumberData), floatingNumberData.poolSize, floatingNumberData.fixedPoolSize);
		textPool = new(() => FloatingText.Create(transformCache, floatingTextData), floatingTextData.poolSize, floatingTextData.fixedPoolSize);

		resourceBarsPool = new(CreateResourceBars, resourceBarsPoolSize, fixedResourceBarsPoolSize);

		damageNumberMaterial = CreateFontMaterial(floatingNumberData.tmpFont, floatingNumberData.damageColor.outlineColor);
		healNumberMaterial = CreateFontMaterial(floatingNumberData.tmpFont, floatingNumberData.healColor.outlineColor);
		staminaNumberMaterial = CreateFontMaterial(floatingNumberData.tmpFont, floatingNumberData.staminaColor.outlineColor);
		manaNumberMaterial = CreateFontMaterial(floatingNumberData.tmpFont, floatingNumberData.manaColor.outlineColor);
	}

	public void Register(FloatingUIObject obj)
	{
		if (obj == null) return;

		updater.Register(obj);
	}

	public void Return(FloatingUIObject obj)
	{
		if (obj == null) return;

		obj.Enable(false);
		if (obj is FloatingNumber number)
		{
			numberPool.Return(number);
		}
		// else if (obj is FloatingResourceBar bar)
		// {
		// 	energyBarPool.Return(bar);
		// }
		else if (obj is FloatingText text)
		{
			textPool.Return(text);
		}
		else if (obj is FloatingResourceBars bars)
		{
			resourceBarsPool.Return(bars);
		}
		else
		{
			Debug.LogWarning("FloatingUIObjectManager: Attempted to return an unknown FloatingUIObject type to pool.");
		}
	}

	public void ActivateDamageNumber(float number, Vector3 location, bool isCrit = false)
	{
		FloatingNumber num = numberPool.Get();
		if (num == null) return;
		num.Activate(number, location, floatingNumberData.damageColor, damageNumberMaterial, isCrit);
	}

	public void ActivateHealNumber(float number, Vector3 location, bool isCrit = false)
	{
		FloatingNumber num = numberPool.Get();
		if (num == null) return;
		num.Activate(number, location, floatingNumberData.healColor, healNumberMaterial, isCrit);
	}

	public void ActivateStaminaNumber(float number, Vector3 location, bool isCrit = false)
	{
		FloatingNumber num = numberPool.Get();
		if (num == null) return;
		num.Activate(number, location, floatingNumberData.staminaColor, staminaNumberMaterial, isCrit);
	}

	public void ActivateManaNumber(float number, Vector3 location, bool isCrit = false)
	{
		FloatingNumber num = numberPool.Get();
		if (num == null) return;
		num.Activate(number, location, floatingNumberData.manaColor, manaNumberMaterial, isCrit);
	}

	// public FloatingResourceBar GetHealthBar(Transform targetTransform, ResourceModule healthModule)
	// {
	// 	FloatingResourceBar bar = energyBarPool.Get();
	// 	if (bar == null) return null;
	// 	bar.Activate(floatingEnergyBarData.healthBarSettings, targetTransform, healthModule);
	// 	return bar;
	// }

	// public FloatingResourceBar GetStaminaBar(Transform targetTransform, ResourceModule staminaModule)
	// {
	// 	FloatingResourceBar bar = energyBarPool.Get();
	// 	if (bar == null) return null;
	// 	bar.Activate(floatingEnergyBarData.staminaBarSettings, targetTransform, staminaModule);
	// 	return bar;
	// }

	// public FloatingResourceBar GetManaBar(Transform targetTransform, ResourceModule manaModule)
	// {
	// 	FloatingResourceBar bar = energyBarPool.Get();
	// 	if (bar == null) return null;
	// 	bar.Activate(floatingEnergyBarData.manaBarSettings, targetTransform, manaModule);
	// 	return bar;
	// }

	public FloatingResourceBars GetResourceBars()
	{
		return resourceBarsPool.Get();
	}

	public void ReturnResourceBars(FloatingResourceBars bars)
	{
		if (bars == null) return;
		bars.SetTarget(null);
		resourceBarsPool.Return(bars);
	}

	public FloatingText GetText(Transform targetTransform, string text, Item.Rarity rarity = Item.Rarity.Common)
	{
		FloatingText floatingText = textPool.Get();
		if (floatingText == null) return null;
		floatingText.Activate(targetTransform, text, UIManager.Instance.GetTextMaterial(rarity));
		return floatingText;
	}

	private static Material CreateFontMaterial(TMP_FontAsset font, Color outlineColor)
	{
		Material mat = new(font.material);
		mat.SetColor("_OutlineColor", outlineColor);
		return mat;
	}

	private FloatingResourceBars CreateResourceBars()
	{
		FloatingResourceBars bars = Instantiate(floatingResourceBarsPrefab, worldSpaceCanvas);
		bars.Initialize();
		return bars;
	}
}