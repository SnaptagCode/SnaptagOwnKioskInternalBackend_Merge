using Serilog;
using SnaptagOwnKioskInternalBackend.Models.Print;
using SnaptagOwnKioskInternalBackend.Models.Print.SnaptagOwnKioskInternalBackend.Models.Print;
using SnaptagOwnKioskInternalBackend.Utility;
using System.Runtime.InteropServices;
using System.Text;

namespace SnaptagOwnKioskInternalBackend.Printer
{
    public class LUCASPrinter
    {
        private static LUCASPrinter lucasPrinter;
        public static LUCASPrinter Instance()
        {
            if (lucasPrinter == null)
            {
                lucasPrinter = new LUCASPrinter();
            }
            return lucasPrinter;
        }
        public LUCASPrinterResultModel InitPrinter()
        {
            LUCASPrinterResultModel res = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };
            try
            {
                res = ConnectPrinter();
            }
            catch (Exception ex) 
            {
                Log.Error($"Error occured on InitPrinter, msg = {ex.Message},stacktrace = {ex.StackTrace}");
            }
            return res; 
        }
        public LUCASPrinterStatusModel? GetPrinerStatus()
        {
            LUCASPrinterStatusModel res = new LUCASPrinterStatusModel();
            try
            {
                res = GetStatus();
            }
            catch(Exception ex)
            {

            }
            return res;
        }

        // Helper Function: Ensure Printer is Ready
        public LUCASPrinterResultModel EnsurePrinterReady()
        {
            LUCASPrinterResultModel res = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };
            
            UInt32 result = 0;
            // Check if a card is already in the printer
            byte isCardPresent = 0;
            result = LUCASPrinterLibrary.R600IsPrtHaveCard(ref isCardPresent);
            if (result != 0)
            {
                res.Error = OutputError(result);
                return res;
            }

            if (isCardPresent != 0)
            {
                Log.Information("Card is already in the printer, ejecting...");
                result = LUCASPrinterLibrary.R600CardEject(0); // Eject existing card
                if (result != 0)
                {
                    res.Error = OutputError(result);
                    return res;
                }
                res.isSuccess = true;
            }
            res.isSuccess = true;

