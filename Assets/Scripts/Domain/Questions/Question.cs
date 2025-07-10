using System.Collections.Generic;
using Master.Auxiliar;

public class Question
{
    public string topic { private set; get; }
    public string question { private set; get; }
    public string answer1 { private set; get; }
    public string answer2 { private set; get; }
    public string answer3 { private set; get; }
    public string correctAnswer { private set; get; }
    public string explanation { private set; get; }

    public string resultAnswer { private set; get; }

    public Question(string topic, string question, string answer1, string answer2, string answer3, string correctAnswer, string explanation, string resultAnswer = " ")
    {
        this.topic = topic;
        this.question = question;
        this.answer1 = answer1;
        this.answer2 = answer2;
        this.answer3 = answer3;
        this.correctAnswer = correctAnswer;
        this.explanation = explanation;

        this.resultAnswer = resultAnswer;
    }

    public void AnswerQuestion(string answer)
    {
        if (string.Equals(answer, correctAnswer))
        {
            resultAnswer = "S";
        }
        else
        {
            resultAnswer = "F";
        }
    }

    public bool IsCorrect()
    {
        if (resultAnswer == "S")
            return true;
        return false;
    }

    public void RandomizeOrderAnswers() // Uso del algoritmo de Fisher-Yates.
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
