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
        // Inicializa el botón con el color que vende.
        myButton = GetComponent<Button>();
        colorBlock = myButton.colors;

        colorBlock.normalColor = sellingColor;
        myButton.colors = colorBlock;

        // Inicialización de componentes necesasrias para la función del botón.
        characterImage = characterObj.GetComponent<Image>();
        coinsManager = EconomyObj.GetComponent<CoinsManager>();

        // Añadida función cuando se pulsa el botón.
        myButton.onClick.AddListener(onButtonClick);
    }

    void onButtonClick()
    {
        if(coinsManager.totalCoins >= sellingPrice && acquired == false) // Si el usuario no tiene en posesión el color y tiene dinero para comprarlo.
        {
            coinsManager.SubtractCoins(sellingPrice);
            acquired = true;
        }else if(coinsManager.totalCoins < sellingPrice && acquired == false) // Si el usuario no tiene en posesión el color y NO tiene dinero para comprarlo.
        {
            Debug.Log("No tienes suficientes monedas.");
        }
        else // Si el usuario tiene en posesión el color.
        {
            characterImage.color = sellingColor;
        }
    }
}
