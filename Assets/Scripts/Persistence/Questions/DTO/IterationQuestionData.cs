using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IterationQuestionData
{
    public string topic;
    public string question;
    public string answer1;
    public string answer2;
    public string answer3;
    public string correctAnswer;
    public string advice;

    public string resultAnswer;

    public IterationQuestionData(string topic, string question, string answer1, string answer2, string answer3, string correctAnswer, string advice)
    {
        this.topic = topic;
        this.question = question;
        this.answer1 = answer1;
        this.answer2 = answer2;
        this.answer3 = answer3;
        this.correctAnswer = correctAnswer;
        this.advice = advice;

        this.resultAnswer = " ";
    }
}
