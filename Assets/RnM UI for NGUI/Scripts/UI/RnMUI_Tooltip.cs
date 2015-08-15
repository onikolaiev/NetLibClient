using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("RPG and MMO UI/Tooltip/Tooltip")]
public class RnMUI_Tooltip : MonoBehaviour
{
	public class AttributeInfo
	{
		public string value;
		public string text;
		public bool singleColumnRow;
		public RectOffset margin;
	}

	public enum AnchorPoint : int
	{
		BottomLeft = 0,
		TopLeft = 1,
		TopRight = 2,
		BottomRight = 3,
	}

	private static RnMUI_Tooltip mInstance;

	public Camera uiCamera;
	public UILabel title;
	public UILabel description;
	public UISprite background;
	public UIWidget container;
	public float fadeDuration = 0.3f;

	public Color attrValueColor = new Color(0.357f, 0.369f, 0.431f, 1f);
	public Color attrTextColor = new Color(0.357f, 0.369f, 0.431f, 1f);
	public GameObject attributesRowPrefab;

	public int attributesMarginTop = 0;
	public int attributesMarginBottom = 0;

	public RnMUI_TooltipAnchor tooltipAnchor;

	private UIWidget anchorPositionTo;
	private Vector3 targetPosition;

	private List<AttributeInfo> mAttributes = new List<AttributeInfo>();
	private List<GameObject> mInstancedAttributesRows = new List<GameObject>();

	private bool mIsVisible = false;
	public bool IsVisible { get { return this.mIsVisible; } }

	/// <summary>
	/// Whether the tooltip is currently visible.
	/// </summary>
	
	static public bool isVisible { get { return (mInstance != null && mInstance.IsVisible); } }

	void Awake()
	{
		mInstance = this;
	}

	void OnDestroy()
	{
		mInstance = null;
	}

	protected virtual void Start()
	{
		this.SetAlpha(0f);

		if (this.uiCamera == null) this.uiCamera = NGUITools.FindCameraForLayer(this.gameObject.layer);
		if (this.uiCamera == null) this.uiCamera = Camera.main;

		if (this.uiCamera == null)
		{
			Debug.LogWarning(this.GetType() + " requires a camera in order to work.", this);
			this.enabled = false;
			return;
		}

		if (this.container == null)
		{
			Debug.LogWarning(this.GetType() + " requires a container to be define in order to work.", this);
			this.enabled = false;
			return;
		}

		if (this.background == null)
		{
			Debug.LogWarning(this.GetType() + " requires a background sprite in order to work.", this);
			this.enabled = false;
			return;
		}
	}

	protected virtual void Update()
	{
		if (this.currentAlpha > 0f)
			this.UpdatePosition();
	}

	/// <summary>
	/// Updates the position of the tooltip.
	/// </summary>
	private void UpdatePosition()
	{
		AnchorPoint anchorPosition = AnchorPoint.BottomLeft;

		if (this.anchorPositionTo != null)
		{
			// Set the world position to the top right corner of the target
			this.transform.position = this.anchorPositionTo.worldCorners[(int)AnchorPoint.TopRight];
		}
		else
		{
			// Set the world position to the target position
			this.transform.position = targetPosition;
		}

		// Now move the position up based on the background height
		this.transform.localPosition = this.transform.localPosition + new Vector3(0f, this.background.height, 0f);

		// Check if the tooltip is going outside the viewport to the left
		if (this.uiCamera.WorldToScreenPoint(this.background.worldCorners[2]).x > Screen.width)
		{
			// move to the left
			this.transform.localPosition = this.transform.localPosition + new Vector3(-(this.background.width + (this.anchorPositionTo != null ? this.anchorPositionTo.width : 0)), 0f, 0f);

			// Update the anchor variable
			anchorPosition = AnchorPoint.BottomRight;
		}
		
		// Check if the tooltip is going outside the viewport to the top
		if (this.uiCamera.WorldToScreenPoint(this.background.worldCorners[1]).y > Screen.height)
		{
			// move to the bottom
			this.transform.localPosition = this.transform.localPosition + new Vector3(0f, -(this.background.height + (this.anchorPositionTo != null ? this.anchorPositionTo.height : 0)), 0f);
		
			// Update the anchor variable
			anchorPosition = (anchorPosition == AnchorPoint.BottomLeft) ? AnchorPoint.TopLeft : AnchorPoint.TopRight;
		}

		if (this.tooltipAnchor != null)
			this.tooltipAnchor.SetAnchorPosition(anchorPosition, this.background.width, this.background.height);
	}

	private void _SetTitle(string tooltipTitle)
	{
		if (this.title != null)
			this.title.text = tooltipTitle;
	}

