using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.Economy
{
    [System.Serializable]
    public class CoinsData
    {
        public int Coins;

        public CoinsData()
        {
            this.Coins = 500;
        }
        public CoinsData(int coins)
        {
            this.Coins = coins;
        }
    }
}