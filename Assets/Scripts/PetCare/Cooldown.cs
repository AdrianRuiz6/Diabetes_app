using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    [SerializeField] private string _my_ID;

    private float _time = 3600;

    private DateTime? _timeButtonPressed;

    private Button _button;
    private Image _image;

    void Start()
    {
        _timeButtonPressed = null;

        _image = GetComponent<Image>();

        _button = GetComponent<Button>();
        _button.onClick.AddListener(ActivateCooldown);
    }

    private void Awake()
    {
        GameEventsPetCare.OnActivateCoolDown += ActivateCoolDownRemotely;
    }

    // TODO: guardar hora (en DataStorage) en la que se activó el botón con el ID de esta instancia.
    // Cargar en el otro lado (AISimulator)
    void OnDestroy() 
    {
        GameEventsPetCare.OnActivateCoolDown -= ActivateCoolDownRemotely;

        if (_timeButtonPressed.HasValue)
        {
            PlayerPrefs.SetString(_my_ID, _timeButtonPressed.Value.ToString());
        }
        else
        {
            PlayerPrefs.SetString(_my_ID, "null"); // Guarda "null" si el valor es null
        }
        PlayerPrefs.Save();
    }

    private void ActivateCooldown()
    {
        _image.color = Color.grey;
        _button.enabled = false;
        StartCoroutine(Timer(_time));
    }

    private void DisableCooldown()
    {
        _image.color = Color.white;
        _button.enabled = true;
    }

    private IEnumerator Timer(float time)
    {
        _timeButtonPressed = DateTime.Now;
        yield return new WaitForSeconds(time);

        DisableCooldown();
    }

    private void ActivateCoolDownRemotely(string externalID, float externalTime)
    {
        if(externalID == _my_ID)
        {
            StopAllCoroutines();
            _image.color = Color.grey;
            _button.enabled = false;
            StartCoroutine(Timer(externalTime));
        }
    }
}
