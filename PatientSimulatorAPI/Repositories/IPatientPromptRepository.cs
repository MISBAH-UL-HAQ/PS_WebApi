using PatientSimulatorAPI.Models;

namespace PatientSimulatorAPI.Repositories
{
    public interface IPatientPromptRepository
    {
        Task<PatientPrompt> GetPatientPromptByIdAsync(int id);
    }
}
