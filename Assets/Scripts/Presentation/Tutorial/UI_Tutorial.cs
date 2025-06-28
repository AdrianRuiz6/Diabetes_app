using System.Collections.Generic;
using UnityEngine;
using Master.Presentation.Animations;
using Master.Domain.Connection;
using Master.Infrastructure;

namespace Master.Presentation.Tutorial
{
    public class UI_Tutorial : MonoBehaviour
    {
        [SerializeField] private GameObject _Tutorial_Section;
        private List<GameObject> _pagesList;

        IConnectionManager _connectionManager;

        void Start()
        {
            _connectionManager = ServiceLocator.Instance.GetService<IConnectionManager>();

            _pagesList = new List<GameObject>();
            GetAllPages();

            CheckToOpenTutorial();
        }

        private void CheckToOpenTutorial()
        {
            if (_connectionManager.isFirstUsage == true)
            {
                OpenTutorial();
                _connectionManager.SetIsFirstUsage(false);
            }
            else
            {
                CloseTutorial();
            }
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
            Animation_PageSliding.Instance.DeactivatePageSliding();
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
            Animation_PageSliding.Instance.ActivatePageSliding();

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