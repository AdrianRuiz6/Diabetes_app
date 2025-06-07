using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Persistence.PetCare
{
    [System.Serializable]
    public class ActionLogDataList
    {
        public List<ActionLogData> actionsLogList = new List<ActionLogData>();
    }
}