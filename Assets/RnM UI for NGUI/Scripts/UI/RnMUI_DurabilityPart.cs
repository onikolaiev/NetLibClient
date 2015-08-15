using UnityEngine;
using System.Collections;

public class RnMUI_DurabilityPart : MonoBehaviour {

	public UISprite targetSprite;

	public string normalSprite;
	public string hoverSprite;

	void Start()
	{
		if (this.targetSprite == null)
			this.targetSprite = this.GetComponent<UISprite>();

		// Get the normal sprite name
		if (this.targetSprite != null)
			this.normalSprite = this.targetSprite.spriteName;
	}

	void OnHover(bool isOver)
	{
		if (this.enabled && this.targetSprite != null)
			this.targetSprite.spriteName = (isOver && !string.IsNullOrEmpty(this.hoverSprite)) ? this.hoverSprite : this.normalSprite;
	}

	void OnTooltip(bool show)
	{
		if (!this.enabled)
			return;

		if (show)
		{
			RnMUI_DurabilityTooltip.SetDurability(Random.Range(0, 100).ToString());
			RnMUI_DurabilityTooltip.Show();
		}
		else
			RnMUI_DurabilityTooltip.Hide();
	}
}
