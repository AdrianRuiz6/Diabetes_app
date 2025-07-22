using System;
using System.Threading.Tasks;
using com.studios.taprobana;
using Master.Domain.PetCare;

namespace Master.Infrastructure
{
    public class OpenAIChatBot : IChatBot
    {
        private readonly ChatCompletionsApi _chatCompletionsApi;

        public OpenAIChatBot()
        {
            string apiKey = "";
            apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            string initialSystemText = "Si te envío el nombre de una comida, me dirás los hidratos de carbono" +
                " que tiene para un niño de 8 años. Si no es una comida, responderás: 'No has escrito" +
                " una comida, prueba otra vez'. Siempre dirás específicamente la oración: 'La comida \"x\" tiene" +
                " x gramos de hidratos de carbono'. Si no tiene hidratos, dirás que tiene 0 gramos.\nTe pongo varios ejemplos:" +
                "\nNombre de la comida: Arroz a la cubana\nRespuesta: La comida \"arroz a la cubana\" tiene aproximadamente 45 gramos de hidratos de carbono." +
                "\nNombre de la comida: Manzana\nRespuesta: La comida \"manzana\" tiene aproximadamente 14 gramos de hidratos de carbono." +
                "\nNombre de la comida: Carrillada\nRespuesta: La comida \"carrillada\" tiene 0 gramos de hidratos de carbono." +
                "\nNombre de la comida: Hamburguesa con queso\nRespuesta: La comida \"hamburguesa con queso\" tiene aproximadamente 30 gramos de hidratos de carbono." +
                "\nNombre de la comida: Ensalada\nRespuesta: La comida \"ensalada\" tiene aproximadamente 11 gramos de hidratos de carbono.";

            // Iniciar chatGPT y mandar el mensaje de System.
            _chatCompletionsApi = new(apiKey);
            _chatCompletionsApi.ConversationHistoryMemory = 0;
            _chatCompletionsApi.SetSystemMessage(initialSystemText);
        }

        public async Task<string> Ask(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            try
            {
                ChatCompletionsRequest request = new ChatCompletionsRequest
                {
                    Model = "gpt-4o",
                    Temperature = 0,
                    MaxTokens = 50,
                    TopP = 1,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0,
                };

                request.AddMessage(new Message(Roles.USER, input));
                ChatCompletionsResponse response = await _chatCompletionsApi.CreateChatCompletionsRequest(request);
                return response.GetResponseMessage();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Error al consultar OpenAI: " + ex.Message);
                return "";
            }
        }
    }
}
