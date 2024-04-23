using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
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

    public GameObject foodBotPanel;
    public GameObject petCarePanel;
    private Image backgroundPetCarePanel;

    public Button goBackBtn;
    public Button sendMessageBtn;
    public TMP_InputField InputUser;
    public TMP_Text feedbackText;

    void Start()
    {
        foodButton = GetComponent<Button>();

        foodButton.onClick.AddListener(openFoodBotPopUp);
        goBackBtn.onClick.AddListener(closeFoodBotPopUp);
        sendMessageBtn.onClick.AddListener(FeedbackIA);

        feedingBarFunctionality = systemsObject.GetComponent<FeedingBar>();
        activityBarFunctionality = systemsObject.GetComponent<ActivityBar>();
        glucoseBarFunctionality = systemsObject.GetComponent<GlucoseBar>();

        foodBot = objectIA.GetComponent<FoodBot>();

        cooldownFood = GetComponent<Cooldown>();

        backgroundPetCarePanel = petCarePanel.GetComponent<Image>();
    }

    private void openFoodBotPopUp()
    {
        // Desactivar cooldown del botón de comida.
        cooldownFood.DisableCooldown();

        // Inhabilitar ventana principal desactivando raycast del panel y desactivando "interactable" de todos los botones.
        backgroundPetCarePanel.raycastTarget = false;

        Button[] buttons_in_panel = petCarePanel.GetComponentsInChildren<Button>();
        foreach(Button button in buttons_in_panel)
        {
            button.interactable = false;
        }

        // Activar panel de la ventana emergente del talkbot de comida.
        foodBotPanel.SetActive(true);
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
        backgroundPetCarePanel.raycastTarget = true;

        Button[] buttons_in_panel = petCarePanel.GetComponentsInChildren<Button>();
        foreach (Button button in buttons_in_panel)
        {
            button.interactable = true;
        }

        // Desactivar panel de la ventana emergente del talkbot de comida.
        foodBotPanel.SetActive(false);

        // Limpiar campos de texto.
        InputUser.text = string.Empty;
        feedbackText.text = string.Empty;
    }

    private void FeedbackIA()
    {
        

        // Recoger texto del talkbot y mandarselo al FoodBot obteniendo así una respuesta.
        if (InputUser.text == "")
        {
            return;
        }
        foodBot.Ask(InputUser.text);

        StartCoroutine(WaitResponse());

        
    }

    IEnumerator WaitResponse()
    {
        string response = "";

        yield return new WaitForSeconds(5);

        response = foodBot.response;

        // Evaluar si la respuesta del FoodBot es válida y, si lo es, realizar las dos acciones siguientes:
        if (response == "")
        {
            feedbackText.text = "ERROR al procesar la respuesta. Prueba otra vez...";
        }
        else
        {
            validAnswer = true;
            DisableWindowPopUp();

            // 1.-Dar feedback al usuario de su elección.
            feedbackText.text = response;

            // 2.-Calcular la glucosa que actualizaría en la variable "modifierGlucoseBar".
            CalculateGlucose(response);
        }
    }

    private void UpdateBars()
    {
        feedingBarFunctionality.UpdateFeeding(modifierFeedingBar);
        activityBarFunctionality.UpdateActivity(modifierActivityBar);
        glucoseBarFunctionality.UpdateGlucose(modifierGlucoseBar);
    }

    private void CalculateGlucose(string responseChatBot)
    {

    }

    private void DisableWindowPopUp()
    {

    }

}
