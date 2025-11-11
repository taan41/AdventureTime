using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(CustomReadOnlyAttribute))]
public class CustomReadOnlyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		GUI.enabled = false; // disable editing
		EditorGUI.PropertyField(position, property, label);
		GUI.enabled = true; // enable editing
	}
}
#endif

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class CustomReadOnlyAttribute : PropertyAttribute { }