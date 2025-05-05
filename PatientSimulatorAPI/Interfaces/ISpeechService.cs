using PatientSimulatorAPI.Models;

namespace PatientSimulatorAPI.Interfaces
{
    public interface ISpeechService
    {
        Task<string> SpeechToTextAsync(Stream audioStream);
        Task<SpeechResult> TextToSpeechAsync(string text);
    }
}
