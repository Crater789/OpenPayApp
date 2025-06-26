/*
MIT License

Copyright (c) 2025 Diego Ramirez

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


using Domain.Entities;
using Domain.Interfaces;
using Openpay;
using Openpay.Entities;
using Openpay.Entities.Request;

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
