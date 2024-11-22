using Microsoft.AspNetCore.Mvc;
using Serilog;
using SnaptagOwnKioskInternalBackend.Models.Print;
using SnaptagOwnKioskInternalBackend.Printer;
using SnaptagOwnKioskInternalBackend.Services;
using SnaptagOwnKioskInternalBackend.Utility;

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
        [HttpPost("PrintDirectly")]
        public async Task<IActionResult> PrintDirectly([FromBody] DirectPrintModel req)
        {
            try
            {
                var t = LUCASPrinter.Instance();
                var res = t.PrintImage(req.FrontImage, req.RearImage);
                return Ok(res);
            }
            catch (Exception ex) 
            {

            }
            return BadRequest();
        }

        [HttpPost("EmbedTest")]
        public IActionResult EmbedTest([FromBody] byte[] img)
        {
            var res = img.GetEmbeddedImage();
            return Ok(res);
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
        [HttpPost("EmbedImage")]
        public IActionResult EmbedImage([FromBody] byte[] image)
        {
            try
            {
                string res = image.GetEmbeddedImage(isFront:false);
                return Ok(res);
            }catch(Exception ex)
            {
                Log.Error($"Error occured on EmbedImage Controller, msg =  {ex.Message},stacktrace = {ex.StackTrace}");
                return BadRequest(ex);
            }
        }
    }
}
