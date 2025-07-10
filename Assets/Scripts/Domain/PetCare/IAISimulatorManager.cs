using System;

namespace Master.Domain.PetCare
{
    public interface IAISimulatorManager
    {
        public int iterationsTotal { get; }
        public void StartSimulation();

        public void FinishSimulationAttribute();
    }
}