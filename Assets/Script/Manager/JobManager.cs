using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JobManager : MonoBehaviour
{
    private static JobManager _instance;
    public static JobManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                _instance = obj.AddComponent<JobManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    Queue<IEnumerator> _jobs = new Queue<IEnumerator>();

    public Coroutine CreateJob(IEnumerator iEnumerator)
    {
        _jobs.Enqueue(iEnumerator);
        return StartCoroutine(iEnumerator);
    }
}