            return res;
        }
        public LUCASPrinterStatusModel GetStatus()
        {
            LUCASPrinterStatusModel? res = null;
            try
            {
                //judge whether machine is broken
                UInt32 mainStatus = 0;      //mainStatus 
                UInt32 errorStatus = 0;     //errorStatus 
                UInt32 warningStatus = 0;   //warningStatus 
                Int16 pChassisTemp = 0;
                Int16 pPrintheadTemp = 0;
                Int16 pHeaterTemp = 0;
                UInt32 pSubStatus = 0;
                byte pMainCode = 0;
                byte pSubCode = 0;
                uint re = LUCASPrinterLibrary.R600QueryPrtStatus(ref pChassisTemp, ref pPrintheadTemp, ref pHeaterTemp, ref mainStatus, ref pSubStatus, ref errorStatus, ref warningStatus, ref pMainCode, ref pSubCode);
                if(re != 0)
                {
                    Log.Error("Failed to getPrintStatus");
                    return res;
                }
                res = new LUCASPrinterStatusModel()
                {
                    MainCode = pMainCode,
                    SubCode = pSubCode,
                    MainStatus = (int)mainStatus,
                    ErrorStatus = (int)errorStatus,
                    WarningStatus = (int)warningStatus,
                    ChassisTemperature = (ushort)pChassisTemp,
                    PrintHeadTemperature = (ushort)pPrintheadTemp,
                    HeaterTemperature = (ushort)pHeaterTemp,
                    SubStatus = (uint)pSubStatus
                };
                Log.Information($"Print Status = {res.ToString()}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured on GetStatus, msg = {ex.Message},stacktrace = {ex.StackTrace}");
            }
            return res;
        }
        public LUCASPrinterResultModel PrintImage(byte[] frontImage, byte[] rearImage)
        {
            LUCASPrinterResultModel res = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };
            string frontPath = "";
            string rearPath = "";
            try
            {
                LUCASPrinterResultModel ready = EnsurePrinterReady();
                bool isReady = ready.isSuccess;
                if (isReady == false)
                {
                    Log.Error("Failed to EnsurePrinterReady");
                    return ready;
                }
                frontPath = frontImage.SaveImageToTempDirectory();
                rearPath = rearImage.SaveImageToTempDirectory();
                LUCASPrinterResultModel frontCommited = PrepareCanvasWithImage(frontPath, true);
                if (frontCommited.isSuccess == false)
                {
                    Log.Error("Failed to PrepareFrontCanvasWithImage");
                    return frontCommited;
                }
                LUCASPrinterResultModel rearCommited = PrepareCanvasWithImage(rearPath, false);
                if (rearCommited.isSuccess == false)
                {
                    Log.Error("Failed to PrepareRearCanvasWithImage");
                    return rearCommited;
                }
                LUCASPrinterResultModel checkFeeder = CheckisFeederEmpty();
                if(checkFeeder.isSuccess == false)
                {
                    Log.Error("Failed to CheckisFeederEmpty");
                    return checkFeeder;
                };
                LUCASPrinterResultModel injectCard = InjectCard();
                if(injectCard.isSuccess == false)
                {
                    Log.Error("Failed to InjectCard");
                    return injectCard;
                }
                IntPtr frontStr = Marshal.StringToHGlobalAnsi(frontPath.ToString());
                IntPtr rearStr = Marshal.StringToHGlobalAnsi(rearPath.ToString());
                LUCASPrinterResultModel printResult = Print(frontStr, rearStr);
                Marshal.FreeHGlobal(frontStr);
                Marshal.FreeHGlobal(rearStr);
                if(printResult.isSuccess == false)
                {
                    Log.Error("Failed to Print");
                    return printResult;
                }
                res = new LUCASPrinterResultModel()
                {
                    isSuccess = true,
                };
            }
            catch (Exception ex)
            {

            }

            return res;
        }
        public LUCASPrinterResultModel Print(IntPtr front, IntPtr rear)
        {
            LUCASPrinterResultModel res = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };
            try
            {
                UInt32 result = 1;
                result = LUCASPrinterLibrary.R600PrintDraw(front,rear);
                if (result != 0)
                {
                    res.Error = OutputError(result);
                    return res;
                }
                res.isSuccess = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured on Print, msg = {ex.Message}, stacktrace = {ex.StackTrace}");
            }
            return res;
        }
        public LUCASPrinterResultModel CheckisFeederEmpty()
        {
            LUCASPrinterResultModel res = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };
            try
            {
                UInt32 result = 1;
                int feederIsNoEmpty = 0;
                result = LUCASPrinterLibrary.R600IsFeederNoEmpty(ref feederIsNoEmpty);
                if (result != 0)
                {
                    res.Error = OutputError(result);
                    return res;
                }
                res.isSuccess = feederIsNoEmpty != 0;
            }
            catch (Exception ex) 
            {
                Log.Error($"Error occured on CheckisFeederEmpty, msg = {ex.Message}, stacktrace = {ex.StackTrace}");
            }
            return res;
        }
        public LUCASPrinterResultModel InjectCard()
        {
            LUCASPrinterResultModel res = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };
            try
            {
                UInt32 result = 1;
                result = LUCASPrinterLibrary.R600CardInject(0);
                if (result != 0)
                {
                    res.Error = OutputError(result);
                    return res;
                }
                res.isSuccess = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured on InjectCard, msg = {ex.Message}, stacktrace = {ex.StackTrace}");
            }
            return res;
        }
        public LUCASPrinterResultModel SetCanvasOrientationToPortrait()
        {
            LUCASPrinterResultModel res = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };
            try
            {
                UInt32 result = LUCASPrinterLibrary.R600SetCanvasPortrait(0);
                if (result != 0)
                {
                    res.Error = OutputError(result);
                    return res;
                }
                res.isSuccess = true;
            }
            catch(Exception ex)
            {
                Log.Error($"Error occured on SetCanvasOrientationToVertical, msg = {ex.Message},stacktrace = {ex.StackTrace}");
            }
            return res;
        }
        public LUCASPrinterResultModel PrepareCanvasWithImage(string imagePath, bool isFront)
        {
            LUCASPrinterResultModel res = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };
            try
            {
                UInt32 result = LUCASPrinterLibrary.R600PrepareCanvas(0, 0); // Colorful (YMC) mode
                if (result != 0)
                {
                    Log.Error($"Error PrepareCanvas, isFront = {isFront}");
                    res.Error = OutputError(result);
                    return res;
                }
                res = SetCanvasOrientationToPortrait();
                if(res.isSuccess == false)
                {
                    Log.Error($"Error SetCanvasOrientationToVertical, isFront = {isFront}");
                    return res;
                }
                Log.Information($"Adding image to {(isFront ? "front" : "back")} side...");
                result = LUCASPrinterLibrary.R600DrawImage(0, 0, 85.6, 53.98, imagePath, 1); // Add image
                if (result != 0)
                {
                    res.Error = OutputError(result);
                    return res;
                }
                StringBuilder path = new StringBuilder(200);
                int len = 200;
                result = LUCASPrinterLibrary.R600CommitCanvas(path, ref len);
                if(result != 0)
                {
                    Log.Information($"{(isFront ? "Front" : "Back")}Image Commited Failed, code = {result},path = {path.ToString()},len = {len.ToString()}");
                    res.Error = OutputError(result);
                    return res;
                }
                Log.Information($"{(isFront ? "Front" : "Back")}Image Commited Successfully, path = {path.ToString()},len = {len.ToString()}");
                res = new LUCASPrinterResultModel()
                {
                    isSuccess = true
                };
            }
            catch (Exception ex) 
            {
                Log.Error($"Error occured on PrepareCanvasWithImage. msg= {ex.Message},stacktrace = {ex.StackTrace}");
            }
            
            return res;
        }

        public LUCASPrinterResultModel ConnectPrinter()
        {
            LUCASPrinterResultModel result = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };
            UInt32 RE = 0;
            StringBuilder enumList = new StringBuilder(500);
            UInt32 listLen = 500;
            int num = 10;
            //enumerate printer

            RE = LUCASPrinterLibrary.R600LibInit();
            if (RE != 0)
            {
                result.Error = OutputError(RE);
                return result;
            }
            RE = LUCASPrinterLibrary.R600EnumUsbPrt(enumList, ref listLen, ref num);
            if (RE != 0)
            {
                result.Error = OutputError(RE);
                return result;
            }
            //Set the USB timeout
            RE = LUCASPrinterLibrary.R600UsbSetTimeout(3000, 3000);
            if (RE != 0)
            {
                result.Error = OutputError(RE);
                return result;
            }
            //choose pritner
            RE = LUCASPrinterLibrary.R600SelectPrt(enumList);
            if (RE != 0)
            {
                result.Error = OutputError(RE);
                return result;
            }
            result.isSuccess = true;
            return result;
        }
        public LUCASPrinterErrorModel OutputError(UInt32 code)
        {
            LUCASPrinterErrorModel res = new LUCASPrinterErrorModel();
            try
            {
                StringBuilder str = new StringBuilder(500);
                int len = 500;
                LUCASPrinterLibrary.R600GetErrorOuterInfo(code, str, ref len);
                res = new LUCASPrinterErrorModel()
                {
                    ErrorCode = (int)code,
                    ErrorMessage = str.ToString(),
                };
                Log.Information($"LUCAS Printer paramErrorCode = {code},ErrorCode : {res.ErrorCode}, Message = {res.ErrorMessage}");
            }
            catch (Exception ex) 
            {
                Log.Error($"Error occured on OutputError in LUCASPRINTER, msg = {ex.Message},stacktrace = {ex.StackTrace}");

            }
            return res;
        }

    }
}
