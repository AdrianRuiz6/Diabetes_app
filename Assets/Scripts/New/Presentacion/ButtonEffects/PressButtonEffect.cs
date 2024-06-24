using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PressButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 _pressedScale;
    private float _pressedFactor;
    private float _pressedAnimationDuration;

    private Vector3 _unpressedScale;
    private float _unpressedAnimationDuration;

    private TiltEffect [] _tiltEffectChildren;

    void Start()
    {
        _pressedFactor = 0.8f;
        _pressedScale = new Vector3
            (
            transform.localScale.x * _pressedFactor,
            transform.localScale.y * _pressedFactor,
            transform.localScale.z * _pressedFactor
            );
        _pressedAnimationDuration = 0.2f;

        _unpressedScale = transform.localScale;
        _unpressedAnimationDuration = 0.2f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(OnPointerDownAnimation());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartCoroutine(OnPointerUpAnimation());
    }

    IEnumerator OnPointerDownAnimation()
    {
        yield return transform.DOScale(_pressedScale, _pressedAnimationDuration).WaitForCompletion();
    }
    
    IEnumerator OnPointerUpAnimation()
    {
        yield return transform.DOScale(_unpressedScale, _unpressedAnimationDuration).WaitForCompletion();
    }
}
