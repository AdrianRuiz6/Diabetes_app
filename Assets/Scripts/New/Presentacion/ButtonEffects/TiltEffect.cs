using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TiltEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private float _tiltAngle = 1f;
    private float _startingRotationOffset;

    private bool _isPressButtonEffect;

    private void Start()
    {
        _startingRotationOffset = Random.Range(0f, 2f * Mathf.PI);

        PressButtonEffect pressButtonEffect = GetComponentInParent<PressButtonEffect>();
        if (pressButtonEffect != null)
        {
            _isPressButtonEffect = true;
        }
        else
        {
            _isPressButtonEffect = false;
        }
    }

    private void Update()
    {
        RotateButton();
    }

    private void RotateButton()
    {
        float rotationAngle = Time.time * _rotationSpeed + _startingRotationOffset;
        float tiltX = Mathf.Sin(rotationAngle) * _tiltAngle;
        float tiltY = Mathf.Cos(rotationAngle) * _tiltAngle;

        transform.rotation = Quaternion.Euler(tiltX, tiltY, transform.rotation.eulerAngles.z);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isPressButtonEffect)
        {
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerDownHandler);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isPressButtonEffect)
        {
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerUpHandler);
        }
    }
}
