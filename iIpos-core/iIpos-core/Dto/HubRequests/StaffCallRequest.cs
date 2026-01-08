namespace iIpos_core.Dto.HubRequests
{
    public class StaffCallRequest
    {
        public string? TableToken { get; set; } 
        public string? Message { get; set; } 
        public bool IsFirstMessage { get; set; }
        public string? TableName { get; set; }
      
    }
}
