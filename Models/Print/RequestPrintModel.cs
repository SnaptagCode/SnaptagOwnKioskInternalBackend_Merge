namespace SnaptagOwnKioskInternalBackend.Models.Print
{
    public class RequestPrintModel
    {
        public int EventIndex { get; set; }
        public int MachineIndex { get; set; }
        public int PurchaseIndex { get; set; }
        public string PhotoAuthNumber { get; set; }
        public byte[] RearImage { get; set; }
    }
}
