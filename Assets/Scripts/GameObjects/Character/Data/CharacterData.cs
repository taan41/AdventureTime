using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Data/Character/Data", order = 0)]
public class CharacterData : CustomSO<CharacterData>
{
	#region Properties & Fields
	[Header("Character Info")]
	public string characterName = "New Character";
	public string description = "Character Description";
	public Sprite icon;
	public Sprite defaultSprite;
	public Character.CharacterTag tag = Character.CharacterTag.Hero;

	[Header("Character Scriptable Data")]
	public CharacterDataActions actions;
	public CharacterDataProperties properties;
	public CharacterDataSettings settings;
	public ComponentData componentData;
	#endregion

	#region Functions
	public override bool IsValid()
	{
		if (actions == null) return false;
		if (properties == null) return false;
		if (settings == null) return false;
		if (componentData == null) return false;

		return true;
	}

	public override void CopyFrom(CharacterData other)
	{
		if (other == null) return;

		characterName = other.characterName;
		description = other.description;
		icon = other.icon;
		defaultSprite = other.defaultSprite;

		actions = other.actions;
		properties = other.properties;
		settings = other.settings;
		componentData = other.componentData;

		copyFrom = null;
		SignalDataChange();
	}
	#endregion
}