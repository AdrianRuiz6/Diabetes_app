using TMPro;
using UnityEngine;
using Master.Domain.GameEvents;
using Master.Infrastructure;

namespace Master.Presentation.Score
{
    public class UI_CurrentScore : MonoBehaviour
    {
        private TMP_Text _currentScore_TMP;

        private IScoreManager _scoreManager;

        private bool _isSimulationFinished = false;

        private void Awake()
        {
            GameEvents_Score.OnResetScore += ResetCurrentScoreTMP;
            GameEvents_Score.OnModifyCurrentScore += ModifyCurrentScoreTMP;
            GameEvents_PetCare.OnFinishedSimulation += StartUI;
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        private void OnDestroy()
        {
            GameEvents_Score.OnResetScore -= ResetCurrentScoreTMP;
            GameEvents_Score.OnModifyCurrentScore -= ModifyCurrentScoreTMP;
            GameEvents_PetCare.OnFinishedSimulation -= StartUI;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_Score.OnResetScore -= ResetCurrentScoreTMP;
                GameEvents_Score.OnModifyCurrentScore -= ModifyCurrentScoreTMP;
                GameEvents_PetCare.OnFinishedSimulation -= StartUI;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_Score.OnResetScore -= ResetCurrentScoreTMP;
            GameEvents_Score.OnModifyCurrentScore -= ModifyCurrentScoreTMP;
            GameEvents_PetCare.OnFinishedSimulation -= StartUI;
        }
#endif

        void Start()
        {
            _scoreManager = ServiceLocator.Instance.GetService<IScoreManager>();
            _currentScore_TMP = GetComponent<TMP_Text>();
        }

        private void ModifyCurrentScoreTMP(int currentScore)
        {
            if (!_isSimulationFinished)
                return;
            _currentScore_TMP.text = currentScore.ToString();
        }

        private void ResetCurrentScoreTMP()
        {
            if (!_isSimulationFinished)
                return;
            _currentScore_TMP.text = 0.ToString();
        }

        private void StartUI()
        {
            _isSimulationFinished = true;
            ModifyCurrentScoreTMP(_scoreManager.currentScore);
        }
    }
}