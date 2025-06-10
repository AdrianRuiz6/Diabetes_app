namespace Master.Domain.PetCare
{
    public interface IAISimulatorManager
    {
        public float initialTimerSeconds { get; }
        public void Simulate();
    }
}