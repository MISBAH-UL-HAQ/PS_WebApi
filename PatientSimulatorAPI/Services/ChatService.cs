
using Azure;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using Microsoft.Extensions.Caching.Memory;
using OpenAI.Chat;
using PatientSimulatorAPI.DTOs;
using PatientSimulatorAPI.Interfaces;
using PatientSimulatorAPI.Models;
using static PatientSimulatorAPI.DTOs.ChatDto;

namespace PatientSimulatorAPI.Services
{
    public class ChatService : IChatService
    {       
            private readonly IMemoryCache _cache;
            private readonly IPatientPromptRepository _promptRepo;
            private readonly IOpenAIService _openAi;
            private readonly MemoryCacheEntryOptions _cacheOptions;

            public ChatService(
                IMemoryCache cache,
                IPatientPromptRepository promptRepo,
                IOpenAIService openAi)
            {
                _cache = cache;
                _promptRepo = promptRepo;
                _openAi = openAi;

                // Cache expires after 30 minutes of inactivity
                _cacheOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                };
            }

            public async Task<ChatResponseDto> ProcessDoctorQuestionAsync(ChatRequestDto request)
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                    throw new ArgumentException("Doctor message cannot be empty.");

                // 1. Try to get conversation history from cache
                List<ChatMessage> messages;
                if (string.IsNullOrWhiteSpace(request.SessionId) || !_cache.TryGetValue(request.SessionId, out messages))
                {
                    // 1a. New conversation
                    if (!request.SelectedPromptId.HasValue || !request.Age.HasValue || string.IsNullOrWhiteSpace(request.Gender))
                        throw new ArgumentException("Prompt ID, Age, and Gender are required for the first message.");

                    var prompt = await _promptRepo.GetPatientPromptByIdAsync(request.SelectedPromptId.Value);
                    if (prompt == null)
                        throw new InvalidOperationException("Invalid prompt ID provided.");

                    string systemText = $@"{prompt.SystemPrompt}
                    You are a {request.Age}-year-old {request.Gender?.ToLower()} patient.
                    Remember: do not reveal that you are an AI.";

                    messages = new List<ChatMessage>
                {
                    new SystemChatMessage(systemText)
                };

                    // Generate a new session ID if not provided
                    request.SessionId = Guid.NewGuid().ToString();
                }

                // 2. Append the doctor's question to conversation
                messages.Add(new UserChatMessage(request.Message));

                // 3. Prepare chat options
                var options = new ChatCompletionOptions
                {
                    Temperature = 0.3f,
                    MaxOutputTokenCount = 200,
                    TopP = 0.95f,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0
                };

                // 4. Get response from OpenAI
                var replyText = await _openAi.GetPatientResponseAsync(messages, options);

                // 5. Add assistant's reply to conversation history
                messages.Add(new AssistantChatMessage(replyText));

                // 6. Store updated history in cache
                _cache.Set(request.SessionId, messages, _cacheOptions);

                // 7. Return response
                return new ChatResponseDto
                {
                    SessionId = request.SessionId,
                    PatientReply = replyText
                };
            }
        }
    }

