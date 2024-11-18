namespace SnaptagOwnKioskInternalBackend.Models.Print.Enum
{
    public enum ErrorStatusCode
    {
        NoError = 3001,
        CoverOpen = 3002,
        RibbonEnd = 3003,
        InvalidRibbon = 3004,
        RibbonJam = 3005,
        RibbonMatchError = 3006,
        FilmEnd = 3007,
        InvalidFilm = 3008,
        FilmJam = 3009,
        FeederCardJam = 3010,
        FlipCardJam = 3011,
        TransferCardJam = 3012,
        CallSupport = 3013,
        HeadFailure = 3014,
        HeatRollFailure = 3015,
        DecurlFailure = 3016,
        FpgaError = 3017,
        RfidAuthError = 3018,
        SubCpuError = 3019,
        InvalidData = 3020,
        RibbonCodeInvalid = 3021
    }

}
