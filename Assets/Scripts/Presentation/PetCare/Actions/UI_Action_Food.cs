using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Master.Presentation.Animations;
using Master.Domain.PetCare;
using Master.Infrastructure;

namespace Master.Presentation.PetCare
{
    public class UI_Action_Food : MonoBehaviour
    {
        [SerializeField] private TMP_Text _feedBackTMP;
        [SerializeField] private TMP_InputField _inputTMP;

        [SerializeField] private Button _openButton;
        [SerializeField] private Button _searchButton;
        [SerializeField] private Button _sendButton;
        [SerializeField] private Image _sendImage;
        [SerializeField] private Button _closeButton;

        [SerializeField] private GameObject _submenuPanel;

        private string _resultBot;
        private float _ration;

        private IPetCareManager _petCareManager;

        void Start()
        {
            _petCareManager = ServiceLocator.Instance.GetService<IPetCareManager>();

            _openButton.onClick.AddListener(OpenSubMenu);
            _closeButton.onClick.AddListener(CloseSubMenu);
            _sendButton.onClick.AddListener(SendInformation);
            _searchButton.onClick.AddListener(SearchInformation);
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
            Animation_PageSliding.Instance.DeactivatePageSliding();
            DeactivateSendButton();
            _resultBot = "";
            _inputTMP.text = "";
            _feedBackTMP.text = "";
            _ration = 0;

            _submenuPanel.SetActive(true);
        }

        private void CloseSubMenu()
        {
            Animation_PageSliding.Instance.ActivatePageSliding();
            _submenuPanel.SetActive(false);
        }

        public async void SearchInformation()
        {
            DeactivateSendButton();

            string input = _inputTMP.text;

            _resultBot = await _petCareManager.GetInformationFromFoodName(input);
            _feedBackTMP.text = _resultBot;
            if (_resultBot == "No has escrito una comida, prueba otra vez." || _resultBot == "Lo siento, ahora mismo no puedo pensar en una respuesta.")
            {
                DeactivateSendButton();
            }
            else
            {
                ActivateSendButton();
            }
                
        }

        public void SendInformation()
        {
            // Se parsea la respuesta de ChatGPT.
            _ration = _petCareManager.ExtractRationsFromText(_resultBot);
            Debug.Log($"FOOD BUTTON -Rations parsed-: {_ration}");

            // Se envía la información a AttributeManager.
            _petCareManager.ActivateFoodAction(_ration, _inputTMP.text);

            // Se desactiva el panel.
            _submenuPanel.SetActive(false);
            Animation_PageSliding.Instance.ActivatePageSliding();
        }
    }
}