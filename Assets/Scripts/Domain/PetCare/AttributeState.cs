namespace Master.Domain.PetCare
{
    public class AttributeState
    {
        public int minValue;
        public int maxValue;
        public AttributeRangeValue rangeValue;

        public AttributeState(int minValue, int maxValue, AttributeRangeValue rangeValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.rangeValue = rangeValue;
        }
    }
}
