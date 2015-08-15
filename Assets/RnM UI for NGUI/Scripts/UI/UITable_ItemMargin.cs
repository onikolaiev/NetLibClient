using UnityEngine;
using System.Collections;

/// <summary>
/// Adds margins to the item that the script is attached to if the item is in a UITableExtended.
/// </summary>

[AddComponentMenu("RPG and MMO UI/Table Extended Item Margin")]
public class UITable_ItemMargin : MonoBehaviour
{
	[SerializeField]
	public enum Condition
	{
		None,
		IfFirst,
		IfLast,
		IgnoreIfFirst,
		IgnoreIfLast,
	}

	public Condition contidion = Condition.None;

	public int left = 0;
	public int right = 0;
	public int top = 0;
	public int bottom = 0;
}
