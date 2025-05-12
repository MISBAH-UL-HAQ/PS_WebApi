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
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("doctor")]
        public async Task<ActionResult<ChatResponseDto>> PostDoctorMessage([FromBody] ChatRequestDto request)
        {
            try
            {
                var response = await _chatService.ProcessDoctorQuestionAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
