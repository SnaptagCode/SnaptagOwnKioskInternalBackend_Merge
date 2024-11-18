using Microsoft.AspNetCore.Mvc;
using Serilog;
using SnaptagOwnKioskInternalBackend.Models.Purchase;
using SnaptagOwnKioskInternalBackend.Services;

namespace SnaptagOwnKioskInternalBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        private readonly PaymentService _service;
        public PaymentController(PaymentService service)
        {
            this._service = service;
        }

        [HttpPost("Purchase")]
        public async Task<IActionResult> Purchase([FromBody] RequestPurchaseModel req)
        {
            try
            {
                ResponsePurchaseModel? res = await _service.Purchase(req);
                return Ok(res);
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured on Purchase Controller, msg = {ex.Message},stacktrace = {ex.StackTrace} ");
            }
            return BadRequest();
        }

    }
}
