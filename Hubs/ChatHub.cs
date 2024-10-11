using System.ClientModel;
using Microsoft.AspNetCore.SignalR;
using OpenAI;
using System.Threading.Tasks;
using OpenAI.Chat;

namespace SupaGPT.Hubs
{
    public class ChatHub : Hub
    {
        private OpenAIClient _openAiClient;

        private static String AI_NAME = "SupaGPT";

        public ChatHub()
        {
            _openAiClient = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        }


        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);

            CollectionResult<StreamingChatCompletionUpdate> completionUpdates =
                _openAiClient.GetChatClient(Environment.GetEnvironmentVariable("OPENAI_MODEL"))
                    .CompleteChatStreaming(message);

            foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0)
                {
                    var chunk = completionUpdate.ContentUpdate[0].Text;
                    await Clients.Caller.SendAsync("ReceiveMessage", AI_NAME, chunk);
                    Console.WriteLine(completionUpdate.ContentUpdate.ToString());
                }
            }
        }
    }
}