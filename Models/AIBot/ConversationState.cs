namespace MarutiTrainingPortal.Models
{
    public class ConversationState
    {
        public string? CurrentFlow { get; set; } // ROUTER, BEGINNER, COURSE_SELECTOR, MENTORSHIP
        public string? UserBackground { get; set; } // Technical, NonTechnical
        public string? UserIntent { get; set; } // Developer, Admin, AI
        public string? SelectedCourse { get; set; }
        public bool IsFirstMessage { get; set; } = true;
        public bool EmailCaptured { get; set; } = false;
        public string? CapturedEmail { get; set; }
        public Dictionary<string, string> FlowData { get; set; } = new();
    }

    public class LeadCapture
    {
        public string Email { get; set; } = string.Empty;
        public string Interest { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
        public string Source { get; set; } = "Chat";
    }

    public class FlowRequest
    {
        public string Message { get; set; } = string.Empty;
        public ConversationState? State { get; set; }
        public List<ChatMessage>? History { get; set; }
    }

    public class FlowResponse
    {
        public string Reply { get; set; } = string.Empty;
        public ConversationState State { get; set; } = new();
        public List<string> QuickActions { get; set; } = new();
        public bool RequiresEmail { get; set; } = false;
        public string? CourseData { get; set; }
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}
