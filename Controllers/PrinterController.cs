using Microsoft.AspNetCore.Mvc;
using Serilog;
using SnaptagOwnKioskInternalBackend.Models.Print;
using SnaptagOwnKioskInternalBackend.Services;

namespace SnaptagOwnKioskInternalBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrinterController : Controller
    {
        private readonly PrinterService _service;
        public PrinterController(PrinterService service)
        {
            this._service = service;
        }
        [HttpPost("PrintImage")]
        public async Task<IActionResult> PrintImage([FromBody]RequestPrintModel req)
        {
            ResponsePrintResultModel? res;
            try
            {
                res = await _service.PrintImage(req);
                return Ok(res);
            }
            catch (Exception ex) 
            {
                Log.Error($"Error occured on PrintImage Controller, msg = {ex.Message}, stacktrace = {ex.StackTrace}");
            }
            return BadRequest();
        }
    }
}
