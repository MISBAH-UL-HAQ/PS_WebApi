namespace PatientSimulatorAPI.DTOs
{
    public class ChatDto
    {
        public class ChatMessageDto
        {
            // Role can be "system", "user", or "assistant"
            public string Role { get; set; }
            public string Message { get; set; }
        }

        public class ChatRequestDto
        {
            // This is the patient’s condition prompt ID (e.g., 1 for Asthma)
            public int SelectedPromptId { get; set; }
            public int Age { get; set; }
            public string Gender { get; set; }
            // Conversation messages (the history) sent by the client
            public List<ChatMessageDto> Messages { get; set; }
        }

        public class ChatResponseDto
        {
            public string PatientReply { get; set; }
        }
    }
}

