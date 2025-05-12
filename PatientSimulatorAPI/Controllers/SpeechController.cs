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
        public async Task<IActionResult> RecognizeSpeech([FromForm] FileUploadDto dto)
        {
            if (dto.AudioFile == null || dto.AudioFile.Length == 0)
                return BadRequest("No audio file.");

            await using var stream = dto.AudioFile.OpenReadStream();
            try
            {
                var text = await _speechService.RecognizeAsync(stream);
                return Ok(new SpeechRecognitionDto { RecognizedText = text });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


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