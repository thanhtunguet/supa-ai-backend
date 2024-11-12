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
        private const string ReceiveMessage = "ReceiveMessage";
        private const string CompleteTyping = "CompleteTyping";

        private static string _aiName = "Susu";

        private OpenAIClient CreateClient(string? endpoint)
        {
            Console.WriteLine($"Endpoint = {endpoint}");
            string? aiEndpoint = String.IsNullOrEmpty(endpoint)
                ? Environment.GetEnvironmentVariable("OPENAI_ENDPOINT")
                : endpoint;
            string aiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
            string aiModel = Environment.GetEnvironmentVariable("OPENAI_MODEL")!;

            bool isNullOrEmpty = string.IsNullOrEmpty(aiEndpoint);

            OpenAIClient client;
            if (isNullOrEmpty)
            {
                client = new OpenAIClient(aiKey);
            }
            else
            {
                OpenAIClientOptions options = new OpenAIClientOptions()
                {
                    Endpoint = new Uri(aiEndpoint!),
                };
                ApiKeyCredential credential = new ApiKeyCredential(aiKey);
                client = new OpenAIClient(credential, options);
            }

            return client;
        }


        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, UserPrompt prompt)
        {
            await Clients.All.SendAsync(ReceiveMessage, user, prompt.Messages);
            ChatCompletionOptions chatCompletionOptions = new ChatCompletionOptions()
            {
                Temperature = 0f,
            };
            List<ChatMessage> chatMessages = new List<ChatMessage> { new SystemChatMessage(prompt.SystemPrompt) };

            foreach (var msg in prompt.Messages)
            {
                if (msg.user == _aiName)
                {
                    chatMessages.Add(new AssistantChatMessage(msg.message));
                }
                else
                {
                    chatMessages.Add(new UserChatMessage(msg.message));
                }
            }

            OpenAIClient client = CreateClient(prompt.Endpoint);

            CollectionResult<StreamingChatCompletionUpdate> completionUpdates =
                client.GetChatClient(prompt.Model)
                    .CompleteChatStreaming(chatMessages, chatCompletionOptions);

            foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0)
                {
                    var chunk = completionUpdate.ContentUpdate[0].Text;
                    await Clients.Caller.SendAsync(ReceiveMessage, _aiName, chunk);
                }

                if (completionUpdate.FinishReason != null)
                {
                    await Clients.Caller.SendAsync(CompleteTyping, _aiName, "");
                }
            }
        }
    }
}