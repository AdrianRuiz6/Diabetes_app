using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CarbohydratesParser : MonoBehaviour
{
    private TMP_Text AIFoodResult;
    private Button acceptButton;

    void Start()
    {
        acceptButton = GetComponent<Button>();

        acceptButton.onClick.AddListener(ParseCarbohydrates);
    }

    void OnDestroy()
    {
        acceptButton.onClick.RemoveAllListeners();
    }

    public void ParseCarbohydrates()
    {
        if(AIFoodResult != null)
        {
            string text = AIFoodResult.text;
            int carbohydrates = ExtractNumberFromText(text);
            AttributeManager.Instance.ActivateFoodButton(carbohydrates);
        }
    }

    private int ExtractNumberFromText(string text)
    {
        int number = 0;
        string numberString = "";
        foreach (char c in text)
        {
            if (char.IsDigit(c))
            {
                numberString += c;
            }
        }
        if (!string.IsNullOrEmpty(numberString))
        {
            int.TryParse(numberString, out number);
        }

        return number;
    }
}
