﻿using System;
using UnityEngine;
using Frankfort.Threading;
using System.Threading;
using System.Collections;


public class SimpleThreadingExample : MonoBehaviour
{

    public int maxThreads = 2;
    public int TestWorkerObjects = 40;
    public int minCalculations = 10000;
    public int maxCalculations = 10000000;
    public float abortAfterSeconds = 3f;


    private ThreadPoolScheduler myThreadScheduler;

    void Awake()
    {
        Application.targetFrameRate = 25;
        myThreadScheduler = Loom.CreateThreadPoolScheduler();

        //--------------- Ending Single threaded routine --------------------
        Loom.StartSingleThread(EndingSingleThreadCoroutine, System.Threading.ThreadPriority.Normal, true);
        //--------------- Ending Single threaded routine --------------------
		

        //--------------- Continues Single threaded routine --------------------
        Loom.StartSingleThread(ContinuesSingleThreadCoroutine, System.Threading.ThreadPriority.Normal, true);
        //--------------- Continues Single threaded routine --------------------
		


        //--------------- Start Multithreaded packages --------------------
        int i = TestWorkerObjects;
        IThreadWorkerObject[] workerObjects = new IThreadWorkerObject[TestWorkerObjects];

        while (--i > -1)
            workerObjects[i] = new LotsOfNumbers(UnityEngine.Random.Range(minCalculations, maxCalculations));

        myThreadScheduler.StartASyncThreads(workerObjects, OnThreadWorkComplete, OnWorkerObjectDone, maxThreads);
        StartCoroutine(AbortAllThreadsAfterDelay()); 
        //--------------- Start Multithreaded packages --------------------	
    }





    //--------------- Managing a simple single Thread --------------------
    private void EndingSingleThreadCoroutine()
    {
        Debug.Log("Starting an Coroutine on a seperate Thread!");

        Loom.WaitForNextFrame(30);
        Loom.DispatchToMainThread(() => Debug.Log("I waited atleast 30 frames. Whats the current frameCount? : " + Time.frameCount), true);

        Loom.WaitForSeconds(10);
        Loom.DispatchToMainThread(() => Debug.Log("I waited atleast 10 seconds. Whats the current frameCount? : " + Time.frameCount), true);

        //Throw error to show safemode nicely throws errors in the MainThread.

        Debug.LogWarning("About the throw an error...");
        throw new Exception("This is an safe Error and should showup in the Console");

        Debug.Log("It won't get here, but the Thread will die anyways.");
    } 



    private void ContinuesSingleThreadCoroutine()
    {
        Debug.Log("Starting an never ending Coroutine on a seperate Thread!");
        while (true)
        {
            Loom.WaitForNextFrame(10); 
            Loom.DispatchToMainThread(() => Debug.Log("I waited atleast 10 frames. Whats the current frameCount? : " + Time.frameCount), true);

            Loom.WaitForSeconds(1); 
            Loom.DispatchToMainThread(() => Debug.Log("I waited atleast 1 second. Whats the current frameCount? : " + Time.frameCount), true);
        }
        Debug.Log("It will never get here, but thats oke...");
    } 
    //--------------- Managing a simple single Thread --------------------






    //--------------- Managing a simple multithreaded ThreadPoolScheduler implementation --------------------
    public void OnWorkerObjectDone(IThreadWorkerObject finishedObj)
    {
        LotsOfNumbers fCast = (LotsOfNumbers)finishedObj;
        Debug.LogWarning("Object done! result: " + fCast.result + ", maxIterations: " + fCast.maxIterations + ", ID: " + Thread.CurrentThread.ManagedThreadId + ", progress: " + (myThreadScheduler.Progress * 100f));
    }

    public void OnThreadWorkComplete(IThreadWorkerObject[] finishedObjects)
    {
        Debug.LogWarning("All work done! Managed Thread ID:" + Thread.CurrentThread.ManagedThreadId);
    }
    private IEnumerator AbortAllThreadsAfterDelay()
    {
        yield return new WaitForSeconds(abortAfterSeconds);

        if (myThreadScheduler.isBusy) //Threaded work didn't finish in the meantime: time to abort.
        {
            Debug.Log("Terminate all worker Threads.");
            myThreadScheduler.AbortASyncThreads();
        }
    }
    //--------------- Managing a simple multithreaded ThreadPoolScheduler implementation --------------------
			
}




public class LotsOfNumbers : IThreadWorkerObject
{
    private bool isAborted;
    public long maxIterations = 0;
    public long result = 0;


    public LotsOfNumbers(long maxIterations)
    {
        this.maxIterations = maxIterations;
    }

    public void ExecuteThreadedWork()
    {
        long i = maxIterations;
        while (--i > -1 && !isAborted)
        {
            if (i == (maxIterations / 2))
            {
                Loom.DispatchToMainThread(() =>
                    Debug.Log("Dispatch: is this the MainThread? " + Loom.CheckIfMainThread()), true);

                Debug.Log("From WorkerThread: is this the MainThread? " + Loom.CheckIfMainThread());

                GameObject cube = (GameObject)Loom.DispatchToMainThreadReturn(TestThreadSafeDispatch, true);
                Loom.DispatchToMainThread(() => cube.name += "_RenamedFrom: " + Thread.CurrentThread.Name);
            }
            result++;
        }
    }

    public void AbortThreadedWork()
    {
        isAborted = true;
    }


    private object TestThreadSafeDispatch()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20));
        cube.name = "MainThreadCube";
        return cube;
    }

    private object ThreadSafeGetFrameCount()
    {
        return Time.frameCount;
    }
}
