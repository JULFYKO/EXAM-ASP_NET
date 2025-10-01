using System.Threading.Tasks;

namespace EXAM_ASP_NET.Interfaces
{
    public interface IAppEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }
}
