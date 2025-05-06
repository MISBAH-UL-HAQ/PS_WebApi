using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using PatientSimulatorAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatientSimulatorAPI.Services

{
    public class AzureOpenAIService : IOpenAIService
    {
        private readonly ChatClient _chatClient;

        public AzureOpenAIService(IConfiguration config)
        {
            var endpoint = new Uri(config["AzureOpenAI:Endpoint"]);
            var credential = new AzureKeyCredential(config["AzureOpenAI:ApiKey"]);
            _chatClient = new AzureOpenAIClient(endpoint, credential)
                          .GetChatClient(config["AzureOpenAI:Deployment"]);
        }

        public async Task<ChatCompletion> GetPatientResponseAsync(IEnumerable<ChatMessage> history, ChatCompletionOptions options)
        {
            return await _chatClient.CompleteChatAsync(history, options);
        }
    }
}
    