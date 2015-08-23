using UnityEngine;
using System.Collections;
using netLogic;
using System.Collections.Generic;

public class RnMUI_LoadScene : MonoBehaviour {

	public string sceneName;
	public UIProgressBar bar;
	public float requiredValue = 1f;
	public bool ignoreFirst = true;
    public GameObject _name;

	public void LoadScene()
    {
        if (Global.GetInstance().GetWSession()._chSelected)
        {
            Global.GetInstance().GetWSession().LoginPlayer();
        }
	}

	
}
