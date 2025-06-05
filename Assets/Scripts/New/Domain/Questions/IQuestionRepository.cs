using Master.Auxiliar;
using System.Collections.Generic;

namespace Master.Domain.Questions
{
    public interface IQuestionRepository
    {
        public void SaveCurrentQuestionIndex(int currentQuestionIndex);

        public int LoadCurrentQuestionIndex();

        public void SaveTimeLeftQuestionTimer(float timeLeft);

        public float LoadTimeLeftQuestionTimer();

        public void SaveUserPerformance(Dictionary<string, FixedSizeQueue<string>> userPerformance);

        public Dictionary<string, FixedSizeQueue<string>> LoadUserPerformance(List<string> allTopics);

        public void ResetUserPerformance();

        public void SaveIterationQuestions(List<Question> iterationQuestions);

        public List<Question> LoadIterationQuestions();

        public void ResetIterationQuestions();

        public List<Question> LoadQuestions();

        public int SaveURLQuestions(string url);

        public string LoadURLQuestions();

        public void ResetQuestionURL();
    }
}