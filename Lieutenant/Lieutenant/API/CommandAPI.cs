using System.Diagnostics;

namespace Lieutenant.API
{
    static class CommandAPI
    {
        private static ProcessStartInfo GenerateProcessStartInfo(string executable)
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = executable,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            return processStartInfo;
        }
        private static readonly Process cmd = new()
        {
            StartInfo = GenerateProcessStartInfo("cmd.exe"),
        };
        public static void PerformCMDCommand(string command)
        {
            cmd.StartInfo.Arguments = $"/C {command}";
            cmd.Start();
            cmd.WaitForExit();
        }
        public static string GetCMDLog(string command)
        {
            PerformCMDCommand(command);
            return cmd.StandardOutput.ReadToEnd();
        }
    }
}
