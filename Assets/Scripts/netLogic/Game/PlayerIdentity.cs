using UnityEngine;
using System.Collections;

public class PlayerIdentity : MonoBehaviour 
{
    private string playerUniqueIdentity;


    private Transform playerTransform;



    void Awake()
    {
        playerTransform = transform;
    }

    void Update()
    {
        if (!true && (playerTransform.name == "" || playerTransform.name == "Player(Clone)"))
        {

        }
    }


    

}
