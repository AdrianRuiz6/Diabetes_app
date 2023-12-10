using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinsManager : MonoBehaviour
{
    public int totalCoins;
    public GameObject coinsText;

    private TextMeshProUGUI coinsTM;

    void Start()
    {
        coinsTM = coinsText.GetComponent<TextMeshProUGUI>();
        UpdateCoinsInterface();
    }

    public void AddCoins(int coins)
    {
        totalCoins += coins;
        UpdateCoinsInterface();
    }

    public void SubtractCoins(int coins)
    {
        totalCoins -= coins;
        UpdateCoinsInterface();
    }

    private void UpdateCoinsInterface()
    {
        coinsTM.text = totalCoins.ToString();
    }
}
