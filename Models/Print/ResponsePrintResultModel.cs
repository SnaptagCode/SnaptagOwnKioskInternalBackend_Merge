namespace SnaptagOwnKioskInternalBackend.Models.Print
{
    public class ResponsePrintResultModel
    {
        public bool isSuccess { get; set; } = false;
        public int Code { get; set; }
        public string? MSG { get; set; }
    }
}
