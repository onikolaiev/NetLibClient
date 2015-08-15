using UnityEngine;
using System.Collections;
using System.Reflection;

[AddComponentMenu("RPG and MMO UI/Flippable Sprite")]
public class UISpriteFlippable : UISprite {

	/// <summary>
	/// Flips the sprite. This method is a hack, please make the mFlip varible of UISprite public to avoid using this method.
	/// </summary>
	/// <param name="direction">Direction.</param>
	public void FlipSprite(UISprite.Flip direction)
	{
		FieldInfo field = typeof(UISprite).GetField("mFlip", BindingFlags.NonPublic | BindingFlags.Instance);

		if (field != null)
			field.SetValue(this, direction);
	}
}
