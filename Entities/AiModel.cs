namespace SupaGPT.Entities;

public class AiModel
{
    public string Name { get; set; }

    public bool IsDefault { get; set; }

    public static AiModel ArceeVylinh = new AiModel() { Name = "arcee-vylinh", IsDefault = true };

    public static AiModel Gpt4oMini = new AiModel() { Name = "gpt-4o-mini", IsDefault = false };

    public static AiModel Gpt4o = new AiModel() { Name = "gpt-4o", IsDefault = false };

    public static List<AiModel> SupportedModels = new List<AiModel>()
    {
        ArceeVylinh,
        Gpt4oMini,
        Gpt4o,
    };
}