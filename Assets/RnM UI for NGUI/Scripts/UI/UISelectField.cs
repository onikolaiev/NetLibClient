using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Select Field can be used to display pop-up menus and drop-down lists.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("RPG and MMO UI/Select Field")]
public class UISelectField : UIWidgetContainer
{
	/// <summary>
	/// Current select field. Only available during the OnSelectionChange event callback.
	/// </summary>
	static public UISelectField current;

	public enum Position
	{
		Auto,
		Above,
		Below,
	}

	public enum AnimationType
	{
		None,
		Fade,
		Scale,
	}

	public Position position = Position.Auto;
	private Position currentPosition = Position.Below;

	/// <summary>
	/// New line-delimited list of items.
	/// </summary>
	
	public List<string> items = new List<string>();

	public UIAtlas atlas;

	public UISprite backgroundElement;
	public UILabel labelElement;
	public UISprite arrowElement;

	public string fieldNormalSprite;
	public string fieldFocusedSprite;
	public Color fieldNormalColor = Color.white;
	public Color fieldFocusedColor = Color.white;

	public Color labelNormalColor = Color.white;
	public Color labelFocusedColor = Color.white;

	public string arrowClosedSprite;
	public string arrowClosedHoverSprite;
	public string arrowOpenedSprite;
	public string arrowOpenedHoverSprite;

	public string listBackgroundSprite;
	public string listHighlightSprite;
	public Color listBackgroundColor = Color.white;
	public Color listHighlightColor = new Color(225f / 255f, 200f / 255f, 150f / 255f, 1f);
	public Color listHighlightLabelColor = Color.white;

	public int listHLMarginLeft = 0;
	public int listHLMarginRight = 0;
	public int listHLMarginTop = 0;
	public int listHLMarginBottom = 0;

	public AnimationType listAnimation = AnimationType.None;
	public float listAnimationDuration = 0.15f;

	public bool animateHighlightPosition = true;
	public bool animateHighlightLabel = true;
	public float animateHighlightDuration = 0.15f;

	public Vector2 listOffset = Vector2.zero;

	public int listPaddingLeft = 0;
	public int listPaddingRight = 0;
	public int listPaddingTop = 0;
	public int listPaddingBottom = 0;

	public Vector2 listAdditionalSize = Vector2.zero;

	public string listSeparatorSprite;

	public int listSepMarginLeft = 0;
	public int listSepMarginRight = 0;
	public int listSepMarginTop = 0;
	public int listSepMarginBottom = 0;

	[HideInInspector][SerializeField] UIFont font; // Use 'bitmapFont' instead
	public UIFont bitmapFont;
	public Font trueTypeFont;

	public Object ambigiousFont
	{
		get
		{
			if (trueTypeFont != null) return trueTypeFont;
			if (bitmapFont != null) return bitmapFont;
			return font;
		}
		set
		{
			if (value is Font)
			{
				trueTypeFont = value as Font;
				bitmapFont = null;
				font = null;
			}
			else if (value is UIFont)
			{
				bitmapFont = value as UIFont;
				trueTypeFont = null;
				font = null;
			}
		}
	}

	public int fontSize = 16;
	public FontStyle fontStyle = FontStyle.Normal;
	public Vector2 padding = new Vector3(4f, 4f);
	public Color textColor = Color.white;
	public bool isLocalized = false;

	public UILabel.Effect labelEffectStyle = UILabel.Effect.None;
	public Color labelEffectColor = Color.black;
	public Vector2 labelEffectDistance = Vector2.one;

	/// <summary>
	/// Callbacks triggered when the select field gets a new item selection.
	/// </summary>
	
	public List<EventDelegate> onChange = new List<EventDelegate>();
	
	// Currently selected item
	[HideInInspector][SerializeField] string mSelectedItem;
	
