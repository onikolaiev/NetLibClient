using UnityEngine;
using System.Collections;
using netLogic;
using netLogic.Constants;

public class onGetRealmList : MonoBehaviour {
    public GameObject label;
    UIPopupList inp;
	// Use this for initialization
	void Start () {
        inp = label.GetComponent<UIPopupList>();
     
      /* foreach (Realm rl in GetInstance().GetLSession.Realmlist)
       {
          // inp.AddItem(rl.Name);
       }*/
	}
	
	// Update is called once per frame
	void Update () {


	}
    void OnClick()
    {
        
    }
    void OnApplicationQuit()
    {

     
   //     GetInstance().GetLSession.Disconnect();
    //    netInstance.Disconnect();
    }
}
