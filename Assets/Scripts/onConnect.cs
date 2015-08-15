using UnityEngine;
using System.Collections;
using netLogic;

public class onConnect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnClick()
    {

        Global.GetInstance().ConnectToLogonServer(Global.GetInstance().username, Global.GetInstance().password);
    }
}
