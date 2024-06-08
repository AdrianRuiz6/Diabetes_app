using com.studios.taprobana;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class FoodBot : MonoBehaviour
{
    private string initialSystemText;

    [HideInInspector] public string response;

    private ChatCompletionsApi chatCompletionsApi;
    private readonly string apiKey = "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";

    private ChatCompletionsRequest chatCompletionsRequest;

    void Start()
    {
        initialSystemText = "Si te envío el nombre de una comida, me dirás los hidratos de carbono" +
            " que tiene para un niño de 8 años. Si no es una comida, responderás: 'No has escrito" +
            " una comida, prueba otra vez'. Siempre dirás específicamente la oración: 'La comida \"x\" tiene" +
            " x gramos de hidratos de carbono'. Si no tiene hidratos, dirás que tiene 0 gramos.";

        StartConversation();
    }

    private void StartConversation()
    {
        // Iniciar chatGPT y mandar el mensaje de System.
        chatCompletionsApi = new(apiKey);
        chatCompletionsApi.ConversationHistoryMemory = 10;
        chatCompletionsApi.SetSystemMessage(initialSystemText);

        chatCompletionsRequest = new ChatCompletionsRequest
        {
            Model = "gpt-4-1106-preview",
            Temperature = 0,
            MaxTokens = 50,
            TopP = 1,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };

        Message message = new(Roles.USER, "Arroz a la cubana");
        chatCompletionsRequest.AddMessage(message);
        message = new(Roles.ASSISTANT, "La comida \"arroz a la cubana\" tiene aproximadamente 45 gramos de hidratos de carbono.");

        message = new(Roles.USER, "Manzana");
        chatCompletionsRequest.AddMessage(message);
        message = new(Roles.ASSISTANT, "La comida \"manzana\" tiene aproximadamente 14 gramos de hidratos de carbono.");

        message = new(Roles.USER, "Carrillada");
        chatCompletionsRequest.AddMessage(message);
        message = new(Roles.ASSISTANT, "La comida \"carrillada\" tiene 0 gramos de hidratos de carbono.");

        message = new(Roles.USER, "Hamburguesa con queso");
        chatCompletionsRequest.AddMessage(message);
        message = new(Roles.ASSISTANT, "La comida \"hamburguesa con queso\" tiene aproximadamente 30 gramos de hidratos de carbono..");

        message = new(Roles.USER, "Ensalada");
        chatCompletionsRequest.AddMessage(message);
        message = new(Roles.ASSISTANT, "La comida \"ensalada\" tiene aproximadamente 11 gramos de hidratos de carbono.");
    }

    public async void Ask(string input)
    {

        if (input == null)
        {
            Debug.LogError("ERROR: entrada nula.");
            response = "";
            return;
        }

        try
        {
            // Mandar pregunta a ChatGPT.
            Message message = new(Roles.USER, input);

            chatCompletionsRequest.AddMessage(message);

            ChatCompletionsResponse res = await chatCompletionsApi.CreateChatCompletionsRequest(chatCompletionsRequest);
            response = res.GetResponseMessage();
            

        }
        catch (OpenAiRequestException exception)
        {
            Debug.LogError("ERROR en la solicitud de OpenAI: " + exception.Message);
            response = "";
        }
        catch (Exception ex)
        {
            Debug.LogError("ERROR general: " + ex.Message);
            response = "";
        }
    }


}
