using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Master.Domain.PetCare.Log;
using Master.Infrastructure;

namespace Master.Presentation.PetCare.Log
{
    public class UI_DateChanger : MonoBehaviour
    {
        [SerializeField] private TMP_Text _date_TMP;
        [SerializeField] private Button _previousDate;
        [SerializeField] private Button _nextDate;

        private IPetCareLogManager _petCareLogManager;

        void Start()
        {
            _petCareLogManager = ServiceLocator.Instance.GetService<IPetCareLogManager>();

            _date_TMP.text = $"{_petCareLogManager.currentDateFilter.ToString("dd")} / {_petCareLogManager.currentDateFilter.ToString("MM")} / {_petCareLogManager.currentDateFilter.Year - 2000}";

            _previousDate.onClick.AddListener(PreviousDate);
            _nextDate.onClick.AddListener(NextDate);
        }

        private void UpdateDate()
        {
            _date_TMP.text = $"{_petCareLogManager.currentDateFilter.ToString("dd")} / {_petCareLogManager.currentDateFilter.ToString("MM")} / {_petCareLogManager.currentDateFilter.Year - 2000}";
        }

        private void PreviousDate()
        {
            _petCareLogManager.ModifyDayFilter(-1);
            UpdateDate();
        }

        private void NextDate()
        {
            _petCareLogManager.ModifyDayFilter(+1);
            UpdateDate();
        }
    }
}