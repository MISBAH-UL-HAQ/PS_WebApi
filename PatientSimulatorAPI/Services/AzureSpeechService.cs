using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using PatientSimulatorAPI.Interfaces;
using PatientSimulatorAPI.Models;
//namespace PatientSimulatorAPI.Services
//{
//    public class AzureSpeechService : ISpeechService
//    {

//        private readonly SpeechConfig _config;

//        public AzureSpeechService(IOptions<AzureSettings> options)
//        {
//            var settings = options.Value;
//            _config = SpeechConfig.FromSubscription(settings.AzureSpeech.ApiKey, settings.AzureSpeech.Region);
//        }

//        public async Task<string> SpeechToTextAsync(Stream audioStream)
//        {
//            // Wrap the incoming stream in a pull stream using our helper.
//            var pullStream = AudioInputStream.CreatePullStream(new BinaryAudioStreamReader(audioStream));
//            using var audioConfig = AudioConfig.FromStreamInput(pullStream);
//            using var recognizer = new SpeechRecognizer(_config, audioConfig);
//            var result = await recognizer.RecognizeOnceAsync();

//            if (result.Reason == ResultReason.RecognizedSpeech)
//                return result.Text;
//            return string.Empty;
//        }

//        //public async Task<SpeechResult> TextToSpeechAsync(string text)
//        //{
//        //    using var audioConfig = AudioConfig.FromDefaultSpeakerOutput();
//        //    using var synthesizer = new SpeechSynthesizer(_config, audioConfig);
//        //    var result = await synthesizer.SpeakTextAsync(text);

//        //    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
//        //    {
//        //        using var audioDataStream = AudioDataStream.FromResult(result);
//        //        using var ms = new MemoryStream();
//        //        // Use the synchronous method, as SaveToWaveStreamAsync is not available.
//        //        audioDataStream.SaveToWaveStream(ms);
//        //        byte[] audioData = ms.ToArray();
//        //        return new SpeechResult { Text = text, AudioData = audioData };
//        //    }

//        //    return new SpeechResult { Text = text, AudioData = Array.Empty<byte>() };
//        //}


//        public async Task<SpeechResult> TextToSpeechAsync(string text)
//        {
//            using var audioConfig = AudioConfig.FromDefaultSpeakerOutput();
//            using var synthesizer = new SpeechSynthesizer(_config, audioConfig);
//            var result = await synthesizer.SpeakTextAsync(text);

//            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
//            {
//                return new SpeechResult
//                {
//                    Text = text,
//                    AudioData = result.AudioData
//                };
//            }

//            return new SpeechResult
//            {
//                Text = text,
//                AudioData = Array.Empty<byte>()
//            };
//        }
//    }
//}


namespace PatientSimulatorAPI.Services
{
    public class AzureSpeechService : ISpeechService
    {
        private readonly SpeechConfig _config;

        public AzureSpeechService(IOptions<AzureSettings> options)
        {
            var settings = options.Value;
            _config = SpeechConfig.FromSubscription(settings.AzureSpeech.ApiKey, settings.AzureSpeech.Region);
            _config.SpeechRecognitionLanguage = "en-US"; // Adjust as needed
        }

        public async Task<string> SpeechToTextAsync(Stream audioStream)
        {
            // Wrap the incoming stream using BinaryAudioStreamReader.
            var pullStream = AudioInputStream.CreatePullStream(new BinaryAudioStreamReader(audioStream));
            using var audioConfig = AudioConfig.FromStreamInput(pullStream);
            using var recognizer = new SpeechRecognizer(_config, audioConfig);
            var result = await recognizer.RecognizeOnceAsync();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                return result.Text;
            }
            else
            {
                // If recognition failed, log only the basic reason
                System.Diagnostics.Debug.WriteLine($"Speech recognition failed. Reason: {result.Reason}");
                return string.Empty;
            }
        }

        public async Task<SpeechResult> TextToSpeechAsync(string text)
        {
            using var audioConfig = AudioConfig.FromDefaultSpeakerOutput();
            using var synthesizer = new SpeechSynthesizer(_config, audioConfig);
            var result = await synthesizer.SpeakTextAsync(text);

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                // Get the audio data stream from the synthesis result.
                using var audioDataStream = AudioDataStream.FromResult(result);

                // Read the synthesized audio data manually.
                var data = new List<byte>();
                byte[] buffer = new byte[4096];
                uint bytesRead = 0;
                do
                {
                    // Note: Swap the parameters—first is the uint (buffer size), second is the buffer.
                    bytesRead = audioDataStream.ReadData((uint)buffer.Length, buffer);
                    if (bytesRead > 0)
                    {
                        data.AddRange(buffer.Take((int)bytesRead));
                    }
                } while (bytesRead > 0);

                return new SpeechResult { Text = text, AudioData = data.ToArray() };
            }

            return new SpeechResult { Text = text, AudioData = System.Array.Empty<byte>() };
        }
    }
}