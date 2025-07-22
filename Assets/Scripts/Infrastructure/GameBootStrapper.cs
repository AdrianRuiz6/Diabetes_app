using Master.Domain.Connection;
using Master.Domain.PetCare;
using Master.Domain.PetCare.Log;
using Master.Domain.Questions;
using Master.Domain.Score;
using Master.Domain.Settings;
using Master.Domain.Shop;
using Master.Persistence.Connection;
using Master.Persistence.PetCare;
using Master.Persistence.Questions;
using Master.Persistence.Score;
using Master.Persistence.Settings;
using Master.Persistence.Shop;
using System.Collections;
using UnityEngine;

namespace Master.Infrastructure
{
    [DefaultExecutionOrder(-1000)]
    public class GameBootStrapper : MonoBehaviour
    {
        private IConnectionManager _connectionManager;
        private IConnectionRepository _connectionRepository;

        private ISettingsManager _settingsManager;
        private ISettingsRepository _settingsRepository;

        private IScoreManager _scoreManager;
        private IScoreLogManager _scoreLogManager;
        private IScoreRepository _scoreRepository;

        private IEconomyManager _economyManager;
        private IShopRepository _shopRepository;

        private IUserPerformanceManager _userPerformanceManager;
        private IQuestionManager _questionManager;
        private IQuestionRepository _questionRepository;

        private IPetCareLogManager _petCareLogManager;
        private IPetCareManager _petCareManager;
        private IAISimulatorManager _aimulatorManager;
        private IPetCareRepository _petCareRepository;

        private IChatBot _chatBot;

        private void Awake()
        {
#if UNITY_STANDALONE_WIN
            int screenHeight = Screen.currentResolution.height;
            int screenWidth = Mathf.RoundToInt(screenHeight * 9f / 16f);
            Screen.SetResolution(screenWidth, screenHeight, true);
#endif

#if UNITY_STANDALONE_LINUX
            int screenHeight = 864;
            int screenWidth = Mathf.RoundToInt(screenHeight * 9f / 16f);
            Screen.SetResolution(screenWidth, screenHeight, false);
#endif
            
            DontDestroyOnLoad(gameObject);
            InitializeServices();
        }

        private void InitializeServices()
        {
            EnvReader.Load();

            _connectionRepository = new DataStorage_Connection();
            _settingsRepository = new DataStorage_Settings();
            _scoreRepository = new DataStorage_Score();
            _shopRepository = new DataStorage_Shop();
            _questionRepository = new DataStorage_Questions();
            _petCareRepository = new DataStorage_PetCare();
            _chatBot = new OpenAIChatBot();

            _connectionManager = new ConnectionManager(_connectionRepository);
            _settingsManager = new SettingsManager(_settingsRepository, _questionRepository);
            _scoreLogManager = new ScoreLogManager(_scoreRepository);
            _scoreManager = new ScoreManager(_scoreRepository, _connectionManager, _scoreLogManager);
            _economyManager = new EconomyManager(_shopRepository);
            _userPerformanceManager = new UserPerformanceManager(_questionRepository);
            _questionManager = new QuestionManager(_questionRepository, _userPerformanceManager, _connectionManager, _economyManager, _scoreManager);
            _petCareLogManager = new PetCareLogManager(_petCareRepository);
            _petCareManager = new PetCareManager(_chatBot, _petCareRepository, _petCareLogManager, _scoreManager, _economyManager, _connectionManager);
            _aimulatorManager = new AISimulatorManager(_petCareManager, _connectionManager, _settingsManager);

            // Inicializacion SettingsManager
            _settingsManager.InitializeDependencies(_petCareManager, _petCareLogManager, _questionManager, _scoreManager);

            // Registro de servicios en ServiceLocator
            ServiceLocator.Instance.RegisterService(_connectionManager);
            ServiceLocator.Instance.RegisterService(_settingsManager);
            ServiceLocator.Instance.RegisterService(_scoreManager);
            ServiceLocator.Instance.RegisterService(_scoreLogManager);
            ServiceLocator.Instance.RegisterService(_economyManager);
            ServiceLocator.Instance.RegisterService(_userPerformanceManager);
            ServiceLocator.Instance.RegisterService(_questionManager);
            ServiceLocator.Instance.RegisterService(_petCareLogManager);
            ServiceLocator.Instance.RegisterService(_petCareManager);
            ServiceLocator.Instance.RegisterService(_aimulatorManager);
        }
    }
}