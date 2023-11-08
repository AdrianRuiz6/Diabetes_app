using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinsManager : MonoBehaviour
{
    public int total_coins;
    public GameObject coinsInterfaceObj;

    private TextMeshProUGUI coinsTM;

    void Start()
    {
        coinsTM = coinsInterfaceObj.GetComponent<TextMeshProUGUI>();
        updateCoinsInterface();
    }

    public void addCoins(int coins)
    {
        total_coins += coins;
        updateCoinsInterface();
    }

    public void subtractCoins(int coins)
    {
        total_coins -= coins;
        updateCoinsInterface();
    }

    private void updateCoinsInterface()
    {
        coinsTM.text = total_coins.ToString();
    }
}
