using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master.Domain.Events;

namespace Master.Domain.Economy
{
    public class EconomyManager : MonoBehaviour
    {
        [SerializeField] private int _coins;

        public static EconomyManager Instance;

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
            _coins = DataStorage.LoadCoins();
            GameEventsEconomy.OnCoinsUpdated?.Invoke(_coins);
        }

        public void AddCoins(int coins)
        {
            _coins += coins;
            GameEventsEconomy.OnCoinsUpdated?.Invoke(_coins);
        }

        public void SubstractCoins(int coins)
        {
            _coins -= coins;
            GameEventsEconomy.OnCoinsUpdated?.Invoke(_coins);
        }

        public int GetCoins()
        {
            return _coins;
        }

        private void OnDestroy()
        {
            DataStorage.SaveCoins(_coins);
        }
    }
}