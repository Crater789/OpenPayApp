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
using Google.Apis.Auth;  
using Openpay;
using Openpay.Entities;
using Openpay.Entities.Request;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly OpenpayAPI _openpay;
        private readonly string _googleClientId;

        public PaymentService(string merchantId, string apiKey, string googleClientId)
        {
            _openpay = new OpenpayAPI(apiKey, merchantId, country: "MX", production: false);
            _googleClientId = googleClientId;
        }

        // Método para validar el token Google
        public async Task<GoogleJsonWebSignature.Payload?> ValidateGoogleTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _googleClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return payload;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateCardPaymentAsync(CardPaymentData payment, string idToken)
        {
            // Validamos token Google antes de proceder
            var payload = await ValidateGoogleTokenAsync(idToken);
            if (payload == null)
                throw new UnauthorizedAccessException("Token de Google inválido o expirado.");

            // Opcional: verificar que el email del token coincida con el del pago
            if (!string.Equals(payload.Email, payment.Email, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("El email del token no coincide con el email del pago.");

            if (payment.Amount <= 0)
                throw new ArgumentException("El monto debe ser mayor a cero.");

            if (string.IsNullOrWhiteSpace(payment.SourceId))
                throw new ArgumentException("SourceId (token de tarjeta) es requerido.");

            try
            {
                return await Task.Run(() =>
                {
                    var customer = new Customer
                    {
                        Name = payment.Name,
                        LastName = payment.LastName,
                        PhoneNumber = payment.PhoneNumber,
                        Email = payment.Email
                    };

                    var createdCustomer = _openpay.CustomerService.Create(customer);

                    var chargeRequest = new ChargeRequest
                    {
                        Method = "card",
                        SourceId = payment.SourceId,
                        Amount = (decimal)payment.Amount,
                        Description = payment.Description ?? "Pago desde app",
                        Currency = payment.Currency ?? "MXN",
                        Customer = createdCustomer,
                        DeviceSessionId = payment.DeviceSessionId,
                        UseCardPoints = "false"
                    };

                    var charge = _openpay.ChargeService.Create(chargeRequest);

                    return charge.Id;
                });
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
