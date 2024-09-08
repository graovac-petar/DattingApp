namespace AppDating.API.DTO
{
    public class PhotoDTO
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public bool IsMain { get; set; }
        public bool isApproved { get; set; }
    }
}