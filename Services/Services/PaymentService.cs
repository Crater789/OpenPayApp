using Domain.Entities;
using Domain.Interfaces;
using Openpay;
using Openpay.Entities;
using Openpay.Entities.Request;
using System;
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

            if (string.IsNullOrWhiteSpace(payment.SourceId))
                throw new ArgumentException("SourceId (token de tarjeta) es requerido.");

            try
            {
                // Crear cliente
                var customer = new Customer
                {
                    Name = payment.Name,
                    LastName = payment.LastName,
                    PhoneNumber = payment.PhoneNumber,
                    Email = payment.Email
                };

                customer = _openpay.CustomerService.Create(customer);

                // Crear el cargo con token
                var chargeRequest = new ChargeRequest
                {
                    Method = "card",
                    SourceId = payment.SourceId,
                    Amount = (decimal)payment.Amount,
                    Description = payment.Description ?? "Pago desde app",
                    Currency = payment.Currency ?? "MXN",
                    Customer = customer,
                    DeviceSessionId = payment.DeviceSessionId,
                    UseCardPoints = "false"
                };

                var charge = _openpay.ChargeService.Create(chargeRequest);

                return charge.Id;
            }
            catch (OpenpayException ex)
            {
                throw new ApplicationException($"Error Openpay: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error inesperado: {ex.Message}", ex);
            }
        }
    }
}
