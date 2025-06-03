using UnityEngine;
using Master.Domain.GameEvents;
using Master.Persistence;
using Master.Persistence.Shop;

namespace Master.Domain.Shop
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
            DataStorage_Shop.SaveTotalCoins(_totalCoins);
            DataStorage_Shop.SaveStashedCoins(_stashedCoins);
        }

        void Start()
        {
            _stashedCoins = DataStorage_Shop.LoadStashedCoins();
            _totalCoins = DataStorage_Shop.LoadTotalCoins();
            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public void AddStashedCoins(int coins)
        {
            _stashedCoins += coins;
            GameEvents_Shop.OnStashedCoinsUpdated?.Invoke(_stashedCoins);
        }

        public void StashedCoinsToTotalCoins()
        {
            _totalCoins += _stashedCoins;
            _stashedCoins = 0;
            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public void AddTotalCoins(int coins)
        {
            _totalCoins += coins;
            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public void SubstractTotalCoins(int coins)
        {
            _totalCoins -= coins;
            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public int GetCoins()
        {
            return _totalCoins;
        }
    }
}