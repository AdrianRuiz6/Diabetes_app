using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;

public class Question : MonoBehaviour
{
    public string topic { get; private set; }
    public string question { get; private set; }
    public string answer1 { get; private set; }
    public string answer2 { get; private set; }
    public string answer3 { get; private set; }
    public string correctAnswer { get; private set; }
    public string advice { get; private set; }

    public char resultAnswer { get; private set; }

    public Question(string topic, string question, string answer1, string answer2, string answer3, string correctAnswer, string advice)
    {
        this.topic = topic;
        this.question = question;
        this.answer1 = answer1;
        this.answer2 = answer2;
        this.answer3 = answer3;
        this.correctAnswer = correctAnswer;
        this.advice = advice;

        this.resultAnswer = ' ';
    }

    public void answerQuestion(string answer)
    {
        if(string.Equals(answer, correctAnswer))
        {
            resultAnswer = 'S';
        }
        else
        {
            resultAnswer = 'F';
        }
    }

    public bool isCorrect()
    {
        if(resultAnswer == 'S')
            return true;
        return false;
    }
}
