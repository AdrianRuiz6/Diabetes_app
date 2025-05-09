using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master.Domain.Events;

namespace Master.Domain.Economy
{
    public class EconomyManager : MonoBehaviour
    {
        [SerializeField] private int _totalCoins;
        [SerializeField] private int _stashedCoins;

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

        private void OnDestroy()
        {
            DataStorage.SaveTotalCoins(_totalCoins);
            DataStorage.SaveStashedCoins(_stashedCoins);
        }

        void Start()
        {
            _stashedCoins = DataStorage.LoadStashedCoins();
            _totalCoins = DataStorage.LoadTotalCoins();
            GameEvents_Economy.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public void AddStashedCoins(int coins)
        {
            _stashedCoins += coins;
            GameEvents_Economy.OnStashedCoinsUpdated?.Invoke(_stashedCoins);
        }

        public void StashedCoinsToTotalCoins()
        {
            _totalCoins += _stashedCoins;
            _stashedCoins = 0;
            GameEvents_Economy.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public void AddTotalCoins(int coins)
        {
            _totalCoins += coins;
            GameEvents_Economy.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public void SubstractTotalCoins(int coins)
        {
            _totalCoins -= coins;
            GameEvents_Economy.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public int GetCoins()
        {
            return _totalCoins;
        }
    }
}