	UIPanel mPanel;
	GameObject listRoot;
	UISprite mBackground;
	UISprite mHighlight;
	UILabel mHighlightedLabel = null;
	List<UILabel> mLabelList = new List<UILabel>();

	// This functionality is no longer needed as the same can be achieved by choosing a
	// OnValueChange notification targeting a label's SetCurrentSelection function.
	// If your code was list.textLabel = myLabel, change it to:
	// EventDelegate.Add(list.onChange, lbl.SetCurrentSelection);
	[HideInInspector][SerializeField] UILabel textLabel;

	/// <summary>
	/// Whether the popup list is currently open.
	/// </summary>
	
	public bool isOpen { get; private set; }

	/// <summary>
	/// Whether the mouse is over the Select Field.
	/// </summary>
	public bool isMouseOver { get; private set; }

	/// <summary>
	/// Current selection.
	/// </summary>
	
	public string value
	{
		get
		{
			return mSelectedItem;
		}
		set
		{
			mSelectedItem = value;

			if (mSelectedItem == null) return;

			#if UNITY_EDITOR
			if (!Application.isPlaying) return;
			#endif

			if (mSelectedItem != null)
				TriggerCallbacks();

			if (this.labelElement != null)
				this.labelElement.text = value;
		}
	}

	/// <summary>
	/// Whether the popup list will be handling keyboard, joystick and controller events.
	/// </summary>
	
	bool handleEvents
	{
		get
		{
			UIButtonKeys keys = GetComponent<UIButtonKeys>();
			return (keys == null || !keys.enabled);
		}
		set
		{
			UIButtonKeys keys = GetComponent<UIButtonKeys>();
			if (keys != null) keys.enabled = !value;
		}
	}
	
	/// <summary>
	/// Whether the popup list is actually usable.
	/// </summary>
	
	bool isValid { get { return bitmapFont != null || trueTypeFont != null; } }
	
	/// <summary>
	/// Active font size.
	/// </summary>
	
	int activeFontSize { get { return (trueTypeFont != null || bitmapFont == null) ? fontSize : bitmapFont.defaultSize; } }
	
	/// <summary>
	/// Font scale applied to the popup list's text.
	/// </summary>
	
	float activeFontScale { get { return (trueTypeFont != null || bitmapFont == null) ? 1f : (float)fontSize / bitmapFont.defaultSize; } }
	
	/// <summary>
	/// Trigger all event notification callbacks.
	/// </summary>
	
	protected void TriggerCallbacks()
	{
		current = this;

		if (EventDelegate.IsValid(onChange))
		{
			EventDelegate.Execute(onChange);
		}

		current = null;
	}
	
	/// <summary>
	/// Remove legacy functionality.
	/// </summary>
	
	void OnEnable()
	{
		// 'font' is no longer used
		if (font != null)
		{
			if (font.isDynamic)
			{
				trueTypeFont = font.dynamicFont;
				fontStyle = font.dynamicFontStyle;
				mUseDynamicFont = true;
			}
			else if (bitmapFont == null)
			{
				bitmapFont = font;
				mUseDynamicFont = false;
			}
			font = null;
		}
		
		// Auto-upgrade to the true type font
		if (trueTypeFont == null && bitmapFont != null && bitmapFont.isDynamic)
		{
			trueTypeFont = bitmapFont.dynamicFont;
			bitmapFont = null;
		}
	}
	
	bool mUseDynamicFont = false;
	
	void OnValidate()
	{
		Font ttf = trueTypeFont;
		UIFont fnt = bitmapFont;
		
		bitmapFont = null;
		trueTypeFont = null;
		
		if (ttf != null && (fnt == null || !mUseDynamicFont))
		{
			bitmapFont = null;
			trueTypeFont = ttf;
			mUseDynamicFont = true;
		}
		else if (fnt != null)
		{
			// Auto-upgrade from 3.0.2 and earlier
			if (fnt.isDynamic)
			{
				trueTypeFont = fnt.dynamicFont;
				fontStyle = fnt.dynamicFontStyle;
				fontSize = fnt.defaultSize;
				mUseDynamicFont = true;
			}
			else
			{
				bitmapFont = fnt;
				mUseDynamicFont = false;
			}
		}
		else
		{
			trueTypeFont = ttf;
			mUseDynamicFont = true;
		}
	}
	
