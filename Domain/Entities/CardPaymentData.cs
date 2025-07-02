namespace Domain.Entities
{
  public class CardPaymentData
{
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }

    public required string SourceId { get; set; } // Token generado en Flutter
    public double Amount { get; set; }
    public required string Description { get; set; }
    public string Currency { get; set; } = "MXN";
    public required string DeviceSessionId { get; set; } // opcional, para antifraude
}

}
