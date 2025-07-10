using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Persistence.Questions
{
    [System.Serializable]
    public class IterationQuestionsDataList
    {
        public List<IterationQuestionData> questions = new List<IterationQuestionData>();
    }
}