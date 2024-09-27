using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextPage : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(ActivateNextPage);
    }

    private void ActivateNextPage()
    {
        GameEventsTutorial.OnNextPage?.Invoke();
    }
}
