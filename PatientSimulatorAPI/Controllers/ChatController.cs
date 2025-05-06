using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientSimulatorAPI.DTOs;
using PatientSimulatorAPI.Interfaces;
using System.Security.Cryptography;
using static PatientSimulatorAPI.DTOs.ChatDto;
namespace PatientSimulatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        //        private readonly IChatService _chatService;

        //        public ChatController(IChatService chatService)
        //        {
        //            _chatService = chatService;
        //        }

        //        /// <summary>
        //        /// Accepts a chat request from the doctor and returns the patient's reply.
        //        /// </summary>
        //        [HttpPost("doctor")]
        //        public async Task<IActionResult> DoctorChat([FromBody] ChatRequestDto request)
        //        {
        //            try
        //            {
        //                ChatResponseDto response = await _chatService.ProcessDoctorQuestionAsync(request);
        //                return Ok(response);
        //            }
        //            catch (Exception ex)
        //            {
        //                // In a production app, use structured logging and avoid exposing sensitive exception messages.
        //                return BadRequest(new { error = ex.Message });
        //            }
        //        }
        //    }
        //}


        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("doctor")]
        public async Task<ActionResult<ChatResponseDto>> PostDoctorQuestion([FromBody] ChatRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _chatService.ProcessDoctorQuestionAsync(request);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return BadRequest(new { error = "Invalid patient prompt ID." });
            }
            catch (RequestFailedException ex)
            {
                return StatusCode(502, new { error = "Error calling OpenAI service: " + ex.Message });
            }
        }
    }
}
