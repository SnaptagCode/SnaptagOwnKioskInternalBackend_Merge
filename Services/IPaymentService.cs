using SnaptagOwnKioskInternalBackend.Models.Purchase;

namespace SnaptagOwnKioskInternalBackend.Services
{
    public interface IPaymentService
    {
        public Task<ResponsePurchaseModel?> Purchase(RequestPurchaseModel req);
    }
}