	private void _SetDescription(string tooltipDescription)
	{
		if (this.description != null)
		{
			if (string.IsNullOrEmpty(tooltipDescription))
				this.description.enabled = false;
			else
				this.description.enabled = true;

			this.description.text = tooltipDescription;
		}
	}

	/// <summary>
	/// Sets the title of the tooltip.
	/// </summary>
	/// <param name="tooltipTitle">Tooltip title.</param>
	public static void SetTitle(string tooltipTitle)
	{
		if (mInstance != null)
			mInstance._SetTitle(tooltipTitle);
	}

	/// <summary>
	/// Sets the description of the tooltip, the description will be disabled if set to null or empty.
	/// </summary>
	/// <param name="tooltipDescription">Tooltip description.</param>
	public static void SetDescription(string tooltipDescription)
	{
		if (mInstance != null)
			mInstance._SetDescription(tooltipDescription);
	}

	private void _AddAttribute(string value, string text, bool singleColumnRow, RectOffset margin)
	{
		// Create new attribute info
		AttributeInfo info = new AttributeInfo();
		info.value = "[" + NGUIText.EncodeColor(this.attrValueColor) + "]" + value + "[-]";
		info.text =  "[" + NGUIText.EncodeColor(this.attrTextColor) + "]" + text + "[-]";
		info.singleColumnRow = singleColumnRow;
		info.margin = margin;

		// Add it to the attribute list
		this.mAttributes.Add(info);
	}

	/// <summary>
	/// Adds an attribute to the tooltip, the align is auto based on the order of attributes.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="text">Text.</param>
	public static void AddAttribute(string value, string text)
	{
		if (mInstance != null)
			mInstance._AddAttribute(value, text, false, new RectOffset());
	}

	/// <summary>
	/// Adds an attribute to the tooltip on a row with a single column.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="text">Text.</param>
	public static void AddAttribute_SingleColumn(string value, string text)
	{
		if (mInstance != null)
			mInstance._AddAttribute(value, text, true, new RectOffset());
	}

	/// <summary>
	/// Adds an attribute to the tooltip on a row with a single column.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="text">Text.</param>
	/// <param name="margin">Margin.</param>
	public static void AddAttribute_SingleColumn(string value, string text, RectOffset margin)
	{
		if (mInstance != null)
			mInstance._AddAttribute(value, text, true, margin);
	}

	private void _SetPosition(UIWidget target)
	{
		if (target == null)
			return;
		
		// Save the target and make the calculations after the show call
		this.anchorPositionTo = target;
	}

	/// <summary>
	/// Sets the position of the tooltip anchored to the given widget.
	/// </summary>
	/// <param name="target">Target.</param>
	public static void SetPosition(UIWidget target)
	{
		if (mInstance != null)
			mInstance._SetPosition(target);
	}

	private void _Show()
	{
		// Cleanup any attributes left, if any at all
		this.Cleanup();

		// Bring the tooltip forward
		NGUITools.BringForward(this.gameObject);

		// Save the position where the tooltip should appear
		this.targetPosition = uiCamera.ScreenToWorldPoint(Input.mousePosition);

		// Prepare the attributes
		if (this.mAttributes.Count > 0 && this.attributesRowPrefab != null)
		{
			bool isLeft = true;
			RnMUI_Tooltip_AttributeRow lastAttrRow = null;
			int lastDepth = this.title.depth;

			// Loop the attributes
			foreach (AttributeInfo info in this.mAttributes)
			{
				// Force left column in case it's a signle column row
				if (info.singleColumnRow)
					isLeft = true;

				if (isLeft)
				{
					// Instantiate a prefab
					GameObject obj = (GameObject)Instantiate(this.attributesRowPrefab);

					// Apply parent
					obj.transform.parent = this.container.transform;

					// Fix position and scale
					obj.transform.localScale = Vector3.one;
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localRotation = Quaternion.identity;

					// Increase the depth
					lastDepth = lastDepth + 1;

					// Apply the new depth
					UIWidget wgt = obj.GetComponent<UIWidget>();

					if (wgt != null)
						wgt.depth = lastDepth;

					// Get the attribute row script referrence
					lastAttrRow = obj.GetComponent<RnMUI_Tooltip_AttributeRow>();

					// Make some changes if it's a single column row
					if (info.singleColumnRow)
					{
						// Destroy the right column
						NGUITools.Destroy(lastAttrRow.rightLabel.gameObject);

						// Double the size of the label
						lastAttrRow.leftLabel.width *= 2;
					}

					// Check if we can set margin to the row
					UITable_ItemMargin margins = obj.GetComponent<UITable_ItemMargin>();

					// If the row does not have the component add it
					if (margins == null)
						margins = obj.AddComponent<UITable_ItemMargin>();

					if (margins != null)
					{
						margins.left = info.margin.left;
						margins.right = info.margin.right;
						margins.top = info.margin.top;
						margins.bottom = info.margin.bottom;

						// If this is the first or last row, apply the global attr margins
						if (this.mInstancedAttributesRows.Count == 0)
							margins.top += this.attributesMarginTop;
						else if (this.mInstancedAttributesRows.Count == (Mathf.RoundToInt((float)this.mAttributes.Count / 2f) - 1))
							margins.bottom += this.attributesMarginBottom;
					}

					// Add it to the instanced objects list
					this.mInstancedAttributesRows.Add(obj);
				}

				// Check if we have a row object to work with
				if (lastAttrRow != null)
				{
					UILabel label = (isLeft) ? lastAttrRow.leftLabel : lastAttrRow.rightLabel;

					// Check if we have the label
					if (label != null)
					{
						// Set the label alpha
						label.alpha = 0f;

						// Set the label text
						label.text = info.value + info.text;

						// Flip is left
						if (!info.singleColumnRow)
							isLeft = !isLeft;
					}

					// Update the row size and depth
					lastAttrRow.UpdateSizeAndDepth();
				}
			}

			// Clear the attributes list, we no longer need it
			this.mAttributes.Clear();

			// Apply greater depth to the description
			if (this.description != null)
				this.description.depth = (lastDepth + 1);
		}

		// Update the tooltip table
		if (this.container != null)
		{
			UITableExtended contTable = this.container.GetComponent<UITableExtended>();
			if (contTable != null)
				contTable.Reposition();
		}

		// Fade In
		this.StopCoroutine("FadeOut");
		this.StartCoroutine("FadeIn");
	}

