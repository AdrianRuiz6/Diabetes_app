using System.Collections.Generic;
using Master.Auxiliar;

namespace Master.Domain.Questions
{
    public class UserPerformanceManager : IUserPerformanceManager
    {
        IQuestionRepository _questionRepository;

        private Dictionary<string, FixedSizeQueue<string>> _userPerformance;

        public UserPerformanceManager(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public void InitializePerformance(List<string> allTopics)
        {
            _userPerformance = new Dictionary<string, FixedSizeQueue<string>>();
            Dictionary<string, FixedSizeQueue<string>>  loadedPerformance = new Dictionary<string, FixedSizeQueue<string>>();

            UtilityFunctions.CopyDictionaryPerformance(_questionRepository.LoadUserPerformance(), loadedPerformance);

            foreach (string topic in allTopics)
            {
                if (!loadedPerformance.TryGetValue(topic, out FixedSizeQueue<string> specificTopicPerformance))
                {
                    specificTopicPerformance = new FixedSizeQueue<string>();
                }
                _userPerformance[topic] = specificTopicPerformance;
            }

            foreach (var kvp in _userPerformance)
            {
                while (kvp.Value.Count() < 10)
                {
                    kvp.Value.Enqueue("P");
                }
            }
        }

        public bool HasPendingAnswers(string topic)
        {
            FixedSizeQueue<string> topicPerformanceQueue = GetTopicPerformance(topic);
            return topicPerformanceQueue.Contains("P");
        }

        public void UpdatePerformance(List<Question> iterationQuestions)
        {
            foreach (var question in iterationQuestions)
            {
                FixedSizeQueue<string> specificTopicPerformance = GetTopicPerformance(question.topic);
                specificTopicPerformance.Enqueue(question.resultAnswer);
            }
            _questionRepository.SaveUserPerformance(_userPerformance);
        }

        public FixedSizeQueue<string> GetTopicPerformance(string topic)
        {
            if (!_userPerformance.TryGetValue(topic, out FixedSizeQueue<string> specificTopicPerformance))
            {
                specificTopicPerformance = new FixedSizeQueue<string>();
                _userPerformance[topic] = specificTopicPerformance;
            }
            return specificTopicPerformance;
        }
    }
}