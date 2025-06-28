using Master.Domain.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Master.Domain.PetCare.Log
{
    public class PetCareLogManager : IPetCareLogManager
    {
        private Mutex _mutex = new Mutex();
        public List<AttributeLog> glycemiaLogList { get; private set; }
        public List<AttributeLog> energyLogList { get; private set; }
        public List<AttributeLog> hungerLogList { get; private set; }

        public List<ActionLog> insulinLogList { get; private set; }
        public List<ActionLog> foodLogList { get; private set; }
        public List<ActionLog> exerciseLogList { get; private set; }

        private HashSet<DateTime> _actionsAvailableTimes = new HashSet<DateTime>();

        public DateTime currentDateFilter { private set; get; } = DateTime.Now.Date;
        public AttributeType currentAttributeFilter { private set; get; } = AttributeType.Glycemia;

        private IPetCareRepository _petCareRepository;

        public PetCareLogManager(IPetCareRepository petCareRepository)
        {
            _petCareRepository = petCareRepository;

            insulinLogList = _petCareRepository.LoadInsulinLog();
            foodLogList = _petCareRepository.LoadFoodLog();
            exerciseLogList = _petCareRepository.LoadExerciseLog();

            glycemiaLogList = _petCareRepository.LoadGlycemiaLog();
            energyLogList = _petCareRepository.LoadEnergyLog();
            hungerLogList = _petCareRepository.LoadHungerLog();

            UpdateActionsAvailableTimes();
        }

        public List<AttributeLog> GetThisDateAttributeLog()
        {
            List<AttributeLog> result = new List<AttributeLog>();

            switch (currentAttributeFilter)
            {
                case AttributeType.Glycemia:
                    foreach (var attribute in glycemiaLogList)
                    {
                        if(attribute.GetDateAndTime().Value.Date == currentDateFilter.Date)
                        {
                            result.Add(attribute);
                        }
                    }
                    break;
                case AttributeType.Energy:
                    foreach (var attribute in energyLogList)
                    {
                        if (attribute.GetDateAndTime().Value.Date == currentDateFilter.Date)
                        {
                            result.Add(attribute);
                        }
                    }
                    break;
                case AttributeType.Hunger:
                    foreach (var attribute in hungerLogList)
                    {
                        if (attribute.GetDateAndTime().Value.Date == currentDateFilter.Date)
                        {
                            result.Add(attribute);
                        }
                    }
                    break;
            }

            return result;
        }

        public void ModifyDayFilter(int amountDaysAdded)
        {
            if (currentDateFilter.AddDays(amountDaysAdded) <= DateTime.Now.Date)
            {
                currentDateFilter = currentDateFilter.AddDays(amountDaysAdded);
                GameEvents_PetCareLog.OnChangedDateFilter?.Invoke();
            }
        }

        public void SetAttributeFilter(AttributeType newAttributeFilter)
        {
            currentAttributeFilter = newAttributeFilter;
            GameEvents_PetCareLog.OnChangedAttributeTypeFilter?.Invoke();
        }

        public void AddAttributeLog(AttributeType attributeType, AttributeLog attributeLog)
        {
            _mutex.WaitOne();
            try
            {
                switch (attributeType)
                {
                    case AttributeType.Glycemia:
                        glycemiaLogList.Add(attributeLog);
                        break;
                    case AttributeType.Hunger:
                        hungerLogList.Add(attributeLog);
                        break;
                    case AttributeType.Energy:
                        energyLogList.Add(attributeLog);
                        break;
                }

                SaveAttributeLog(attributeType);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void AddActionLog(ActionType actionType, ActionLog actionLog)
        {
            _mutex.WaitOne();
            try
            {
                switch (actionType)
                {
                    case ActionType.Insulin:
                        insulinLogList.Add(actionLog);
                        break;
                    case ActionType.Food:
                        foodLogList.Add(actionLog);
                        break;
                    case ActionType.Exercise:
                        exerciseLogList.Add(actionLog);
                        break;
                }

                SaveActionLog(actionType);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void ClearThisDateAttributeLog(AttributeType attributeType)
        {
            _mutex.WaitOne();
            try
            {
                List<AttributeLog> attributesToClear = new List<AttributeLog>();
                List<AttributeLog> newAttributesList = new List<AttributeLog>();

                switch (attributeType)
                {
                    case AttributeType.Glycemia:
                        attributesToClear = glycemiaLogList;
                        break;
                    case AttributeType.Hunger:
                        attributesToClear = hungerLogList;
                        break;
                    case AttributeType.Energy:
                        attributesToClear = energyLogList;
                        break;
                }

                foreach (AttributeLog attributeLog in attributesToClear)
                {
                    if (attributeLog.GetDateAndTime().Value.Date != DateTime.Now.Date)
                    {
                        newAttributesList.Add(attributeLog);
                    }
                }

                attributesToClear.Clear();
                attributesToClear.AddRange(newAttributesList);

                SaveAttributeLog(attributeType);

                GameEvents_PetCareLog.OnResetGraph?.Invoke();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void ClearThisDateActionLog(ActionType actionType)
        {
            _mutex.WaitOne();
            try
            {
                List<ActionLog> actionsToClear = new List<ActionLog>();
                List<ActionLog> newActionsList = new List<ActionLog>();

                switch (actionType)
                {
                    case ActionType.Insulin:
                        actionsToClear = insulinLogList;
                        break;
                    case ActionType.Food:
                        actionsToClear = foodLogList;
                        break;
                    case ActionType.Exercise:
                        actionsToClear = exerciseLogList;
                        break;
                }

                foreach (ActionLog actionLog in actionsToClear)
                {
                    if (actionLog.GetDateAndTime().Value.Date != DateTime.Now.Date)
                    {
                        newActionsList.Add(actionLog);
                    }
                }

                actionsToClear.Clear();
                actionsToClear.AddRange(newActionsList);

                SaveActionLog(actionType);

                GameEvents_PetCareLog.OnUpdatedActionsLog();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        private void SaveAttributeLog(AttributeType attributeType)
        {
            switch (attributeType)
            {
                case AttributeType.Glycemia:
                    _petCareRepository.SaveGlycemiaLog(glycemiaLogList);
                    break;
                case AttributeType.Hunger:
                    _petCareRepository.SaveHungerLog(hungerLogList);
                    break;
                case AttributeType.Energy:
                    _petCareRepository.SaveEnergyLog(energyLogList);
                    break;
            }

            GameEvents_PetCareLog.OnUpdatedAttributesLog?.Invoke(attributeType);
        }

        private void SaveActionLog(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Insulin:
                    _petCareRepository.SaveInsulinLog(insulinLogList);
                    break;
                case ActionType.Food:
                    _petCareRepository.SaveFoodLog(foodLogList);
                    break;
                case ActionType.Exercise:
                    _petCareRepository.SaveExerciseLog(exerciseLogList);
                    break;
            }

            UpdateActionsAvailableTimes();
            GameEvents_PetCareLog.OnUpdatedActionsLog?.Invoke();
        }

        private void UpdateActionsAvailableTimes()
        {
            _actionsAvailableTimes.Clear();

            IEnumerable<ActionLog> allActionLogs = insulinLogList.Concat(exerciseLogList).Concat(foodLogList);

            foreach (ActionLog actionLog in allActionLogs)
            {
                DateTime? dateTime = actionLog.GetDateAndTime();
                if (dateTime.HasValue)
                {
                    _actionsAvailableTimes.Add(dateTime.Value);
                }
            }
        }

        public List<DateTime> GetActionsAvailableTimesThisDate(DateTime thisDate)
        {
            return _actionsAvailableTimes.Where(dateTime => dateTime.Date == thisDate.Date).ToList();
        }
    }
}