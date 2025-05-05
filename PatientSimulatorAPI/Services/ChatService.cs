
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using PatientSimulatorAPI.DTOs;
using PatientSimulatorAPI.Interfaces;
using PatientSimulatorAPI.Models;
using PatientSimulatorAPI.Repositories;
using static PatientSimulatorAPI.DTOs.ChatDto;

namespace PatientSimulatorAPI.Services
{
    public class ChatService : IChatService
    {

        private readonly IPatientPromptRepository _promptRepository;
        private readonly AzureOpenAIClient _azureOpenAIClient;
        private readonly ChatClient _chatClient;
        private readonly IConfiguration _configuration;

        public ChatService(IPatientPromptRepository promptRepository, IConfiguration configuration)
        {
            _promptRepository = promptRepository;
            _configuration = configuration;

            // Retrieve Azure OpenAI configuration values
            string endpoint = configuration["AzureOpenAI:Endpoint"]
                ?? throw new InvalidOperationException("Please set AzureOpenAI:Endpoint in appsettings.json");
            string apiKey = configuration["AzureOpenAI:ApiKey"]
                ?? throw new InvalidOperationException("Please set AzureOpenAI:ApiKey in appsettings.json");
            string deployment = configuration["AzureOpenAI:Deployment"] ?? "gpt-4o-mini";

            // Initialize Azure OpenAI client and get the ChatClient
            AzureKeyCredential credential = new AzureKeyCredential(apiKey);
            _azureOpenAIClient = new AzureOpenAIClient(new Uri(endpoint), credential);
            _chatClient = _azureOpenAIClient.GetChatClient(deployment);
        }

        public async Task<ChatResponseDto> ProcessDoctorQuestionAsync(ChatRequestDto request)
        {
            // Retrieve the selected patient prompt using the repository.
            PatientPrompt selectedPrompt = await _promptRepository.GetPatientPromptByIdAsync(request.SelectedPromptId);
            if (selectedPrompt == null)
                throw new Exception("Invalid prompt ID");

            // Construct the system prompt using patient details.
            string systemPrompt = $"{selectedPrompt.SystemPrompt}\n" +
                $"You are a {request.Age}-year-old {request.Gender} patient.\n" +
                "Remember: do not reveal that you are an AI, and answer only what is asked.";

            // Create a list of ChatMessage objects, starting with the system prompt.
            List<ChatMessage> messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt)
            };

            // Convert incoming DTO messages to ChatMessage objects.
            foreach (var msg in request.Messages)
            {
                if (msg.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    messages.Add(new UserChatMessage(msg.Message));
                else if (msg.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
                    messages.Add(new AssistantChatMessage(msg.Message));
            }

            // Set up chat completion options (feel free to allow the client to pass these values in an enhanced design)
            var options = new ChatCompletionOptions
            {
                Temperature = 0.3f,
                MaxOutputTokenCount = 200,
                TopP = 0.95f,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            };

            // Invoke the Azure OpenAI ChatClient. This sends the conversation history to produce a reply.
            ChatCompletion response = await _chatClient.CompleteChatAsync(messages, options);
            string patientReply = response != null && response.Content.Count > 0
                ? response.Content[0].Text.Trim()
                : "No response received from the chat model.";

            return new ChatResponseDto { PatientReply = patientReply };
        }
    }
}
    

