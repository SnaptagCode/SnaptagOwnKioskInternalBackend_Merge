using static System.Net.Mime.MediaTypeNames;

namespace SnaptagOwnKioskInternalBackend.Utility
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    public static class ImageUtility
    {
        /// <summary>
        /// byte[]에서 이미지 확장자를 얻고, TempDirectory에 파일을 저장한 뒤 전체 경로를 반환
        /// </summary>
        /// <param name="imageData">이미지 데이터</param>
        /// <returns>생성된 파일의 전체 경로</returns>
        public static string SaveImageToTempDirectory(this byte[] imageData)
        {
            string extension = GetImageExtension(imageData);
            if (extension == null)
                throw new ArgumentException("유효하지 않은 이미지 데이터입니다.");

            // TempDirectory 경로 생성
            string tempDirectory = Path.GetTempPath();
            string fileName = $"IMG_{DateTime.Now:yyyyMMdd_HHmmssfff}{extension}";
            string fullPath = Path.Combine(tempDirectory, fileName);

            // TempDirectory에 파일 저장
            File.WriteAllBytes(fullPath, imageData);

            return fullPath;
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
