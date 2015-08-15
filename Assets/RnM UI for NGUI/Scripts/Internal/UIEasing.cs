using UnityEngine;
using System.Collections;

public class UIEasing {

	public enum EasingType
	{
		easeLinear,
		easeSwing,
		easeInQuad,
		easeOutQuad,
		easeInOutQuad,
		easeInCubic,
		easeOutCubic,
		easeInOutCubic,
		easeInQuart,
		easeOutQuart,
		easeInOutQuart,
		easeInQuint,
		easeOutQuint,
		easeInOutQuint,
		easeInSine,
		easeOutSine,
		easeInOutSine,
		easeInExpo,
		easeOutExpo,
		easeInOutExpo,
		easeInCirc,
		easeOutCirc,
		easeInOutCirc,
		easeInElastic,
		easeOutElastic,
		easeInOutElastic,
		easeInBack,
		easeOutBack,
		easeInOutBack,
		easeInBounce,
		easeOutBounce,
		easeInOutBounce,
	}

	public static float Ease(EasingType type, float t, float b, float c, float d)
	{
		switch (type)
		{
			case EasingType.easeLinear:
			{
				return c*t/d + b;
			}

			case EasingType.easeSwing:
			{
				return -c *(t/=d)*(t-2f) + b;
			}

			case EasingType.easeInQuad:
	    	{
				return c*(t/=d)*t + b;
			}

			case EasingType.easeOutQuad:
	   	 	{
				return -c *(t/=d)*(t-2) + b;
			}
			
			case EasingType.easeInOutQuad:
	    	{
				if ((t/=d/2) < 1) return c/2*t*t + b;
				return -c/2 * ((--t)*(t-2) - 1) + b;
			}
			
			case EasingType.easeInCubic:
	    	{
				return c*(t/=d)*t*t + b;
			}
			
			case EasingType.easeOutCubic:
	    	{
				return c*((t=t/d-1)*t*t + 1) + b;
			}
			
			case EasingType.easeInOutCubic:
	    	{
				if ((t/=d/2) < 1) return c/2*t*t*t + b;
				return c/2*((t-=2)*t*t + 2) + b;
			}

			
			case EasingType.easeInQuart:
	    	{
				return c*(t/=d)*t*t*t + b;
			}
			
			case EasingType.easeOutQuart:
	    	{
				return -c * ((t=t/d-1)*t*t*t - 1) + b;
			}
			
			case EasingType.easeInOutQuart:
	    	{
				if ((t/=d/2) < 1) return c/2*t*t*t*t + b;
				return -c/2 * ((t-=2)*t*t*t - 2) + b;
			}
			
			case EasingType.easeInQuint:
	    	{
				return c*(t/=d)*t*t*t*t + b;
			}
			
			case EasingType.easeOutQuint:
	    	{
				return c*((t=t/d-1)*t*t*t*t + 1) + b;
			}
			
			case EasingType.easeInOutQuint:
	    	{
				if ((t/=d/2) < 1) return c/2*t*t*t*t*t + b;
				return c/2*((t-=2)*t*t*t*t + 2) + b;
			}
			
			case EasingType.easeInSine:
	    	{
				return -c * Mathf.Cos(t/d * (Mathf.PI/2)) + c + b;
			}
			
			case EasingType.easeOutSine:
	    	{
				return c * Mathf.Sin(t/d * (Mathf.PI/2)) + b;
			}
			
			case EasingType.easeInOutSine:
	    	{
				return -c/2 * (Mathf.Cos(Mathf.PI*t/d) - 1) + b;
			}
			
			case EasingType.easeInExpo:
	    	{
				return (t==0) ? b : c * Mathf.Pow(2, 10 * (t/d - 1)) + b;
			}
			
			case EasingType.easeOutExpo:
	    	{
				return (t==d) ? b+c : c * (-Mathf.Pow(2, -10 * t/d) + 1) + b;
			}
			
			case EasingType.easeInOutExpo:
	    	{
				if (t==0) return b;
				if (t==d) return b+c;
				if ((t/=d/2) < 1) return c/2 * Mathf.Pow(2, 10 * (t - 1)) + b;
				return c/2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
			}
			
			case EasingType.easeInCirc:
	    	{
				return -c * (Mathf.Sqrt(1 - (t/=d)*t) - 1) + b;
			}
			
			case EasingType.easeOutCirc:
	    	{
				return c * Mathf.Sqrt(1 - (t=t/d-1)*t) + b;
			}
			
			case EasingType.easeInOutCirc:
	    	{
				if ((t/=d/2) < 1) return -c/2 * (Mathf.Sqrt(1 - t*t) - 1) + b;
				return c/2 * (Mathf.Sqrt(1 - (t-=2)*t) + 1) + b;
			}
			
			case EasingType.easeInBack:
	    	{
				float s = 1.70158f;
				return c*(t/=d)*t*((s+1f)*t - s) + b;
			}
			
			case EasingType.easeOutBack:
	    	{
				float s = 1.70158f;
				return c*((t=t/d-1f)*t*((s+1f)*t + s) + 1f) + b;
			}
			
			case EasingType.easeInOutBack:
	    	{
				float s = 1.70158f; 
				if ((t/=d/2f) < 1f) return c/2f*(t*t*(((s*=(1.525f))+1f)*t - s)) + b;
				return c/2f*((t-=2f)*t*(((s*=(1.525f))+1f)*t + s) + 2f) + b;
			}

			case EasingType.easeInBounce:
	    	{
				return c - Ease(EasingType.easeOutBounce, d-t, 0f, c, d) + b;
			}
			
			case EasingType.easeOutBounce:
	    	{
				if ((t/=d) < (1f/2.75f)) {
					return c*(7.5625f*t*t) + b;
				} else if (t < (2f/2.75f)) {
					return c*(7.5625f*(t-=(1.5f/2.75f))*t + .75f) + b;
				} else if (t < (2.5f/2.75f)) {
					return c*(7.5625f*(t-=(2.25f/2.75f))*t + .9375f) + b;
				} else {
					return c*(7.5625f*(t-=(2.625f/2.75f))*t + .984375f) + b;
				}
			}
			
			case EasingType.easeInOutBounce:
	    	{
				if (t < d/2f) return Ease(EasingType.easeInBounce, t*2f, 0f, c, d) * .5f + b;
				return Ease(EasingType.easeOutBounce, t*2f-d, 0f, c, d) * .5f + c*.5f + b;
			}
			
			case EasingType.easeInElastic:
	    	{
				float s=1.70158f; float p=0f; float a=c;
				if (t==0f) return b;
				if ((t/=d)==1f) return b+c;
				if (p==0f) p=d*.3f;
				if (a < Mathf.Abs(c)) { a=c; s=p/4f; }
				else s = p/(2f*Mathf.PI) * Mathf.Asin(c/a);
				if (float.IsNaN(s)) s = 0f;
				return -(a*Mathf.Pow(2f,10f*(t-=1f)) * Mathf.Sin((t*d-s)*(2f*Mathf.PI)/p )) + b;
			}
			
			case EasingType.easeOutElastic:
	    	{
				float s=1.70158f; float p=0f; float a=c;
				if (t==0f) return b; if ((t/=d)==1f) return b+c; if (p==0f) p=d*.3f;
				if (a < Mathf.Abs(c)) { a=c; s=p/4f; }
				else s = p/(2f*Mathf.PI) * Mathf.Asin(c/a);
				if (float.IsNaN(s)) s = 0f;
				return a*Mathf.Pow(2f,-10f*t) * Mathf.Sin((t*d-s)*(2f*Mathf.PI)/p ) + c + b;
			}
			
			case EasingType.easeInOutElastic:
	    	{
				float s=1.70158f; float p=0f; float a=c;
				if (t==0f) return b; if ((t/=d/2f)==2f) return b+c; if (p==0f) p=d*(.3f*1.5f);
				if (a < Mathf.Abs(c)) { a=c; s=p/4f; }
				else s = p/(2f*Mathf.PI) * Mathf.Asin(c/a);
				if (float.IsNaN(s)) s = 0f;
				if (t < 1f) return -.5f*(a*Mathf.Pow(2f,10f*(t-=1f)) * Mathf.Sin((t*d-s)*(2f*Mathf.PI)/p )) + b;
				return a*Mathf.Pow(2f,-10f*(t-=1f)) * Mathf.Sin((t*d-s)*(2f*Mathf.PI)/p )*.5f + c + b;
			}
		}

		// Default linear
		return c*t/d + b;
	}
}
