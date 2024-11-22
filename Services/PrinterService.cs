using Serilog;
using SnaptagOwnKioskInternalBackend.DBContexts;
using SnaptagOwnKioskInternalBackend.Models.Print;

namespace SnaptagOwnKioskInternalBackend.Services
{
    public class PrinterService : IPrinterService
    {
        private readonly SnaptagKioskDBContext _context;
        public PrinterService(SnaptagKioskDBContext context) 
        { 
            this._context = context;
        }

        public async Task<object> GetPrinterStatus()
        {
            return new object { };
        }

        public async Task<ResponsePrintResultModel> PrintImage(RequestPrintModel req)
        {
            ResponsePrintResultModel? res = null;
            try
            {
                await Task.Delay(120000);
                return new ResponsePrintResultModel()
                {
                    isSuccess = true,
                };
            }
            catch (Exception ex) 
            {
                Log.Error($"Error occured on PrintImage, msg = {ex.Message}, stacktrace = {ex.StackTrace}");
            }
            return res;
        }
    }
}
