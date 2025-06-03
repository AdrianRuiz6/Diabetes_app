namespace Master.Domain.Questions
{
    public abstract class StateQuestions
    {
        public abstract void Execute(AgentQuestions agent);
        public abstract void OnExit();
    }
}