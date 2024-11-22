using System.Diagnostics;

namespace SnaptagOwnKioskInternalBackend.Utility
{
    public class LabcodeWrapper
    {
        public static string LabCodeX64Wrapper
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Wrapper", "LabCodeWrapperUtility.exe");
            }
        }
        public static int RunLabCodeConsole(string inputPath, string writePath)
        {
            try
            {
                string exePath = LabCodeX64Wrapper; // 32비트 콘솔 애플리케이션의 경로
                string args = $"{inputPath} {writePath}";

                // 프로세스 시작 정보 설정
                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        return int.Parse(output);
                    }
                    else
                    {
                        string error = process.StandardError.ReadToEnd();
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
