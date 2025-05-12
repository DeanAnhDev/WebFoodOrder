
using FoodOrder.Application.Common.Models;

namespace FoodOrder.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDto email);
    }
}
