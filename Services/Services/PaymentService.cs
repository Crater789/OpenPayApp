using Domain.Entities;
using Domain.Interfaces;
using Openpay;
using Openpay.Entities;
using Openpay.Entities.Request;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly OpenpayAPI _openpay;

        public PaymentService(string merchantId, string apiKey)
        {
            _openpay = new OpenpayAPI(apiKey, merchantId, country: "CO", false);
        }

        public async Task<string> CreateCardPaymentAsync(CardPaymentData payment)
        {
            // 1. Crear cliente
            var customer = new Customer
            {
                Name = payment.Name,
                LastName = payment.LastName,
                PhoneNumber = payment.PhoneNumber,
                Email = payment.Email
            };
            customer = _openpay.CustomerService.Create(customer);

            // 2. Crear tarjeta asociada al cliente
            var card = new Card
            {
                CardNumber = payment.CardNumber,
                HolderName = payment.HolderName,
                Cvv2 = payment.Cvv2,
                ExpirationMonth = payment.ExpirationMonth,
                ExpirationYear = payment.ExpirationYear,
                DeviceSessionId = payment.DeviceSessionId // pon aquí el DeviceSessionId real que tengas
            };
            card = _openpay.CardService.Create(customer.Id, card);

            // 3. Crear cargo con tarjeta y cliente
            var chargeRequest = new ChargeRequest
            {
                Method = "card",
                SourceId = card.Id,
                Amount = payment.Amount,
                Description = "Pago desde la app",
                Currency = "COP",
                Customer = customer,
                DeviceSessionId = card.Id
            };
            Charge charge = _openpay.ChargeService.Create(chargeRequest);

            return charge.Id;
        }

    }
}
