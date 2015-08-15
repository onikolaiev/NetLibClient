using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviour 
{
    [SerializeField]
    Camera playerCamera;
    [SerializeField]
   // public AudioListener playerAudioListener;

    private int latency;
    private Text latencyText;

	// Use this for initialization
	void Start () 
    {
        if (true)
        {
            try
            {
             //   nClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
            }
            catch(Exception ex)
            {
                Debug.Log("Error resolving local client." + ex.Message);
            }

            playerCamera.enabled = true;
          //  playerAudioListener.enabled = true;
 
            GetComponent<CharacterController>().enabled = true;
            GetComponent<PlayerCharacterController>().enabled = true;

//            latencyText = GameObject.Find("Latency Text").GetComponent<Text>();
        }
	}

    void Update()
    {
        ShowLatency();
    }

    void ShowLatency()
    {
        if(true)
        {
//            latency = nClient.GetRTT();
        //    latencyText.text = latency.ToString();
        }
    }
}
