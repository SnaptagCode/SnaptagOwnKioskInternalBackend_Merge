namespace SnaptagOwnKioskInternalBackend.Models.Print
{
    using global::SnaptagOwnKioskInternalBackend.Models.Print.Enum;
    using System;

    namespace SnaptagOwnKioskInternalBackend.Models.Print
    {
        public class LUCASPrinterStatusModel
        {
            public int MainStatus { get; set; }
            public int ErrorStatus { get; set; }
            public int WarningStatus { get; set; }
            public uint SubStatus { get; set; }
            public ushort ChassisTemperature { get; set; }
            public ushort PrintHeadTemperature { get; set; }
            public ushort HeaterTemperature { get; set; }
            public byte MainCode { get; set; }
            public byte SubCode { get; set; }

            // Enum Properties
            public MainStatusCode ParsedMainStatus => (MainStatusCode)MainStatus;
            public ErrorStatusCode ParsedErrorStatus => (ErrorStatusCode)ErrorStatus;
            public WarningStatusCode ParsedWarningStatus => (WarningStatusCode)WarningStatus;

            public LUCASPrinterStatusModel()
            {
                
            }
            // Constructor
            public LUCASPrinterStatusModel(
                int mainStatus,
                int errorStatus,
                int warningStatus,
                uint subStatus,
                ushort chassisTemperature = 0,
                ushort printHeadTemperature = 0,
                ushort heaterTemperature = 0,
                byte mainCode = 0,
                byte subCode = 0)
            {
                MainStatus = mainStatus;
                ErrorStatus = errorStatus;
                WarningStatus = warningStatus;
                SubStatus = subStatus;
                ChassisTemperature = chassisTemperature;
                PrintHeadTemperature = printHeadTemperature;
                HeaterTemperature = heaterTemperature;
                MainCode = mainCode;
                SubCode = subCode;
            }

            // ToString override for debugging
            public override string ToString()
            {
                return $"MainStatus: {ParsedMainStatus}, ErrorStatus: {ParsedErrorStatus}, WarningStatus: {ParsedWarningStatus}, " +
                       $"SubStatus: {SubStatus}, ChassisTemp: {ChassisTemperature}, PrintHeadTemp: {PrintHeadTemperature}, " +
                       $"HeaterTemp: {HeaterTemperature}, MainCode: {MainCode}, SubCode: {SubCode}";
            }
        }

    }

}
