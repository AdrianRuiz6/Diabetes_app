using com.studios.taprobana;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBot : MonoBehaviour
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

    public string Ask(string input)
    {
        string response = "ERROR al procesar la respuesta.";

        try
        {
            // Mandar pregunta a ChatGPT.
            ChatCompletionsRequest chatCompletionsRequest = new ChatCompletionsRequest();
            Message message = new Message(Roles.USER, input);

            chatCompletionsRequest.AddMessage(message);

            // Recoger respuesta de ChatGPT.
            ChatCompletionsResponse res = chatCompletionsApi.CreateChatCompletionsRequest(chatCompletionsRequest).Result;
            response = res.GetResponseMessage();
            return response;
        }
        catch (OpenAiRequestException exception)
        {
            Debug.LogError(exception);
        }
        return response;
    }


}
