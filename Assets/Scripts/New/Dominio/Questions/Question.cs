using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string topic;
    public string question;
    public string answer1;
    public string answer2;
    public string answer3;
    public string correctAnswer;
    public string advice;

    public char resultAnswer;

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
        if (string.Equals(answer, correctAnswer))
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
        if (resultAnswer == 'S')
            return true;
        return false;
    }

    public void RandomizeOrderAnswer() // Uso del algoritmo de Fisher-Yates.
    {
        System.Random random = new System.Random();
        List<string> auxAnswerList = new List<string>();
        auxAnswerList.Add(answer1);
        auxAnswerList.Add(answer2);
        auxAnswerList.Add(answer3);

        UtilityFunctions.RandomizeList(auxAnswerList);

        answer1 = auxAnswerList[0];
        answer2 = auxAnswerList[1];
        answer3 = auxAnswerList[2];
    }
}
