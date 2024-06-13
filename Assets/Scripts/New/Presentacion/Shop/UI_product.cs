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

    private void Awake()
    {
        GameEventsEconomy.OnProductEquiped += OnProductEquiped;
        GameEventsEconomy.OnProductBought += OnProductBought;

        _boughtName = "Bought_icon_" + _productName;
        _equipedName = "Equiped_icon_" + _productName;

        _isBought = false;
    }

    void OnDestroy()
    {
        GameEventsEconomy.OnProductEquiped -= OnProductEquiped;
        GameEventsEconomy.OnProductBought -= OnProductBought;
    }

    private void OnProductEquiped(string productName, Color sellingColor)
    {
        DeleteBoughtIcon();

        if (productName == _productName)
        {
            _isBought = true;

            GameObject equipedIconInstance = Instantiate(_EquipedIcon, transform.position, transform.rotation);
            equipedIconInstance.name = _equipedName;
            equipedIconInstance.transform.SetParent(transform);
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
        if(productName == _productName)
        {
            _isBought = true;

            GameObject boughtIconInstance = Instantiate(_BoughtIcon, transform.position, transform.rotation);
            boughtIconInstance.name = _boughtName;
            boughtIconInstance.transform.SetParent(transform);
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
