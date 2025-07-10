using NUnit.Framework;
using UnityEngine.TestTools;

using System;
using Master.Domain.PetCare;
using Master.Domain.Settings;
using Master.Domain.Connection;
using Master.Domain.PetCare.Log;
using Master.Domain.Questions;
using Master.Domain.Score;
using Master.Domain.Shop;
using System.Collections.Generic;
using System.Threading.Tasks;
using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System.Collections;

namespace Master.Tests
{
    public class UpdateAttributesTests
    {
        [UnityTest]
        public IEnumerator DefaultUpdate()
        {
            IPetCareRepository _petCareRepository = new DummyPetCareRepository();
            _petCareRepository.SaveGlycemia(120);
            _petCareRepository.SaveEnergy(50);
            _petCareRepository.SaveHunger(50);

            IPetCareManager _petCareManager = new PetCareManager(
                new DummyChatBot(),
                _petCareRepository,
                new DummyPetCareLogManager(),
                new DummyScoreManager(),
                new DummyEconomyManager(),
                new DummyConnectionManager()
            );

            ISettingsManager settings = new DummySettingsManager();
            IConnectionManager connection = new DummyConnectionManager();

            BTree glycemiaTree = new BT_Glycemia(_petCareManager, settings, connection);
            BTree energyTree = new BT_Energy(_petCareManager, settings, connection);
            BTree hungerTree = new BT_Hunger(_petCareManager, settings, connection);

            int countAttributes = 0;
            bool testCompleted = false;

            AttributeUpdateIntervalInfo info = new AttributeUpdateIntervalInfo(DateTime.Now.Date.AddHours(5), 120, 50, 50, false, false, false);

            glycemiaTree.EnqueueAttribute(info);
            energyTree.EnqueueAttribute(info);
            hungerTree.EnqueueAttribute(info);

            void Handler()
            {
                countAttributes++;
                if (countAttributes >= 3)
                {
                    testCompleted = true;
                }
            }

            GameEvents_PetCare.OnFinishedExecutionAttributesBTree += Handler;

            for (int i = 0; i < 10 && !testCompleted; i++)
            {
                glycemiaTree.Run();
                energyTree.Run();
                hungerTree.Run();

                yield return new UnityEngine.WaitForSeconds(1f);
            }

            GameEvents_PetCare.OnFinishedExecutionAttributesBTree -= Handler;

            Assert.IsTrue(testCompleted, "Los BTree no terminaron.");

            // Asserts de valores esperados
            Assert.Contains(_petCareManager.glycemiaValue, new[] { 115, 125 });
            Assert.AreEqual(52, _petCareManager.energyValue);
            Assert.AreEqual(52, _petCareManager.hungerValue);
        }

        [UnityTest]
        public IEnumerator UpdateAttributes_BadLowGlycemia_BadHighEnergy_AllActionsDeactivated()
        {
            IPetCareRepository _petCareRepository = new DummyPetCareRepository();
            _petCareRepository.SaveGlycemia(40);
            _petCareRepository.SaveEnergy(90);
            _petCareRepository.SaveHunger(50);

            IPetCareManager _petCareManager = new PetCareManager(
                new DummyChatBot(),
                _petCareRepository,
                new DummyPetCareLogManager(),
                new DummyScoreManager(),
                new DummyEconomyManager(),
                new DummyConnectionManager()
            );

            ISettingsManager settings = new DummySettingsManager();
            IConnectionManager connection = new DummyConnectionManager();

            BTree glycemiaTree = new BT_Glycemia(_petCareManager, settings, connection);
            BTree energyTree = new BT_Energy(_petCareManager, settings, connection);
            BTree hungerTree = new BT_Hunger(_petCareManager, settings, connection);

            int countAttributes = 0;
            bool testCompleted = false;

            AttributeUpdateIntervalInfo info = new AttributeUpdateIntervalInfo(DateTime.Now.Date.AddHours(5), 40, 90, 50, false, false, false);

            glycemiaTree.EnqueueAttribute(info);
            energyTree.EnqueueAttribute(info);
            hungerTree.EnqueueAttribute(info);

            void Handler()
            {
                countAttributes++;
                if (countAttributes >= 3)
                {
                    testCompleted = true;
                }
            }

            GameEvents_PetCare.OnFinishedExecutionAttributesBTree += Handler;

            for (int i = 0; i < 10 && !testCompleted; i++)
            {
                glycemiaTree.Run();
                energyTree.Run();
                hungerTree.Run();

                yield return new UnityEngine.WaitForSeconds(1f);
            }

            GameEvents_PetCare.OnFinishedExecutionAttributesBTree -= Handler;

            Assert.IsTrue(testCompleted, "Los BTree no terminaron.");

            // Asserts de valores esperados
            Assert.AreEqual(47, _petCareManager.glycemiaValue);
            Assert.AreEqual(85, _petCareManager.energyValue);
            Assert.AreEqual(55, _petCareManager.hungerValue);
        }

