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

        private void Awake()
        {
            GameEvents_Score.OnModifyHighestScore += ModifyHighestScoreTMP;
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        private void OnDestroy()
        {
            GameEvents_Score.OnModifyHighestScore -= ModifyHighestScoreTMP;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_Score.OnModifyHighestScore -= ModifyHighestScoreTMP;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_Score.OnModifyHighestScore -= ModifyHighestScoreTMP;
        }
#endif

        void Start()
        {
            _scoreManager = ServiceLocator.Instance.GetService<IScoreManager>();

            _highestScore_TMP = GetComponent<TMP_Text>();
            ModifyHighestScoreTMP(_scoreManager.highestScore);
        }

        private void ModifyHighestScoreTMP(int highestScore)
        {
            _highestScore_TMP.text = highestScore.ToString();
        }
    }
}