	/// <summary>
	/// Send out the selection message on start.
	/// </summary>
	
	void Start()
	{
		// Auto-upgrade legacy functionality
		if (textLabel != null)
		{
			EventDelegate.Add(onChange, textLabel.SetCurrentSelection);
			textLabel = null;
			#if UNITY_EDITOR
			NGUITools.SetDirty(this);
			#endif
		}
		
		if (Application.isPlaying)
		{
			// Automatically choose the first item
			if (string.IsNullOrEmpty(mSelectedItem))
			{
				if (items.Count > 0) value = items[0];
			}
			else
			{
				string s = mSelectedItem;
				mSelectedItem = null;
				value = s;
			}
		}
	}
	
	/// <summary>
	/// Localize the text label.
	/// </summary>
	
	void OnLocalize() { if (isLocalized) TriggerCallbacks(); }
	
	/// <summary>
	/// Visibly highlight the specified transform by moving the highlight sprite to be over it.
	/// </summary>
	
	void Highlight(UILabel lbl, bool instant)
	{
		// Change the previous label color
		if (this.mHighlightedLabel != null)
		{
			if (instant || !this.animateHighlightLabel)
				this.mHighlightedLabel.color = this.textColor;
			else
				TweenColor.Begin(this.mHighlightedLabel.gameObject, this.animateHighlightDuration, this.textColor).method = UITweener.Method.EaseOut;
		}

		// Save the new highlighted label
		this.mHighlightedLabel = lbl;

		// Animate the new label highlight
		if (this.mHighlight != null)
		{
			// Don't allow highlighting while the label is animating to its intended position
			TweenPosition tp = lbl.GetComponent<TweenPosition>();
			if (tp != null && tp.enabled) return;

			// Calculate the height for the highlight based on the label
			int hlHeight = Mathf.RoundToInt(lbl.CalculateBounds().size.y + (this.padding.y * 2f) - (this.listHLMarginTop + this.listHLMarginBottom));

			if (instant || !this.animateHighlightPosition)
				this.mHighlight.height = hlHeight;
			else
				TweenHeight.Begin(this.mHighlight, this.animateHighlightDuration, hlHeight).method = UITweener.Method.EaseOut;

			// Calculate the highligh position
			Vector3 pos = lbl.cachedTransform.localPosition;
			pos -= new Vector3((this.listPaddingLeft + this.padding.x), 0f, 0f);
			pos += new Vector3(0f, this.padding.y, 0f);
			pos += new Vector3(this.listHLMarginLeft, -this.listHLMarginTop, 0f);

			if (instant || !this.animateHighlightPosition)
				this.mHighlight.cachedTransform.localPosition = pos;
			else
				TweenPosition.Begin(this.mHighlight.gameObject, this.animateHighlightDuration, pos).method = UITweener.Method.EaseOut;
		}

		// Change the label color
		if (instant || !this.animateHighlightLabel)
			lbl.color = this.listHighlightLabelColor;
		else
			TweenColor.Begin(lbl.gameObject, this.animateHighlightDuration, this.listHighlightLabelColor).method = UITweener.Method.EaseOut;
	}
	
	/// <summary>
	/// Event function triggered when the mouse hovers over an item.
	/// </summary>
	
	void OnItemHover(GameObject go, bool isOver)
	{
		if (isOver)
		{
			UILabel lbl = go.GetComponent<UILabel>();
			Highlight(lbl, false);
		}
	}
	
	/// <summary>
	/// Select the specified label.
	/// </summary>
	
