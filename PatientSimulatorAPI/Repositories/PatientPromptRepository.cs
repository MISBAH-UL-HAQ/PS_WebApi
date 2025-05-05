
using System.Text.Json;
using PatientSimulatorAPI.Models;
using Microsoft.Extensions.Configuration;
namespace PatientSimulatorAPI.Repositories
{
    public class PatientPromptRepository : IPatientPromptRepository
    {
        private readonly List<PatientPrompt> _patientPrompts;

        public PatientPromptRepository(IConfiguration configuration)
        {
            // Read the JSON file. Ensure PatientPrompts.json is placed in the project root.
            string promptJson = File.ReadAllText("PatientPrompts.json");
            var promptCollection = JsonSerializer.Deserialize<PatientPromptCollection>(promptJson);
            _patientPrompts = promptCollection?.PatientPrompts ?? new List<PatientPrompt>();
        }

        public async Task<PatientPrompt> GetPatientPromptByIdAsync(int id)
        {
            var prompt = _patientPrompts.FirstOrDefault(p => p.Id == id);
            return await Task.FromResult(prompt);
        }
    }
}
 