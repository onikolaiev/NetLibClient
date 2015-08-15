using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(RnMUI_SpellSlot))]
public class RnMUI_SpellSlotInspector : RnMUI_IconSlotInspector
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUIUtility.labelWidth = 120f;
		RnMUI_SpellSlot mSlot = target as RnMUI_SpellSlot;

		if (mSlot.GetInternalType() == RnMUI_IconSlot.InternalType.Normal)
		{
			EditorGUILayout.Space();

			RnMUI_SlotCooldown cooldown = EditorGUILayout.ObjectField("Cooldown Handle", mSlot.cooldownHandle, typeof(RnMUI_SlotCooldown), true) as RnMUI_SlotCooldown;
			
			if (mSlot.cooldownHandle != cooldown)
				mSlot.cooldownHandle = cooldown;

			DrawEvents();
		}
	}

	public void DrawEvents()
	{
		RnMUI_SpellSlot mSlot = target as RnMUI_SpellSlot;
	
		EditorGUIUtility.labelWidth = 100f;
		
		NGUIEditorTools.DrawEvents("On Assign", mSlot, mSlot.onAssign);
		NGUIEditorTools.DrawEvents("On Unassign", mSlot, mSlot.onUnassign);
	}
}

