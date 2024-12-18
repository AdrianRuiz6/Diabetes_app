using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

public abstract class ButtonsPetCare : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    [SerializeField] protected TMP_Text ValueTMP;

    [SerializeField] private Button _openButton;
    [SerializeField] private Button _sendButton;
    [SerializeField] private Button _closeButton;

    [SerializeField] private GameObject _submenuPanel;

    void Start()
    {
        _slider.onValueChanged.AddListener(UpdatedValueSlider);

        _openButton.onClick.AddListener(OpenSubMenu);
        _closeButton.onClick.AddListener(CloseSubMenu);
        _sendButton.onClick.AddListener(SendInformation);
    }

    public abstract void UpdatedValueSlider(float value);

    private void OpenSubMenu()
    {
        PageSliding.Instance.DeactivatePageSliding();
        _submenuPanel.SetActive(true);

        _slider.value = 1;
        UpdatedValueSlider(1);
    }

    private void CloseSubMenu()
    {
        PageSliding.Instance.ActivatePageSliding();
        _submenuPanel.SetActive(false);
    }

    public virtual void SendInformation()
    {
        _submenuPanel.SetActive(false);
    }
}
