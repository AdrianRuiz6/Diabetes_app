using System.Collections.Generic;
using UnityEngine;

public class UI_Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject _Tutorial_Section;
    private List<GameObject> _pagesList;

    void Start()
    {
        _pagesList = new List<GameObject>();

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
    }

    private void GetAllPages()
    {
        for(int childIndex = 0;  childIndex < _Tutorial_Section.transform.childCount; childIndex++)
        {
            GameObject newPage = _Tutorial_Section.transform.GetChild(childIndex).gameObject;
            _pagesList.Add(newPage);
        }
    }

    public void OpenTutorial()
    {
        PageSliding.Instance.MoveToInitialPage();
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
        for(int childIndex = 0; childIndex < _pagesList.Count; childIndex++)
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
