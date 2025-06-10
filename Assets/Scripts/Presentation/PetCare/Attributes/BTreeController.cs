using Master.Domain.BehaviorTree;
using Master.Domain.Connection;
using Master.Domain.GameEvents;
using Master.Domain.PetCare;
using Master.Domain.Settings;
using System;
using UnityEngine;

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

        void OnDestroy()
        {
            GameEvents_PetCare.OnExecuteAttributesBTree -= OnAttributeExecution;
        }

        void Start()
        {
            _petCareManager = ServiceLocator.Instance.GetService<IPetCareManager>();
            _settingsManager = ServiceLocator.Instance.GetService<ISettingsManager>();
            _connectionManager = ServiceLocator.Instance.GetService<IConnectionManager>();

            switch (_treeType)
            {
                case TreeType.Activity:
                    _bTree = new BT_Activity(_petCareManager, _settingsManager, _connectionManager);
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

        private void OnAttributeExecution(DateTime currentDateTime)
        {
            _bTree.EnqueueAttribute(currentDateTime);
        }
    }
}