using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UI_product : MonoBehaviour
{
    [SerializeField] private string _productName;
    [SerializeField] private GameObject _BoughtIcon;
    [SerializeField] private GameObject _EquipedIcon;

    private string _boughtName;
    private string _equipedName;

    private bool _isBought;

    private Vector3 _buttonsPosition;

    private void Awake()
    {
        GameEventsEconomy.OnProductEquipped += OnProductEquiped;
        GameEventsEconomy.OnProductBought += OnProductBought;

        _boughtName = "Bought_icon_" + _productName;
        _equipedName = "Equiped_icon_" + _productName;

        _buttonsPosition = new Vector3(-3.4000001f, -41.2999992f, 0.998049974f);

        _isBought = false;
    }

    void OnDestroy()
    {
        GameEventsEconomy.OnProductEquipped -= OnProductEquiped;
        GameEventsEconomy.OnProductBought -= OnProductBought;
    }

    private void OnProductEquiped(string productName)
    {
        DeleteBoughtIcon();

        if (productName == _productName)
        {
            _isBought = true;

            GameObject equipedIconInstance = Instantiate(_EquipedIcon, _buttonsPosition, transform.rotation);
            equipedIconInstance.name = _equipedName;
            equipedIconInstance.transform.SetParent(transform, false);
        }
        else
        {
            DeleteEquipedIcon();
            if(_isBought == true)
            {
                OnProductBought(_productName);
            }
        }
    }

    private void OnProductBought(string productName)
    {
        DeleteEquipedIcon();

        if(productName == _productName)
        {
            _isBought = true;

            GameObject boughtIconInstance = Instantiate(_BoughtIcon, _buttonsPosition, transform.rotation);
            boughtIconInstance.name = _boughtName;
            boughtIconInstance.transform.SetParent(transform, false);
        }
    }

    private void DeleteBoughtIcon()
    {
        GameObject boughtIconToDestroy = GameObject.Find(_boughtName);

        if (boughtIconToDestroy != null)
            Destroy(boughtIconToDestroy);
    }

    private void DeleteEquipedIcon()
    {
        GameObject EquipedIconToDestroy = GameObject.Find(_equipedName);

        if(EquipedIconToDestroy != null)
            Destroy(EquipedIconToDestroy);
    }
}
