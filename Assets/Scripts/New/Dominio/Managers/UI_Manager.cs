using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.Managers{
    public class UI_Manager : MonoBehaviour
    {
        public static UI_Manager Instance;

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
    }
}

