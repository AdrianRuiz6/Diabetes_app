using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;

public class CAgent : MonoBehaviour
{
    private State currentState;

    void Awake()
    {
        GameEventsQuestions.OnExecuteQuestionSearch += ExecuteQuestionSearch;
    }

    private void OnDestroy()
    {
        GameEventsQuestions.OnExecuteQuestionSearch -= ExecuteQuestionSearch;
    }

    void Start()
    {
        ExecuteQuestionSearch();
    }

    public void ExecuteQuestionSearch()
    {
        currentState = new CheckUserPerformance();
    }

    void Update()
    {
        currentState.Execute(this);
    }

    public void Change_state(State newState)
    {
        currentState.OnExit();
        currentState = newState;
    }
}
