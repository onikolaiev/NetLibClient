using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using netLogic;
using netLogic.Network;
using System.Timers;
using netLogic.Constants;
using System;
using System.Runtime.InteropServices;

public class PlayerPosition : netInstance 
{
    private Vector3 syncPos;

    public Transform playerTransform;

    float posLerpRate = 18f;
    float posNormalLerpRate = 18f;
    float posFastLerpRate = 27f;

    private Vector3 lastPos;
    private float threshholdPos = 0.3f;

    private List<Vector3> syncPosQue = new List<Vector3>();
    private float minQueuedApproachDist = 0.1f;
    [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
    public static extern uint MM_GetTime();

    private bool useQueuedInterpolation = false;

    void Start()
    {
        posLerpRate = posNormalLerpRate;
    }

    // Update is called once per frame
    void Update ()
    {
        LerpPosition();
    }

	// FixedUpdate is called on a fixed interval
	void FixedUpdate () 
    {
        SendPosition();
	}

    void LerpPosition()
    {
        if(!true)
        {
            if (useQueuedInterpolation)
            {
                QueuedInterpolation();
            }
            else
            {
                StandardInterpolation();
            }
        }
    }


    void CmdSendPosition(Vector3 pos)
    {
        syncPos = pos;
    }


    void SendPosition()
    {
        if(true && Vector3.Distance(playerTransform.position, lastPos) > threshholdPos)
        {
            // Debug.Log("ClientCallback:SendPosition()");
            CmdSendPosition(playerTransform.position);
            lastPos = playerTransform.position;
            // Debug.Log(playerTransform.position.ToString());
        }
    }

    void OnSyncPositionValues(Vector3 latestPosition)
    {
        syncPos = latestPosition;
        syncPosQue.Add(syncPos);
    }

    void StandardInterpolation()
    {
        playerTransform.position = Vector3.Lerp(playerTransform.position, syncPos, Time.deltaTime * posLerpRate);

    //    Heartbeat(playerTransform.gameObject);
    
    
    }

    private static float swapByteOrder(float value)
    {
        Byte[] buffer = BitConverter.GetBytes(value);
        Array.Reverse(buffer, 0, buffer.Length);
        return BitConverter.ToSingle(buffer, 0);
    }

    void QueuedInterpolation()
    {
        // Only lerp if we have a destination
        if (syncPosQue.Count > 0)
        {
            playerTransform.position = Vector3.Lerp(playerTransform.position, syncPosQue[0], Time.deltaTime * posLerpRate);

            if (Vector3.Distance(playerTransform.position, syncPosQue[0]) < minQueuedApproachDist)
            {
                syncPosQue.RemoveAt(0);
            }

            if (syncPosQue.Count > 10)
            {
                posLerpRate = posFastLerpRate;
            }
            else
            {
                posLerpRate = posNormalLerpRate;
            }

            // Debug.Log(syncPosQue.Count.ToString());
        }
    }
}
