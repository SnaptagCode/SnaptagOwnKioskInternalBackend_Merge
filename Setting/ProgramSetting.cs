namespace SnaptagOwnKioskInternalBackend.Setting
{
    public class ProgramSetting
    {
        public int AlphaCode { get; set; } = 1;
        public int BravoCode { get; set; } = 1;
        public int CharlieCode { get; set; } = 1;
        public int DeltaCode { get; set; } = 1;
        public int EchoCode { get; set; } = 1;
        public ulong FoxtrotCode { get; set; } = 1;
        public int Size { get; set; } = 3;
        public int Strength { get; set; } = 16;
        public string BackendAddress { get; set; } = @"https://devsnaptagkioskownapi.snaptag-kiosk.com/";
        public string UploadPurchaseRecordURL { get; set; } = @"Purchase/UploadPurchasRecord";
        public string UpdatePurchaseStateURL { get; set; } = @"Purchase/UpdatedPurchasRecord";
    }
}
