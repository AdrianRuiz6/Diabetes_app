using Master.Auxiliar;
using Master.Domain.Connection;
using Master.Domain.GameEvents;
using Master.Domain.Questions;
using Master.Domain.Shop;
using Master.Infrastructure;
using Master.Presentation.Questions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Master.Tests
{
    public class GenerateNewQuestionsTests
    {
        [SetUp]
        public void SetUp()
        {
            GameEvents_Questions.OnActivateLoadingPanelUI = () => { };
            GameEvents_Questions.OnDeactivateLoadingPanelUI = () => { };
            GameEvents_Questions.OnActivateTimerPanelUI = () => { };
            GameEvents_Questions.OnPrepareTimerUI = (float value) => { };
            GameEvents_Questions.OnActivateQuestionPanelUI = () => { };
            GameEvents_Questions.OnFinishQuestionSearch = () => { };
        }

        [UnityTest]
        public IEnumerator Generate_FullReset()
        {
            // Crar entrada de rendimiento del usuario
            DummyQuestionRepository questionRepository = new DummyQuestionRepository();

            // Actividades Extraordinarias
            Dictionary<string, FixedSizeQueue<string>> newUserPerformance = new Dictionary<string, FixedSizeQueue<string>>();
            FixedSizeQueue<string> performance = new FixedSizeQueue<string>();
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            newUserPerformance.Add("Actividades Extraordinarias", performance);

            // Alimentación y Ejercicio
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            newUserPerformance.Add("Alimentación y Ejercicio", performance);

            // Hipoglucemia e Hiperglucemia
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            newUserPerformance.Add("Hipoglucemia e Hiperglucemia", performance);

            // Situaciones Extremas
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            newUserPerformance.Add("Situaciones Extremas", performance);

            // Teoría
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            performance.Enqueue("P");
            newUserPerformance.Add("Teoría", performance);

            questionRepository.SaveUserPerformance(newUserPerformance);


            // Inicializar QuestionManager, AgentQuestions y UserPerformanceManager
            ServiceLocator.Instance.ClearServices();
            IUserPerformanceManager userPerformanceManager = new UserPerformanceManager(questionRepository);
            IQuestionManager questionManager = new QuestionManager(questionRepository, userPerformanceManager, new DummyConectionManager(), new DummyEconomyManager(), new DummyScoreManager());
            ServiceLocator.Instance.RegisterService(userPerformanceManager);
            ServiceLocator.Instance.RegisterService(questionManager);
            GameObject gameObjectTest = new GameObject("AgentQuestionsTest");
            AgentQuestions agentQuestions = gameObjectTest.AddComponent<AgentQuestions>();

            // Generar preguntas
            questionManager.InitializeQuestions();

            // Contabilizar la cantidad de preguntas de cada tema
            while (questionManager.isFSMExecuting)
            {
                yield return new UnityEngine.WaitForSeconds(1f);
            }

            Dictionary<string, int> result = new Dictionary<string, int>
            {
                { "Actividades Extraordinarias", 0 },
                { "Alimentación y Ejercicio", 0 },
                { "Hipoglucemia e Hiperglucemia", 0 },
                { "Situaciones Extremas", 0 },
                { "Teoría", 0 }
            };

            for (int i = 0; i < 10; i++)
            {
                Question currentQuestion = questionManager.GetNextQuestion();
                result[currentQuestion.topic]++;
                questionManager.Answer(".");
            }


            // Comprobar si el test ha sido un éxito
            Assert.AreEqual(2, result["Actividades Extraordinarias"]);
            Assert.AreEqual(2, result["Alimentación y Ejercicio"]);
            Assert.AreEqual(2, result["Hipoglucemia e Hiperglucemia"]);
            Assert.AreEqual(2, result["Situaciones Extremas"]);
            Assert.AreEqual(2, result["Teoría"]);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Generate_0P_0P_0P_1P_1P()
        {
            // Crar entrada de rendimiento del usuario
            DummyQuestionRepository questionRepository = new DummyQuestionRepository();

            // Actividades Extraordinarias
            Dictionary<string, FixedSizeQueue<string>> newUserPerformance = new Dictionary<string, FixedSizeQueue<string>>();
            FixedSizeQueue<string> performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            newUserPerformance.Add("Actividades Extraordinarias", performance);

            // Alimentación y Ejercicio
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            newUserPerformance.Add("Alimentación y Ejercicio", performance);

            // Hipoglucemia e Hiperglucemia
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            newUserPerformance.Add("Hipoglucemia e Hiperglucemia", performance);

            // Situaciones Extremas
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("P");
            newUserPerformance.Add("Situaciones Extremas", performance);

            // Teoría
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("P");
            newUserPerformance.Add("Teoría", performance);

            questionRepository.SaveUserPerformance(newUserPerformance);


            // Inicializar QuestionManager, AgentQuestions y UserPerformanceManager
            ServiceLocator.Instance.ClearServices();
            IUserPerformanceManager userPerformanceManager = new UserPerformanceManager(questionRepository);
            IQuestionManager questionManager = new QuestionManager(questionRepository, userPerformanceManager, new DummyConectionManager(), new DummyEconomyManager(), new DummyScoreManager());
            ServiceLocator.Instance.RegisterService(userPerformanceManager);
            ServiceLocator.Instance.RegisterService(questionManager);
            GameObject gameObjectTest = new GameObject("AgentQuestionsTest");
            AgentQuestions agentQuestions = gameObjectTest.AddComponent<AgentQuestions>();

            // Generar preguntas
            questionManager.InitializeQuestions();

            // Contabilizar la cantidad de preguntas de cada tema
            while(questionManager.isFSMExecuting)
            {
                yield return new UnityEngine.WaitForSeconds(1f);
            }

            Dictionary<string, int> result = new Dictionary<string, int>
            {
                { "Actividades Extraordinarias", 0 },
                { "Alimentación y Ejercicio", 0 },
                { "Hipoglucemia e Hiperglucemia", 0 },
                { "Situaciones Extremas", 0 },
                { "Teoría", 0 }
            };

            for (int i = 0; i < 10; i++)
            {
                Question currentQuestion = questionManager.GetNextQuestion();
                result[currentQuestion.topic]++;
                questionManager.Answer(".");
            }


            // Comprobar si el test ha sido un éxito
            Assert.AreEqual(0, result["Actividades Extraordinarias"]);
            Assert.AreEqual(0, result["Alimentación y Ejercicio"]);
            Assert.AreEqual(0, result["Hipoglucemia e Hiperglucemia"]);
            Assert.AreEqual(5, result["Situaciones Extremas"]);
            Assert.AreEqual(5, result["Teoría"]);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Generate_5S5F_5S5F_5S5F_5S5F_5S5F()
        {
            // Crar entrada de rendimiento del usuario
            DummyQuestionRepository questionRepository = new DummyQuestionRepository();

            // Actividades Extraordinarias
            Dictionary<string, FixedSizeQueue<string>> newUserPerformance = new Dictionary<string, FixedSizeQueue<string>>();
            FixedSizeQueue<string> performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            newUserPerformance.Add("Actividades Extraordinarias", performance);

            // Alimentación y Ejercicio
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            newUserPerformance.Add("Alimentación y Ejercicio", performance);

            // Hipoglucemia e Hiperglucemia
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            newUserPerformance.Add("Hipoglucemia e Hiperglucemia", performance);

            // Situaciones Extremas
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            newUserPerformance.Add("Situaciones Extremas", performance);

            // Teoría
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            newUserPerformance.Add("Teoría", performance);

            questionRepository.SaveUserPerformance(newUserPerformance);


            // Inicializar QuestionManager, AgentQuestions y UserPerformanceManager
            ServiceLocator.Instance.ClearServices();
            IUserPerformanceManager userPerformanceManager = new UserPerformanceManager(questionRepository);
            IQuestionManager questionManager = new QuestionManager(questionRepository, userPerformanceManager, new DummyConectionManager(), new DummyEconomyManager(), new DummyScoreManager());
            ServiceLocator.Instance.RegisterService(userPerformanceManager);
            ServiceLocator.Instance.RegisterService(questionManager);
            GameObject gameObjectTest = new GameObject("AgentQuestionsTest");
            AgentQuestions agentQuestions = gameObjectTest.AddComponent<AgentQuestions>();

            // Generar preguntas
            questionManager.InitializeQuestions();

            // Contabilizar la cantidad de preguntas de cada tema
            while (questionManager.isFSMExecuting)
            {
                yield return new UnityEngine.WaitForSeconds(1f);
            }

            Dictionary<string, int> result = new Dictionary<string, int>
            {
                { "Actividades Extraordinarias", 0 },
                { "Alimentación y Ejercicio", 0 },
                { "Hipoglucemia e Hiperglucemia", 0 },
                { "Situaciones Extremas", 0 },
                { "Teoría", 0 }
            };

            for (int i = 0; i < 10; i++)
            {
                Question currentQuestion = questionManager.GetNextQuestion();
                result[currentQuestion.topic]++;
                questionManager.Answer(".");
            }


            // Comprobar si el test ha sido un éxito
            Assert.AreEqual(2, result["Actividades Extraordinarias"]);
            Assert.AreEqual(2, result["Alimentación y Ejercicio"]);
            Assert.AreEqual(2, result["Hipoglucemia e Hiperglucemia"]);
            Assert.AreEqual(2, result["Situaciones Extremas"]);
            Assert.AreEqual(2, result["Teoría"]);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Generate_2S8F_5S5F_7S3F_1S9F_10S0F()
        {
            // Crar entrada de rendimiento del usuario
            DummyQuestionRepository questionRepository = new DummyQuestionRepository();

            // Actividades Extraordinarias
            Dictionary<string, FixedSizeQueue<string>> newUserPerformance = new Dictionary<string, FixedSizeQueue<string>>();
            FixedSizeQueue<string> performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            newUserPerformance.Add("Actividades Extraordinarias", performance);

            // Alimentación y Ejercicio
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            newUserPerformance.Add("Alimentación y Ejercicio", performance);

            // Hipoglucemia e Hiperglucemia
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            newUserPerformance.Add("Hipoglucemia e Hiperglucemia", performance);

            // Situaciones Extremas
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            performance.Enqueue("F");
            newUserPerformance.Add("Situaciones Extremas", performance);

            // Teoría
            performance = new FixedSizeQueue<string>();
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            performance.Enqueue("S");
            newUserPerformance.Add("Teoría", performance);

            questionRepository.SaveUserPerformance(newUserPerformance);


            // Inicializar QuestionManager, AgentQuestions y UserPerformanceManager
            ServiceLocator.Instance.ClearServices();
            IUserPerformanceManager userPerformanceManager = new UserPerformanceManager(questionRepository);
            IQuestionManager questionManager = new QuestionManager(questionRepository, userPerformanceManager, new DummyConectionManager(), new DummyEconomyManager(), new DummyScoreManager());
            ServiceLocator.Instance.RegisterService(userPerformanceManager);
            ServiceLocator.Instance.RegisterService(questionManager);
            GameObject gameObjectTest = new GameObject("AgentQuestionsTest");
            AgentQuestions agentQuestions = gameObjectTest.AddComponent<AgentQuestions>();

            // Generar preguntas
            questionManager.InitializeQuestions();

            // Contabilizar la cantidad de preguntas de cada tema
            while (questionManager.isFSMExecuting)
            {
                yield return new UnityEngine.WaitForSeconds(1f);
            }

            Dictionary<string, int> result = new Dictionary<string, int>
            {
                { "Actividades Extraordinarias", 0 },
                { "Alimentación y Ejercicio", 0 },
                { "Hipoglucemia e Hiperglucemia", 0 },
                { "Situaciones Extremas", 0 },
                { "Teoría", 0 }
            };

            for (int i = 0; i < 10; i++)
            {
                Question currentQuestion = questionManager.GetNextQuestion();
                result[currentQuestion.topic]++;
                questionManager.Answer(".");
            }


            // Comprobar si el test ha sido un éxito
            Assert.AreEqual(3, result["Actividades Extraordinarias"]);
            Assert.AreEqual(2, result["Alimentación y Ejercicio"]);
            Assert.AreEqual(1, result["Hipoglucemia e Hiperglucemia"]);
            Assert.AreEqual(4, result["Situaciones Extremas"]);
            Assert.AreEqual(0, result["Teoría"]);

            yield return null;
        }

        // Dummies
        private class DummyQuestionRepository : IQuestionRepository
        {
            private Dictionary<string, FixedSizeQueue<string>> _userPerformance = new Dictionary<string, FixedSizeQueue<string>>();

            public int LoadCurrentQuestionIndex()
            {
                return 0;
            }

            public List<Question> LoadIterationQuestions()
            {
                return new List<Question>();
            }

            public List<Question> LoadQuestions()
            {
                return new List<Question>
                {
                    // Actividades Extraordinarias
                    new Question("Actividades Extraordinarias", "¿Pregunta 1?", "A", "B", "C", "A", "Consejo"),
                    new Question("Actividades Extraordinarias", "¿Pregunta 2?", "A", "B", "C", "B", "Consejo"),
                    new Question("Actividades Extraordinarias", "¿Pregunta 3?", "A", "B", "C", "C", "Consejo"),
                    new Question("Actividades Extraordinarias", "¿Pregunta 4?", "A", "B", "C", "A", "Consejo"),
                    new Question("Actividades Extraordinarias", "¿Pregunta 5?", "A", "B", "C", "B", "Consejo"),
                    new Question("Actividades Extraordinarias", "¿Pregunta 6?", "A", "B", "C", "C", "Consejo"),
                    new Question("Actividades Extraordinarias", "¿Pregunta 7?", "A", "B", "C", "A", "Consejo"),
                    new Question("Actividades Extraordinarias", "¿Pregunta 8?", "A", "B", "C", "B", "Consejo"),
                    new Question("Actividades Extraordinarias", "¿Pregunta 9?", "A", "B", "C", "C", "Consejo"),
                    new Question("Actividades Extraordinarias", "¿Pregunta 10?", "A", "B", "C", "A", "Consejo"),

                    // Alimentación y Ejercicio
                    new Question("Alimentación y Ejercicio", "¿Pregunta 11?", "A", "B", "C", "B", "Consejo"),
                    new Question("Alimentación y Ejercicio", "¿Pregunta 12?", "A", "B", "C", "C", "Consejo"),
                    new Question("Alimentación y Ejercicio", "¿Pregunta 13?", "A", "B", "C", "A", "Consejo"),
                    new Question("Alimentación y Ejercicio", "¿Pregunta 14?", "A", "B", "C", "B", "Consejo"),
                    new Question("Alimentación y Ejercicio", "¿Pregunta 15?", "A", "B", "C", "C", "Consejo"),
                    new Question("Alimentación y Ejercicio", "¿Pregunta 16?", "A", "B", "C", "A", "Consejo"),
                    new Question("Alimentación y Ejercicio", "¿Pregunta 17?", "A", "B", "C", "B", "Consejo"),
                    new Question("Alimentación y Ejercicio", "¿Pregunta 18?", "A", "B", "C", "C", "Consejo"),
                    new Question("Alimentación y Ejercicio", "¿Pregunta 19?", "A", "B", "C", "A", "Consejo"),
                    new Question("Alimentación y Ejercicio", "¿Pregunta 20?", "A", "B", "C", "B", "Consejo"),

                    // Hipoglucemia e Hiperglucemia
                    new Question("Hipoglucemia e Hiperglucemia", "¿Pregunta 21?", "A", "B", "C", "C", "Consejo"),
                    new Question("Hipoglucemia e Hiperglucemia", "¿Pregunta 22?", "A", "B", "C", "A", "Consejo"),
                    new Question("Hipoglucemia e Hiperglucemia", "¿Pregunta 23?", "A", "B", "C", "B", "Consejo"),
                    new Question("Hipoglucemia e Hiperglucemia", "¿Pregunta 24?", "A", "B", "C", "C", "Consejo"),
                    new Question("Hipoglucemia e Hiperglucemia", "¿Pregunta 25?", "A", "B", "C", "A", "Consejo"),
                    new Question("Hipoglucemia e Hiperglucemia", "¿Pregunta 26?", "A", "B", "C", "B", "Consejo"),
                    new Question("Hipoglucemia e Hiperglucemia", "¿Pregunta 27?", "A", "B", "C", "C", "Consejo"),
                    new Question("Hipoglucemia e Hiperglucemia", "¿Pregunta 28?", "A", "B", "C", "A", "Consejo"),
                    new Question("Hipoglucemia e Hiperglucemia", "¿Pregunta 29?", "A", "B", "C", "B", "Consejo"),
                    new Question("Hipoglucemia e Hiperglucemia", "¿Pregunta 30?", "A", "B", "C", "C", "Consejo"),

                    // Situaciones Extremas
                    new Question("Situaciones Extremas", "¿Pregunta 31?", "A", "B", "C", "A", "Consejo"),
                    new Question("Situaciones Extremas", "¿Pregunta 32?", "A", "B", "C", "B", "Consejo"),
                    new Question("Situaciones Extremas", "¿Pregunta 33?", "A", "B", "C", "C", "Consejo"),
                    new Question("Situaciones Extremas", "¿Pregunta 34?", "A", "B", "C", "A", "Consejo"),
                    new Question("Situaciones Extremas", "¿Pregunta 35?", "A", "B", "C", "B", "Consejo"),
                    new Question("Situaciones Extremas", "¿Pregunta 36?", "A", "B", "C", "C", "Consejo"),
                    new Question("Situaciones Extremas", "¿Pregunta 37?", "A", "B", "C", "A", "Consejo"),
                    new Question("Situaciones Extremas", "¿Pregunta 38?", "A", "B", "C", "B", "Consejo"),
                    new Question("Situaciones Extremas", "¿Pregunta 39?", "A", "B", "C", "C", "Consejo"),
                    new Question("Situaciones Extremas", "¿Pregunta 40?", "A", "B", "C", "A", "Consejo"),

                    // Teoría
                    new Question("Teoría", "¿Pregunta 41?", "A", "B", "C", "B", "Consejo"),
                    new Question("Teoría", "¿Pregunta 42?", "A", "B", "C", "C", "Consejo"),
                    new Question("Teoría", "¿Pregunta 43?", "A", "B", "C", "A", "Consejo"),
                    new Question("Teoría", "¿Pregunta 44?", "A", "B", "C", "B", "Consejo"),
                    new Question("Teoría", "¿Pregunta 45?", "A", "B", "C", "C", "Consejo"),
                    new Question("Teoría", "¿Pregunta 46?", "A", "B", "C", "A", "Consejo"),
                    new Question("Teoría", "¿Pregunta 47?", "A", "B", "C", "B", "Consejo"),
                    new Question("Teoría", "¿Pregunta 48?", "A", "B", "C", "C", "Consejo"),
                    new Question("Teoría", "¿Pregunta 49?", "A", "B", "C", "A", "Consejo"),
                    new Question("Teoría", "¿Pregunta 50?", "A", "B", "C", "B", "Consejo"),
                };
            }


            public float LoadTimeLeftQuestionTimer()
            {
                return 0f;
            }

            public string LoadURLQuestions()
            {
                throw new System.NotImplementedException();
            }

            public Dictionary<string, FixedSizeQueue<string>> LoadUserPerformance()
            {
                return _userPerformance;
            }

            public void ResetIterationQuestions()
            {
                throw new System.NotImplementedException();
            }

            public void ResetQuestionURL()
            {
                throw new System.NotImplementedException();
            }

            public void ResetUserPerformance()
            {
                throw new System.NotImplementedException();
            }

            public void SaveCurrentQuestionIndex(int currentQuestionIndex)
            {
                
            }

            public void SaveIterationQuestions(List<Question> iterationQuestions)
            {
                
            }

            public void SaveTimeLeftQuestionTimer(float timeLeft)
            {
                throw new System.NotImplementedException();
            }

            public int SaveURLQuestions(string url)
            {
                throw new System.NotImplementedException();
            }

            public void SaveUserPerformance(Dictionary<string, FixedSizeQueue<string>> userPerformance)
            {
                UtilityFunctions.CopyDictionaryPerformance(userPerformance, _userPerformance);
            }
        }

        private class DummyConectionManager : IConnectionManager
        {
            public DateTime lastDisconnectionDateTime => DateTime.Now.AddSeconds(-1);

            public DateTime currentConnectionDateTime => DateTime.Now;

            public bool isFirstUsage => throw new NotImplementedException();

            public bool IsConnected(DateTime dateTimeToEvaluate)
            {
                throw new NotImplementedException();
            }

            public void SetDisconnectionDate(DateTime newDisconnectionDate)
            {
                throw new NotImplementedException();
            }

            public void SetIsFirstUsage(bool newIsFirstUsage)
            {
                throw new NotImplementedException();
            }
        }

        private class DummyEconomyManager : IEconomyManager
        {
            public int totalCoins => throw new NotImplementedException();

            public int stashedCoins => throw new NotImplementedException();

            public void AddStashedCoins(int coins)
            {
                throw new NotImplementedException();
            }

            public void AddTotalCoins(int coins)
            {

            }

            public Dictionary<string, ProductState> LoadAllProducts()
            {
                throw new NotImplementedException();
            }

            public void SaveAllProducts(Dictionary<string, ProductState> allProducts)
            {
                throw new NotImplementedException();
            }

            public void StashedCoinsToTotalCoins()
            {
                throw new NotImplementedException();
            }

            public void SubstractTotalCoins(int coins)
            {
                throw new NotImplementedException();
            }
        }

        private class DummyScoreManager : IScoreManager
        {
            public int currentScore => throw new NotImplementedException();

            public int highestScore => throw new NotImplementedException();

            public void AddScore(int addedScore, DateTime? time, string activity)
            {

            }

            public void ResetScore()
            {
                throw new NotImplementedException();
            }

            public void SubstractScore(int substractedScore, DateTime? time, string activity)
            {

            }
        }
    }
}