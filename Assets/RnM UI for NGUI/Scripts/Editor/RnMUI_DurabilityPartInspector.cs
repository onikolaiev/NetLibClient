using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(RnMUI_DurabilityPart))]
public class RnMUI_DurabilityPartInspector : Editor {

	private RnMUI_DurabilityPart part;
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		NGUIEditorTools.SetLabelWidth(100f);
		part = target as RnMUI_DurabilityPart;
		
		NGUIEditorTools.DrawProperty("Target Sprite", serializedObject, "targetSprite");
		
		if (part.targetSprite != null)
		{
			if (NGUIEditorTools.DrawHeader("Sprites"))
			{
				NGUIEditorTools.BeginContents();
				EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
				{
					SerializedObject obj = new SerializedObject(part.targetSprite);
					obj.Update();
					SerializedProperty atlas = obj.FindProperty("mAtlas");
					NGUIEditorTools.DrawSpriteField("Normal", obj, atlas, obj.FindProperty("mSpriteName"));
					obj.ApplyModifiedProperties();
					
					NGUIEditorTools.DrawSpriteField("Hover", serializedObject, atlas, serializedObject.FindProperty("hoverSprite"), true);
				}
				EditorGUI.EndDisabledGroup();
				NGUIEditorTools.EndContents();
			}
		}
		
		serializedObject.ApplyModifiedProperties();
	}
}
