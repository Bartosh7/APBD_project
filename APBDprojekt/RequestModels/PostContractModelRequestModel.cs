namespace APBDprojekt.RequestModels;

public class PostContractModelRequestModel
{
    public DateTime PaymentStartTime { get; set; }

    public DateTime PaymentEndTime { get; set; }

    public int YearsOfVersionSupport { get; set; }
}