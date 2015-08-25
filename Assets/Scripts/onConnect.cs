using UnityEngine;
using System.Collections;
using netLogic;
using UnityEngine.UI;

public class onConnect : MonoBehaviour {
    public GameObject login;
    public GameObject pass;
   
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

   public  void OnConnectToRealm()
    {
        Global.GetInstance().username = login.GetComponent<InputField>().text;
        Global.GetInstance().password = pass.GetComponent<InputField>().text;
        Global.GetInstance().ConnectToLogonServer(Global.GetInstance().username, Global.GetInstance().password);
    }
}