	void Select(UILabel lbl, bool instant)
	{
		Highlight(lbl, instant);

		UIEventListener listener = lbl.gameObject.GetComponent<UIEventListener>();
		value = listener.parameter as string;
		
		UIPlaySound[] sounds = GetComponents<UIPlaySound>();
		
		for (int i = 0, imax = sounds.Length; i < imax; ++i)
		{
			UIPlaySound snd = sounds[i];
			
			if (snd.trigger == UIPlaySound.Trigger.OnClick)
			{
				NGUITools.PlaySound(snd.audioClip, snd.volume, 1f);
			}
		}
	}
	
	/// <summary>
	/// Event function triggered when the drop-down list item gets clicked on.
	/// </summary>
	
	void OnItemPress(GameObject go, bool isPressed) { if (isPressed) Select(go.GetComponent<UILabel>(), true); }
	
	/// <summary>
	/// React to key-based input.
	/// </summary>
	
	void OnKey(KeyCode key)
	{
		if (enabled && NGUITools.GetActive(gameObject) && handleEvents)
		{
			int index = mLabelList.IndexOf(mHighlightedLabel);
			if (index == -1) index = 0;
			
			if (key == KeyCode.UpArrow)
			{
				if (index > 0)
				{
					Select(mLabelList[--index], false);
				}
			}
			else if (key == KeyCode.DownArrow)
			{
				if (index + 1 < mLabelList.Count)
				{
					Select(mLabelList[++index], false);
				}
			}
			else if (key == KeyCode.Escape)
			{
				OnSelect(false);
			}
		}
	}
	
	/// <summary>
	/// Get rid of the popup dialog when the selection gets lost.
	/// </summary>
	
	void OnSelect(bool isSelected)
	{
		// Handle opening and closing of the list
		if (this.listRoot)
		{
			// Handle opening of the list
			if (isSelected)
			{
				this.isOpen = true;

				if (this.listAnimation == AnimationType.Fade)
				{
					TweenAlpha.Begin(this.listRoot, this.listAnimationDuration, 1f).method = UITweener.Method.EaseOut;
				}
				else if (this.listAnimation == AnimationType.Scale)
				{
					// Prepare the scale
					this.listRoot.transform.localScale = new Vector3(1f, 0f, 1f);

					// Set the panel alpha to 1
					UIPanel.Find(this.listRoot.transform).alpha = 1f;

					// Begin the animation
					TweenScale.Begin(this.listRoot, this.listAnimationDuration, Vector3.one).method = UITweener.Method.EaseOut;

					// Check the position of the list
					if (this.currentPosition == Position.Above)
					{
						Vector3 originalPos = this.listRoot.transform.localPosition;

						// Change the position of the list to the bottom of the list
						this.listRoot.transform.localPosition = new Vector3(originalPos.x, (originalPos.y - this.mBackground.height), originalPos.z);

						// Tween the position to math the scaling tween
						TweenPosition.Begin(this.listRoot, this.listAnimationDuration, originalPos).method = UITweener.Method.EaseOut;
					}
				}
			}
			else // Handle closing of the list
			{
				this.isOpen = false;

				if (this.listAnimation == AnimationType.Fade)
				{
					TweenAlpha tween = TweenAlpha.Begin(this.listRoot, this.listAnimationDuration, 0f);
					tween.onFinished.Add(new EventDelegate(Cleanup));
					tween.method = UITweener.Method.EaseIn;
				}
				else if (this.listAnimation == AnimationType.Scale)
				{
					TweenScale tween = TweenScale.Begin(this.listRoot, this.listAnimationDuration, new Vector3(1f, 0f, 1f));
					tween.onFinished.Add(new EventDelegate(Cleanup));
					tween.method = UITweener.Method.EaseIn;

					// Check the position of the list
					if (this.currentPosition == Position.Above)
					{
						Vector3 originalPos = this.listRoot.transform.localPosition;

						// Tween the position to math the scaling tween
						TweenPosition.Begin(this.listRoot, this.listAnimationDuration, new Vector3(originalPos.x, (originalPos.y - this.mBackground.height), originalPos.z)).method = UITweener.Method.EaseIn;
					}
				}
				else
				{
					this.Cleanup();
				}
			}
		}

		if (this.backgroundElement != null)
		{
			this.backgroundElement.spriteName = (isSelected) ? this.fieldFocusedSprite : this.fieldNormalSprite;
			this.backgroundElement.color = (isSelected) ? this.fieldFocusedColor : this.fieldNormalColor;
			this.backgroundElement.MakePixelPerfect();
		}

		if (this.labelElement != null)
			this.labelElement.color = (isSelected) ? this.labelFocusedColor : this.labelNormalColor;

		this.UpdateArrowSprite();
	}

