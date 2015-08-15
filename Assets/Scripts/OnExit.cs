using UnityEngine;
using netLogic;
using System;
public class OnExit : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnApplicationQuit()
    {
            Global.GetInstance().GetLSession().Disconnect();
            Global.GetInstance().GetWSession().HardDisconnect();
                
            GC.Collect(1,GCCollectionMode.Forced);

    }
}
