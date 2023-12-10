using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorCustomization : MonoBehaviour
{
    public GameObject characterObj;
    public GameObject EconomyObj;
    public Color sellingColor;
    public int sellingPrice;

    private Image characterImage;
    private CoinsManager coinsManager;
    private bool acquired;
    private Image myImage;
    private Button myButton;
    private ColorBlock colorBlock;

    void Start()
    {
        // Inicializa el bot�n con el color que vende.
        myButton = GetComponent<Button>();
        colorBlock = myButton.colors;

        colorBlock.normalColor = sellingColor;
        myButton.colors = colorBlock;

        // Inicializaci�n de componentes necesasrias para la funci�n del bot�n.
        characterImage = characterObj.GetComponent<Image>();
        coinsManager = EconomyObj.GetComponent<CoinsManager>();

        // A�adida funci�n cuando se pulsa el bot�n.
        myButton.onClick.AddListener(onButtonClick);
    }

    void onButtonClick()
    {
        if(coinsManager.totalCoins >= sellingPrice && acquired == false) // Si el usuario no tiene en posesi�n el color y tiene dinero para comprarlo.
        {
            coinsManager.SubtractCoins(sellingPrice);
            acquired = true;
        }else if(coinsManager.totalCoins < sellingPrice && acquired == false) // Si el usuario no tiene en posesi�n el color y NO tiene dinero para comprarlo.
        {
            Debug.Log("No tienes suficientes monedas.");
        }
        else // Si el usuario tiene en posesi�n el color.
        {
            characterImage.color = sellingColor;
        }
    }
}
