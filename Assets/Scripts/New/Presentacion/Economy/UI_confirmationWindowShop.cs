using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_ConfirmationWindowShop : MonoBehaviour
{
    [SerializeField] private GameObject _windowPanel;
    [SerializeField] private TextMeshProUGUI _priceTMP;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    GameObject _currentProductImage;

    private Action onConfirmPurchase;

    public static UI_ConfirmationWindowShop Instance;

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
        _windowPanel.SetActive(false);
        confirmButton.onClick.AddListener(ConfirmPurchase);
        cancelButton.onClick.AddListener(CancelPurchase);
    }

    public void ShowConfirmationWindow(GameObject productImage, string productPrice, Action confirmAction)
    {
        PageSliding.Instance.DeactivatePageSliding();

        float left = 242.8945f;
        float top = 712.4613f;
        float right = 242.8943f;
        float bottom = 648.4613f;
        float posZ = 0.9980469f;

        _currentProductImage = Instantiate(productImage, _windowPanel.transform);
        RectTransform rectTransformProductImage = _currentProductImage.GetComponent<RectTransform>();
        rectTransformProductImage.offsetMin = new Vector2(left, bottom);
        rectTransformProductImage.offsetMax = new Vector2(-right, -top);
        Vector3 position = rectTransformProductImage.localPosition;
        position.z = posZ;
        rectTransformProductImage.localPosition = position;

        _priceTMP.text = productPrice;
        onConfirmPurchase = confirmAction;

        _windowPanel.SetActive(true);
    }

    private void ConfirmPurchase()
    {
        PageSliding.Instance.ActivatePageSliding();

        onConfirmPurchase?.Invoke();
        _windowPanel.SetActive(false);
        Destroy(_currentProductImage);
    }

    private void CancelPurchase()
    {
        PageSliding.Instance.ActivatePageSliding();

        _windowPanel.SetActive(false);
        Destroy(_currentProductImage);
    }
}
