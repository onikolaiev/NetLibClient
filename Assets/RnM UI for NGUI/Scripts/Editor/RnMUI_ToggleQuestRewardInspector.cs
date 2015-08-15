using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(RnMUI_ToggleQuestReward))]
public class RnMUI_ToggleQuestRewardInspector : Editor {

	private RnMUI_ToggleQuestReward toggle;

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		NGUIEditorTools.SetLabelWidth(100f);
		toggle = target as RnMUI_ToggleQuestReward;

		NGUIEditorTools.DrawProperty("Target", serializedObject, "target");

		if (toggle.target != null)
		{
			if (NGUIEditorTools.DrawHeader("Sprites"))
			{
				NGUIEditorTools.BeginContents();
				EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
				{
					SerializedObject obj = new SerializedObject(toggle.target);
					obj.Update();
					SerializedProperty atlas = obj.FindProperty("mAtlas");
					NGUIEditorTools.DrawSpriteField("Normal", obj, atlas, obj.FindProperty("mSpriteName"));
					obj.ApplyModifiedProperties();

					NGUIEditorTools.DrawSpriteField("Selected", serializedObject, atlas, serializedObject.FindProperty("activeSprite"), true);
				}
				EditorGUI.EndDisabledGroup();
				NGUIEditorTools.EndContents();
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}
