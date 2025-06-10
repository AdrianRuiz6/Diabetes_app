using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

using Master.Auxiliar;
using Master.Domain.GameEvents;
using Master.Persistence.Questions;
using Master.Persistence.Connection;
using Master.Domain.Connection;

namespace Master.Domain.Questions
{
    public class QuestionManager : IQuestionManager
    {
        IQuestionRepository _questionRepository;
        IUserPerformanceManager _userPerformanceManager;
        IConnectionManager _connectionManager;

        public Dictionary<string, List<Question>> allQuestions { private set; get; }
        private List<Question> _iterationQuestions;
        public int currentQuestionIndex { private set; get; } = 0;
        private int _maxQuestionIndex = 10;
        public float maxTimerSeconds { private set; get; } = 30f;

        public bool isFSMExecuting { private set; get; }

        System.Random random = new System.Random();

        public QuestionManager(IQuestionRepository questionRepository, IUserPerformanceManager userPerformanceManager, IConnectionManager connectionManager)
        {
            _questionRepository = questionRepository;
            _userPerformanceManager = userPerformanceManager;
            _connectionManager = connectionManager;

            isFSMExecuting = false;
        }

        public bool InitializeQuestions(bool isChangingQuestions = false)
        {
            GameEvents_Questions.OnActivateLoadingPanelUI.Invoke();

            allQuestions = new Dictionary<string, List<Question>>();
            List<string> allTopics = new List<string>();
            List<Question> allQuestionsList = new List<Question>();
            UtilityFunctions.CopyList(_questionRepository.LoadQuestions(), allQuestionsList);

            if (allQuestionsList == null || allQuestionsList.Count == 0)
            {
                return false;
            }

            foreach (Question question in allQuestionsList)
            {
                if (allQuestions.ContainsKey(question.topic))
                {
                    allQuestions[question.topic].Add(question);
                }
                else
                {
                    List<Question> questions = new List<Question>() { question };
                    allQuestions.Add(question.topic, questions);
                }
            }

            foreach (var kvp in allQuestions)
            {
                allTopics.Add(kvp.Key);
            }

            _userPerformanceManager.InitializePerformance(allTopics);

            _iterationQuestions = _questionRepository.LoadIterationQuestions();
            currentQuestionIndex = _questionRepository.LoadCurrentQuestionIndex();

            if (_iterationQuestions.Count == 0)
            {
                isFSMExecuting = true;
                GameEvents_Questions.OnActivateLoadingPanelUI?.Invoke();
            }
            else
            {
                FinishQuestionSearch();
            }

            if (isChangingQuestions == false)
            {
                InitializeTimerQuestions();
            }


            return true;
        }

        public void StartQuestionSearch()
        {
            isFSMExecuting = true;
            GameEvents_Questions.OnActivateTimerPanelUI.Invoke();
            GameEvents_Questions.OnActivateLoadingPanelUI?.Invoke();
            GameEvents_Questions.OnPrepareTimerUI?.Invoke(maxTimerSeconds);
        }

        public void FinishQuestionSearch()
        {
            isFSMExecuting = false;

            GameEvents_Questions.OnFinishQuestionSearch?.Invoke();
            GameEvents_Questions.OnDeactivateLoadingPanelUI.Invoke();
        }

        public void FinishTimerQuestions()
        {
            GameEvents_Questions.OnActivateQuestionPanelUI.Invoke();
            GameEvents_Questions.OnPrepareFirstQuestionUI.Invoke();
        }

        public void InitializeTimerQuestions()
        {
            float previousTimeLeft = _questionRepository.LoadTimeLeftQuestionTimer();
            DateTime lastTimeDisconnected = _connectionManager.lastDisconnectionDateTime;

            TimeSpan timeDisconnected = DateTime.Now - lastTimeDisconnected;
            float currentTimeLeft = previousTimeLeft - (float)timeDisconnected.TotalSeconds;

            if (currentTimeLeft > 0)
            {
                GameEvents_Questions.OnActivateTimerPanelUI.Invoke();
                GameEvents_Questions.OnPrepareTimerUI?.Invoke(currentTimeLeft);
            }
            else
            {
                FinishTimerQuestions();
            }
        }

        #region Calculate Proportions
        public Dictionary<string, float> CalculateAppearanceProportions()
        {
            // Calculates the percentage of successes in _iterationQuestions.
            Dictionary<string, float> topicAccuracy = CalculateAccuracy();

            // Calculates inverse percentage based on topicAccuracy. 
            Dictionary<string, float> inverseTopicAccuracy = CalculatePriority(topicAccuracy);

            // Normalizes inverse percentages.
            Dictionary<string, float> normalizedProportions = CalculateNormal(inverseTopicAccuracy);

            return normalizedProportions;
        }

        private Dictionary<string, float> CalculateAccuracy()
        {
            Dictionary<string, float> topicAccuracy = new Dictionary<string, float>();

            foreach (var topic in allQuestions.Keys)
            {
                FixedSizeQueue<string> userPerformanceTopic = _userPerformanceManager.GetTopicPerformance(topic);

                if (userPerformanceTopic.Count() != 0)
                {
                    int correctAnswers = 0;
                    foreach (string answer in userPerformanceTopic)
                    {
                        if (answer == "S")
                            correctAnswers++;
                    }
                    float accuracy = (float)correctAnswers / _maxQuestionIndex * 100;
                    topicAccuracy[topic] = accuracy;
                    Debug.Log($"Accuracy for {topic}: {accuracy}");
                }
            }
            return topicAccuracy;
        }

