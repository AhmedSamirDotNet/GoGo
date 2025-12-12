namespace Snap.APIs.DTOs
{
    public class PendingDto
    {
        public int id { get; set; }
        public string driverFullname { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public string userId { get; set; } // Added userId property
    }
}
