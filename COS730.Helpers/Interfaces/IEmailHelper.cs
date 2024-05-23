namespace COS730.Helpers.Interfaces
{
    public interface IEmailHelper
    {
        void SendEmail(string email, string subject, string message);
    }
}
