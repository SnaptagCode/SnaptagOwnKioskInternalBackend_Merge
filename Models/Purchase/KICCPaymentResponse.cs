using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Serilog;

namespace SnaptagOwnKioskInternalBackend.Models.Purchase
{
    public class KICCPaymentResponse
    {
        // 응답 성공 여부
        public string SuccessCode { get; private set; }

        // 응답 메시지 (실패 시)
        public string Message { get; private set; }

        // TID
        public string TerminalId { get; private set; }

        // 카드 번호
        public string CardNumber { get; private set; }

        // 결제 금액
        public string Amount { get; private set; }

        // 승인 날짜 및 시간 (yyMMddHHmmssf)
        public DateTime ApprovalDateTime { get; private set; }

        // 승인 번호
        public string ApprovalNumber { get; private set; }

        // 결과 코드
        public string ResultCode { get; private set; }
        public string AuthSeqNum { get; private set; }

        public string JsonResponse { get; set; }
        // 생성자
        public KICCPaymentResponse() { }

        // JSONP 응답 문자열을 받아서 객체 초기화
        public KICCPaymentResponse(string jsonpResponse) : this()
        {
            try
            {
                // JSONP 포맷에서 JSON 문자열로 변환
                string jsonString = jsonpResponse
                                    .Replace("jsonp12345678983543344(", "")
                                    .TrimEnd(')')
                                    .Replace('\'', '"');

                JsonResponse = jsonString;

                // JSON 문자열을 JObject로 파싱
                JObject jsonObject = JObject.Parse(jsonString);

                // 필드별로 값을 파싱하여 설정
                SuccessCode = jsonObject["SUC"]?.ToString();
                Message = jsonObject["MSG"]?.ToString();
                TerminalId = jsonObject["RQ02"]?.ToString(); // TID
                CardNumber = jsonObject["RQ04"]?.ToString(); // CardNumber
                Amount = jsonObject["RQ07"]?.ToString(); // Amount

                // RS07 필드는 yyMMddHHmmssf 형식의 날짜와 시간을 포함
                if (DateTime.TryParseExact(jsonObject["RS07"]?.ToString(), "yyMMddHHmmssf", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                {
                    ApprovalDateTime = parsedDate;
                }

                ApprovalNumber = jsonObject["RS09"]?.ToString(); // approvalNo
                ResultCode = jsonObject["RS04"]?.ToString(); // ResultCode
                AuthSeqNum = jsonObject["RS08"]?.ToString(); // AuthSEQNUM
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured on KiccPaymentResponse Constructor, Param = {jsonpResponse}, Details = {ex}");
            }

        }

        // 성공 여부를 확인하는 메서드
        public bool IsSuccess()
        {
            int code = 0;
            int.TryParse(SuccessCode, out code);
            bool res = SuccessCode == "00" || code == 0;
            Log.Information($"Current State = {JsonConvert.SerializeObject(this, Formatting.Indented)}, IsSuccessed = {res}");
            return res;
        }

        // 응답 메시지 가져오기
        public string GetMessage()
        {
            return Message;
        }

        // 객체를 JSON 문자열로 변환하는 메서드
        public JObject ToJson()
        {
            JObject jsonObject = new JObject
            {
                { nameof(SuccessCode), SuccessCode },
                { nameof(Message), Message },
                { nameof(TerminalId), TerminalId },
                { nameof(CardNumber), CardNumber },
                { nameof(Amount), Amount },
                { nameof(ApprovalDateTime), ApprovalDateTime.ToString("yyMMddHHmmssf") },
                { nameof(ApprovalNumber), ApprovalNumber },
                { nameof(AuthSeqNum), AuthSeqNum },
                { nameof(ResultCode), ResultCode }
            };

            return jsonObject;
        }
    }
}
