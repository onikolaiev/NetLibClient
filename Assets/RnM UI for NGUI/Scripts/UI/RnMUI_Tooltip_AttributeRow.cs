using UnityEngine;
using System.Collections;

[AddComponentMenu("RPG and MMO UI/Tooltip/Attribute Row")]
public class RnMUI_Tooltip_AttributeRow : MonoBehaviour {

	public UIWidget container;
	public UITableExtended table;

	public UILabel leftLabel;
	public UILabel rightLabel;

	void Start()
	{
		if (this.container == null)
			this.container = this.GetComponent<UIWidget>();

		if (this.table == null)
			this.table = this.GetComponent<UITableExtended>();
	}

	/// <summary>
	/// Updates the size of the container and depth of the children.
	/// </summary>
	public void UpdateSizeAndDepth()
	{
		if (this.container != null && this.leftLabel != null && this.rightLabel != null)
		{
			int depth = this.container.depth;

			this.leftLabel.depth = depth + 1;
			this.rightLabel.depth = depth + 2;
		}

		if (this.table != null)
			this.table.Reposition();
	}
}
