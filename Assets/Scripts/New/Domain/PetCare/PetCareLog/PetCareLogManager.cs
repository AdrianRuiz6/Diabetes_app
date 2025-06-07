using Master.Domain.GameEvents;
using Master.Persistence.PetCare;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Master.Domain.PetCare.Log
{
    public class PetCareLogManager
    {
        public List<AttributeLog> glycemiaLogList { get; private set; }
        public List<AttributeLog> activityLogList { get; private set; }
        public List<AttributeLog> hungerLogList { get; private set; }

        public List<ActionLog> insulinLogList { get; private set; }
        public List<ActionLog> foodLogList { get; private set; }
        public List<ActionLog> exerciseLogList { get; private set; }

        private HashSet<DateTime> _actionsAvailableTimes = new HashSet<DateTime>();

        public PetCareLogManager()
        {
            insulinLogList = DataStorage_PetCare.LoadInsulinLog();
            foodLogList = DataStorage_PetCare.LoadFoodLog();
            exerciseLogList = DataStorage_PetCare.LoadExerciseLog();

            glycemiaLogList = DataStorage_PetCare.LoadGlycemiaLog();
            activityLogList = DataStorage_PetCare.LoadActivityLog();
            hungerLogList = DataStorage_PetCare.LoadHungerLog();
        }

        public void AddAttributeLog(AttributeType attributeType, AttributeLog actionLog)
        {
            switch (attributeType)
            {
                case AttributeType.Glycemia:
                    glycemiaLogList.Add(actionLog);
                    break;
                case AttributeType.Hunger:
                    hungerLogList.Add(actionLog);
                    break;
                case AttributeType.Activity:
                    activityLogList.Add(actionLog);
                    break;
            }

            SaveAttributeLog(attributeType);
        }

        public void AddActionLog(ActionType actionType, ActionLog attributeLog)
        {
            switch (actionType)
            {
                case ActionType.Insulin:
                    insulinLogList.Add(attributeLog);
                    break;
                case ActionType.Food:
                    foodLogList.Add(attributeLog);
                    break;
                case ActionType.Exercise:
                    exerciseLogList.Add(attributeLog);
                    break;
            }

            SaveActionLog(actionType);
        }

        public void ClearThisDateAttributeLog(AttributeType attributeType)
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
                case AttributeType.Activity:
                    attributesToClear = activityLogList;
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
        }

        public void ClearThisDateActionLog(ActionType actionType)
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
        }

        private void SaveAttributeLog(AttributeType attributeType)
        {
            switch (attributeType)
            {
                case AttributeType.Glycemia:
                    DataStorage_PetCare.SaveGlycemiaLog(glycemiaLogList);
                    break;
                case AttributeType.Hunger:
                    DataStorage_PetCare.SaveHungerLog(hungerLogList);
                    break;
                case AttributeType.Activity:
                    DataStorage_PetCare.SaveActivityLog(activityLogList);
                    break;
            }

            GameEvents_PetCareLog.OnUpdatedAttributesLog?.Invoke(attributeType);
        }

        private void SaveActionLog(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Insulin:
                    DataStorage_PetCare.SaveInsulinLog(insulinLogList);
                    break;
                case ActionType.Food:
                    DataStorage_PetCare.SaveFoodLog(foodLogList);
                    break;
                case ActionType.Exercise:
                    DataStorage_PetCare.SaveExerciseLog(exerciseLogList);
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