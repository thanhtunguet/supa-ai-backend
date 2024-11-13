namespace SupaGPT.Entities;

public class AiEndpoint
{
    public string Url { get; set; }

    public string DefaultModel { get; set; }

    public string Name { get; set; }

    public static AiEndpoint PrivateGPT = new AiEndpoint()
    {
        Url = "https://privategpt.thanhtunguet.info/v1",
        DefaultModel = AiModel.Llama31.Name,
        Name = "PrivateGPT",
    };

    public static AiEndpoint ArceeVylinh = new AiEndpoint()
    {
        Url = "https://gpt.thanhtunguet.info/v1",
        DefaultModel = AiModel.ArceeVylinh.Name,
        Name = "ArceeVylinh",
    };

    public static AiEndpoint OpenAI = new AiEndpoint()
    {
        Url = "",
        DefaultModel = AiModel.Gpt4oMini.Name,
        Name = "OpenAI",
    };



    public static List<AiEndpoint> Values = new List<AiEndpoint>()
    {
        PrivateGPT,
        ArceeVylinh,
        OpenAI,
    };
}
