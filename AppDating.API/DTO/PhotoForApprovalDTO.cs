namespace AppDating.API.DTO
{
    public class PhotoForApprovalDTO
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public string? Username { get; set; } // optional as this matches the
        public bool IsApproved { get; set; }
    }
}
