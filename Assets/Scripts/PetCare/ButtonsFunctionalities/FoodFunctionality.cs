using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodFunctionality : MonoBehaviour
{

    private Button foodButton;

    public GameObject systemsObject;
    private FeedingBar feedingBarFunctionality;
    private ActivityBar activityBarFunctionality;
    private GlucoseBar glucoseBarFunctionality;

    public float modifierFeedingBar;
    public float modifierActivityBar;
    private float modifierGlucoseBar;

    public GameObject objectIA;
    private FoodBot foodBot;

    private Cooldown cooldownFood;
    private bool validAnswer = false;

    void Start()
    {
        foodButton = GetComponent<Button>();
        foodButton.onClick.AddListener(openFoodBotPopUp);

        feedingBarFunctionality = systemsObject.GetComponent<FeedingBar>();
        activityBarFunctionality = systemsObject.GetComponent<ActivityBar>();
        glucoseBarFunctionality = systemsObject.GetComponent<GlucoseBar>();

        foodBot = objectIA.GetComponent<FoodBot>();

        cooldownFood = GetComponent<Cooldown>();
    }

    private void openFoodBotPopUp()
    {
        // Desactivar cooldown del botón de comida.
        cooldownFood.DisableCooldown();

        // Inhabilitar ventana principal desactivando raycast del panel y desactivando "interactable" de todos los botones.

        // Activar panel de la ventana emergente del talkbot de comida.
    }

    private void closeFoodBotPopUp()
    {
        // Activar cooldown del botón de comida y actualizar los atributos si se ha dado una respuesta válida.
        if (validAnswer)
        {
            cooldownFood.ActivateCooldown();
            UpdateBars();
        }
        validAnswer = false;
        

        // Habilitar ventana principal activando raycast del panel y activando "interactable" de todos los botones.

        // Desactivar panel de la ventana emergente del talkbot de comida.
    }

    private void FeedbackIA()
    {
        // Recoger texto del talkbot y mandarselo al FoodBot obteniendo así una respuesta.

        // Evaluar si la respuesta del FoodBot es válida y, si lo es, realizar las dos acciones siguientes:

        // 1.-Dar feedback al usuario de su elección.

        // 2.-Calcular la glucosa que actualizaría en la variable "modifierGlucoseBar".

    }

    private void UpdateBars()
    {
        feedingBarFunctionality.UpdateFeeding(modifierFeedingBar);
        activityBarFunctionality.UpdateActivity(modifierActivityBar);
        glucoseBarFunctionality.UpdateGlucose(modifierGlucoseBar);
    }
}