        private Dictionary<string, float> CalculatePriority(Dictionary<string, float> topicAccuracy)
        {
            Dictionary<string, float> topicPriority = new Dictionary<string, float>();
            foreach (var topic in topicAccuracy.Keys)
            {
                float priority = 100 - topicAccuracy[topic];
                topicPriority[topic] = priority;
                Debug.Log($"Priority for {topic}: {priority}");
            }

            return topicPriority;
        }

        private Dictionary<string, float> CalculateNormal(Dictionary<string, float> topicPriority)
        {
            float sumPriority = topicPriority.Values.Sum();
            Dictionary<string, float> proportions = new Dictionary<string, float>();
            foreach (var topic in topicPriority.Keys)
            {
                float proportion = (topicPriority[topic] / sumPriority) * 10;
                proportions[topic] = proportion;
                Debug.Log($"Proportion for {topic}: {proportion}");
            }

            return proportions;
        }
        #endregion

        public Dictionary<string, int> AdjustQuestionCount(Dictionary<string, float> appearanceProportions)
        {
            Dictionary<string, int> amountQuestionsPerTopic = new Dictionary<string, int>();

            // Calculates the amount of questions per topic.
            foreach (var topic in appearanceProportions.Keys)
            {
                amountQuestionsPerTopic[topic] = Mathf.RoundToInt(appearanceProportions[topic]);
                Debug.Log($"Amount of questions for {topic}: {amountQuestionsPerTopic[topic]}");
            }

            int totalAmountQuestions = amountQuestionsPerTopic.Values.Sum();

            while (totalAmountQuestions != 10)
            {
                if (totalAmountQuestions > 10)
                {
                    List<string> descendingTopicAmount = amountQuestionsPerTopic.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

                    foreach (var topic in descendingTopicAmount)
                    {
                        amountQuestionsPerTopic[topic]--;
                        totalAmountQuestions--;
                        Debug.Log($"Decreased questions for {topic}: {amountQuestionsPerTopic[topic]}");

                        if (totalAmountQuestions == 10)
                            break;
                    }
                }
                else if (totalAmountQuestions < 10)
                {
                    List<string> ascendingTopicAmount = amountQuestionsPerTopic.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

                    foreach (var topic in ascendingTopicAmount)
                    {
                        amountQuestionsPerTopic[topic]++;
                        totalAmountQuestions++;
                        Debug.Log($"Increased questions for {topic}: {amountQuestionsPerTopic[topic]}");

                        if (totalAmountQuestions == 10)
                            break;
                    }
                }
            }

            return amountQuestionsPerTopic;
        }

        public void SelectRandomQuestions(Dictionary<string, int> amountQuestionsPerTopic = null)
        {
            if (amountQuestionsPerTopic == null)
            {
                amountQuestionsPerTopic = new Dictionary<string, int>();

                while (amountQuestionsPerTopic.Values.Sum() < 10)
                {
                    foreach (string topic in allQuestions.Keys)
                    {
                        if (_userPerformanceManager.HasPendingAnswers(topic))
                            if (amountQuestionsPerTopic.ContainsKey(topic))
                            {
                                amountQuestionsPerTopic[topic]++;
                            }
                            else
                            {
                                amountQuestionsPerTopic.Add(topic, 1);
                            }

                        if (amountQuestionsPerTopic.Values.Sum() == 10)
                            break;
                    }
                }
            }

            foreach (string topic in amountQuestionsPerTopic.Keys)
            {
                int amount = amountQuestionsPerTopic[topic];

                while (amount > 0)
                {
                    AddRandomQuestion(topic, amountQuestionsPerTopic);
                    amount--;
                }
            }
        }

        private void AddRandomQuestion(string topic, Dictionary<string, int> amountQuestionsPerTopic)
        {
            bool repeatedQuestion = false;
            Question newQuestion;
            int attempts = 0;
            int maxAttempts = 100;

            do
            {
                newQuestion = allQuestions[topic][random.Next(allQuestions[topic].Count)];
                repeatedQuestion = _iterationQuestions.Contains(newQuestion);
                attempts++;
            } while (repeatedQuestion && attempts < maxAttempts);

            _iterationQuestions.Add(newQuestion);
        }

        public void RandomizeOrderQuestions()
        {
            UtilityFunctions.RandomizeList(_iterationQuestions);

            foreach (Question question in _iterationQuestions)
            {
                question.RandomizeOrderAnswers();
            }

            _questionRepository.SaveIterationQuestions(_iterationQuestions);
        }

        public Question NextQuestion()
        {
            if (currentQuestionIndex + 1 < _maxQuestionIndex)
            {
                currentQuestionIndex++;
                SetCurrentQuestionIndex(currentQuestionIndex);

                return _iterationQuestions[currentQuestionIndex];
            }
            else
            {
                SearchNewQuestions();
                return null;
            }
        }

        private void SearchNewQuestions()
        {
            _userPerformanceManager.UpdatePerformance(_iterationQuestions);
            _iterationQuestions = new List<Question>();

            SetCurrentQuestionIndex(0);

            StartQuestionSearch();
        }

        public string GetCorrectAnswer()
        {
            return _iterationQuestions[0].correctAnswer;
        }

        public void SetCurrentQuestionIndex(int newCurrentQuestionIndex)
        {
            currentQuestionIndex = newCurrentQuestionIndex;
            _questionRepository.SaveCurrentQuestionIndex(currentQuestionIndex);
        }
    }
}