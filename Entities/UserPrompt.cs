namespace SupaGPT.Entities;

public class UserPrompt
{
    public string Name { get; set; }

    public string Model { get; set; }

    public List<ChatMessageDTO> Messages { get; set; }

    public string SystemPrompt { get; set; }
}