using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("RPG and MMO UI/Icon Slot")]
public class RnMUI_IconSlot : UIDragDropItem
{
	public enum InternalType
	{
		Normal,
		Temporary,
	}
	
	public enum HoverEffectType
	{
		None,
		Color,
		Sprite,
	}
	
	public enum PressEffectType
	{
		None,
		Color,
		Sprite,
	}
	
	protected InternalType internalType = InternalType.Normal;
	public InternalType GetInternalType() { return this.internalType; }

	public UITexture iconSprite;
	public bool dragAndDropEnabled = true;
	public bool IsStatic = false;
	public bool AllowThrowAway = true;

	public HoverEffectType hoverEffectType = HoverEffectType.None;
	public UISprite hoverEffectSprite;
	public Color hoverEffectColor = Color.white;
	public float hoverEffectSpeed = 0.15f;
	
	public PressEffectType pressEffectType = PressEffectType.None;
	public UISprite pressEffectSprite;
	public Color pressEffectColor = Color.black;
	public float pressEffectSpeed = 0.15f;
	public bool pressEffectInstaOut = true;
	private bool IsPressed = false;

	private Object sourceSlot;

	protected override void Start()
	{
		this.mTrans = this.transform;
		this.mCollider = this.GetComponent<Collider>();
		
		if (this.iconSprite == null)
			this.iconSprite = this.GetComponentInChildren<UITexture>();
		
		// Check if we have no icon
		if (this.iconSprite == null)
		{
			Debug.LogWarning(this.GetType() + " requires that you define a icon sprite in order to work.", this);
			this.enabled = false;
			return;
		}
		
		if (this.internalType == InternalType.Normal)
		{
			// Hide the hover and press effects sprites if assigned
			if (this.hoverEffectSprite != null)
				this.hoverEffectSprite.alpha = 0f;
			
			if (this.pressEffectSprite != null)
				this.pressEffectSprite.alpha = 0f;
		}

		// Call the on start
		this.OnStart();
	}

	/// <summary>
	/// Raises the start event.
	/// </summary>
	public virtual void OnStart() { }

	/// <summary>
	/// Sets the source slot, used by the temporary slot.
	/// </summary>
	/// <param name="slot">Slot.</param>
	public void SetSource(Object slot)
	{
		this.sourceSlot = slot;
	}
	
	/// <summary>
	/// Gets the source slot.
	/// </summary>
	/// <returns>The source.</returns>
	public RnMUI_IconSlot GetSource()
	{
		return this.sourceSlot as RnMUI_IconSlot;
	}
	
	/// <summary>
	/// Determines whether this slot is assigned.
	/// </summary>
	/// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
	public virtual bool IsAssigned()
	{
		return (this.GetIcon() != null);
	}
	
	/// <summary>
	/// Assign the specified slot by icon.
	/// </summary>
	/// <param name="icon">Icon.</param>
	public bool Assign(Texture icon)
	{
		if (icon == null)
			return false;
		
		// Unassign this slot
		this.Unassign();

		// Set the icon
		this.SetIcon(icon);

		return true;
	}

	/// <summary>
	/// Assign the specified slot by source object.
	/// </summary>
	/// <param name="source">Source.</param>
	public virtual bool Assign(Object source)
	{
		if (source is Texture)
		{
			return this.Assign(source as Texture);
		}
		else if (source is RnMUI_IconSlot)
		{
			RnMUI_IconSlot sourceSlot = source as RnMUI_IconSlot;

			if (sourceSlot != null)
				return this.Assign(sourceSlot.GetIcon());
		}

		return false;
	}
	
	/// <summary>
	/// Unassign this slot.
	/// </summary>
	public virtual void Unassign()
	{
		// Remove the icon
		this.SetIcon(null);
	}

	/// <summary>
	/// Gets the icon of this slot if it's set.
	/// </summary>
	/// <returns>The icon.</returns>
	public Texture GetIcon()
	{
		return this.iconSprite.mainTexture;
	}
	
