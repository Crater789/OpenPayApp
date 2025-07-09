// Domain/Interfaces/IPaymentService.cs
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreateCardPaymentAsync(CardPaymentData payment, string idToken);
    }
}
