using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("RPG and MMO UI/Equip Slot")]
public class RnMUI_EquipSlot : RnMUI_IconSlot
{
	public UIEquipmentType equipType = UIEquipmentType.None;

	private UIItemInfo itemInfo;
	
	public override void OnStart()
	{
		this.dragAndDropEnabled = true;
		this.AllowThrowAway = false;
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
			{
				// Check if the equipment type matches the target slot
				if (!this.CheckEquipType(sourceSlot.GetItemInfo()))
					return false;

				return this.Assign(sourceSlot.GetItemInfo());
			}
		}
		else if (source is RnMUI_EquipSlot)
		{
			RnMUI_EquipSlot sourceSlot = source as RnMUI_EquipSlot;
			
			if (sourceSlot != null)
			{
				// Check if the equipment type matches the target slot
				if (!this.CheckEquipType(sourceSlot.GetItemInfo()))
					return false;

				// Type matches
				return this.Assign(sourceSlot.GetItemInfo());
			}
		}
		
		// Default
		return false;
	}

	/// <summary>
	/// Checks if the given item can assigned to this slot.
	/// </summary>
	/// <returns><c>true</c>, if equip type was checked, <c>false</c> otherwise.</returns>
	/// <param name="info">Info.</param>
	public virtual bool CheckEquipType(UIItemInfo info)
	{
		if (this.internalType != InternalType.Temporary && info.EquipType != this.equipType)
			return false;

		return true;
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
	/// This method is raised when the slot is denied to be thrown away and returned to it's source.
	/// </summary>
	protected override void OnThrowAwayDenied()
	{
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

