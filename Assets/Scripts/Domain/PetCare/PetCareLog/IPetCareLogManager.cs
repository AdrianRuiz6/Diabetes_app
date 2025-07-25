using System.Collections.Generic;
using System;

namespace Master.Domain.PetCare.Log
{
    public interface IPetCareLogManager
    {
        public List<AttributeLog> glycemiaLogList { get; }
        public List<AttributeLog> energyLogList { get; }
        public List<AttributeLog> hungerLogList { get; }

        public List<ActionLog> insulinLogList { get; }
        public List<ActionLog> foodLogList { get; }
        public List<ActionLog> exerciseLogList { get; }

        public DateTime currentDateFilter { get; }
        public AttributeType currentAttributeFilter { get; }

        public List<AttributeLog> GetThisDateAttributeLog();
        public void ModifyDayFilter(int amountDays);

        public void SetAttributeFilter(AttributeType newAttributeFilter);

        public void AddAttributeLog(AttributeType attributeType, AttributeLog actionLog);

        public void AddActionLog(ActionType actionType, ActionLog attributeLog);

        public void ClearThisDateAttributeLog(AttributeType attributeType);

        public void ClearThisDateActionLog(ActionType actionType);

        public List<DateTime> GetActionsAvailableTimesThisDate(DateTime thisDate);
    }
}