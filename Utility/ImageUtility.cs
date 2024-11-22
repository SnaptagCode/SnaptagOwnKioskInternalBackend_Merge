using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SnaptagOwnKioskInternalBackend.Utility
{
    using Serilog;
    using SnaptagOwnKioskInternalBackend.Setting;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;

    public static class ImageUtility
    {

        [DllImport("Libs\\LabCode_x64.dll", EntryPoint = "getLabCodeImageFullW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern int getLabCodeImageFullW([MarshalAs(UnmanagedType.LPWStr)] string keyPath, [MarshalAs(UnmanagedType.LPWStr)] string inputPath, [MarshalAs(UnmanagedType.LPWStr)] string writePath, int size, int stength, int alphaCode, int bravoCode, int charlieCode, int deltaCode, int echoCode, ulong foxtrotCode);

        public static string LabcurityPath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Libs", "labcurity.txt");
            }
        }
        public static string GetEmbeddedImage(this byte[] image, bool isFront = true)
        {
            string res = "";
            string file = image.SaveImageToTempDirectory(isFront);
            try
            {
                ProgramSettingManager setting = ProgramSettingManager.Instance;


                string[] input_alphaValue = new string[6] //alpha 값. 추후 남오차장님께서 구체화 예정
                {
                    "26", "20", "10", "8", "6", "14"
                };
                string temp = Path.GetFileName(file);
                string tempExt = Path.GetExtension(file);
                string tempDir = Path.GetDirectoryName(file);

                string encryptPath = Path.Combine(tempDir, $"enc_{temp}");
                bool isLabcurityExists = File .Exists(LabcurityPath);
                bool isOriginalFileExists = File.Exists(file);

                int result = getLabCodeImageFullW(LabcurityPath, file, encryptPath, setting.Settings.Size, setting.Settings.Strength, setting.Settings.AlphaCode, setting.Settings.BravoCode, setting.Settings.CharlieCode, setting.Settings.DeltaCode, setting.Settings.EchoCode, setting.Settings.FoxtrotCode);
                Log.Information($"LabcurityPath = {LabcurityPath},originalFile = {file},encryptedFile = {encryptPath}, result = {result}");
                if (result == 0)
                {
                    setting.Settings.FoxtrotCode = setting.Settings.FoxtrotCode + 1;
                    setting.SaveSettings();
                    res = encryptPath;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error occured on GetLabcodeEmbeddedImageFilePath, msg = {ex.Message},stacktrace = {ex.StackTrace}");
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
        public static string GetEmbeddedImage(this string image)
        {
            string res = "";
            string file = image;
            try
            {
                ProgramSettingManager setting = ProgramSettingManager.Instance;
                string temp = Path.GetFileName(file);
                string tempExt = Path.GetExtension(file);
                string tempDir = Path.GetDirectoryName(file);
                string encryptPath = Path.Combine(tempDir, $"enc_{temp}");
                bool isLabcurityExists = File.Exists(LabcurityPath);
                bool isOriginalFileExists = File.Exists(file);

                ulong rnd = (ulong)Enumerable.Range(1, 10000).OrderBy(x => Guid.NewGuid()).First();
                int result = getLabCodeImageFullW(LabcurityPath, file, encryptPath, setting.Settings.Size, setting.Settings.Strength, setting.Settings.AlphaCode, setting.Settings.BravoCode, setting.Settings.CharlieCode, setting.Settings.DeltaCode, setting.Settings.EchoCode, rnd);
                Log.Information($"LabcurityPath = {LabcurityPath},originalFile = {file},encryptedFile = {encryptPath}, result = {result}");
                if (result == 0)
                {
                    res = encryptPath;
                }

            }
            catch (Exception ex)
            {
                Log.Error($"Error occured on GetLabcodeEmbeddedImageFilePath, msg = {ex.Message},stacktrace = {ex.StackTrace}");
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
        /// <summary>
        /// byte[]에서 이미지 확장자를 얻고, TempDirectory에 파일을 저장한 뒤 전체 경로를 반환
        /// </summary>
        /// <param name="imageData">이미지 데이터</param>
        /// <returns>생성된 파일의 전체 경로</returns>
        public static string SaveImageToTempDirectory(this byte[] imageData, bool isFront = false)
        {

            if (!isFront)
            {
                imageData = FlipImageVertically(imageData);
            }
            string extension = GetImageExtension(imageData);
            if (extension == null)
                throw new ArgumentException("유효하지 않은 이미지 데이터입니다.");

            // TempDirectory 경로 생성
            string tempDirectory = Path.GetTempPath();
            string fileName = $"img{DateTime.Now:yyyyMMdd_HHmmssfff}{extension}";
            string fullPath = Path.Combine(tempDirectory, fileName);

            // TempDirectory에 파일 저장
            File.WriteAllBytes(fullPath, imageData);

            return fullPath;
        }
        /// <summary>
        /// byte[] 이미지를 상하반전하고 새로운 byte[]로 반환
        /// </summary>
        /// <param name="imageData">이미지 데이터</param>
        /// <returns>상하반전된 이미지 데이터</returns>
        public static byte[] FlipImageVertically(this byte[] imageData)
        {
            using (MemoryStream inputStream = new MemoryStream(imageData))
            using (Image image = Image.FromStream(inputStream))
            {
                // 이미지를 상하반전
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                using (MemoryStream outputStream = new MemoryStream())
                {
                    // 이미지를 원래 포맷으로 저장
                    ImageFormat format = GetImageFormat(image);
                    image.Save(outputStream, format);

                    return outputStream.ToArray();
                }
            }
        }
        /// <summary>
        /// Image 객체에서 이미지 포맷 추출
        /// </summary>
        private static ImageFormat GetImageFormat(Image image)
        {
            if (ImageFormat.Jpeg.Equals(image.RawFormat)) return ImageFormat.Jpeg;
            if (ImageFormat.Png.Equals(image.RawFormat)) return ImageFormat.Png;
            if (ImageFormat.Bmp.Equals(image.RawFormat) || ImageFormat.MemoryBmp.Equals(image.RawFormat)) return ImageFormat.Bmp;
            if (ImageFormat.Gif.Equals(image.RawFormat)) return ImageFormat.Gif;
            if (ImageFormat.Tiff.Equals(image.RawFormat)) return ImageFormat.Tiff;
            throw new ArgumentException("알 수 없는 이미지 포맷입니다.");
        }

        /// <summary>
        /// byte[] 데이터에서 이미지 확장자를 추출
        /// </summary>
        /// <param name="imageData">이미지 데이터</param>
        /// <returns>이미지 확장자 (예: .jpg, .png 등)</returns>
        private static string GetImageExtension(byte[] imageData)
        {
            using (var ms = new MemoryStream(imageData))
            {
                try
                {
                    using (var img = Image.FromStream(ms))
                    {
                        if (ImageFormat.Jpeg.Equals(img.RawFormat)) return ".jpg";
                        if (ImageFormat.Png.Equals(img.RawFormat)) return ".png";
                        if (ImageFormat.Gif.Equals(img.RawFormat)) return ".gif";
                        if (ImageFormat.Bmp.Equals(img.RawFormat)) return ".bmp";
                        if (ImageFormat.Tiff.Equals(img.RawFormat)) return ".tiff";
                        return ".unknown";
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

    }
}
