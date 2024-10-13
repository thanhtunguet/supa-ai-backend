using System.ClientModel;
using Microsoft.AspNetCore.SignalR;
using OpenAI;
using System.Threading.Tasks;
using OpenAI.Chat;
using SupaGPT.Entities;

namespace SupaGPT.Hubs
{
    public class ChatHub : Hub
    {
        private string? OPENAI_ENDPOINT;
        private string OPENAI_API_KEY;
        private string OPENAI_MODEL;

        private static string AI_NAME = "Susu";

        private OpenAIClient _openAiClient;

        public ChatHub()
        {
            OPENAI_ENDPOINT = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT");
            OPENAI_API_KEY = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
            OPENAI_MODEL = Environment.GetEnvironmentVariable("OPENAI_MODEL")!;
            bool isNullOrEmpty = string.IsNullOrEmpty(OPENAI_ENDPOINT);

            if (isNullOrEmpty)
            {
                _openAiClient = new OpenAIClient(OPENAI_API_KEY);
            }
            else
            {
                OpenAIClientOptions options = new OpenAIClientOptions()
                {
                    Endpoint = new Uri(OPENAI_ENDPOINT!),
                };
                ApiKeyCredential credential = new ApiKeyCredential(OPENAI_API_KEY);
                _openAiClient = new OpenAIClient(credential, options);
            }
        }


        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, List<ChatMessageDTO> messages)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, messages);
            ChatCompletionOptions chatCompletionOptions = new ChatCompletionOptions()
            {
                Temperature = 0f,
            };
            List<ChatMessage> chatMessages = new List<ChatMessage>();

            for (int i = 0; i < messages.Count; i++)
            {
                ChatMessageDTO msg = messages[i];
                if (msg.user == AI_NAME)
                {
                    chatMessages.Add(new AssistantChatMessage(msg.message));
                }
                else
                {
                    chatMessages.Add(new UserChatMessage(msg.message));
                }
            }

            CollectionResult<StreamingChatCompletionUpdate> completionUpdates =
                _openAiClient.GetChatClient(OPENAI_MODEL)
                    .CompleteChatStreaming(chatMessages, chatCompletionOptions);

            foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0)
                {
                    var chunk = completionUpdate.ContentUpdate[0].Text;
                    await Clients.Caller.SendAsync("ReceiveMessage", AI_NAME, chunk);
                }

                if (completionUpdate.FinishReason != null)
                {
                    await Clients.Caller.SendAsync("CompleteTyping", AI_NAME, "");
                }
            }
        }
    }
}