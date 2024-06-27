namespace APBDprojekt.ResponseModels;

public class PostContractPaymentResponseModel
{
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
    public int ContractPaymentId { get; set; }
    public decimal AlreadyPaid { get; set; }
    public decimal Price { get; set; }
    public decimal LastPayment { get; set; }
}