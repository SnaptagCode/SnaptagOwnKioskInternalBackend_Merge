using SnaptagOwnKioskInternalBackend.Models.Print;

namespace SnaptagOwnKioskInternalBackend.Services
{
    public interface IPrinterService
    {
        public Task<ResponsePrintResultModel?> PrintImage(RequestPrintModel req);
        public Task<object> GetPrinterStatus();
    }
}
