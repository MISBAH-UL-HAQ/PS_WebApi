
namespace PatientSimulatorAPI.DTOs
{
    public class ChatDto
    {
        public class ChatRequestDto
        {
            /// <summary>
            /// The GUID for this conversation. 
            /// Leave blank on first call to have server generate one.
            /// </summary>
            public string? SessionId { get; set; }

            /// <summary>
            /// Only on the very first call:
            /// The patient’s condition prompt ID (e.g., 1 for Asthma).
            /// </summary>
            public int? SelectedPromptId { get; set; }

            /// <summary>
            /// Only on the very first call: patient’s age.
            /// </summary>
            public int? Age { get; set; }

            /// <summary>
            /// Only on the very first call: patient’s gender ("male"/"female").
            /// </summary>
            public string? Gender { get; set; }

            /// <summary>
            /// The doctor’s latest question/text.
            /// </summary>
            public string Message { get; set; } = string.Empty;
        }

        public class ChatResponseDto
        {
            /// <summary>
            /// The conversation GUID to persist on the client.
            /// </summary>
            public string SessionId { get; set; } = string.Empty;

            /// <summary>
            /// The AI-generated patient reply.
            /// </summary>
            public string PatientReply { get; set; } = string.Empty;
        }
    }
}
