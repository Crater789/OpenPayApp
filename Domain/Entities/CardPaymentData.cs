namespace Domain.Entities
{
    public class CardPaymentData
    {
        // Datos cliente
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        // Datos tarjeta (para crear token) - opcionales, no requeridos para pago
        public string? CardNumber { get; set; }
        public string? HolderName { get; set; }
        public string? Cvv2 { get; set; }
        public string? ExpirationMonth { get; set; }
        public string? ExpirationYear { get; set; }

        // Para pagos
        public decimal Amount { get; set; }

        // Token tarjeta (obligatorio para pago)
        public string SourceId { get; set; }

        public string DeviceSessionId { get; set; }

        // Opcionales
        public string Currency { get; set; } = "COP";
        public string Description { get; set; }
    }
}