	private void Cleanup()
	{
		this.mLabelList.Clear();
		this.handleEvents = false;
		
		Destroy(this.listRoot);
		
		this.mBackground = null;
		this.mHighlight = null;
		this.listRoot = null;
	}

	/// <summary>
	/// Display the drop-down list when the game object gets clicked on.
	/// </summary>
	
	void OnClick()
	{
		if (this.enabled && NGUITools.GetActive(this.gameObject) && this.listRoot == null && this.atlas != null && this.isValid && this.items.Count > 0)
		{
			// Create the dropdown list
			this.CreateList();

			// Trigger the OnSelect
			this.OnSelect(true);

			// Handle Animations
		}
		else
			OnSelect(false);
	}

	/// <summary>
	/// Raises the hover event.
	/// </summary>
	/// <param name="isOver">If set to <c>true</c> is over.</param>
	void OnHover(bool isOver)
	{
		this.isMouseOver = isOver;
		this.UpdateArrowSprite();
	}

	public void UpdateArrowSprite()
	{
		if (this.arrowElement != null)
		{
			if (this.isOpen)
			{
				if (!string.IsNullOrEmpty(this.arrowOpenedSprite) && !string.IsNullOrEmpty(this.arrowOpenedHoverSprite))
					this.arrowElement.spriteName = (this.isMouseOver) ? this.arrowOpenedHoverSprite : this.arrowOpenedSprite;
			}
			else
			{
				if (!string.IsNullOrEmpty(this.arrowClosedSprite) && !string.IsNullOrEmpty(this.arrowClosedHoverSprite))
					this.arrowElement.spriteName = (this.isMouseOver) ? this.arrowClosedHoverSprite : this.arrowClosedSprite;
			}
			
			this.arrowElement.MakePixelPerfect();
		}
	}

	/// <summary>
	/// Creates the dropdown list with all it's options.
	/// </summary>
	/// 
	private void CreateList()
	{
		// Clear the labels list
		this.mLabelList.Clear();
		
		// Automatically locate the panel responsible for this object
		if (this.mPanel == null)
		{
			this.mPanel = UIPanel.Find(this.transform);
			if (this.mPanel == null) return;
		}
		
		// Disable the navigation script
		this.handleEvents = true;
		
		// Calculate the dimensions of the object triggering the popup list so we can position it below it
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.transform.parent, this.transform);
		
		// Create the root object for the list
		this.listRoot = this.CreateListRoot();
		
		// Add a sprite for the background
		this.mBackground = this.CreateBackground();
		
		// Background width
		this.mBackground.width = Mathf.RoundToInt(bounds.size.x + this.listAdditionalSize.x);
		
		// Starting background height
		int bgHeight = Mathf.RoundToInt(this.listAdditionalSize.y + this.listPaddingTop + this.listPaddingBottom);
		
		// Starting offsets
		float offsetX = this.listPaddingLeft;
		float offsetY = 0f - this.listPaddingTop;
		