	/// <summary>
	/// Sets the icon of this slot.
	/// </summary>
	/// <param name="iconTex">The icon texture.</param>
	public void SetIcon(Texture iconTex)
	{
		this.iconSprite.mainTexture = iconTex;
		
		if (iconTex != null && !this.iconSprite.gameObject.activeSelf) this.iconSprite.gameObject.SetActive(true);
		if (iconTex == null && this.iconSprite.gameObject.activeSelf) this.iconSprite.gameObject.SetActive(false);
	}
	
	public void OnHover(bool isOver)
	{
		if (this.hoverEffectType == HoverEffectType.Sprite)
		{
			if (this.hoverEffectSprite != null)
				TweenAlpha.Begin(this.hoverEffectSprite.gameObject, this.hoverEffectSpeed, ((isOver && !this.IsPressed) ? 1f : 0f));
		}
		else if (this.hoverEffectType == HoverEffectType.Color)
		{
			if (this.IsAssigned())
				TweenColor.Begin(this.iconSprite.gameObject, this.hoverEffectSpeed, ((isOver && !this.IsPressed) ? this.hoverEffectColor : Color.white));
		}
	}
	
	public void OnPress(bool isDown)
	{
		if (this.pressEffectType == PressEffectType.Sprite)
		{
			if (this.pressEffectSprite != null)
				TweenAlpha.Begin(this.pressEffectSprite.gameObject, ((this.pressEffectInstaOut) ? 0f : this.pressEffectSpeed), (isDown ? 1f : 0f));
		}
		else if (this.pressEffectType == PressEffectType.Color)
		{
			if (this.IsAssigned())
				TweenColor.Begin(this.iconSprite.gameObject, ((!isDown && this.pressEffectInstaOut) ? 0f : this.pressEffectSpeed), (isDown ? this.pressEffectColor : Color.white));
		}
		
		this.IsPressed = isDown;
		this.OnHover(((isDown) ? false : UICamera.IsHighlighted(this.gameObject)));
	}

	/// <summary>
	/// Raises the click event.
	/// </summary>
	public virtual void OnClick() { }

	/// <summary>
	/// Raises the tooltip event.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show.</param>
	public virtual void OnTooltip(bool show) { }

	protected override void OnDragDropStart()
	{
		if (!this.enabled || !this.IsAssigned() || !this.dragAndDropEnabled)
			return;
		
		if (this.internalType == InternalType.Temporary)
		{
			base.OnDragDropStart();
		}
		else
		{
			if (this.mTouchID != int.MinValue) return;
			
			// Disable the hover and pressed states
			this.OnPress(false);
			this.OnHover(false);
			
			// Create and get temporary slot
			GameObject icon = this.CreateTemporary();
			
			// Prepare the temporary slot
			this.AttachToTemporary(icon);

			// Unassign this slot
			if (!this.IsStatic)
				this.Unassign();
			
			// Make some internal calls
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
			
			UICamera.currentTouch.pressed = icon;
			UICamera.currentTouch.dragged = icon;
		}
	}

	/// <summary>
	/// Determines whether this slot can swap with the specified target slot.
	/// </summary>
	/// <returns><c>true</c> if this instance can swap with the specified target; otherwise, <c>false</c>.</returns>
	/// <param name="target">Target.</param>
	public virtual bool CanSwapWith(Object target)
	{
		return (target is RnMUI_IconSlot);
	}

