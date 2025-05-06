using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.Json;
using PatientSimulatorAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using PatientSimulatorAPI.Interfaces;

namespace PatientSimulatorAPI.Repositories
{
    public class JsonPatientPromptRepository : IPatientPromptRepository
    {
        private readonly string _filePath;

        public JsonPatientPromptRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "PatientPrompts.json");
        }

        public async Task<IEnumerable<PatientPrompt>> GetAllAsync()
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"PatientPrompts.json not found at {_filePath}");

            var json = await File.ReadAllTextAsync(_filePath);
            var collection = JsonSerializer.Deserialize<PatientPromptCollection>(json);
            return collection?.PatientPrompts
                   ?? throw new InvalidOperationException("Loaded JSON but no prompts found.");
        }

        public async Task<PatientPrompt?> GetByIdAsync(int id)
        {
            var prompts = await GetAllAsync();
            return prompts.FirstOrDefault(p => p.Id == id);
        }

        public async Task<PatientPrompt?> GetPatientPromptByIdAsync(int selectedPromptId)
        {
            // Alias to GetByIdAsync
            return await GetByIdAsync(selectedPromptId);
        }
    }
}



