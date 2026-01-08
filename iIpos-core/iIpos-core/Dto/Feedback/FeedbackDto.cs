namespace iIpos_core.Dto.Feedback
{
    public class FeedbackDto
    {
        public string Token { get; set; } = null!;
        public int Rating { get; set; }
        public List<string> NegativeFeedbackTags { get; set; } = new();
        public string? Comments { get; set; }
        public string? CustomerPhoneNumber { get; set; }
    }
}