		// Calculate the max width for a label
		int labelWidth = Mathf.RoundToInt(this.mBackground.width - (this.listPaddingLeft + this.listPaddingRight) - (this.padding.x * 2f));
		
		// Run through all items and create labels for each one
		for (int i = 0, imax = this.items.Count; i < imax; ++i)
		{
			string s = this.items[i];
			
			if (string.IsNullOrEmpty(s))
				continue;
			
			// Create the label
			UILabel label = NGUITools.AddWidget<UILabel>(this.listRoot);
			label.pivot = UIWidget.Pivot.TopLeft;
			label.bitmapFont = this.bitmapFont;
			label.trueTypeFont = this.trueTypeFont;
			label.fontSize = this.fontSize;
			label.fontStyle = fontStyle;
			label.text = this.isLocalized ? Localization.Get(s) : s;
			label.color = this.textColor;
			label.effectStyle = this.labelEffectStyle;
			label.effectColor = this.labelEffectColor;
			label.effectDistance = this.labelEffectDistance;
			label.overflowMethod = UILabel.Overflow.ResizeHeight;
			label.width = labelWidth;
			label.Update();
			
			Bounds labelBounds = label.CalculateBounds();
			
			// Label position
			label.cachedTransform.localPosition = new Vector3((offsetX + this.padding.x), (offsetY - this.padding.y), 0f);
			
			// Add to the offset
			offsetY -= labelBounds.size.y;
			offsetY -= (this.padding.y * 2f);
			
			// Add to the background height
			bgHeight += Mathf.RoundToInt(labelBounds.size.y + (this.padding.y * 2f));

			// Add collider
			NGUITools.AddWidgetCollider(label.gameObject);

			BoxCollider bc = label.GetComponent<BoxCollider>();
			if (bc != null)
			{
				bc.center = labelBounds.center;
				bc.size = labelBounds.size + new Vector3((this.padding.x * 2f), (this.padding.y * 2f), 0f);
			}

			// Add an event listener
			UIEventListener listener = UIEventListener.Get(label.gameObject);
			listener.onHover = OnItemHover;
			listener.onPress = OnItemPress;
			listener.parameter = s;
			
			// Add this label to the list
			this.mLabelList.Add(label);
			
			// Create a separator if not at the last label
			if (i != (imax - 1) && !string.IsNullOrEmpty(this.listSeparatorSprite))
			{
				UISprite separator = this.CreateSeparator();
				separator.width = (this.mBackground.width - (this.listSepMarginLeft + this.listSepMarginRight));
				separator.transform.localPosition = new Vector3((this.mBackground.width / 2), (offsetY - this.listSepMarginTop), 0f);
				separator.Update();

				// calculate the space used vertically
				int sepUsedSpace = (this.listSepMarginTop + this.listSepMarginBottom) + Mathf.RoundToInt(separator.CalculateBounds().size.y);

				offsetY -= sepUsedSpace;
				bgHeight += sepUsedSpace;
			}
		}
		
		// Apply background height
		this.mBackground.height = bgHeight;
		this.mBackground.MakePixelPerfect();
		
		// Add a sprite for the highlight
		this.mHighlight = this.CreateHightlight();
		this.mHighlight.width = this.mBackground.width - (this.listHLMarginLeft + this.listHLMarginRight);
		this.mHighlight.height = 0;
		this.mHighlight.MakePixelPerfect();
		
		// Adjust the labels to be above the hightlight but leave the separators below
		// Also move the highlight over the selected label
		int depth = this.mHighlight.depth;
		foreach (UILabel label in this.mLabelList)
		{
			depth++;
			label.depth = depth;
			
			// Move the highlight here if this is the right label
			if (!string.IsNullOrEmpty(this.mSelectedItem) && this.mSelectedItem == label.text)
				this.Highlight(label, true);
		}
		
		// if there is no selected label, just select the first one
		if (this.mHighlightedLabel == null && this.mLabelList[0] != null)
			this.Highlight(this.mLabelList[0], true);
		
