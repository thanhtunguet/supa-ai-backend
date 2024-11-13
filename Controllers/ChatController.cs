using Microsoft.AspNetCore.Mvc;
using SupaGPT.Entities;

namespace SupaGPT.Controllers;

[Route("[controller]")]
public class ChatController : Controller
{
    [HttpGet("/api/models")]
    public async Task<List<AiModel>> GetModels()
    {
        return await Task.FromResult(AiModel.SupportedModels);
    }

    [HttpGet("/api/endpoints")]
    public async Task<List<AiEndpoint>> GetEndpoints()
    {
        return await Task.FromResult(AiEndpoint.Values);
    }
}