        [UnityTest]
        public IEnumerator UpdateAttributes_AllActionsActivated()
        {
            IPetCareRepository _petCareRepository = new DummyPetCareRepository();
            _petCareRepository.SaveGlycemia(120);
            _petCareRepository.SaveEnergy(50);
            _petCareRepository.SaveHunger(50);

            IPetCareManager _petCareManager = new PetCareManager(
                new DummyChatBot(),
                _petCareRepository,
                new DummyPetCareLogManager(),
                new DummyScoreManager(),
                new DummyEconomyManager(),
                new DummyConnectionManager()
            );

            ISettingsManager settings = new DummySettingsManager();
            IConnectionManager connection = new DummyConnectionManager();

            BTree glycemiaTree = new BT_Glycemia(_petCareManager, settings, connection);
            BTree energyTree = new BT_Energy(_petCareManager, settings, connection);
            BTree hungerTree = new BT_Hunger(_petCareManager, settings, connection);

            int countAttributes = 0;
            bool testCompleted = false;

            AttributeUpdateIntervalInfo info = new AttributeUpdateIntervalInfo(DateTime.Now.Date.AddHours(5), 120, 50, 50, true, true, true);

            glycemiaTree.EnqueueAttribute(info);
            energyTree.EnqueueAttribute(info);
            hungerTree.EnqueueAttribute(info);

            void Handler()
            {
                countAttributes++;
                if (countAttributes >= 3)
                {
                    testCompleted = true;
                }
            }

            GameEvents_PetCare.OnFinishedExecutionAttributesBTree += Handler;

            for (int i = 0; i < 10 && !testCompleted; i++)
            {
                glycemiaTree.Run();
                energyTree.Run();
                hungerTree.Run();

                yield return new UnityEngine.WaitForSeconds(1f);
            }

            GameEvents_PetCare.OnFinishedExecutionAttributesBTree -= Handler;

            Assert.IsTrue(testCompleted, "Los BTree no terminaron.");

            // Asserts de valores esperados
            Assert.AreEqual(118, _petCareManager.glycemiaValue);
            Assert.AreEqual(49, _petCareManager.energyValue);
            Assert.AreEqual(49, _petCareManager.hungerValue);
        }

        // Dummies

        private class DummyChatBot : IChatBot
        {
            public Task<string> Ask(string input)
            {
                throw new NotImplementedException();
            }
        }

        private class DummyPetCareLogManager : IPetCareLogManager
        {
            public List<AttributeLog> glycemiaLogList => throw new NotImplementedException();

            public List<AttributeLog> energyLogList => throw new NotImplementedException();

            public List<AttributeLog> hungerLogList => throw new NotImplementedException();

            public List<ActionLog> insulinLogList => throw new NotImplementedException();

            public List<ActionLog> foodLogList => throw new NotImplementedException();

            public List<ActionLog> exerciseLogList => throw new NotImplementedException();

            public DateTime currentDateFilter => throw new NotImplementedException();

            public AttributeType currentAttributeFilter => throw new NotImplementedException();

            public void AddActionLog(ActionType actionType, ActionLog attributeLog)
            {
                throw new NotImplementedException();
            }

            public void AddAttributeLog(AttributeType attributeType, AttributeLog actionLog)
            {
                
            }

            public void ClearThisDateActionLog(ActionType actionType)
            {
                throw new NotImplementedException();
            }

            public void ClearThisDateAttributeLog(AttributeType attributeType)
            {
                throw new NotImplementedException();
            }

            public List<DateTime> GetActionsAvailableTimesThisDate(DateTime thisDate)
            {
                throw new NotImplementedException();
            }

            public List<AttributeLog> GetThisDateAttributeLog()
            {
                throw new NotImplementedException();
            }

            public void ModifyDayFilter(int amountDays)
            {
                throw new NotImplementedException();
            }

            public void SetAttributeFilter(AttributeType newAttributeFilter)
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
                throw new NotImplementedException();
            }
        }

        private class DummyEconomyManager : IEconomyManager
        {
            public int totalCoins => throw new NotImplementedException();

            public int stashedCoins => throw new NotImplementedException();

            public void AddStashedCoins(int coins)
            {
                
            }

