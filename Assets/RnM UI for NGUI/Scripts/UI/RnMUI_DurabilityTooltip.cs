using UnityEngine;
using System.Collections;

[AddComponentMenu("RPG and MMO UI/Durability Tooltip")]
public class RnMUI_DurabilityTooltip : MonoBehaviour {

	protected static RnMUI_DurabilityTooltip mInstance;

	public UILabel percentLabel;
	public Camera uiCamera;
	public bool fading = true;
	public float fadeDuration = 0.3f;
	public Vector2 offset = Vector2.zero;

	public float currentAlpha { get { return (this.percentLabel != null) ? this.percentLabel.alpha : 0f; } }
	public bool isVisible { get { return (this.currentAlpha > 0f); } }

	void Awake()
	{
		mInstance = this;
	}
	
	void OnDestroy()
	{
		mInstance = null;
	}

	void Start()
	{
		// Hide the tooltip
		this.SetAlpha(0f);

		// Try finding the camera if not set
		if (this.uiCamera == null)
			this.uiCamera = NGUITools.FindCameraForLayer(this.gameObject.layer);
	}

	public void SetAlpha(float alpha)
	{
		UIWidget[] widgets = this.GetComponentsInChildren<UIWidget>();

		foreach (UIWidget w in widgets)
			w.alpha = alpha;
	}

	/// <summary>
	/// Sets the durability string.
	/// </summary>
	/// <param name="durability">Durability.</param>
	public static void SetDurability(string durabilityPct)
	{
		if (mInstance != null)
			mInstance._SetDurability(durabilityPct);
	}

	private void _SetDurability(string durabilityPct)
	{
		if (this.percentLabel != null)
			this.percentLabel.text = durabilityPct + "%";
	}

	/// <summary>
	/// Show the tooltip.
	/// </summary>
	public static void Show()
	{
		if (mInstance != null)
			mInstance._Show();
	}

	private void _Show()
	{
		// Update the position of the tooltip
		Vector3 position = Input.mousePosition;
		
		if (this.uiCamera != null)
		{
			// Since the screen can be of different than expected size, we want to convert
			// mouse coordinates to view space, then convert that to world position.
			position.x = Mathf.Clamp01(position.x / Screen.width);
			position.y = Mathf.Clamp01(position.y / Screen.height);
			
			// Update the absolute position and save the local one
			this.transform.position = this.uiCamera.ViewportToWorldPoint(position);
			position = this.transform.localPosition;
			position.x = offset.x + Mathf.Round(position.x);
			position.y = offset.y + Mathf.Round(position.y);
			this.transform.localPosition = position;
		}

		this.StopCoroutine("FadeOut");
		this.StartCoroutine("FadeIn");
	}
	
	/// <summary>
	/// Hide the tooltip.
	/// </summary>
	public static void Hide()
	{
		if (mInstance != null)
			mInstance._Hide();
	}

	private void _Hide()
	{
		// Target alpha
		this.StopCoroutine("FadeIn");
		this.StartCoroutine("FadeOut");
	}

	// Show / Hide fade animation coroutine
	private IEnumerator FadeIn()
	{
		// Get the timestamp
		float startTime = Time.time;
		
		// Calculate the time we need to fade in from the current alpha
		float internalDuration = (this.fadeDuration * (1f - this.currentAlpha));
		
		// Update the start time
		startTime -= (this.fadeDuration - internalDuration);
		
		// Fade In
		while (Time.time < (startTime + this.fadeDuration))
		{
			float RemainingTime = (startTime + this.fadeDuration) - Time.time;
			float ElapsedTime = this.fadeDuration - RemainingTime;
			
			// Update the alpha by the percentage of the time elapsed
			this.SetAlpha(ElapsedTime / this.fadeDuration);
			
			yield return 0;
		}
		
		// Make sure it's 1
		this.SetAlpha(1.0f);
	}
	
	// Show / Hide fade animation coroutine
	private IEnumerator FadeOut()
	{
		if (this.currentAlpha == 0f)
			yield break;
		
		// Get the timestamp
		float startTime = Time.time;
		
		// Calculate the time we need to fade out from the current alpha
		float internalDuration = (this.fadeDuration * this.currentAlpha);
		
		// Update the start time
		startTime -= (this.fadeDuration - internalDuration);
		
		// Fade In
		while (Time.time < (startTime + this.fadeDuration))
		{
			float RemainingTime = (startTime + this.fadeDuration) - Time.time;
			
			// Update the alpha by the percentage of the time elapsed
			this.SetAlpha(RemainingTime / this.fadeDuration);
			
			yield return 0;
		}
		
		// Make sure it's 0
		this.SetAlpha(0f);
	}
}
