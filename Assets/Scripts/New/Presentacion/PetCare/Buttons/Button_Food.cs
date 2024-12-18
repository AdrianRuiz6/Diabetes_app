using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Button_Food : MonoBehaviour
{
    [SerializeField] private TMP_Text _feedBackTMP;
    [SerializeField] private TMP_InputField _inputTMP;

    [SerializeField] private Button _openButton;
    [SerializeField] private Button _searchButton;
    [SerializeField] private Button _sendButton;
    [SerializeField] private Image _sendImage;
    [SerializeField] private Button _closeButton;

    [SerializeField] private GameObject _submenuPanel;

    [SerializeField] private FoodBot _foodBot;

    private string _resultBot;
    private float _ration;

    void Start()
    {
        _openButton.onClick.AddListener(OpenSubMenu);
        _closeButton.onClick.AddListener(CloseSubMenu);
        _sendButton.onClick.AddListener(SendInformation);
        _searchButton.onClick.AddListener(SearchInformation);

        _foodBot = new FoodBot();
    }

    private void ActivateSendButton()
    {
        _sendImage.color = Color.white;
        _sendButton.interactable = true;
    }
    private void DeactivateSendButton()
    {
        _sendImage.color = Color.gray;
        _sendButton.interactable = false;
    }

    private void OpenSubMenu()
    {
        PageSliding.Instance.DeactivatePageSliding();
        DeactivateSendButton();
        _resultBot = "";
        _inputTMP.text = "";
        _feedBackTMP.text = "";
        _ration = 0;

        _submenuPanel.SetActive(true);        
    }

    private void CloseSubMenu()
    {
        PageSliding.Instance.ActivatePageSliding();
        _submenuPanel.SetActive(false);
    }

    public void SearchInformation()
    {
        StartCoroutine(WaitForBotResponse());
    }

    private IEnumerator WaitForBotResponse()
    {
        yield return StartCoroutine(_foodBot.Ask(_inputTMP.text));

        yield return new WaitUntil(() => _foodBot.isResponseAvailable);

        _resultBot = _foodBot.GetResponse();

        if (!string.IsNullOrEmpty(_resultBot))
        {
            _feedBackTMP.text = _resultBot;
            ActivateSendButton();
        }
        else
        {
            _feedBackTMP.text = "Lo siento, ahora mismo no puedo pensar en una respuesta :/";
        }
    }
    public void SendInformation()
    {
        // Se parsea la respuesta de ChatGPT.
        _ration = ExtractRationsFromText(_resultBot);
        Debug.Log($"FOOD BUTTON -Rations parsed-: {_ration}");

        // Se envía la información a AttributeManager.
        AttributeManager.Instance.ActivateFoodButton(_ration, _inputTMP.text);

        // Se desactiva el panel.
        _submenuPanel.SetActive(false);
    }

    private float ExtractRationsFromText(string text)
    {
        float number = 0f;
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
            float.TryParse(numberString, out number);
            number /= 10;
        }

        return number;
    }
}