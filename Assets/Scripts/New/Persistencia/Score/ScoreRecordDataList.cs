using Master.Domain.Shop;
using System.Collections.Generic;

namespace Master.Persistence.Score
{
    [System.Serializable]
    public class ScoreRecordDataList
    {
        public List<ScoreRecordData> score = new List<ScoreRecordData>();
    }
}