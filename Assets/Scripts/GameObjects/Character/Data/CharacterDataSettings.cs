using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterSettings", menuName = "Data/Character/Settings")]
public class CharacterDataSettings : ScriptableObject
{
	public Character.CharacterRule rules = new();
	public Character.CharacterConfiguration configs = new();
	public Character.CharacterTradingSetting trading = new();
	public Character.CharacterAISetting ai = new();
	public IInteractable.InteractableRule interactable = new();
}