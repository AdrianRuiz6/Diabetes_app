using com.studios.taprobana;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

namespace Master.Domain.PetCare
{
    public class FoodBot
    {
        private string initialSystemText;

        private string _response;
        public bool isResponseAvailable;

        private ChatCompletionsApi chatCompletionsApi;
        private readonly string apiKey = "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";

        private ChatCompletionsRequest chatCompletionsRequest;

        public FoodBot() { }

        private void StartConversation()
        {
            initialSystemText = "Si te envío el nombre de una comida, me dirás los hidratos de carbono" +
                " que tiene para un niño de 8 años. Si no es una comida, responderás: 'No has escrito" +
                " una comida, prueba otra vez'. Siempre dirás específicamente la oración: 'La comida \"x\" tiene" +
                " x gramos de hidratos de carbono'. Si no tiene hidratos, dirás que tiene 0 gramos.\nTe pongo varios ejemplos:" +
                "\nNombre de la comida: Arroz a la cubana\nRespuesta: La comida \"arroz a la cubana\" tiene aproximadamente 45 gramos de hidratos de carbono." +
                "\nNombre de la comida: Manzana\nRespuesta: La comida \"manzana\" tiene aproximadamente 14 gramos de hidratos de carbono." +
                "\nNombre de la comida: Carrillada\nRespuesta: La comida \"carrillada\" tiene 0 gramos de hidratos de carbono." +
                "\nNombre de la comida: Hamburguesa con queso\nRespuesta: La comida \"hamburguesa con queso\" tiene aproximadamente 30 gramos de hidratos de carbono." +
                "\nNombre de la comida: Ensalada\nRespuesta: La comida \"ensalada\" tiene aproximadamente 11 gramos de hidratos de carbono.";
            _response = "";
            isResponseAvailable = false;

            // Iniciar chatGPT y mandar el mensaje de System.
            chatCompletionsApi = new(apiKey);
            chatCompletionsApi.ConversationHistoryMemory = 10;
            chatCompletionsApi.SetSystemMessage(initialSystemText);

            chatCompletionsRequest = new ChatCompletionsRequest
            {
                Model = "gpt-4o",
                Temperature = 0,
                MaxTokens = 50,
                TopP = 1,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };
        }

        public IEnumerator Ask(string input)
        {
            StartConversation();

            if (string.IsNullOrEmpty(input))
            {
                Debug.LogError("ERROR: entrada nula.");
                _response = "";
                yield break;
            }

            // Mandar pregunta a ChatGPT
            Message message = new(Roles.USER, input);
            chatCompletionsRequest.AddMessage(message);

            ChatCompletionsResponse res = null;
            var task = chatCompletionsApi.CreateChatCompletionsRequest(chatCompletionsRequest);

            yield return new WaitUntil(() => task.IsCompleted);

            try
            {
                res = task.Result;
                _response = res.GetResponseMessage();
                isResponseAvailable = true;
            }
            catch (OpenAiRequestException exception)
            {
                Debug.LogError("ERROR en la solicitud de OpenAI: " + exception.Message);
                _response = "";
            }
            catch (Exception ex)
            {
                Debug.LogError("ERROR general: " + ex.Message);
                _response = "";
            }
        }



        public string GetResponse()
        {
            return _response;
        }
    }
}