using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIToggle))]
public class RnMUI_ToggleQuestReward : MonoBehaviour {

	private UIToggle toggle;
	public UISprite target;

	public string normalSprite;
	public string activeSprite;

	void Start()
	{
		// Get the toggle component
		if (this.toggle == null)
			this.toggle = this.GetComponent<UIToggle>();

		// Get the normal sprite
		if (this.target != null)
			this.normalSprite = this.target.spriteName;

		// Trigger StateChanged just in case the NGUI call fails
		this.StateChanged();

		// Hook the on change event
		if (this.toggle != null)
			this.toggle.onChange.Add(new EventDelegate(StateChanged));
	}

	private void StateChanged()
	{
		if (this.toggle == null || this.target == null)
			return;

		if (this.toggle.value && !string.IsNullOrEmpty(this.activeSprite))
		{
			this.target.spriteName = this.activeSprite;
		}
		else
		{
			this.target.spriteName = this.normalSprite;
		}
	}
}
