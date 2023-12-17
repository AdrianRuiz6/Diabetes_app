using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{

    public float time;
    private float timer;

    private bool isCoolingDown = false;

    private Button button;
    private Image image;

    void Start()
    {
        timer = time;

        image = GetComponent<Image>();

        button = GetComponent<Button>();
        button.onClick.AddListener(ActivateCooldown);
    }

    void Update()
    {
        if (isCoolingDown)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                DisableCooldown();
            }
        }
        else
        {
            DisableCooldown();
        }
    }

    public void ActivateCooldown()
    {
        image.color = Color.grey;
        button.enabled = false;
        isCoolingDown = true;
    }

    public void DisableCooldown()
    {
        image.color = Color.white;
        button.enabled = true;
        isCoolingDown = false;
        timer = time;
    }
}
