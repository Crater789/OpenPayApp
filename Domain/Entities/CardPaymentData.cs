namespace Domain.Entities
{
  public class CardPaymentData
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public string SourceId { get; set; } // Token generado en Flutter
    public double Amount { get; set; }
    public string Description { get; set; }
    public string Currency { get; set; } = "MXN";
    public string DeviceSessionId { get; set; } // opcional, para antifraude
}

}