	protected override void OnDragDropRelease(GameObject surface)
	{
		if (surface != null)
		{
			RnMUI_IconSlot targetSlot = surface.GetComponent<RnMUI_IconSlot>();

			if (targetSlot != null)
			{
				if (!targetSlot.dragAndDropEnabled)
				{
					// Return the spell to the source
					if (this.GetSource() != null)
						this.GetSource().Assign(this);
				}
				else
				{
					bool assignSuccess = false;

					// Normal empty slot assignment
					if (!targetSlot.IsAssigned())
					{
						// Assign the target slot with the info from this one
						assignSuccess = targetSlot.Assign(this);
					}
					// The target slot is assigned
					else
					{
						// If the target slot is not static
						// and we have a source slot that is not static
						if (!targetSlot.IsStatic && this.GetSource() != null && !this.GetSource().IsStatic)
						{
							// Check if we can swap
							if (targetSlot.CanSwapWith(this.GetSource()))
							{
								// Swap the slots
								this.GetSource().Assign(targetSlot);
								assignSuccess = targetSlot.Assign(this);
							}
							// Cannot be swapped
							else
							{
								// Return the dragged slot to it's source
								assignSuccess = this.GetSource().Assign(this);
							}
						}
						// If the target slot is static
						// and we have a source slot that is not a static one
						else if (targetSlot.IsStatic && this.GetSource() != null && !this.GetSource().IsStatic)
						{
							// Return the dragged slot to it's source
							assignSuccess = this.GetSource().Assign(this);
						}
						// If the target slot is not static
						// and the source slot is a static one
						else if (!targetSlot.IsStatic && this.GetSource() != null && this.GetSource().IsStatic)
						{
							targetSlot.Unassign();
							assignSuccess = targetSlot.Assign(this);
						}
					}

					// If assigning to the new slot failed, return to the source if any
					if (!assignSuccess && this.GetSource() != null && !this.GetSource().IsStatic)
						this.GetSource().Assign(this);
				}
			}
			else
			{
				// No target slot found
				// Check if throwing away is disabled and if so return the slot to it's source
				if (!this.AllowThrowAway && this.GetSource() != null)
				{
					this.GetSource().Assign(this);
					this.GetSource().OnThrowAwayDenied();
				}
			}
		}
		else
		{
			// No surface found
			// Check if throwing away is disabled and if so return the slot to it's source
			if (!this.AllowThrowAway && this.GetSource() != null)
			{
				this.GetSource().Assign(this);
				this.GetSource().OnThrowAwayDenied();
			}
		}
		
		// Destroy this one as it's no longer needed
		if (this.internalType == InternalType.Temporary && this.OnThrowAway())
			NGUITools.Destroy(this.transform.parent.gameObject);
	}

	/// <summary>
	/// This method is raised to confirm throwing away the slot.
	/// </summary>
	protected virtual bool OnThrowAway()
	{
		return true;
	}

	/// <summary>
	/// This method is raised when the slot is denied to be thrown away and returned to it's source.
	/// </summary>
	protected virtual void OnThrowAwayDenied()
	{
	}

	protected virtual GameObject CreateTemporary()
	{
		// Create temporary panel
		GameObject panelObj = new GameObject("_TemporaryPanel");
		panelObj.layer = this.gameObject.layer;
		panelObj.transform.parent = NGUITools.GetRoot(this.gameObject).transform;
		panelObj.transform.localScale = Vector3.one;
		panelObj.transform.localRotation = Quaternion.identity;
		panelObj.transform.localPosition = Vector3.one;
		
		// Apply depth
		UIPanel panel = panelObj.AddComponent<UIPanel>();
		panel.depth = UIPanel.nextUnusedDepth;
		
		// Create temporary icon
		GameObject icon = new GameObject("_TemporaryIcon");
		icon.layer = this.gameObject.layer;
		icon.transform.parent = panelObj.transform;
		
		// Fix it's position
		icon.transform.position = NGUITools.FindCameraForLayer(this.gameObject.layer).ScreenToWorldPoint(Input.mousePosition);
		icon.transform.rotation = this.transform.rotation;
		icon.transform.localScale = this.transform.localScale;
		
		// Add the UITexture component
		UITexture tex = icon.AddComponent<UITexture>();
		tex.width = this.iconSprite.width;
		tex.height = this.iconSprite.height;
		tex.depth = NGUITools.CalculateNextDepth(icon);
		
		return icon;
	}

	/// <summary>
	/// Attachs this slot to a temporary one (Cloning).
	/// </summary>
	/// <param name="temporary">Temporary.</param>
	protected virtual void AttachToTemporary(GameObject temporary)
	{
		// Add the slot component
        RnMUI_IconSlot slot = temporary.AddComponent<RnMUI_IconSlot>();
		slot.internalType = InternalType.Temporary;
		slot.AllowThrowAway = this.AllowThrowAway;
		slot.Start();
		slot.Assign(this);
		slot.SetSource(this);
		slot.OnDragDropStart();
	}
}

