using UnityEngine;
using System.Collections;

public class RnMUI_Assign_ItemSlot : MonoBehaviour {
	
	public RnMUI_ItemSlot slot;
	public NewUI_ItemDatabase itemDatabase;
	public int assignItem;
	
	void Start()
	{
		if (this.slot == null)
			this.slot = this.GetComponent<RnMUI_ItemSlot>();
		
		if (this.slot == null || this.itemDatabase == null)
		{
			this.Destruct();
			return;
		}
		
		this.slot.Assign(this.itemDatabase.GetByID(this.assignItem));
		this.Destruct();
	}
	
	private void Destruct()
	{
		DestroyImmediate(this);
	}
}
