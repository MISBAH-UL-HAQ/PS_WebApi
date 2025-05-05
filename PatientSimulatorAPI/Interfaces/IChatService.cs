using static PatientSimulatorAPI.DTOs.ChatDto;

namespace PatientSimulatorAPI.Interfaces
{
    public interface IChatService
    {
        Task<ChatResponseDto> ProcessDoctorQuestionAsync(ChatRequestDto request);
    }
}

