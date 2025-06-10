using Master.Auxiliar;
using Master.Persistence.Questions;
using System.Collections.Generic;

namespace Master.Domain.Questions
{
    public interface IUserPerformanceManager
    {
        public void InitializePerformance(List<string> allTopics);

        public bool HasPendingAnswers(string topic);

        public void UpdatePerformance(List<Question> iterationQuestions);

        public FixedSizeQueue<string> GetTopicPerformance(string topic);
    }
}