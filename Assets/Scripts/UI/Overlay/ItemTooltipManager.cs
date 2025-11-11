using UnityEngine;

public class ItemTooltipManager : MonoBehaviour
{
	public static ItemTooltipManager Instance { get; private set; } = null;

	[SerializeField] private ItemTooltipData initialData = null;
	public ItemTooltipData Data { get; private set; }

	private InputManager InputManagerRef => InputManager.Instance;
	private ItemData currentDataRef;
	private ItemData compareDataRef;
	private bool requireShift = false;

	private ItemTooltip itemTooltip = null;

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

		InputManagerRef.OnControlInputFlagChanged += RefreshTooltip;

		if (initialData != null)
		{
			SetData(initialData);
		}
	}

	public void SetData(ItemTooltipData data)
	{
		if (data == Data) return;

		Data = data;

		if (Data == null) return;

		if (itemTooltip != null)
		{
			Destroy(itemTooltip.gameObject);
		}
		itemTooltip = Instantiate(Data.tooltipPrefab);
		itemTooltip.Setup(
			Data.tooltipOffset,
			Data.positiveStatColor,
			Data.negativeStatColor)
		;
		itemTooltip.Enable(false);

		RefreshTooltip();
	}

	public void Show(ItemData currentData, ItemData compareData = null, bool requireShiftForComparison = false)
	{
		currentDataRef = currentData;
		compareDataRef = compareData;
		requireShift = requireShiftForComparison;

		RefreshTooltip();
	}

	public void RefreshTooltip()
	{
		bool shiftHeld = (InputManagerRef.ControlInputFlag & InputManager.ControlInputFlags.Shift) != 0;

		if ((!requireShift || shiftHeld) && compareDataRef != null)
		{
			itemTooltip.Show(currentDataRef, compareDataRef);
			return;
		}

		itemTooltip.Show(currentDataRef);
	}
}