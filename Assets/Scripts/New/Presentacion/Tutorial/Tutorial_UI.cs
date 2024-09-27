using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_UI : MonoBehaviour
{
    private PageSliding _pageSliding;

    [SerializeField] private GameObject _Tutorial_Section;
    private List<GameObject> _pagesList;

    private void Awake()
    {
        GameEventsTutorial.OnOpenTutorial += OpenTutorial;
        GameEventsTutorial.OnCloseTutorial += CloseTutorial;
        GameEventsTutorial.OnNextPage += NextPage;
        GameEventsTutorial.OnPrevPage += PreviousPage;
    }

    void Start()
    {
        _pagesList = new List<GameObject>();
        _pageSliding = GetComponent<PageSliding>();

        GetAllPages();

        if (DataStorage.LoadIsFirstUsage() == true)
        {
            OpenTutorial();
        }
        else
        {
            CloseTutorial();
        }
    }

    private void OnDestroy()
    {
        DataStorage.SaveIsFirstUsage();

        GameEventsTutorial.OnOpenTutorial -= OpenTutorial;
        GameEventsTutorial.OnCloseTutorial -= CloseTutorial;
        GameEventsTutorial.OnNextPage -= NextPage;
        GameEventsTutorial.OnPrevPage -= PreviousPage;
    }

    private void GetAllPages()
    {
        for(int childIndex = 0;  childIndex < _Tutorial_Section.transform.childCount; childIndex++)
        {
            GameObject newPage = _Tutorial_Section.transform.GetChild(childIndex).gameObject;
            _pagesList.Add(newPage);
        }
    }

    private void OpenTutorial()
    {
        _pageSliding.MoveToInitialPage();
        _pageSliding.enabled = false;
        _Tutorial_Section.SetActive(true);

        foreach (GameObject page in _pagesList)
        {
            page.SetActive(false);
        }
        _pagesList[0].SetActive(true);
    }

    private void CloseTutorial()
    {
        _pageSliding.enabled = true;
        _Tutorial_Section.SetActive(false);

        foreach (GameObject page in _pagesList)
        {
            page.SetActive(false);
        }
    }

    private void NextPage()
    {
        for(int childIndex = 0; childIndex < _pagesList.Count; childIndex++)
        {
            if (_pagesList[childIndex].activeSelf == true)
            {
                _pagesList[childIndex].SetActive(false);
                _pagesList[++childIndex].SetActive(true);
            }
        }
    }

    private void PreviousPage()
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
