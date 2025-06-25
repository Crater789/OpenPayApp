namespace OpenPayApi.Dto
{
    public class PaymentDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public string Cvv2 { get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear { get; set; }
        public string HolderName { get; set; }
        public string DeviceSessionId { get; set; }
    }
}
