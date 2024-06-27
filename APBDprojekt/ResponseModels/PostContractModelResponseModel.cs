namespace APBDprojekt.ResponseModels;

public class PostContractModelResponseModel
{
    public Decimal PriceWithDiscounts { get; set; }
    
    public DateTime PaymentStartTime { get; set; }
    
    public DateTime PaymentEndTime { get; set; }
    
    public DateTime EndOfSupport { get; set; }
    
    public string CurrentSoftwareVersion { get; set; }
    
    public int ClientId { get; set; }
    
    public int SoftwareId { get; set; }
    
}