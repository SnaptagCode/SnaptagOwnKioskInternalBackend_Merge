namespace SnaptagOwnKioskInternalBackend.Models.Print
{
    public class LUCASPrinterResultModel
    {
        public bool isSuccess { get; set; } = false;
        public LUCASPrinterErrorModel? Error { get; set; }
    }
}
