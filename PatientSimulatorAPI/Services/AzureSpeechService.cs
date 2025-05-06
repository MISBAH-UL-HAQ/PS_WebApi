using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
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
            var pushStream = AudioInputStream.CreatePushStream();
            using var binaryReader = new BinaryReader(audioStream);
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (bytesRead > 0)
                {
                    var chunk = new byte[bytesRead];
                    Array.Copy(buffer, chunk, bytesRead);
                    pushStream.Write(chunk);
                }
            }
            pushStream.Close();

            using var audioConfig = AudioConfig.FromStreamInput(pushStream);
            using var recognizer = new SpeechRecognizer(_speechConfig, audioConfig);
            var result = await recognizer.RecognizeOnceAsync();
            if (result.Reason == ResultReason.RecognizedSpeech)
                return result.Text;
            else
                throw new InvalidOperationException($"Speech recognition failed: {result.Reason}");
        }

        public async Task<byte[]> SynthesizeAsync(string text)
        {
            using var synthesizer = new SpeechSynthesizer(_speechConfig, null);
            var result = await synthesizer.SpeakTextAsync(text);
            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                return result.AudioData;
            else
                throw new InvalidOperationException($"TTS synthesis failed: {result.Reason}");
        }
    }
}