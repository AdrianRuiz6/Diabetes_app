using UnityEngine;
using UnityEngine.UI;
using Master.Domain.PetCare.Log;
using Master.Domain.PetCare;
using Master.Domain.Score;

namespace Master.Presentation.PetCare.Log
{
    public class UI_AttributeChanger : MonoBehaviour
    {
        [SerializeField] private Button _glycemiaButton;
        [SerializeField] private Button _activityButton;
        [SerializeField] private Button _hungerButton;

        private IPetCareLogManager _petCareLogManager;

        void Start()
        {
            _petCareLogManager = ServiceLocator.Instance.GetService<IPetCareLogManager>();

            _glycemiaButton.onClick.AddListener(UpdateGlycemia);
            _activityButton.onClick.AddListener(UpdateActivity);
            _hungerButton.onClick.AddListener(UpdateHunger);
        }

        private void UpdateGlycemia()
        {
            _petCareLogManager.SetAttributeFilter(AttributeType.Glycemia);
        }
        private void UpdateActivity()
        {
            _petCareLogManager.SetAttributeFilter(AttributeType.Activity);
        }
        private void UpdateHunger()
        {
            _petCareLogManager.SetAttributeFilter(AttributeType.Hunger);
        }
    }
}