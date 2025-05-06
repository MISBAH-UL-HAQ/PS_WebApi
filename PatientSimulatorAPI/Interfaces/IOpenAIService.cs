using Azure.AI.OpenAI;
using Azure;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI.Chat;

namespace PatientSimulatorAPI.Interfaces
{
    public interface IOpenAIService
    {
        Task<ChatCompletion> GetPatientResponseAsync(IEnumerable<ChatMessage> history, ChatCompletionOptions options);
    }
}
