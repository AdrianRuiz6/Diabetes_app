using Master.Domain.Shop;
using Master.Domain.GameEvents;
using UnityEngine;
using UnityEngine.UI;
using Master.Presentation.Animations;
using Master.Presentation.Sound;
using Master.Infrastructure;

namespace Master.Presentation.Shop
{
    public class UI_StashedCoins : MonoBehaviour
    {
        private Button _stashedCoinsBtn;
        [SerializeField] private GameObject _stashedCoinsObject;
        [SerializeField] private GameObject _winCoinsPrefab;

        IEconomyManager _economyManager;

        void Awake()
        {
            GameEvents_Shop.OnStashedCoinsUpdated += UpdateStashedCoins;
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        void OnDestroy()
        {
            GameEvents_Shop.OnStashedCoinsUpdated -= UpdateStashedCoins;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_Shop.OnStashedCoinsUpdated -= UpdateStashedCoins;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_Shop.OnStashedCoinsUpdated -= UpdateStashedCoins;
        }
#endif

        void Start()
        {
            _economyManager = ServiceLocator.Instance.GetService<IEconomyManager>();

            _stashedCoinsBtn = GetComponent<Button>();
            _stashedCoinsBtn.onClick.AddListener(OnStashedCoinsClicked);

            UpdateStashedCoins();
        }

        private void UpdateStashedCoins()
        {
            _stashedCoinsObject.SetActive(_economyManager.stashedCoins > 0);
        }

        public void OnStashedCoinsClicked()
        {
            if (_stashedCoinsObject.activeSelf)
            {
                _economyManager.StashedCoinsToTotalCoins();

                // Reproducir sonido y sistema de particulas monedas.
                SoundManager.Instance.PlaySoundEffect("Interaction");
                Vector3 localMousePosition = transform.InverseTransformPoint(Input.mousePosition);
                AnimationManager.Instance.PlayAnimation(_winCoinsPrefab, new Vector3(localMousePosition.x - 6, localMousePosition.y, localMousePosition.z), new Vector3(2.5f, 5f, 1), transform.gameObject);
                _stashedCoinsObject.SetActive(false);
            }
        }
    }
}