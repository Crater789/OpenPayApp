namespace OpenPayApi.Dto
{
    public class PaymentDto
    {
        public required string SourceId { get; set; }  // opcional
        public string Description { get; set; } = "Pago desde app";

        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public decimal Amount { get; set; }

        public required string CardNumber { get; set; }
        public required string HolderName { get; set; }
        public required string Cvv2 { get; set; }
        public required string ExpirationMonth { get; set; }
        public required string ExpirationYear { get; set; }
        public required string DeviceSessionId { get; set; }
    }


}
