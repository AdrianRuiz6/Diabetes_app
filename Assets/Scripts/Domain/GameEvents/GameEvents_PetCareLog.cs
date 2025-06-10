using System;
using Master.Domain.PetCare;

namespace Master.Domain.GameEvents
{
    public class GameEvents_PetCareLog
    {
        public static Action OnChangedDateFilter;
        public static Action OnChangedAttributeTypeFilter;

        public static Action<AttributeType> OnUpdatedAttributesLog;
        public static Action OnUpdatedActionsLog;
    }
}