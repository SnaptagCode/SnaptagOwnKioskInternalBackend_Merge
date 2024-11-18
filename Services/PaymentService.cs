
using Newtonsoft.Json;
using Serilog;
using SnaptagOwnKioskInternalBackend.DBContexts;
using SnaptagOwnKioskInternalBackend.DBContexts.DBModels;
using SnaptagOwnKioskInternalBackend.Models.Purchase;

namespace SnaptagOwnKioskInternalBackend.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly SnaptagKioskDBContext _context;
        private readonly HttpClient _httpClient;
        public PaymentService(SnaptagKioskDBContext context) 
        {
            this._context = context;
        }
        public async Task<ResponsePurchaseModel?> Purchase(RequestPurchaseModel req)
        {
            ResponsePurchaseModel? res = null;
            string reqJson = JsonConvert.SerializeObject(req);
            string responseBody = string.Empty;
            try
            {
                Log.Information($"Payment Service Started, req = {reqJson}");
#if DEBUG
                await Task.Delay(5000);
                res = new ResponsePurchaseModel()
                {
                    PurchaseIndex = -1,
                    isSuccess = true,
                    MSG = ""
                };
                return res;
#endif
                KiccPaymentData kiccPayRequest = new KiccPaymentData(req);
                string url = kiccPayRequest.GetFullURL;
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // 요청이 성공하지 않으면 예외 발생
                responseBody = await response.Content.ReadAsStringAsync();
                Log.Information($"Successfully got response from KICC PaymentRequest, result = {responseBody}");
                KICCPaymentResponse result = new KICCPaymentResponse(responseBody);
                if (result.IsSuccess() == false)
                {
                    res = new ResponsePurchaseModel()
                    {
                        PurchaseIndex = -1,
                        isSuccess = false,
                        MSG = result.GetMessage()
                    };
                }
                PurchaseHistoryModel? record = await SavePurchaseRecord(req, result);
                if (record == null || record.Index == null) 
                {
                    return null;
                }
                res = new ResponsePurchaseModel()
                {
                    PurchaseIndex = record.Index,
                    isSuccess = true,
                    MSG = result.GetMessage()
                };

            }
            catch (Exception ex)
            {
                res = null;
                Log.Error($"Error occured on Purchase Methodm , req = {reqJson},msg = {ex.Message},stacktrace = {ex.StackTrace}");
            }
            return res;
        }

        public async Task<PurchaseHistoryModel?> SavePurchaseRecord(RequestPurchaseModel req, KICCPaymentResponse purchaseReq)
        {
            PurchaseHistoryModel? res = null;
            try
            {
                Log.Information($"SavePurchaseRecord Started");
                int amt = req.PurchaseAmount;
                int.TryParse(purchaseReq.Amount, out amt);
                res = new PurchaseHistoryModel()
                {
                    EventIndex = req.EventIndex,
                    MachineIndex = req.MachineIndex,
                    PhotoAuthNumber = req.PhotoAuthNumber,
                    Amount = amt,
                    PurchaseDate = purchaseReq.ApprovalDateTime,
                    AuthSeqNum = purchaseReq.AuthSeqNum,
                    ApprovalNumber = purchaseReq.ApprovalNumber,
                    isPrinted = false,
                    isUploaded = false,
                    isRefunded = false,
                    Details = purchaseReq.JsonResponse
                };
                _context.PurchaseHistories.Add(res!);
                bool saveres = (await _context.SaveChangesAsync()) != 0;
                Log.Information($"isPurchaseRecord Saved = {saveres}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured on SavePurchaseRecord, msg = {ex.Message},stacktrace = {ex.StackTrace}");
            }
            return res;
        }
    }
}
