using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("RPG and MMO UI/Slider Options")]
public class UISliderOptions : MonoBehaviour {

	public UISlider slider;

	public Color normalColor = Color.white;
	public Color activeColor = Color.white;
	public bool animate = true;
	public float animationDuration = 0.15f;

	[SerializeField] List<UILabel> optionLabels = new List<UILabel>();
	private UILabel currentlyActiveLabel;

	void Start()
	{
		if (this.slider == null) this.slider = this.GetComponent<UISlider>();
		if (this.slider == null) NGUITools.FindInParents<UISlider>(this.transform);

		if (this.slider == null)
		{
			this.enabled = false;
			return;
		}

		// Hook the value change event
		this.slider.onChange.Add(new EventDelegate(OnValueChange));
	}

	public void OnValueChange()
	{
		if (this.optionLabels.Count == 0)
			return;

		// Calculate the index of the current slider value
		int index = 0;

		if (this.slider.value > 0f)
		{
			float perStep = (1f / (this.slider.numberOfSteps - 1));
			index = Mathf.RoundToInt(this.slider.value / perStep);
		}

		// Check if we have that index in the list
		if (index < 0 || index >= this.optionLabels.Count)
			return;

		// Find the label for the current slider value
		UILabel activeLabel = this.optionLabels[index];

		// If we have animations enabled
		if (this.animate)
		{
			// Change the currently active label color
			if (this.currentlyActiveLabel != null)
				TweenColor.Begin(this.currentlyActiveLabel.gameObject, this.animationDuration, this.normalColor).method = UITweener.Method.EaseOut;

			// Change the new label color
			if (activeLabel != null)
				TweenColor.Begin(activeLabel.gameObject, this.animationDuration, this.activeColor).method = UITweener.Method.EaseIn;
		}
		else
		{
			// Change the currently active label color
			if (this.currentlyActiveLabel != null)
				this.currentlyActiveLabel.color = this.normalColor;
			
			// Change the new label color
			if (activeLabel != null)
				activeLabel.color = this.activeColor;
		}

		// Save the label game object for later use
		this.currentlyActiveLabel = activeLabel;
	}
}
