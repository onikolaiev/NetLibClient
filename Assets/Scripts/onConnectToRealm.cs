using UnityEngine;
using System.Collections;
using netLogic;

public class onConnectToRealm : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnClick()
    {
        Global.GetInstance().ConnectToWorldServer();
        Global.GetInstance().GetWSession().Connect();
    }
}
