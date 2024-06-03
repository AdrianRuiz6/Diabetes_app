using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesMutex : MonoBehaviour
{
    public static AttributesMutex Instance;

    private bool mutex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mutex = false;
    }

    public void Lock()
    {
        mutex = true;
    }

    public void Unlock()
    {
        mutex = false;
    }

    public void WaitForMutex()
    {
        while(mutex) { }
    }
}