	private void _Hide()
	{
		// Target alpha
		this.StopCoroutine("FadeIn");
		this.StartCoroutine("FadeOut");
	}

	private void OnFadeOut()
	{
		//Destroy the instanced attributes and empty the list
		this.Cleanup();

		// Clear the anchor to
		this.anchorPositionTo = null;
	}

	private void Cleanup()
	{
		//Destroy the attributes
		foreach (GameObject obj in this.mInstancedAttributesRows)
		{
			if (obj != null)
				DestroyImmediate(obj);
		}
		
		// Clear the list
		this.mInstancedAttributesRows.Clear();
	}

	/// <summary>
	/// Show the tooltip.
	/// </summary>
	public static void Show()
	{
		if (mInstance != null)
			mInstance._Show();
	}

	/// <summary>
	/// Hide the tooltip.
	/// </summary>
	public static void Hide()
	{
		if (mInstance != null)
			mInstance._Hide();
	}

	/// <summary>
	/// Set the alpha of all widgets.
	/// </summary>
	private void SetAlpha(float val)
	{
		UIWidget[] widgets = this.GetComponentsInChildren<UIWidget>();
		
		for (int i = 0, imax = widgets.Length; i < imax; ++i)
		{
			UIWidget w = widgets[i];
			Color c = w.color;
			c.a = val;
			w.color = c;
		}
	}

	private float currentAlpha { get { return this.background.alpha; } }

	// Show / Hide fade animation coroutine
	private IEnumerator FadeIn()
	{
		this.mIsVisible = true;
		
		// Get the timestamp
		float startTime = Time.time;
		
		// Calculate the time we need to fade in from the current alpha
		float internalDuration = (this.fadeDuration * (1f - this.currentAlpha));
		
		// Update the start time
		startTime -= (this.fadeDuration - internalDuration);
		
		// Fade In
		while (Time.time < (startTime + this.fadeDuration))
		{
			float RemainingTime = (startTime + this.fadeDuration) - Time.time;
			float ElapsedTime = this.fadeDuration - RemainingTime;
			
			// Update the alpha by the percentage of the time elapsed
			this.SetAlpha(ElapsedTime / this.fadeDuration);
			
			yield return 0;
		}
		
		// Make sure it's 1
		this.SetAlpha(1.0f);
	}
	
	// Show / Hide fade animation coroutine
	private IEnumerator FadeOut()
	{
		if (!this.IsVisible)
			yield break;
		
		// Get the timestamp
		float startTime = Time.time;
		
		// Calculate the time we need to fade out from the current alpha
		float internalDuration = (this.fadeDuration * this.currentAlpha);
		
		// Update the start time
		startTime -= (this.fadeDuration - internalDuration);
		
		// Fade In
		while (Time.time < (startTime + this.fadeDuration))
		{
			float RemainingTime = (startTime + this.fadeDuration) - Time.time;
			
			// Update the alpha by the percentage of the time elapsed
			this.SetAlpha(RemainingTime / this.fadeDuration);
			
			yield return 0;
		}
		
		// Make sure it's 0
		this.SetAlpha(0f);
		this.mIsVisible = false;
		this.OnFadeOut();
	}
}
