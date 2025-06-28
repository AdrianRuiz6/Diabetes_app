using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Master.Presentation.Animations;

namespace Master.Presentation.PetCare
{
    public abstract class UI_Actions_PetCare : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        [SerializeField] protected TMP_Text ValueTMP;

        [SerializeField] private Button _openButton;
        [SerializeField] private Button _sendButton;
        [SerializeField] private Button _closeButton;

        [SerializeField] private GameObject _submenuPanel;

        protected virtual void Start()
        {
            _slider.onValueChanged.AddListener(UpdatedValueSlider);

            _openButton.onClick.AddListener(OpenSubMenu);
            _closeButton.onClick.AddListener(CloseSubMenu);
            _sendButton.onClick.AddListener(SendInformation);
        }

        public abstract void UpdatedValueSlider(float value);

        private void OpenSubMenu()
        {
            Animation_PageSliding.Instance.DeactivatePageSliding();
            _submenuPanel.SetActive(true);

            _slider.value = 1;
            UpdatedValueSlider(1);
        }

        private void CloseSubMenu()
        {
            Animation_PageSliding.Instance.ActivatePageSliding();
            _submenuPanel.SetActive(false);
        }

        public virtual void SendInformation()
        {
            Animation_PageSliding.Instance.ActivatePageSliding();
            _submenuPanel.SetActive(false);
        }
    }
}