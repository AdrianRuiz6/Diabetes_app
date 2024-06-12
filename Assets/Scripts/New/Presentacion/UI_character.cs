using Master.Domain.Events;
using UnityEngine.UI;
using UnityEngine;

public class UI_character : MonoBehaviour
{
    private Image _characterImage;

    private void Awake()
    {
        GameEventsEconomy.OnProductEquiped += OnProductEquiped;
    }

    void OnDestroy()
    {
        GameEventsEconomy.OnProductEquiped -= OnProductEquiped;
    }

    private void Start()
    {
        // Inicialización de Imagen del personaje.
        _characterImage = GetComponent<Image>();
    }

    private void OnProductEquiped(string productName, Color sellingColor)
    {
        _characterImage.color = sellingColor;
    }
}
