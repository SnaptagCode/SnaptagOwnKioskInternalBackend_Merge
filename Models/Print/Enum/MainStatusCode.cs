namespace SnaptagOwnKioskInternalBackend.Models.Print.Enum
{
    public enum MainStatusCode
    {
        PrinterInfo = 1001,
        Initialize = 1002,
        PreHeat = 1003,
        Ready = 1004,
        Printing = 1005,
        CardFeed = 1006,
        PrintingYellow = 1007,
        PrintingMagenta = 1008,
        PrintingCyan = 1009,
        PrintingBlack = 1010,
        PrintingBlack2 = 1011,
        PrintingHeatSeal = 1012,
        PrintingI = 1013,
        Transferring = 1014,
        Reverse = 1015,
        DecurlFront = 1016,
        DecurlBack = 1017,
        CardEjection = 1018,
        Clean = 1019,
        CleanEnd = 1020,
        FirmwareUpdating = 1021,
        CardDataRW = 1022,
        OptionSetting = 1023,
        TestPrinting = 1024
    }

}
