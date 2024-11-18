namespace SnaptagOwnKioskInternalBackend.Models.Purchase
{
    public class ResponsePurchaseModel
    {
        public int? PurchaseIndex { get; set; }
        public bool isSuccess { get; set; } = false;
        public string? MSG { get; set; }
    }
}
