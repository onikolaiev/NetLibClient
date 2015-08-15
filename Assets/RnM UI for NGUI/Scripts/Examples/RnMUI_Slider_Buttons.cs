using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UISlider))]
public class RnMUI_Slider_Buttons : MonoBehaviour {
	
	public UISlider bar;
	
	void Start()
	{
		if (this.bar == null)
			this.bar = this.GetComponent<UISlider>();
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
