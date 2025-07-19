using UnityEngine;
using TMPro;
using Master.Domain.GameEvents;
using Master.Infrastructure;

namespace Master.Presentation.Score
{
    public class UI_HighestScore : MonoBehaviour
    {
        private TMP_Text _highestScore_TMP;

        private IScoreManager _scoreManager;

        private bool _isSimulationFinished = false;

        private void Awake()
        {
            GameEvents_Score.OnModifyHighestScore += ModifyHighestScoreTMP;
            GameEvents_PetCare.OnFinishedSimulation += StartUI;
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        private void OnDestroy()
        {
            GameEvents_Score.OnModifyHighestScore -= ModifyHighestScoreTMP;
            GameEvents_PetCare.OnFinishedSimulation -= StartUI;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_Score.OnModifyHighestScore -= ModifyHighestScoreTMP;
            GameEvents_PetCare.OnFinishedSimulation -= StartUI;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_Score.OnModifyHighestScore -= ModifyHighestScoreTMP;
            GameEvents_PetCare.OnFinishedSimulation -= StartUI;
        }
#endif

        void Start()
        {
            _scoreManager = ServiceLocator.Instance.GetService<IScoreManager>();

            _highestScore_TMP = GetComponent<TMP_Text>();
        }

        private void ModifyHighestScoreTMP(int highestScore)
        {
            if (!_isSimulationFinished)
                return;
            _highestScore_TMP.text = highestScore.ToString();
        }

        private void StartUI()
        {
            _isSimulationFinished = true;
            ModifyHighestScoreTMP(_scoreManager.highestScore);
        }
    }
}