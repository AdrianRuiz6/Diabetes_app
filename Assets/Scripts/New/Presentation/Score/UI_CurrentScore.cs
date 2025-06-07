using System;
using TMPro;
using UnityEngine;
using Master.Domain.GameEvents;
using Master.Domain.Score;

namespace Master.Presentation.Score
{
    public class UI_CurrentScore : MonoBehaviour
    {
        private TMP_Text _currentScore_TMP;

        private void Awake()
        {
            GameEvents_Score.OnResetScore += ResetCurrentScoreTMP;
            GameEvents_Score.OnModifyCurrentScore += ModifyCurrentScoreTMP;
        }

        private void OnDestroy()
        {
            GameEvents_Score.OnResetScore -= ResetCurrentScoreTMP;
            GameEvents_Score.OnModifyCurrentScore -= ModifyCurrentScoreTMP;
        }

        void Start()
        {
            _currentScore_TMP = GetComponent<TMP_Text>();
            ModifyCurrentScoreTMP(ScoreManager.currentScore);
        }

        private void ModifyCurrentScoreTMP(int currentScore)
        {
            _currentScore_TMP.text = currentScore.ToString();
        }

        private void ResetCurrentScoreTMP()
        {
            _currentScore_TMP.text = 0.ToString();
        }
    }
}