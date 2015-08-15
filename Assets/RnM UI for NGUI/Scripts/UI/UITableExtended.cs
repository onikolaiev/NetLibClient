using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("RPG and MMO UI/Table Extended")]
public class UITableExtended : UITable {

	public enum SortBy
	{
		Default,
		Depth,
	}

	public SortBy overrideSorting = SortBy.Default;

	/// <summary>
	/// If set to true, the container will be resized to fit the contents.
	/// </summary>
	/// 
	public bool resizeContainer = false;

	/// <summary>
	/// Function that sorts items by depth.
	/// </summary>
	
	static protected int SortByDepth(Transform a, Transform b)
	{
		UIWidget aw = a.GetComponent<UIWidget>();
		UIWidget bw = b.GetComponent<UIWidget>();

		if (aw != null && bw != null)
			return aw.depth.CompareTo(bw.depth);

		return 0;
	}
	
	/// <summary>
	/// Want your own custom sorting logic? Override this function.
	/// </summary>
	
	protected override void Sort(List<Transform> list)
	{
		if (overrideSorting == SortBy.Default)
			list.Sort(UIGrid.SortByName);
		else
			list.Sort(SortByDepth);
	}

	void OnEnable()
	{
		this.Reposition();
	}

	/// <summary>
	/// Recalculate the position of all elements within the table, sorting them alphabetically if necessary.
	/// </summary>
	
	[ContextMenu("Execute")]
	public override void Reposition()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
		{
			mReposition = true;
			return;
		}
		
		if (!mInitDone) Init();
		
		mReposition = false;
		Transform myTrans = transform;
		mChildren.Clear();
		List<Transform> ch = children;
		if (ch.Count > 0) RepositionVariableSizeNew(ch);
		
		if (keepWithinPanel && mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(myTrans, true);
			UIScrollView sv = mPanel.GetComponent<UIScrollView>();
			if (sv != null) sv.UpdateScrollbars(true);
		}
		
		if (onReposition != null)
			onReposition();
	}

	private RectOffset GetMargins(Transform trans, bool isFirst, bool isLast)
	{
		RectOffset margins = new RectOffset();

		// Check if the item has margins
		UITable_ItemMargin[] marginScripts = trans.GetComponents<UITable_ItemMargin>();
		
		foreach (UITable_ItemMargin m in marginScripts)
		{
			bool apply = false;

			// Check the apply condition
			switch (m.contidion)
			{
				case UITable_ItemMargin.Condition.None:
					apply = true;
					break;
				case UITable_ItemMargin.Condition.IfFirst:
					apply = isFirst;
					break;
				case UITable_ItemMargin.Condition.IfLast:
					apply = isLast;
					break;
				case UITable_ItemMargin.Condition.IgnoreIfFirst:
					apply = (isFirst) ? false : true;
					break;
				case UITable_ItemMargin.Condition.IgnoreIfLast:
					apply = (isLast) ? false : true;
					break;
			}

			// Apply the margin if the condition was met
			if (apply)
			{
				margins.left +=  m.left;
				margins.right += m.right;
				margins.top += m.top;
				margins.bottom += m.bottom;
			}
		}

		return margins;
	}

	/// <summary>
	/// Positions the grid items, taking their own size into consideration.
	/// </summary>
	/// 
	protected void RepositionVariableSizeNew(List<Transform> children)
	{
		float xOffset = 0;
		float yOffset = 0;
		
		int cols = columns > 0 ? children.Count / columns + 1 : 1;
		int rows = columns > 0 ? columns : children.Count;
		
		Bounds[,] bounds = new Bounds[cols, rows];
		Bounds[] boundsRows = new Bounds[rows];
		Bounds[] boundsCols = new Bounds[cols];
		
		int x = 0;
		int y = 0;
		
		for (int i = 0, imax = children.Count; i < imax; ++i)
		{
			Transform t = children[i];
			Bounds b = NGUIMath.CalculateRelativeWidgetBounds(t, !hideInactive);
			
			Vector3 scale = t.localScale;
			b.min = Vector3.Scale(b.min, scale);
			b.max = Vector3.Scale(b.max, scale);
			bounds[y, x] = b;
			
			boundsRows[x].Encapsulate(b);
			boundsCols[y].Encapsulate(b);
			
			if (++x >= columns && columns > 0)
			{
				x = 0;
				++y;
			}
		}
		
		x = 0;
		y = 0;
		
		for (int i = 0, imax = children.Count; i < imax; ++i)
		{
			Transform t = children[i];
			RectOffset margins = this.GetMargins(t, (i == 0), (i == (imax - 1)));
			Bounds b = bounds[y, x];
			Bounds br = boundsRows[x];
			Bounds bc = boundsCols[y];

			Vector3 pos = t.localPosition;
			pos.x = xOffset + b.extents.x - b.center.x;
			pos.x += b.min.x - br.min.x + padding.x + margins.left;
			
			if (direction == Direction.Down)
			{
				pos.y = -yOffset - b.extents.y - b.center.y;
				pos.y += (b.max.y - b.min.y - bc.max.y + bc.min.y) * 0.5f - padding.y - margins.top;
			}
			else
			{
				pos.y = yOffset + (b.extents.y - b.center.y);
				pos.y -= (b.max.y - b.min.y - bc.max.y + bc.min.y) * 0.5f - padding.y - margins.top;
			}
			
			xOffset += br.max.x - br.min.x + padding.x * 2f + (margins.left + margins.right);
			
			t.localPosition = pos;
			
			if (++x >= columns && columns > 0)
			{
				x = 0;
				++y;
				
				xOffset = 0f;
				yOffset += bc.size.y + padding.y * 2f + (margins.top + margins.bottom);
			}

			// In case we're at the last column
			// we need to increase the y offset by a row for the sizing process
			if (i == (imax - 1) && x > 0 && x < columns)
				yOffset += bc.size.y + padding.y * 2f + (margins.top + margins.bottom);
		}

		// Update the container's height
		if (this.resizeContainer)
		{
			UIWidget container = this.GetComponent<UIWidget>();

			if (container != null)
			{
				container.height = Mathf.RoundToInt(yOffset);
				container.Update();
			}
		}
	}
}
