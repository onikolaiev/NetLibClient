using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(RnMUI_Tab))]
public class RnMUI_TabInspector : Editor
{
	RnMUI_Tab mTab;
	
	public override void OnInspectorGUI()
	{
		EditorGUIUtility.labelWidth = 100f;
		mTab = target as RnMUI_Tab;

		serializedObject.Update();

		EditorGUILayout.Space();
		NGUIEditorTools.DrawProperty("Target Content", serializedObject, "targetContent");
		EditorGUILayout.Space();
		NGUIEditorTools.DrawProperty("Link With Tab", serializedObject, "linkWith");
		EditorGUILayout.Space();

		if (NGUIEditorTools.DrawHeader("Tab Label"))
		{
			NGUIEditorTools.BeginContents();
			EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
			{
				NGUIEditorTools.DrawProperty("Label", serializedObject, "tabLabel");

				if (mTab.tabLabel != null)
				{
					EditorGUILayout.Space();

					SerializedObject obj = new SerializedObject(mTab.tabLabel);
					obj.Update();
					NGUIEditorTools.DrawProperty("Color Inactive", obj, "mColor");
					obj.ApplyModifiedProperties();

					NGUIEditorTools.DrawProperty("Color Active", serializedObject, "activeLabelColor");
					NGUIEditorTools.DrawProperty("Color Hovered", serializedObject, "hoverLabelColor");
				}
			}
			EditorGUI.EndDisabledGroup();
			NGUIEditorTools.EndContents();
		}

		serializedObject.ApplyModifiedProperties();
	}
}