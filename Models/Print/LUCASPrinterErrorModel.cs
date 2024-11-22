using SnaptagOwnKioskInternalBackend.Models.Print.Enum;

namespace SnaptagOwnKioskInternalBackend.Models.Print
{
    public class LUCASPrinterErrorModel
    {
        public int ErrorCode { get; set; }
        public LUCASSdkApiErrorCode ErrorEnum { get; set; } = LUCASSdkApiErrorCode.CannotGetErrorCode;
        public string ErrorMessage { get; set; }
    }
}
