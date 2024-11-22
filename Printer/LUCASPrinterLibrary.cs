using System.Runtime.InteropServices;
using System.Text;

namespace SnaptagOwnKioskInternalBackend.Printer
{
    public static class LUCASPrinterLibrary
    {
        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600LibInit();

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600LibClear();
        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600SetCanvasPortrait(int nPortrait);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600GetErrorOuterInfo(UInt32 errcode, StringBuilder outputstr, ref int len);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600EnumUsbPrt(StringBuilder szEnumList, ref uint pEnumListLen, ref int pNum);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600SelectPrt(StringBuilder szPrt);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600GetRibbonRemaining(ref int pRemaining);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600PrepareCanvas(int nChromaticMode, int nMonoChroMode);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600DrawImage(double dX, double dY, double dWidth, double dHeight, String szImgFilePath, int nSetNoAbsoluteBlack);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600SetFont(String szFontName, float fSize);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600DrawText(double dX, double dY, double width, double height, String szText, int nSetNoAbsoluteBlack);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600SetTextIsStrong(int nStrong);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600DrawBarCode(double dX, double dY, double dWidth, double dHeight, String szData, int nSetNoAbsoluteBlack);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600CommitCanvas(StringBuilder szImgInfo, ref int pImgInfoLen);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600IsFeederNoEmpty(ref int pFlag);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600CardEject(byte ucDestPos);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600CardInject(byte ucDestPos);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600CardMove(byte ucDestPos);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600GetCardPos(ref int pPos);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600UsbSetTimeout(int nReadTimeout, int nWriteTimeout);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600IsPrtHaveCard(ref byte pFlag);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600PrintDraw(IntPtr szImgInfoFront, IntPtr szImgInfoBack);

        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600QueryPrtStatus(ref Int16 pChassisTemp, ref Int16 pPrintheadTemp, ref Int16 pHeaterTemp, ref UInt32 pMainStatus,
            ref UInt32 pSubStatus, ref UInt32 pErrorStatus, ref UInt32 pWarningStatus, ref byte pMainCode, ref byte pSubCode);



        //unsigned int DSSDK R600SetWaterMarkParam(int nRotation, unsigned char isNeedMirror);
        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600SetWaterMarkParam(int nRotation, Int16 isNeedMirror);
        //unsigned int DSSDK R600SetWaterMarkParam(int nRotation, unsigned char isNeedMirror);
        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600SetImagePara(int whiteTransparency,int nRotation, float fScale);

        // unsigned int DSSDK R600SetWaterMarkThreshold(int Threshold);
        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600SetWaterMarkThreshold(int Threshold);

        //unsigned int DSSDK R600DrawWaterMark(double dX, double dY, double width, double height, const char *szImgFilePath);
        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600DrawWaterMark(double dX, double dY, double width, double height, String szImgFilePath);


        //unsigned int DSSDK R600SetRibbonOpt(unsigned char isWrite, unsigned int key, char* value, unsigned int valueLen)
        [DllImport("Libs\\libDSRetransfer600App.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 R600SetRibbonOpt(byte isWrite, UInt32 key, string value, int valueLen);
    }
}
