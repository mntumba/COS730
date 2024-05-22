namespace COS730.Models.Responses
{
    public class AuthResponse
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool IsVerified { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
