using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(RnMUI_EquipSlot))]
public class RnMUI_EquipSlotInspector : RnMUI_IconSlotInspector
{
	public override void OnInspectorGUI()
	{
		EditorGUIUtility.labelWidth = 120f;
		EditorGUILayout.Space();

		RnMUI_EquipSlot slot = target as RnMUI_EquipSlot;

		UIEquipmentType eType = (UIEquipmentType)EditorGUILayout.EnumPopup("Equip Type", slot.equipType);

		if (eType != slot.equipType)
			slot.equipType = eType;

		DrawTargetSprite();
		//DrawDragAndDrop();
		DrawBehaviour();
	}
}

