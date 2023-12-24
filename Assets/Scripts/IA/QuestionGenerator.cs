using com.studios.taprobana;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionGenerator : MonoBehaviour
{

    private ChatCompletionsApi chatCompletionsApi;
    private readonly string apiKey = "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";

    void Start()
    {
        StartConversation();
    }


    private void StartConversation()
    {
        // Iniciar chatGPT y mandar el mensaje de System.
        chatCompletionsApi = new(apiKey);
        chatCompletionsApi.ConversationHistoryMemory = 5;
        chatCompletionsApi.SetSystemMessage("INITIAL MESSAGE...");
    }

    private async void GenerateQuestions()
    {
        
    }
}
