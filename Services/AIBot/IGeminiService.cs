using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Services
{
    public interface IGeminiService
    {
        Task<ChatResponse> SendMessageAsync(ChatRequest request);
    }
}
