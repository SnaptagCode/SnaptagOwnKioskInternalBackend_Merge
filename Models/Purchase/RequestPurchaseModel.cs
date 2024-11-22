namespace SnaptagOwnKioskInternalBackend.Models.Purchase
{
    public class RequestPurchaseModel
    {
        public int EventIndex { get; set; }
        public int MachineIndex { get; set; }
        public string PhotoAuthNumber { get; set; }
        public int PaymentAmount { get; set; } = 10000;
        public int Timeout { get; set; } = 30;
    }
}
