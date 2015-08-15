using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIProgressBar))]
public class RnMUI_ProgressBar_Buttons : MonoBehaviour {

	public UIProgressBar bar;

	void Start()
	{
		if (this.bar == null)
			this.bar = this.GetComponent<UIProgressBar>();
	}

	public void OnClickPlus()
	{
		if (this.bar == null)
			return;

		if (this.bar.numberOfSteps > 0)
			this.bar.value = this.bar.value + (1f / (this.bar.numberOfSteps - 1));
		else
			this.bar.value = this.bar.value + 0.1f;
	}

	public void OnClickMinus()
	{
		if (this.bar == null)
			return;

		if (this.bar.numberOfSteps > 0)
			this.bar.value = this.bar.value - (1f / (this.bar.numberOfSteps - 1));
		else
			this.bar.value = this.bar.value - 0.1f;
	}
}
