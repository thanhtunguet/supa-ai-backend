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
}
