using UnityEngine;
using System.Collections;

[AddComponentMenu("RPG and MMO UI/Tooltip/Anchor")]
public class RnMUI_TooltipAnchor : MonoBehaviour {

	public UISpriteFlippable anchorSprite;
	public Vector2 offset = Vector2.zero;

	void Start()
	{
		if (this.anchorSprite == null)
			this.anchorSprite = this.GetComponent<UISpriteFlippable>();
	}

	public void SetAnchorPosition(RnMUI_Tooltip.AnchorPoint point, int tooltipWidth, int tooltipHeight)
	{
		if (this.anchorSprite == null)
			return;

		switch (point)
		{
			case RnMUI_Tooltip.AnchorPoint.BottomLeft:
			{
				this.anchorSprite.cachedTransform.localPosition = new Vector3(this.offset.x, (0f - (this.offset.y + (tooltipHeight - this.anchorSprite.height))), 0f);
				this.anchorSprite.FlipSprite(UISprite.Flip.Nothing);
				break;
			}
			case RnMUI_Tooltip.AnchorPoint.BottomRight:
			{
				this.anchorSprite.cachedTransform.localPosition = new Vector3(((this.offset.x * -1) + (tooltipWidth - this.anchorSprite.width)), (0f - (this.offset.y + (tooltipHeight - this.anchorSprite.height))), 0f);
				this.anchorSprite.FlipSprite(UISprite.Flip.Horizontally);
				break;
			}
			case RnMUI_Tooltip.AnchorPoint.TopLeft:
			{
				this.anchorSprite.cachedTransform.localPosition = new Vector3(this.offset.x, this.offset.y, 0f);
				this.anchorSprite.FlipSprite(UISprite.Flip.Vertically);
				break;
			}
			case RnMUI_Tooltip.AnchorPoint.TopRight:
			{
				this.anchorSprite.cachedTransform.localPosition = new Vector3(((this.offset.x * -1) + (tooltipWidth - this.anchorSprite.width)), this.offset.y, 0f);
				this.anchorSprite.FlipSprite(UISprite.Flip.Both);
				break;
			}
		}
	}
}
