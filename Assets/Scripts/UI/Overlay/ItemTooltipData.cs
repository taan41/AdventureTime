using UnityEngine;

[CreateAssetMenu(fileName = "Item Tooltip Data", menuName = "Data/UI/Item Tooltip Manager Data")]
public class ItemTooltipData : ScriptableObject
{
	public ItemTooltip tooltipPrefab;
	public Vector3 tooltipOffset;
	public Color positiveStatColor;
	public Color negativeStatColor;
}