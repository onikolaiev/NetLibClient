using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("RPG and MMO UI/Item Slot")]
public class RnMUI_ItemSlot : RnMUI_IconSlot
{
	private UIItemInfo itemInfo;
	
	public override void OnStart()
	{
	}
	
	/// <summary>
	/// Gets the ItemInfo of the item assigned to this slot.
	/// </summary>
	/// <returns>The item info.</returns>
	public UIItemInfo GetItemInfo()
	{
		return itemInfo;
	}
	
	/// <summary>
	/// Determines whether this slot is assigned.
	/// </summary>
	/// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
	public override bool IsAssigned()
	{
		return (this.itemInfo != null);
	}
	
	/// <summary>
	/// Assign the slot by item info.
	/// </summary>
	/// <param name="itemInfo">Item info.</param>
	public bool Assign(UIItemInfo itemInfo)
	{
		if (itemInfo == null)
			return false;
		
		// Use the base class assign
		if (this.Assign(itemInfo.Icon))
		{
			// Set the item info
			this.itemInfo = itemInfo;

			// Success
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Assign the slot by the passed source slot.
	/// </summary>
	/// <param name="source">Source.</param>
	public override bool Assign(Object source)
	{
		if (source is RnMUI_ItemSlot)
		{
			RnMUI_ItemSlot sourceSlot = source as RnMUI_ItemSlot;
			
			if (sourceSlot != null)
				return this.Assign(sourceSlot.GetItemInfo());
		}
		else if (source is RnMUI_EquipSlot)
		{
			RnMUI_EquipSlot sourceSlot = source as RnMUI_EquipSlot;
			
			if (sourceSlot != null)
				return this.Assign(sourceSlot.GetItemInfo());
		}
		
		// Default
		return false;
	}
	
	/// <summary>
	/// Unassign this slot.
	/// </summary>
	public override void Unassign()
	{
		// Remove the icon
		base.Unassign();
		
		// Clear the item info
		this.itemInfo = null;
	}
	
	/// <summary>
	/// Determines whether this slot can swap with the specified target slot.
	/// </summary>
	/// <returns><c>true</c> if this instance can swap with the specified target; otherwise, <c>false</c>.</returns>
	/// <param name="target">Target.</param>
	public override bool CanSwapWith(Object target)
	{
		return ((target is RnMUI_ItemSlot) || (target is RnMUI_EquipSlot));
	}

	/// <summary>
	/// Raises the click event.
	/// </summary>
	public override void OnClick()
	{
		if (!this.IsAssigned())
			return;
	}

	/// <summary>
	/// Shows a tooltip with the given item info.
	/// </summary>
	/// <param name="info">Info.</param>
	/// <param name="widget">Widget.</param>
	public static void ShowTooltip(UIItemInfo info, UIWidget widget)
	{
		if (info == null)
			return;

		// Set the title and description
		RnMUI_Tooltip.SetTitle(info.Name);
		RnMUI_Tooltip.SetDescription(info.Description);
		
		// Item types
		RnMUI_Tooltip.AddAttribute(info.Type, "");
		RnMUI_Tooltip.AddAttribute(info.Subtype, "");
		
		if (info.ItemType == 1)
		{
			RnMUI_Tooltip.AddAttribute(info.Damage.ToString(), " Damage");
			RnMUI_Tooltip.AddAttribute(info.AttackSpeed.ToString("0.0"), " Attack speed");
			
			RnMUI_Tooltip.AddAttribute_SingleColumn("(" + ((float)info.Damage / info.AttackSpeed).ToString("0.0") + " damage per second)", "");
		}
		else
		{
			RnMUI_Tooltip.AddAttribute(info.Block.ToString(), " Block");
			RnMUI_Tooltip.AddAttribute(info.Armor.ToString(), " Armor");
		}
		
		RnMUI_Tooltip.AddAttribute_SingleColumn("", "+" + info.Stamina.ToString() + " Stamina", new RectOffset(0, 0, 7, 0));
		RnMUI_Tooltip.AddAttribute_SingleColumn("", "+" + info.Strength.ToString() + " Strength");
		
		// Set the tooltip position
		RnMUI_Tooltip.SetPosition(widget);
		
		// Show the tooltip
		RnMUI_Tooltip.Show();
	}
	
	/// <summary>
	/// Raises the tooltip event.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show.</param>
	public override void OnTooltip(bool show)
	{
		if (show && this.IsAssigned())
		{
			// Show the tooltip
			RnMUI_ItemSlot.ShowTooltip(this.itemInfo, this.iconSprite as UIWidget);
			
			// Prevent hide
			return;
		}
		
		// Default hide
		RnMUI_Tooltip.Hide();
	}
}

