using Domain.Entities;
using Domain.Interfaces;
using Openpay;
using Openpay.Entities;
using Openpay.Entities.Request;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly OpenpayAPI _openpay;

        public PaymentService(string merchantId, string apiKey)
        {
            _openpay = new OpenpayAPI(apiKey, merchantId, country: "MX", production: false);
        }

        public async Task<string> CreateCardPaymentAsync(CardPaymentData payment)
        {
            if (payment.Amount <= 0)
                throw new ArgumentException("El monto debe ser mayor a cero.");

            try
            {
                var customer = new Customer
                {
                    Name = payment.Name,
                    LastName = payment.LastName,
                    PhoneNumber = payment.PhoneNumber,
                    Email = payment.Email
                };

                customer = _openpay.CustomerService.Create(customer);

                if (!string.IsNullOrWhiteSpace(payment.SourceId))
                {
                    var chargeRequest = new ChargeRequest
                    {
                        Method = "card",
                        SourceId = payment.SourceId,
                        Amount = payment.Amount,
                        Description = payment.Description ?? "Pago desde app",
                        Currency = payment.Currency ?? "MXN", // Ajustado a MXN
                        Customer = customer,
                        DeviceSessionId = payment.DeviceSessionId
                    };

                    var charge = _openpay.ChargeService.Create(chargeRequest);
                    return charge.Id;
                }
                else
                {
                    var card = new Card
                    {
                        CardNumber = payment.CardNumber,
                        HolderName = payment.HolderName,
                        Cvv2 = payment.Cvv2,
                        ExpirationMonth = payment.ExpirationMonth,
                        ExpirationYear = payment.ExpirationYear,
                        DeviceSessionId = payment.DeviceSessionId
                    };

                    var createdCard = _openpay.CardService.Create(customer.Id, card);

                    var chargeRequest = new ChargeRequest
                    {
                        Method = "card",
                        SourceId = createdCard.Id,
                        Amount = payment.Amount,
                        Description = payment.Description ?? "Pago desde app",
                        Currency = payment.Currency ?? "MXN",
                        Customer = customer,
                        DeviceSessionId = payment.DeviceSessionId
                    };

                    var charge = _openpay.ChargeService.Create(chargeRequest);
                    return charge.Id;
                }
            }
            catch (OpenpayException ex)
            {
                // Maneja errores específicos de Openpay, puedes loguear o transformar el error
                throw new ApplicationException($"Error Openpay: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Otros errores generales
                throw new ApplicationException($"Error inesperado: {ex.Message}", ex);
            }
        }



    }
}