            public void AddTotalCoins(int coins)
            {
                throw new NotImplementedException();
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

        private class DummyConnectionManager : IConnectionManager
        {
            public DateTime lastDisconnectionDateTime => DateTime.Now;

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

        private class DummySettingsManager : ISettingsManager
        {
            public TimeSpan initialTime => new TimeSpan(0, 0, 0);

            public TimeSpan finishTime => new TimeSpan(23, 59, 0);

            public float soundEffectsVolume => throw new NotImplementedException();

            public bool ChangeQuestions()
            {
                throw new NotImplementedException();
            }

            public void ConfirmChangeRangeTime(float currentInitialHour, float currentFinishHour)
            {
                throw new NotImplementedException();
            }

            public void InitializeDependencies(IPetCareManager petCareManager, IPetCareLogManager petCareLogManager, IQuestionManager questionManager, IScoreManager scoreManager, IScoreLogManager scoreLogManager)
            {
                throw new NotImplementedException();
            }

            public bool IsInRange(TimeSpan currentTime)
            {
                return true;
            }

            public bool ResetQuestions()
            {
                throw new NotImplementedException();
            }

            public void SetFinishHour(int newHour)
            {
                throw new NotImplementedException();
            }

            public void SetInitialHour(int newHour)
            {
                throw new NotImplementedException();
            }

            public void SetSoundEffectsVolume(float volume)
            {
                throw new NotImplementedException();
            }

            public int TryChangingQuestionsURL(string input)
            {
                throw new NotImplementedException();
            }
        }

        private class DummyPetCareRepository : IPetCareRepository
        {
            private int glycemia = 120;
            private int energy = 50;
            private int hunger = 50;

            public int LoadEnergy()
            {
                return energy;
            }

            public List<AttributeLog> LoadEnergyLog()
            {
                throw new NotImplementedException();
            }

            public DateTime LoadExerciseCooldownEndTime()
            {
                return DateTime.Now.AddSeconds(-1);
            }

            public DateTime LoadExerciseEffectsEndTime()
            {
                return DateTime.Now.AddSeconds(-1);
            }

            public List<ActionLog> LoadExerciseLog()
            {
                throw new NotImplementedException();
            }

            public DateTime LoadFoodCooldownEndTime()
            {
                return DateTime.Now.AddSeconds(-1);
            }

            public DateTime LoadFoodEffectsEndTime()
            {
                return DateTime.Now.AddSeconds(-1);
            }

            public List<ActionLog> LoadFoodLog()
            {
                throw new NotImplementedException();
            }

            public int LoadGlycemia()
            {
                return glycemia;
            }

            public List<AttributeLog> LoadGlycemiaLog()
            {
                throw new NotImplementedException();
            }

            public int LoadHunger()
            {
                return hunger;
            }

            public List<AttributeLog> LoadHungerLog()
            {
                throw new NotImplementedException();
            }

            public DateTime LoadInsulinCooldownEndTime()
            {
                return DateTime.Now.AddSeconds(-1);
            }

            public DateTime LoadInsulinEffectsEndTime()
            {
                return DateTime.Now.AddSeconds(-1);
            }

            public List<ActionLog> LoadInsulinLog()
            {
                throw new NotImplementedException();
            }

            public DateTime LoadNextIterationStartTime()
            {
                return DateTime.Now;
            }

            public void SaveEnergy(int EnergyValue)
            {
                energy = EnergyValue;
            }

            public void SaveEnergyLog(List<AttributeLog> energyLogList)
            {
                throw new NotImplementedException();
            }

            public void SaveExerciseCooldownEndTime(DateTime exerciseCooldownEndTime)
            {
                
            }

            public void SaveExerciseEffectsEndTime(DateTime exerciseEffectsEndTime)
            {
                
            }

            public void SaveExerciseLog(List<ActionLog> exerciseLogList)
            {
                throw new NotImplementedException();
            }

            public void SaveFoodCooldownEndTime(DateTime foodCooldownEndTime)
            {
                
            }

            public void SaveFoodEffectsEndTime(DateTime foodEffectsEndTime)
            {
                
            }

            public void SaveFoodLog(List<ActionLog> foodLogList)
            {
                throw new NotImplementedException();
            }

            public void SaveGlycemia(int glycemiaValue)
            {
                glycemia = glycemiaValue;
            }

            public void SaveGlycemiaLog(List<AttributeLog> glycemiaLogList)
            {
                throw new NotImplementedException();
            }

            public void SaveHunger(int hungerValue)
            {
                hunger = hungerValue;
            }

            public void SaveHungerLog(List<AttributeLog> hungerLogList)
            {
                throw new NotImplementedException();
            }

            public void SaveInsulinCooldownEndTime(DateTime insulinCooldownEndTime)
            {
                
            }

            public void SaveInsulinEffectsEndTime(DateTime insulinEffectsEndTime)
            {
                
            }

            public void SaveInsulinLog(List<ActionLog> insulinLogList)
            {
                throw new NotImplementedException();
            }

            public void SaveNextIterationStartTime(DateTime startTimeInterval)
            {
                throw new NotImplementedException();
            }
        }
    }
}