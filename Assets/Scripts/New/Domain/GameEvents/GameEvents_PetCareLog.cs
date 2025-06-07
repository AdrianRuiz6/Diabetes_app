using System;
using Master.Domain.PetCare;

namespace Master.Domain.GameEvents
{
    public class GameEvents_PetCareLog
    {
        public static Action<DateTime> OnChangedDateFilter;
        public static Action<AttributeType> OnChangedAttributeTypeFilter;

        public static Action<AttributeType> OnUpdatedAttributesLog;
        public static Action OnUpdatedActionsLog;
    }
}