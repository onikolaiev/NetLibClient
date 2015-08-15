using UnityEngine;
using System.Collections;

public class RnMUI_Assign_SpellSlot : MonoBehaviour {

	public RnMUI_SpellSlot slot;
	public NewUI_SpellDatabase spellDatabase;
	public int assignSpell;

	void Start()
	{
		if (this.slot == null)
			this.slot = this.GetComponent<RnMUI_SpellSlot>();

		if (this.slot == null || this.spellDatabase == null)
		{
			this.Destruct();
			return;
		}

		this.slot.Assign(this.spellDatabase.GetByID(this.assignSpell));
		this.Destruct();
	}

	private void Destruct()
	{
		DestroyImmediate(this);
	}
}
