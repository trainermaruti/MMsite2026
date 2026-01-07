using SkillTechNavigator.Models;

namespace SkillTechNavigator.Services
{
    public interface IGeminiService
    {
        Task<ChatResponse> SendMessageAsync(ChatRequest request);
    }
}
