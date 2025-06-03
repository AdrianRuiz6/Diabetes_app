using UnityEngine;
using Master.Domain.Questions;

namespace Master.Domain.Questions
{
    public class State_WaitForRestart : StateQuestions
    {
        private string _newState;

        public override void Execute(AgentQuestions agent)
        {
            if (agent.executing == true)
            {
                _newState = "CheckUserPerformance";
                agent.Change_state(new State_CheckUserPerformance());
            }
        }

        public override void OnExit()
        {
            Debug.Log("State changed to: " + _newState);
        }
    }
}