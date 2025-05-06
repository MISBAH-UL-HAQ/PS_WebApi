using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using PatientSimulatorAPI.DTOs;
using PatientSimulatorAPI.Interfaces;

namespace PatientSimulatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechController : ControllerBase
    {

        //        // Declare a private field for configuration
        //        private readonly IConfiguration _configuration;

        //        public SpeechController(IConfiguration configuration)
        //        {
        //            _configuration = configuration;
        //        }

        //        /// <summary>
        //        /// STT Endpoint: Accepts an audio file (WAV format) and returns transcribed text.
        //        /// </summary>
        //        [HttpPost("stt")]
        //        public async Task<IActionResult> RecognizeSpeech([FromForm] IFormFile audioFile)
        //        {
        //            if (audioFile == null || audioFile.Length == 0)
        //                return BadRequest("No audio file provided.");

        //            // Save the uploaded audio file temporarily.
        //            var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "_" + audioFile.FileName);
        //            using (var stream = new FileStream(tempFilePath, FileMode.Create))
        //            {
        //                await audioFile.CopyToAsync(stream);
        //            }

        //            // Configure the SpeechRecognizer with the Azure Speech subscription.
        //            var speechKey = _configuration["AzureSpeech:ApiKey"];
        //            var speechRegion = _configuration["AzureSpeech:Region"];
        //            var config = SpeechConfig.FromSubscription(speechKey, speechRegion);

        //            // Use the saved file as input (ensure the file is a supported WAV file).
        //            using var audioConfig = AudioConfig.FromWavFileInput(tempFilePath);
        //            using var recognizer = new SpeechRecognizer(config, audioConfig);
        //            var result = await recognizer.RecognizeOnceAsync();

        //            // Clean up the temporary file.
        //            System.IO.File.Delete(tempFilePath);

        //            // Check result and return the recognized text.
        //            if (result.Reason == ResultReason.RecognizedSpeech)
        //                return Ok(new { recognizedText = result.Text });
        //            else
        //                return StatusCode(500, new { error = $"Speech recognition failed: {result.Reason}" });
        //        }

        //        /// <summary>
        //        /// TTS Endpoint: Accepts text, synthesizes speech, and returns the audio file.
        //        /// </summary>
        //        [HttpPost("tts")]
        //        [Produces("audio/wav")]  // Helps Swagger understand that the response is an audio file.
        //        public async Task<IActionResult> SynthesizeSpeech([FromBody] TTSRequest request)
        //        {
        //            if (request == null || string.IsNullOrWhiteSpace(request.Text))
        //                return BadRequest("Text is required for TTS.");

        //            var speechKey = _configuration["AzureSpeech:ApiKey"];
        //            var speechRegion = _configuration["AzureSpeech:Region"];
        //            var config = SpeechConfig.FromSubscription(speechKey, speechRegion);

        //            // Instantiate the SpeechSynthesizer.
        //            using var synthesizer = new SpeechSynthesizer(config);
        //            var result = await synthesizer.SpeakTextAsync(request.Text);

        //            if (result.Reason != ResultReason.SynthesizingAudioCompleted)
        //                return StatusCode(500, new { error = $"TTS failed with reason: {result.Reason}" });

        //            // Get the synthesized audio data as byte array.
        //            byte[] audioData = result.AudioData;

        //            // Return the audio data as a file with MIME type audio/wav.
        //            return File(audioData, "audio/wav", "synthesized.wav");
        //        }
        //    }
        //}








        //        private readonly ISpeechService _speechService;

        //        public SpeechController(ISpeechService speechService)
        //        {
        //            _speechService = speechService;
        //        }

        //        /// <summary>
        //        /// Speech-to-Text (STT) endpoint.
        //        /// It expects an audio file (WAV) sent as form-data.
        //        /// </summary>
        //        [HttpPost("stt")]
        //        public async Task<IActionResult> RecognizeSpeech([FromForm] FileUploadDto fileUpload)
        //        {
        //            if (fileUpload == null || fileUpload.AudioFile == null || fileUpload.AudioFile.Length == 0)
        //                return BadRequest("No audio file provided.");

        //            using var stream = fileUpload.AudioFile.OpenReadStream();
        //            var recognizedText = await _speechService.SpeechToTextAsync(stream);
        //            return Ok(new { recognizedText });
        //        }

        //        /// <summary>
        //        /// Text-to-Speech (TTS) endpoint.
        //        /// It accepts JSON input containing text to be synthesized.
        //        /// Returns the synthesized audio as a WAV file.
        //        /// </summary>
        //        [HttpPost("tts")]
        //        public async Task<IActionResult> SynthesizeSpeech([FromBody] DTOs.TTSRequest request)
        //        {
        //            if (request == null || string.IsNullOrWhiteSpace(request.Text))
        //                return BadRequest("Text is required for TTS.");

        //            var speechResult = await _speechService.TextToSpeechAsync(request.Text);
        //            return File(speechResult.AudioData, "audio/wav", "synthesized.wav");
        //        }
        //    }
        //}






        private readonly ISpeechService _speechService;

        public SpeechController(ISpeechService speechService)
        {
            _speechService = speechService;
        }

        /// <summary>
        /// Speech-to-Text (STT) endpoint.
        /// Expects an audio file (WAV) sent as form-data.
        /// </summary>
        [HttpPost("stt")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RecognizeSpeech([FromForm] FileUploadDto fileUpload)
        {
            if (fileUpload?.AudioFile == null || fileUpload.AudioFile.Length == 0)
                return BadRequest("No audio file provided.");

            using var stream = fileUpload.AudioFile.OpenReadStream();
            // Change this line:

            var recognizedText = await _speechService.RecognizeAsync(stream);

            // And change the return to use your DTO:
            return Ok(new SpeechRecognitionDto { RecognizedText = recognizedText });
        }

        //private readonly ISpeechService _speechService;

        //public SpeechController(ISpeechService speechService)
        //{
        //    _speechService = speechService;
        //}

        //[HttpPost("stt")]
        //public async Task<ActionResult<SpeechRecognitionDto>> Recognize([FromForm] IFormFile audioFile)
        //{
        //    if (audioFile == null || audioFile.Length == 0)
        //        return BadRequest(new { error = "Audio file is required." });

        //    using var stream = audioFile.OpenReadStream();
        //    var text = await _speechService.RecognizeAsync(stream);
        //    return Ok(new SpeechRecognitionDto { RecognizedText = text });
        //}

        //    [HttpPost("tts")]
        //    public async Task<IActionResult> Synthesize([FromBody] SpeechSynthesisRequestDto dto)
        //    {
        //        if (string.IsNullOrWhiteSpace(dto.Text))
        //            return BadRequest(new { error = "Text is required for synthesis." });

        //        var audioBytes = await _speechService.SynthesizeAsync(dto.Text);
        //        return File(audioBytes, "audio/wav", "patient.wav");
        //    }
        //}
        [HttpPost("tts")]
        [Produces("audio/wav")]
        public async Task<IActionResult> SynthesizeSpeech([FromBody] DTOs.TTSRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("Text is required for TTS.");

            var audioData = await _speechService.SynthesizeAsync(request.Text);
            return File(audioData, "audio/wav");
        }
    }
}