using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("RPG and MMO UI/Spell Slot")]
public class RnMUI_SpellSlot : RnMUI_IconSlot
{
	public static RnMUI_SpellSlot current;

	public RnMUI_SlotCooldown cooldownHandle;
	private UISpellInfo spellInfo;

	/// <summary>
	/// Assign event listener.
	/// </summary>
	public List<EventDelegate> onAssign = new List<EventDelegate>();
	
	/// <summary>
	/// Unassign event listener.
	/// </summary>
	public List<EventDelegate> onUnassign = new List<EventDelegate>();

	public override void OnStart()
	{
		// Try finding the cooldown handler
		if (this.cooldownHandle == null) this.cooldownHandle = this.GetComponent<RnMUI_SlotCooldown>();
		if (this.cooldownHandle == null) this.cooldownHandle = this.GetComponentInChildren<RnMUI_SlotCooldown>();
	}

	/// <summary>
	/// Gets the spell info of the spell assigned to this slot.
	/// </summary>
	/// <returns>The spell info.</returns>
	public UISpellInfo GetSpellInfo()
	{
		return spellInfo;
	}

	/// <summary>
	/// Determines whether this slot is assigned.
	/// </summary>
	/// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
	public override bool IsAssigned()
	{
		return (this.spellInfo != null);
	}

	/// <summary>
	/// Assign the slot by spell info.
	/// </summary>
	/// <param name="spellInfo">Spell info.</param>
	public bool Assign(UISpellInfo spellInfo)
	{
		if (spellInfo == null)
			return false;

		// Use the base class assign
		if (this.Assign(spellInfo.Icon))
		{
			// Set the spell info
			this.spellInfo = spellInfo;
			
			// Check if we have a cooldown handler
			if (this.cooldownHandle != null)
				this.cooldownHandle.OnAssignSpell(spellInfo);

			// Execute the assign event listener
			this.ExecuteOnAssign();

			// Success
			return true;
		}

		return false;
	}

	/// <summary>
	/// Executes the on assign event listener.
	/// </summary>
	protected virtual void ExecuteOnAssign()
	{
		current = this;
		EventDelegate.Execute(this.onAssign);
		current = null;
	}

	/// <summary>
	/// Assign the slot by the passed source slot.
	/// </summary>
	/// <param name="source">Source.</param>
	public override bool Assign(Object source)
	{
		if (source is RnMUI_SpellSlot)
		{
			RnMUI_SpellSlot sourceSlot = source as RnMUI_SpellSlot;
			
			if (sourceSlot != null)
				return this.Assign(sourceSlot.GetSpellInfo());
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
		
		// Clear the spell info
		this.spellInfo = null;
		
		// Check if we have a cooldown handler
		if (this.cooldownHandle != null)
			this.cooldownHandle.OnUnassign();

		// Execute the unassign event listener
		this.ExecuteOnUnassign();
	}

	/// <summary>
	/// Executes the on unassign event listener.
	/// </summary>
	protected virtual void ExecuteOnUnassign()
	{
		current = this;
		EventDelegate.Execute(this.onUnassign);
		current = null;
	}

	/// <summary>
	/// Determines whether this slot can swap with the specified target slot.
	/// </summary>
	/// <returns><c>true</c> if this instance can swap with the specified target; otherwise, <c>false</c>.</returns>
	/// <param name="target">Target.</param>
	public override bool CanSwapWith(Object target)
	{
		return (target is RnMUI_SpellSlot);
	}

	/// <summary>
	/// Raises the click event.
	/// </summary>
	public override void OnClick()
	{
		if (!this.IsAssigned())
			return;
		
		// Check if the slot is on cooldown
		if (this.cooldownHandle != null)
		{
			if (this.cooldownHandle.IsOnCooldown)
				return;
			
			this.cooldownHandle.StartCooldown(this.spellInfo.Cooldown);
		}
	}

	/// <summary>
	/// Raises the tooltip event.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show.</param>
	public override void OnTooltip(bool show)
	{
		if (show && this.IsAssigned())
		{
			// Set the title and description
			RnMUI_Tooltip.SetTitle(this.spellInfo.Name);
			RnMUI_Tooltip.SetDescription(this.spellInfo.Description);
			
			if (this.spellInfo.Flags.Has(UISpellInfo_Flags.Passive))
			{
				RnMUI_Tooltip.AddAttribute("Passive", "");
			}
			else
			{
				// Power consumption
				if (this.spellInfo.PowerCost > 0f)
				{
					if (this.spellInfo.Flags.Has(UISpellInfo_Flags.PowerCostInPct))
						RnMUI_Tooltip.AddAttribute(this.spellInfo.PowerCost.ToString("0") + "%", " Energy");
					else
						RnMUI_Tooltip.AddAttribute(this.spellInfo.PowerCost.ToString("0"), " Energy");
				}
				
				// Range
				if (this.spellInfo.Range > 0f)
				{
					if (this.spellInfo.Range == 1f)
						RnMUI_Tooltip.AddAttribute("Melee range", "");
					else
						RnMUI_Tooltip.AddAttribute(this.spellInfo.Range.ToString("0"), " yd range");
				}
				
				// Cast time
				if (this.spellInfo.CastTime == 0f)
					RnMUI_Tooltip.AddAttribute("Instant", "");
				else
					RnMUI_Tooltip.AddAttribute(this.spellInfo.CastTime.ToString("0.0"), " sec cast");
				
				// Cooldown
				if (this.spellInfo.Cooldown > 0f)
					RnMUI_Tooltip.AddAttribute(this.spellInfo.Cooldown.ToString("0.0"), " sec cooldown");
			}
			
			// Set the tooltip position
			RnMUI_Tooltip.SetPosition(this.iconSprite as UIWidget);
			
			// Show the tooltip
			RnMUI_Tooltip.Show();
			
			// Prevent hide
			return;
		}
		
		// Default hide
		RnMUI_Tooltip.Hide();
	}
}

