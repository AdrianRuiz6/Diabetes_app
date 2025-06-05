using System.Collections.Generic;
using UnityEngine;
using Master.Presentation.Animations;
using Master.Persistence;
using Master.Persistence.Connection;
using Master.Domain.GameEvents;

namespace Master.Presentation.Tutorial
{
    public class UI_Tutorial : MonoBehaviour
    {
        [SerializeField] private GameObject _Tutorial_Section;
        private List<GameObject> _pagesList;

        private void Awake()
        {
            GameEvents_Tutorial.OnOpenTutorial += OpenTutorial;
            GameEvents_Tutorial.OnCloseTutorial += CloseTutorial;
        }

        private void OnDestroy()
        {
            GameEvents_Tutorial.OnOpenTutorial -= OpenTutorial;
            GameEvents_Tutorial.OnCloseTutorial -= CloseTutorial;
        }

        void Start()
        {
            _pagesList = new List<GameObject>();

            GetAllPages();
        }

        private void GetAllPages()
        {
            for (int childIndex = 0; childIndex < _Tutorial_Section.transform.childCount; childIndex++)
            {
                GameObject newPage = _Tutorial_Section.transform.GetChild(childIndex).gameObject;
                _pagesList.Add(newPage);
            }
        }

        public void OpenTutorial()
        {
            Animation_PageSliding.Instance.MoveToInitialPage();
            _Tutorial_Section.SetActive(true);

            foreach (GameObject page in _pagesList)
            {
                page.SetActive(false);
            }
            _pagesList[0].SetActive(true);
        }

        public void CloseTutorial()
        {
            _Tutorial_Section.SetActive(false);

            foreach (GameObject page in _pagesList)
            {
                page.SetActive(false);
            }
        }

        public void NextPage()
        {
            for (int childIndex = 0; childIndex < _pagesList.Count; childIndex++)
            {
                if (_pagesList[childIndex].activeSelf == true)
                {
                    _pagesList[childIndex].SetActive(false);
                    _pagesList[++childIndex].SetActive(true);
                }
            }
        }

        public void PreviousPage()
        {
            for (int childIndex = 0; childIndex < _pagesList.Count; childIndex++)
            {
                if (_pagesList[childIndex].activeSelf == true)
                {
                    _pagesList[childIndex].SetActive(false);
                    _pagesList[--childIndex].SetActive(true);
                }
            }
        }
    }
}