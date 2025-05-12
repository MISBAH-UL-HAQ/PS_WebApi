using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using PatientSimulatorAPI.Interfaces;
using PatientSimulatorAPI.Models;
namespace PatientSimulatorAPI.Services
{

    public class AzureSpeechService : ISpeechService
    {
        private readonly SpeechConfig _speechConfig;

        public AzureSpeechService(IConfiguration config)
        {
            var key = config["AzureSettings:AzureSpeech:ApiKey"];
            var region = config["AzureSettings:AzureSpeech:Region"];

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(region))
                throw new ApplicationException("Azure Speech configuration is missing.");

            _speechConfig = SpeechConfig.FromSubscription(key, region);
        }
       public async Task<string> RecognizeAsync(Stream audioStream)
{
    var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.wav");

    try
    {
        // 1. Save the incoming stream to a temp file
        await using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await audioStream.CopyToAsync(fileStream);
        }

        // 2. Now that writing is done, we can read from it
        var audioConfig = AudioConfig.FromWavFileInput(tempFilePath);
        var recognizer = new SpeechRecognizer(_speechConfig, audioConfig);

        var result = await recognizer.RecognizeOnceAsync();

        if (result.Reason == ResultReason.RecognizedSpeech)
            return result.Text;

        throw new InvalidOperationException($"STT failed: {result.Reason}");
    }
    finally
    {
        // Deleting only after recognition completes to avoid file-in-use errors
        if (File.Exists(tempFilePath))
        {
            try
            {
                File.Delete(tempFilePath);
            }
            catch
            {
                // Optional: log deletion failure instead of crashing
            }
        }
    }
}

        public async Task<byte[]> SynthesizeAsync(string text)
        {
            using var synthesizer = new SpeechSynthesizer(_speechConfig, null);
            var result = await synthesizer.SpeakTextAsync(text);
            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                return result.AudioData;
            throw new InvalidOperationException($"TTS failed: {result.Reason}");
        }
    }
}