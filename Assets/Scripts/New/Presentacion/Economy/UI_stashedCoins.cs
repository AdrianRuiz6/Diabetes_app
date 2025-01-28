using Master.Domain.Economy;
using Master.Domain.Events;
using UnityEngine;
using UnityEngine.UI;

public class UI_stashedCoins : MonoBehaviour
{
    private Button _stashedCoinsBtn;
    [SerializeField] private GameObject _stashedCoinsObject;
    [SerializeField] private GameObject _winCoinsPrefab;

    void Awake()
    {
        GameEventsEconomy.OnStashedCoinsUpdated += UpdateStashedCoins;
    }

    void OnDestroy()
    {
        GameEventsEconomy.OnStashedCoinsUpdated -= UpdateStashedCoins;
    }

    void Start()
    {
        _stashedCoinsBtn = GetComponent<Button>();
        _stashedCoinsBtn.onClick.AddListener(OnStashedCoinsClicked);
        UpdateStashedCoins(DataStorage.LoadStashedCoins());
    }

    private void UpdateStashedCoins(int coins)
    {
        _stashedCoinsObject.SetActive(coins > 0);
    }

    public void OnStashedCoinsClicked()
    {
        if(_stashedCoinsObject.activeSelf)
        {
            EconomyManager.Instance.StashedCoinsToTotalCoins();
            _stashedCoinsObject.SetActive(false);

            // Reproducir sonido y sistema de particulas monedas.
            SoundManager.Instance.PlaySoundEffect("CollectMoney");
            Vector3 localMousePosition = transform.InverseTransformPoint(Input.mousePosition);
            AnimationManager.Instance.PlayAnimation(_winCoinsPrefab, new Vector3(localMousePosition.x - 6, localMousePosition.y, localMousePosition.z), new Vector3(2.5f,5f,1), transform.gameObject);
        }
    }
}
