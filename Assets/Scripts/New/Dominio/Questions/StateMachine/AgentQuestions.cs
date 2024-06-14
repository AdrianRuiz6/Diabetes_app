using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;

public class AgentQuestions : MonoBehaviour
{
    private State currentState;
    public bool executing { get; private set; }

    void Awake()
    {
        GameEventsQuestions.OnExecuteQuestionSearch += ExecuteQuestionSearch;
        GameEventsQuestions.OnFinalizedCreationQuestions += FinalizeQuestionSearch;
    }

    private void OnDestroy()
    {
        GameEventsQuestions.OnExecuteQuestionSearch -= ExecuteQuestionSearch;
        GameEventsQuestions.OnFinalizedCreationQuestions -= FinalizeQuestionSearch;
    }

    void Start()
    {
        executing = false;
    }

    void Update()
    {
        if (executing == true)
        {
            currentState.Execute(this);
        }
    }

    public void Change_state(State newState)
    {
        currentState.OnExit();
        currentState = newState;
    }

    private void ExecuteQuestionSearch()
    {
        currentState = new CheckUserPerformance();
        executing = true;
    }

    private void FinalizeQuestionSearch()
    {
        executing = false;
    }
}
