
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
        public PaymentService(SnaptagKioskDBContext context, HttpClient httpClient) 
        {
            this._context = context;
            this._httpClient = httpClient;
        }
        public async Task<ResponsePurchaseModel?> Purchase(RequestPurchaseModel req)
        {
            ResponsePurchaseModel? res = null;
            string reqJson = JsonConvert.SerializeObject(req);
            string responseBody = string.Empty;
            try
            {
                Log.Information($"Payment Service Started, req = {reqJson}");
                /*
#if DEBUG
                await Task.Delay(5000);
                res = new ResponsePurchaseModel()
                {
                    PurchaseIndex = -1,
                    isSuccess = true,
                    MSG = ""
                };
                return res;
#endif*/
                KiccPaymentData kiccPayRequest = new KiccPaymentData(req);
                string url = kiccPayRequest.GetFullURL;
                Log.Information($"Payment Service Started,URL = {url} req = {reqJson}");
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // 요청이 성공하지 않으면 예외 발생
                responseBody = await response.Content.ReadAsStringAsync();
#if DEBUG
                responseBody = @"jsonp12345678983543344({'SUC':'00','RQ01':'D1','RQ02':'0788888','RQ03':'A','RQ04':'532750**********','RQ05':'****','RQ06':'','RQ07':'5000','RQ08':'','RQ09':'','RQ10':'','RQ11':'','RQ12':'','RQ13':'454','RQ14':'','RQ15':'','RQ16':'40','RQ17':'KICC','RS01':'P','RS02':'J','RS03':'0001','RS04':'0000','RS05':'006','RS06':'0000','RS07':'2411180814431','RS08':'180863812215','RS09':'99081443','RS10':'N*','RS11':'006','RS12':'토스뱅크','RS13':'8103227112','RS14':'하나카드','RS15':'d','RS16':'','RS17':'               TEST용테스트 거래임','RS18':'Y','RS19':'1168119948','RS20':'','RS21':'**','RS22':'','RS23':'','RS24':'','RS25':'','RS26':'','RS27':'','RS28':'','RS29':'00000000','RS30':'','RS31':'C  ','RS32':' ','RS33':'','RS34':''})";
#endif
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
                    return res;
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
                int amt = req.PaymentAmount;
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
