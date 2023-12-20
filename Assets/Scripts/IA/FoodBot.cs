using com.studios.taprobana;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FoodBot : MonoBehaviour
{

    public TMP_Text feedback_test;

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

    public async void Ask(string input)
    {
        try
        {
            // Mandar pregunta a ChatGPT.
            ChatCompletionsRequest chatCompletionsRequest = new ChatCompletionsRequest();
            Message message = new(Roles.USER, input);

            chatCompletionsRequest.AddMessage(message);

            ChatCompletionsResponse res = await chatCompletionsApi.CreateChatCompletionsRequest(chatCompletionsRequest);
            feedback_test.text = res.GetResponseMessage();
            

        }
        catch (OpenAiRequestException exception)
        {
            Debug.LogError(exception);
        }
    }


}
