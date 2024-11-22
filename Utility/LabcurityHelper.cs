using Serilog;
using System.Runtime.InteropServices;

namespace SnaptagOwnKioskInternalBackend.Utility
{
    public class LabcurityHelper
    {

        [DllImport("Libs\\LabCode_x64.dll", EntryPoint = "getLabCodeImageFullW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int getLabCodeImageFullW(
        [MarshalAs(UnmanagedType.LPWStr)] string keyPath,
        [MarshalAs(UnmanagedType.LPWStr)] string inputPath,
        [MarshalAs(UnmanagedType.LPWStr)] string writePath,
        int size,
        int strength,
        ulong foxtrotCode,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] inputAlpha);

        static string[] input_alphaValue = new string[] //alpha 값. 추후 남오차장님께서 구체화 예정
            {
                "26", "20", "10", "8", "6", "14"
            };
        public static string LabcurityPath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Libs", "labcurity.txt");
            }
        }
        public string GetLabcodeEmbeddedImageFilePath(byte[] image)
        {
            string res = "";
            string file = image.SaveImageToTempDirectory();
            try
            {
                string temp = Path.GetFileName(file);
                string tempExt = Path.GetExtension(file);
                string tempDir = Path.GetDirectoryName(file);

                string encryptPath = Path.Combine(tempDir, $"enc_{temp}.{tempExt}");

                int result = getLabCodeImageFullW(LabcurityPath, file, encryptPath, 3, 16, 247UL, input_alphaValue);
                Log.Information($"LabcurityPath = {LabcurityPath},originalFile = {file},encryptedFile = {encryptPath}, result = {result}");
                if(result != 0)
                {
                    res = encryptPath;
                }

            }
            catch (Exception ex)
            {
                Log.Information($"Error occured on GetLabcodeEmbeddedImageFilePath, msg = {ex.Message},stacktrace = {ex.StackTrace}");
            }
            finally
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }

            return res;
        }
    }
}
