using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    [SerializeField] private string _myID;
    private Image _image;

    private bool _isInCD;

    void Start()
    {
        _image = GetComponent<Image>();
        _isInCD = false;
    }

    private void Awake()
    {
        GameEventsPetCare.OnActivateCoolDown += ActivateCoolDown;
        GameEventsPetCare.OnDeactivateCoolDown += DeactivateCoolDown;
    }

    void OnDestroy() 
    {
        GameEventsPetCare.OnActivateCoolDown -= ActivateCoolDown;
        GameEventsPetCare.OnDeactivateCoolDown -= DeactivateCoolDown;
    }

    void Update()
    {
        if (_isInCD)
        {
            ActivateEffect();
        }
    }

    private void ActivateCoolDown(string externalID)
    {
        if(externalID == _myID)
        {
            _isInCD = true;
        }
    }

    private void DeactivateCoolDown(string externalID)
    {
        if(externalID == _myID)
        {
            _isInCD = false;

            DeactivateEffect();
        }
    }

    private void ActivateEffect()
    {
        // TODO: efecto de activado.
    }

    private void DeactivateEffect()
    {
        // TODO: efecto de desactivado.
    }
}
