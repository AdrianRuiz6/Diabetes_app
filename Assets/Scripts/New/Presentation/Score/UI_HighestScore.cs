using UnityEngine;
using TMPro;
using Master.Domain.GameEvents;
using Master.Domain.Score;

namespace Master.Presentation.Score
{
    public class UI_HighestScore : MonoBehaviour
    {
        private TMP_Text _highestScore_TMP;

        private void Awake()
        {
            GameEvents_Score.OnModifyHighestScore += ModifyHighestScoreTMP;
        }

        private void OnDestroy()
        {
            GameEvents_Score.OnModifyHighestScore -= ModifyHighestScoreTMP;
        }

        void Start()
        {
            _highestScore_TMP = GetComponent<TMP_Text>();
            ModifyHighestScoreTMP(ScoreManager.highestScore);
        }

        private void ModifyHighestScoreTMP(int highestScore)
        {
            _highestScore_TMP.text = highestScore.ToString();
        }
    }
}