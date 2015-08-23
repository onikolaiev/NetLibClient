using UnityEngine;
using System.Collections;
using netLogic;

public class onConnect : MonoBehaviour {
    public GameObject pass;
    public GameObject login;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnClick()
    {
        Global.GetInstance().username = login.GetComponent<UILabel>().text;
        Global.GetInstance().password = pass.GetComponent<UIInput>().value;
        Global.GetInstance().ConnectToLogonServer(Global.GetInstance().username, Global.GetInstance().password);
    }
}
