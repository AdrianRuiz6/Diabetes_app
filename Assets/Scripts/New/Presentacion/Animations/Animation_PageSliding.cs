// code from https://pressstart.vip/tutorials/2019/06/1/96/level-selector.html

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Animation_PageSliding : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 panelLocation;
    [SerializeField] private float percentThreshold = 0.2f;
    [SerializeField] private float easing = 0.2f;
    [SerializeField] private int totalPages = 5;
    [SerializeField] private int initialPage = 3;
    private int currentPage = 1;
    private bool _isWorking = true;

    public static Animation_PageSliding Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentPage = initialPage;
        panelLocation = transform.position;
    }
    public void OnDrag(PointerEventData data)
    {
        if (!_isWorking)
            return;

        float difference = data.pressPosition.x - data.position.x;
        transform.position = panelLocation - new Vector3(difference, 0, 0);
    }
    public void OnEndDrag(PointerEventData data)
    {
        if (!_isWorking)
            return;

        float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = panelLocation;
            if (percentage > 0 && currentPage < totalPages)
            {
                currentPage++;
                newLocation += new Vector3(-Screen.width, 0, 0);
            }
            else if (percentage < 0 && currentPage > 1)
            {
                currentPage--;
                newLocation += new Vector3(Screen.width, 0, 0);
            }
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
        }
    }
    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }

    public void MoveToInitialPage()
    {
        int pageDifference = initialPage - currentPage;

        Vector3 newLocation = panelLocation;
        currentPage = initialPage;
        newLocation += new Vector3(-Screen.width * pageDifference, 0, 0);

        StartCoroutine(SmoothMove(transform.position, newLocation, easing));

        panelLocation = newLocation;
    }

    public void ActivatePageSliding()
    {
        _isWorking = true;
    }

    public void DeactivatePageSliding()
    {
        _isWorking = false;
    }
}
