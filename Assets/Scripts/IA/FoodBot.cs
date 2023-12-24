using com.studios.taprobana;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FoodBot : MonoBehaviour
{
    private string initialSystemText;

    [HideInInspector] public string response;

    private ChatCompletionsApi chatCompletionsApi;
    private readonly string apiKey = "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";

    void Start()
    {
        StartConversation();

        initialSystemText = "Si te envío el nombre de una comida, me dirás los hidratos de carbono" +
            " que tiene para un niño de 8 años. Si no es una comida, responderás: 'No has escrito" +
            " una comida, prueba otra vez'. Siempre dirás específicamente la oración: 'La " +
            "comida \"x\" tiene x gramos de hidratos de carbono. Si no tiene hidratos, dirás que " +
            "tiene 0 gramos.";
    }

    private void StartConversation()
    {
        // Iniciar chatGPT y mandar el mensaje de System.
        chatCompletionsApi = new(apiKey);
        chatCompletionsApi.ConversationHistoryMemory = 5;
        chatCompletionsApi.SetSystemMessage(initialSystemText);
    }

    public async void Ask(string input)
    {
        try
        {
            Debug.Log(input.GetType());
            // Mandar pregunta a ChatGPT.
            ChatCompletionsRequest chatCompletionsRequest = new ChatCompletionsRequest();
            Message message = new(Roles.USER, input);

            chatCompletionsRequest.AddMessage(message);

            ChatCompletionsResponse res = await chatCompletionsApi.CreateChatCompletionsRequest(chatCompletionsRequest);
            response = res.GetResponseMessage();
            

        }
        catch (OpenAiRequestException exception)
        {
            Debug.LogError(exception);
        }
    }


}