		// Determine list placement
		bool placeAbove = (position == Position.Above);
		
		// In case it's set to auto
		if (position == Position.Auto)
		{
			UICamera cam = UICamera.FindCameraForLayer(this.gameObject.layer);
			if (cam != null)
			{
				// Get the background's bottom left corner in world space
				Vector3 botLeft = this.mBackground.worldCorners[0];
				Vector3 viewPos = cam.cachedCamera.WorldToViewportPoint(botLeft);
				
				// Place above if the list is going outside the viewport
				placeAbove = (viewPos.y < 0f);
			}
		}

		// Set the current position variable
		this.currentPosition = (placeAbove) ? Position.Above : Position.Below;

		// If we need to place the popup list above the item, we need to reposition everything by the size of the list
		if (placeAbove)
			this.RepositionAbove();
	}

	/// <summary>
	/// Repositions the dropdown list above the trigger object.
	/// </summary>
	private void RepositionAbove()
	{
		// Calculate the dimensions of the object triggering the popup list so we can position it below it
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.transform.parent, this.transform);

		this.listRoot.transform.localPosition += new Vector3(0f, (this.mBackground.height + ((this.listOffset.y * -1) + bounds.size.y)), 0f);
	}

	private GameObject CreateListRoot()
	{
		// Calculate the dimensions of the object triggering the popup list so we can position it below it
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.transform.parent, this.transform);
		
		// Create the root object for the list
		GameObject obj = new GameObject("Drop-down List");
		obj.layer = this.gameObject.layer;
		
		// Prepare the list transform
		obj.transform.parent = this.transform.parent;
		obj.transform.localPosition = new Vector3(((this.listOffset.x + bounds.center.x) - bounds.extents.x), ((this.listOffset.y + bounds.center.y) - bounds.extents.y), 0f);
		obj.transform.localRotation = Quaternion.identity;
		obj.transform.localScale = Vector3.one;

		// Add a panel
		UIPanel panel = obj.AddComponent<UIPanel>();
		panel.depth = UIPanel.nextUnusedDepth;
		panel.alpha = (this.listAnimation != AnimationType.None) ? 0f : 1f;

		return obj;
	}

	private UISprite CreateBackground()
	{
		UISprite bg = NGUITools.AddSprite(this.listRoot, this.atlas, this.listBackgroundSprite);
		bg.pivot = UIWidget.Pivot.TopLeft;
		bg.depth = NGUITools.CalculateNextDepth(this.mPanel.gameObject);
		bg.color = this.listBackgroundColor;
		bg.transform.localPosition = Vector3.zero;
		bg.transform.localScale = Vector3.zero;
		bg.transform.localRotation = Quaternion.identity;

		return bg;
	}

	private UISprite CreateHightlight()
	{
		UISprite hl = NGUITools.AddSprite(this.listRoot, this.atlas, this.listHighlightSprite);
		hl.pivot = UIWidget.Pivot.TopLeft;
		hl.type = UISprite.Type.Sliced;
		hl.depth = NGUITools.CalculateNextDepth(this.mPanel.gameObject);
		hl.color = this.listHighlightColor;
		hl.transform.localPosition = Vector3.zero;
		hl.transform.localScale = Vector3.zero;
		hl.transform.localRotation = Quaternion.identity;

		return hl;
	}

	private UISprite CreateSeparator()
	{
		UISprite sep = NGUITools.AddSprite(this.listRoot, this.atlas, this.listSeparatorSprite);
		sep.pivot = UIWidget.Pivot.Top;
		sep.depth = NGUITools.CalculateNextDepth(this.mPanel.gameObject);
		sep.color = Color.white;
		sep.transform.localPosition = Vector3.zero;
		sep.transform.localScale = Vector3.zero;
		sep.transform.localRotation = Quaternion.identity;
		sep.MakePixelPerfect();

		return sep;
	}
}
