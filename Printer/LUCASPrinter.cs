using Serilog;
using SnaptagOwnKioskInternalBackend.Models.Print;
using SnaptagOwnKioskInternalBackend.Models.Print.SnaptagOwnKioskInternalBackend.Models.Print;
using SnaptagOwnKioskInternalBackend.Utility;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace SnaptagOwnKioskInternalBackend.Printer
{
    /// <summary>
    /// 프린트를 위한 싱글톤 객체, 
    /// 주의! 버그 있음(이미지 출력을 위해 캔버스 생성 시,빈 텍스트를 만들지 않으면 이미지 출력 안 됨) 
    /// </summary>
    public class LUCASPrinter
    {
        private const double width = 53.98;
        private const double height = 85.6;
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
            catch (Exception ex)
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
            LUCASPrinterStatusModel? res = new LUCASPrinterStatusModel();
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
                if (re != 0)
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
                LUCASPrinterResultModel setRibbonOpt = SetRibbonOpt();
                if (setRibbonOpt.isSuccess == false)
                {
                    Log.Error("Failed to setRibbonOpt");
                    return setRibbonOpt;
                };
                frontPath = frontImage.SaveImageToTempDirectory(isFront: true).GetEmbeddedImage();
                rearPath = rearImage.SaveImageToTempDirectory(isFront: false).GetEmbeddedImage();
                StringBuilder frontStrBuilder = new StringBuilder();
                StringBuilder rearStrBuilder = new StringBuilder();
                LUCASPrinterResultModel frontCommited = PrepareCanvasWithImage(ref frontStrBuilder, frontPath, true);
                if (frontCommited.isSuccess == false)
                {
                    Log.Error("Failed to PrepareFrontCanvasWithImage");
                    return frontCommited;
                }
                LUCASPrinterResultModel rearCommited = PrepareCanvasWithImage(ref rearStrBuilder, rearPath, false);
                if (rearCommited.isSuccess == false)
                {
                    Log.Error("Failed to PrepareRearCanvasWithImage");
                    return rearCommited;
                }
                LUCASPrinterResultModel checkFeeder = CheckisFeederEmpty();
                if (checkFeeder.isSuccess == false)
                {
                    Log.Error("Failed to CheckisFeederEmpty");
                    return checkFeeder;
                };
                LUCASPrinterResultModel injectCard = InjectCard();
                if (injectCard.isSuccess == false)
                {
                    Log.Error("Failed to InjectCard");
                    return injectCard;
                }
                IntPtr frontStr = Marshal.StringToHGlobalAnsi(frontStrBuilder.ToString());
                IntPtr rearStr = Marshal.StringToHGlobalAnsi(rearStrBuilder.ToString());
                LUCASPrinterResultModel printResult = Print(frontStr, rearStr);
                Marshal.FreeHGlobal(frontStr);
                Marshal.FreeHGlobal(rearStr);
                if (printResult.isSuccess == false)
                {
                    Log.Error("Failed to Print");
                    return printResult;
                }
                LUCASPrinterResultModel ejectResult = EjectCard();
                if (ejectResult.isSuccess == false)
                {
                    Log.Error("Failed to EjectCard");
                    return ejectResult;
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
                result = LUCASPrinterLibrary.R600PrintDraw(front, rear);
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
        public LUCASPrinterResultModel EjectCard()
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
                result = LUCASPrinterLibrary.R600CardEject(1); // Eject existing card
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
                UInt32 result = LUCASPrinterLibrary.R600SetCanvasPortrait(1);
                if (result != 0)
                {
                    res.Error = OutputError(result);
                    return res;
                }
                res.isSuccess = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured on SetCanvasOrientationToVertical, msg = {ex.Message},stacktrace = {ex.StackTrace}");
            }
            return res;
        }
        public LUCASPrinterResultModel PrepareCanvasWithImage(ref StringBuilder str, string imagePath, bool isFront)
        {
            LUCASPrinterResultModel res = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };
            try
            {
                res = SetCanvasOrientationToPortrait();
                if (res.isSuccess == false)
                {
                    Log.Error($"Error SetCanvasOrientationToVertical, isFront = {isFront}");
                    return res;
                }

                UInt32 result = LUCASPrinterLibrary.R600PrepareCanvas(0, 0); // Colorful (YMC) mode
                if (result != 0)
                {
                    Log.Error($"Error PrepareCanvas, isFront = {isFront}");
                    res.Error = OutputError(result);
                    return res;
                }

                Log.Information($"Adding image to {(isFront ? "front" : "back")} side...");
                result = LUCASPrinterLibrary.R600DrawImage(0, 0, width, height, imagePath, 1); // Add image
                if (result != 0)
                {
                    res.Error = OutputError(result);
                    return res;
                }
                /*
                if(isFront == false)
                {
                    result = LUCASPrinterLibrary.R600SetImagePara(1, 180, 1);
                    if (result != 0)
                    {
                        res.Error = OutputError(result);
                        return res;
                    }
                }*/
                // 
                LUCASPrinterLibrary.R600SetFont("black", 7);
                LUCASPrinterLibrary.R600SetTextIsStrong(1);
                LUCASPrinterLibrary.R600DrawText(26, 5, 52, 6, " ", 1);
                str = new StringBuilder(200);
                int len = 200;
                result = LUCASPrinterLibrary.R600CommitCanvas(str, ref len);
                if (result != 0)
                {
                    Log.Information($"{(isFront ? "Front" : "Back")}Image Commited Failed, code = {result},path = {str.ToString()},len = {len.ToString()}");
                    res.Error = OutputError(result);
                    return res;
                }
                Log.Information($"{(isFront ? "Front" : "Back")}Image Commited Successfully, path = {str.ToString()},len = {len.ToString()}");
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
        public byte[] FlipImage180(byte[] imageBytes)
        {
            // byte[] -> Bitmap 변환
            using (var ms = new MemoryStream(imageBytes))
            {
                using (var bitmap = new Bitmap(ms))
                {
                    // 180도 뒤집기
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);

                    // Bitmap -> byte[] 변환
                    using (var resultStream = new MemoryStream())
                    {
                        bitmap.Save(resultStream, ImageFormat.Png); // PNG로 저장
                        return resultStream.ToArray();
                    }
                }
            }
        }
        //RE = MethodGroup.R600SetRibbonOpt(1, 0, printYmcsMode.ToString(), printYmcsMode.ToString().Length + 1);
        public LUCASPrinterResultModel SetRibbonOpt()
        {
            LUCASPrinterResultModel res = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };
            try
            {
                UInt32 result = LUCASPrinterLibrary.R600SetRibbonOpt(1, 0, "2", 2);
                if (result != 0)
                {
                    res.Error = OutputError(result);
                    return res;
                }
                res.isSuccess = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured on SetRibbonOpt, msg = {ex.Message},stacktrace = {ex.StackTrace}");
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
                    ErrorEnum = (Models.Print.Enum.LUCASSdkApiErrorCode)((int)code)
                };
                Log.Information($"LUCAS Printer paramErrorCode = {code},ErrorCode : {res.ErrorCode}, Message = {res.ErrorMessage}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured on OutputError in LUCASPRINTER, msg = {ex.Message},stacktrace = {ex.StackTrace}");

            }
            return res;
        }
        public LUCASPrinterResultModel PrintTest(byte[] frontImage, byte[] rearImage)
        {
            LUCASPrinterResultModel res = new LUCASPrinterResultModel()
            {
                isSuccess = false
            };

            try
            {
                rearImage = rearImage.FlipImageVertically();
                // Initialize the printer
                UInt32 initResult = LUCASPrinterLibrary.R600LibInit();
                if (initResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(initResult)
                    };
                }

                // Enumerate and select printer
                StringBuilder enumList = new StringBuilder(500);
                UInt32 listLen = 500;
                int num = 0;
                UInt32 enumResult = LUCASPrinterLibrary.R600EnumUsbPrt(enumList, ref listLen, ref num);
                if (enumResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(enumResult)
                    };
                }

                UInt32 selectResult = LUCASPrinterLibrary.R600SelectPrt(enumList);
                if (selectResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(selectResult)
                    };
                }
                /*
                // Set ribbon option to YMC mode
                UInt32 ribbonResult = LUCASPrinterLibrary.R600SetRibbonOpt(1, 0, "2", 2);
                if (ribbonResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(ribbonResult)
                    };
                }
                */
                // Prepare images for printing
                string frontPath = frontImage.SaveImageToTempDirectory().GetEmbeddedImage();
                string rearPath = rearImage.SaveImageToTempDirectory().GetEmbeddedImage();

                StringBuilder frontCanvasStr = new StringBuilder(200);
                StringBuilder rearCanvasStr = new StringBuilder(200);

                UInt32 frontCanvasResult = LUCASPrinterLibrary.R600PrepareCanvas(0, 0); // YMC mode
                if (frontCanvasResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(frontCanvasResult)
                    };
                }

                UInt32 frontDrawResult = LUCASPrinterLibrary.R600DrawImage(0, 0, width, height, frontPath, 0);
                if (frontDrawResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(frontDrawResult)
                    };
                }

                int frontLen = 200;
                UInt32 frontCommitResult = LUCASPrinterLibrary.R600CommitCanvas(frontCanvasStr, ref frontLen);
                if (frontCommitResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(frontCommitResult)
                    };
                }

                UInt32 rearCanvasResult = LUCASPrinterLibrary.R600PrepareCanvas(0, 0); // YMC mode
                if (rearCanvasResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(rearCanvasResult)
                    };
                }

                UInt32 rearDrawResult = LUCASPrinterLibrary.R600DrawImage(0, 0, width, height, rearPath, 0);
                if (rearDrawResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(rearDrawResult)
                    };
                }

                int rearLen = 200;
                UInt32 rearCommitResult = LUCASPrinterLibrary.R600CommitCanvas(rearCanvasStr, ref rearLen);
                if (rearCommitResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(rearCommitResult)
                    };
                }

                // Inject card
                UInt32 injectResult = LUCASPrinterLibrary.R600CardInject(0);
                if (injectResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(injectResult)
                    };
                }

                // Print
                IntPtr frontPtr = Marshal.StringToHGlobalAnsi(frontCanvasStr.ToString());
                IntPtr rearPtr = Marshal.StringToHGlobalAnsi(rearCanvasStr.ToString());
                UInt32 printResult = LUCASPrinterLibrary.R600PrintDraw(frontPtr, rearPtr);
                Marshal.FreeHGlobal(frontPtr);
                Marshal.FreeHGlobal(rearPtr);

                if (printResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(printResult)
                    };
                }

                // Eject card
                UInt32 ejectResult = LUCASPrinterLibrary.R600CardEject(0);
                if (ejectResult != 0)
                {
                    return new LUCASPrinterResultModel()
                    {
                        isSuccess = false,
                        Error = OutputError(ejectResult)
                    };
                }

                // Success
                res.isSuccess = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error in PrintTest: {ex.Message}, StackTrace: {ex.StackTrace}");
                res.isSuccess = false;
                res.Error = new LUCASPrinterErrorModel()
                {
                    ErrorMessage = ex.Message
                };
            }

            return res;
        }


    }
}
