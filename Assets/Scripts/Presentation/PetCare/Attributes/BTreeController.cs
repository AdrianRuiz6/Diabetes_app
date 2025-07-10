using Master.Domain.BehaviorTree;
using Master.Domain.Connection;
using Master.Domain.GameEvents;
using Master.Domain.PetCare;
using Master.Domain.Settings;
using UnityEngine;
using Master.Infrastructure;

namespace Master.Presentation.PetCare
{
    public class BTreeController : MonoBehaviour
    {
        [SerializeField] private TreeType _treeType;
        private BTree _bTree;

        private IPetCareManager _petCareManager;
        private ISettingsManager _settingsManager;
        private IConnectionManager _connectionManager;

        void Awake()
        {
            GameEvents_PetCare.OnExecuteAttributesBTree += OnAttributeExecution;
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        void OnDestroy()
        {
            GameEvents_PetCare.OnExecuteAttributesBTree -= OnAttributeExecution;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_PetCare.OnExecuteAttributesBTree -= OnAttributeExecution;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_PetCare.OnExecuteAttributesBTree -= OnAttributeExecution;
        }
#endif


        void Start()
        {
            _petCareManager = ServiceLocator.Instance.GetService<IPetCareManager>();
            _settingsManager = ServiceLocator.Instance.GetService<ISettingsManager>();
            _connectionManager = ServiceLocator.Instance.GetService<IConnectionManager>();

            switch (_treeType)
            {
                case TreeType.Energy:
                    _bTree = new BT_Energy(_petCareManager, _settingsManager, _connectionManager);
                    break;
                case TreeType.Glycemia:
                    _bTree = new BT_Glycemia(_petCareManager, _settingsManager, _connectionManager);
                    break;
                case TreeType.Hunger:
                    _bTree = new BT_Hunger(_petCareManager, _settingsManager, _connectionManager);
                    break;
            }
        }

        void Update()
        {
            _bTree.Run();
        }

        private void OnAttributeExecution(AttributeUpdateIntervalInfo intervalInfo)
        {
            Debug.Log($"[BTreeController - {_treeType}] Recibido intervalo {intervalInfo.dateTime}");
            _bTree.EnqueueAttribute(intervalInfo);
        }
    }
}