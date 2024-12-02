﻿namespace SnaptagOwnKioskInternalBackend.Models.Print.Enum
{
    public enum LUCASSdkApiErrorCode
    {
        NullReceivedPoint = 8388608, // Null received point. Null data address
        InvalidReceivedData = 8388609, // Invalid received data. (Received data fails to satisfy the calling conditions for the function)
        FunctionNotImplemented = 8388610, // The function is not implemented
        NullThreadStructure = 8388613, // Null thread structure for combination printing
        Timeout = 8388614, // Timeout
        ReceiveBufferOverflow = 8388615, // Receive buffer overflow
        CharacterConversionFailed = 8388616, // Character conversion failed
        LibraryNotInitialized = 8388617, // The dynamic library is not initialized, please initialize first
        FailedToObtainTlsVariables = 8388618, // Failed to obtain TLS variables
        NoPrinterNameFound = 8404992, // No printer name found
        MainStatusReportingFailure = 8421376, // Main status reporting failure
        PrinterBusy = 8421377, // Printer busy
        CardCaseEmpty = 8421378, // Card case empty
        CardInPrinter = 8421379, // Card in the printer
        CardInReader = 8421380, // Card in the card reader
        NoCardInPrinter = 8421381, // No card in the printer
        CardJammed = 8421382, // Card jammed
        NoCardInReader = 8421383, // No card in the card reader
        ExitSetupMode = 8421385, // Please exit setup mode
        CannotGetMainStatusCode = 8421386, // Cannot get main status code from R600StatusReference
        CannotGetWarningCode = 8421387, // Cannot get warning code from R600StatusReference
        CannotGetErrorCode = 8421388, // Cannot get error code from R600StatusReference
        CannotGetStatusDescription = 8421389, // Cannot get status description code from R600StatusReference
        UnknownCardPosition = 8421390, // Position of the card in the printer not known
        InvalidOperationCall = 8421391, // Invalid calling for actual operation
        InvalidCardTransferCall = 8421392, // Calling the card transfer operation is not practical, please pay attention to the parameter values
        MissingThreeInOneReader = 8421393, // The printer does not have a three-in-one card reader
        MissingContactCardReader = 8421394, // The printer does not have a contact card reader
        MissingContactlessCardReader = 8421395, // The printer does not have a contactless card reader
        MissingMagneticStripeReader = 8421396, // The printer does not have a magnetic stripe reader
        TimeoutTransferPosition = 8421397, // Waiting for transfer position timeout
        TimeoutContactPosition = 8421398, // Waiting for contact position timeout
        TimeoutContactlessPosition = 8421399, // Waiting for contactless position timeout
        TimeoutThreeInOneSlot = 8421400, // Waiting for three-in-one card reading slot timeout
        TimeoutFlipPosition = 8421401, // Waiting for flip position timeout
        TimeoutMagneticStripeBrush = 8421402, // Waiting for magnetic stripe pre brush bit timeout
        TimeoutMagneticCardSlot = 8421403, // Waiting for the magnetic stripe to brush the card slot timeout
        InvalidResolution = 8437760, // Invalid resolution
        InvalidRibbonType = 8437762, // Invalid ribbon type
        InvalidHalfGridOffset = 8437763, // The offset of the half-grid ribbon cannot exceed 43.0106 mm
        ErrorOpeningXmlFile = 8454144, // Error opening XML configuration file
        XmlRootElementNotFound = 8454145, // Unable to find XML root element
        XmlElementNotFound = 8454146, // Unable to find XML element
        XmlNodeNameMismatch = 8454147, // XML node names do not match
        EwlCertificationFailed = 8454148, // EWL certification analysis failed
        UnsupportedCardReaderManufacturer = 8454149, // Card reader manufacturer not supported
        IncorrectPrinterStatusDecoding = 8454150, // Incorrect printer status decoding table
        InvalidThreeInOneReaderOption = 8454151, // 3-in-1 card reader is not configured in the card reader option
        ForbiddenThreeInOneSetup = 8454152, // Not allowed to setup 3-in-1 card reader option
        WhitelistPasswordWrong = 8454153, // Password for checking whitelist is wrong
        ErrorOpeningEncryptedFile = 8470528, // Failed to open encrypted file
        NullFilePath = 8470529, // Null file path
        RepeatedProgramOpen = 8486912, // Due to repeated opening of the relevant printing program, it cannot be used!
        MagneticCardTrackVerificationFailed = 8503296, // Magnetic stripe card failed to read and verify the written track information
        MagneticCardRetryReadFailed = 8503297, // Magnetic stripe card failed to retry reading track information
        MagneticCardRetryWriteFailed = 8503298, // Magnetic stripe card failed to retry writing track information
        MagneticCardTrackVerificationError = 8503299, // Failed to verify track data after successfully writing track information
        InvalidFunctionResolution = 16777216, // Invalid function caused by incorrect resolution
        OutOfRangeParameters = 16777217, // Out of range parameters
        InvalidDirectDrawNull = 16777218, // DSDirectDrawIsNull is null
        YmcColorResolutionMismatch = 16793600, // Width of the image's YMC-color does not match the YMK resolution
        KColorResolutionMismatch = 16793601, // Width of the image's K-color does not match the K resolution
        MissingDuplexOption = 16809984, // Duplex option is not installed
        InvalidHalfGridOffsetV2 = 16809985, // The offset of the half-grid ribbon should not be greater than 44.7 mm
        RibbonExtensionNull = 16809986, // The ribbon extension setting structure returns null
        InvalidResolutionType = 16809987, // No corresponding resolution type
        MissingFilmOptSetting = 16809988, // No set for FilmOpt
        ParameterBufferTooSmall = 25165824, // The parameter buffer is too small
        UsbHandlerOpenFailed = 25182208, // Open the USB port handler failed
        UsbHandlerCloseFailed = 25182209, // Close the USB port handler failed
        UsbDataSendFailed = 25182210, // Fail to send data from USB
        UsbAsyncWriteFailed = 25182211, // USB asynchronous write pending failed
        UsbReadFailed = 25182213, // USB read failed
        WsaStartupError = 25198592, // WSAStartup network communication initialization error
        SerialPortOpenFailed = 25214976, // Opening serial port handler failed
        ParallelPortOpenFailed = 25231360, // Opening parallel port handler failed
        DeviceInfoNull = 25247744, // Null device information handler returned
        UsbPrinterNotSelected = 83886080, // USB printer is not the current selection
        BarcodeCreationFailed = 92274688, // Barcode creation failed
        BarcodeEncodingFailed = 92274689, // Barcode encoding failed
        GdiNotInitialized = 92307460, // GDI has not initialized drawing
        PointerNull = 125829120, // Pointer is null
        InvalidParameter = 125829121, // Invalid parameter
        CacheTooSmall = 125829123, // Receive cache too small
        UnknownResult = 125861897, // Unknown result
        CardSwipingFailed = 125861898, // Card swiping failed
        InvalidTrackData = 125861899 // Invalid track data. Please check the data
    }
}
