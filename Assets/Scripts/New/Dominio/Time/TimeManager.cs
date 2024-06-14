using Master.Domain.Economy;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

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

    private void OnDestroy()
    {
        DataStorage.SaveDisconnectionDate();
    }
}
