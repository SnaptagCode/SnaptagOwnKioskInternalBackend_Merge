namespace SnaptagOwnKioskInternalBackend.Models.Purchase
{
    public class KiccPaymentData
    {
        public readonly string BASIC_URL = @"http://127.0.0.1:8090/?callback=jsonp12345678983543344&REQ=";

        public int Amount { get; set; } = 1000;
        public int Timeout { get; set; } = 30;
        public string RequestPuchaseParameter
        {
            get
            {
                string paymentTypeCode = "D1"; // D1 : 결제 D4 : 취소
                return $"{paymentTypeCode}^^{Amount.ToString("D4")}^^^^^^^^^{Timeout}^{AdditionalField}^^^^^^^^^^^^^^^^^^";
            }
        }
        // Additional field with default value of 'A'
        public string AdditionalField { get; set; } = "A";

        public KiccPaymentData()
        {

        }
        public KiccPaymentData(RequestPurchaseModel req) : this()
        {
            Amount = req.PaymentAmount;
            Timeout = req.Timeout;

        }
        public string GetFullURL
        {
            get
            {
                string result = $"{BASIC_URL}{RequestPuchaseParameter}";
                return result;
            }
        }

    }
}
