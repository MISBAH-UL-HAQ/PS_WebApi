namespace PatientSimulatorAPI.DTOs
{
    public class ChatDto
    {
        public class ChatMessageDto
        {
            // Role can be "system", "user", or "assistant"
            /// <summary>"system", "user", or "assistant"</summary>
            public string Role { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }

        public class ChatRequestDto
        {
            // This is the patient’s condition prompt ID (e.g., 1 for Asthma)
            public int SelectedPromptId { get; set; }
            public int Age { get; set; }
            public string Gender { get; set; } = string.Empty;

            /// <summary>Include the full conversation history, starting with the initial system prompt</summary>
            public List<ChatMessageDto> Messages { get; set; } = new();
        }

        public class ChatResponseDto
        {
            public string PatientReply { get; set; } = string.Empty;
        }
    }
}

