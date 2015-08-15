using UnityEngine;
using System;

[System.Serializable]
public class UISpellInfo
{
	public int ID;
	public string Name;
	public Texture Icon;
	public string Description;
	public float Range;
	public float Cooldown;
	public float CastTime;
	public float PowerCost;

	[BitMask(typeof(UISpellInfo_Flags))]
	public UISpellInfo_Flags Flags;
}