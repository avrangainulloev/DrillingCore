namespace DrillingCore.Application.DTOs
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public bool IsAuthenticated { get; set; }

    }
}
