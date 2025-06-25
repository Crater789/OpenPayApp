using Openpay;
using Openpay.Entities;
using Openpay.Entities.Request;
using Application.DTOs;
using Domain.Interfaces;

public class OpenpayPaymentService : IPaymentService
{
    private readonly OpenpayAPI _openpay;

    public OpenpayPaymentService(IConfiguration config)
    {
        _openpay = new OpenpayAPI(
            config["Openpay:ApiKey"],
            config["Openpay:MerchantId"],
            country: "CO"
        );
    }

    public async Task<string> CreateCardPaymentAsync(PaymentDto payment)
    {
        var card = new Card
        {
            CardNumber = payment.CardNumber,
            Cvv2 = payment.Cvv2,
            ExpirationMonth = payment.ExpirationMonth,
            ExpirationYear = payment.ExpirationYear,
            HolderName = payment.HolderName
        };

        var newCard = _openpay.CardService.Create(card);

        var request = new ChargeRequest
        {
            Method = "card",
            SourceId = newCard.Id,
            Description = "Payment with Openpay",
            Amount = payment.Amount,
            Currency = "COP",
            Customer = new Customer
            {
                Name = payment.Name,
                LastName = payment.LastName,
                PhoneNumber = payment.PhoneNumber,
                Email = payment.Email
            },
            DeviceSessionId = newCard.Id
        };

        var charge = _openpay.ChargeService.Create(request);
        return charge.Id;
    }
}
