using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits_UI : MonoBehaviour
{
    [SerializeField] private GameObject _Credits_Section;

    void Start()
    {
        CloseCredits();
    }

    public void OpenCredits()
    {
        _Credits_Section.SetActive(true);
    }

    public void CloseCredits()
    {
        _Credits_Section.SetActive(false);
    }
}
