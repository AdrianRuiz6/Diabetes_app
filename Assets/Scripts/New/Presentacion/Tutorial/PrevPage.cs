using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrevPage : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(ActivatePrevPage);
    }

    private void ActivatePrevPage()
    {
        GameEventsTutorial.OnPrevPage?.Invoke();
    }
}
