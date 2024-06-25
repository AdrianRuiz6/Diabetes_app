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
    [SerializeField] private GameObject _EquippedIcon;

    private string _boughtName;
    private string _equippedName;

    private bool _isBought;

    private Vector3 _buttonsPosition;

    private void Awake()
    {
        GameEventsEconomy.OnProductEquipped += OnProductEquipped;
        GameEventsEconomy.OnProductBought += OnProductBought;

        _boughtName = "Bought_icon_" + _productName;
        _equippedName = "Equipped_icon_" + _productName;

        _buttonsPosition = new Vector3(-3.4000001f, -41.2999992f, 0.998049974f);

        _isBought = false;
    }

    void OnDestroy()
    {
        GameEventsEconomy.OnProductEquipped -= OnProductEquipped;
        GameEventsEconomy.OnProductBought -= OnProductBought;
    }

    private void OnProductEquipped(string productName)
    {
        if (productName == _productName)
        {
            DeleteBoughtIcon();
            _isBought = true;

            GameObject equippedIconInstance = Instantiate(_EquippedIcon, _buttonsPosition, transform.rotation);
            equippedIconInstance.name = _equippedName;
            equippedIconInstance.transform.SetParent(transform, false);
        }
        else
        {
            DeleteEquippedIcon();
            if (_isBought == true)
            {
                OnProductBought(_productName);
            }
        }
    }

    private void OnProductBought(string productName)
    {
        if (productName == _productName)
        {
            DeleteEquippedIcon();
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

    private void DeleteEquippedIcon()
    {
        GameObject EquipedIconToDestroy = GameObject.Find(_equippedName);

        if (EquipedIconToDestroy != null)
            Destroy(EquipedIconToDestroy);
    }
}
