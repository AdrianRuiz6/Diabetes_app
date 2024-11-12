using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AttributeChanger : MonoBehaviour
{
    [SerializeField] private Button _glycemiaButton;
    [SerializeField] private Button _activityButton;
    [SerializeField] private Button _hungerButton;

    private DateTime _currentDate = DateTime.Now.Date;

    void Start()
    {
        _glycemiaButton.onClick.AddListener(UpdateGlycemia);
        _activityButton.onClick.AddListener(UpdateActivity);
        _hungerButton.onClick.AddListener(UpdateHunger);
    }

    private void UpdateGlycemia()
    {
        GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Glycemia);
    }
    private void UpdateActivity()
    {
        GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Activity);
    }
    private void UpdateHunger()
    {
        GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Hunger);
    }
}
