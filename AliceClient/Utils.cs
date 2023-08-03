using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Diagnostics;

namespace AliceClient
{
    internal static class Utils
    {
        internal static void WaitForClose()
        {
            // wait for 1 second before exiting
            Thread.Sleep(1000);
            //Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();
        }
        internal static bool IsAdministrator()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            else // UNIX system, check if user is root
            {
                return Environment.GetEnvironmentVariable("USER") == "root";
            }
        }
        internal static bool IsGitAvailable()
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "git",
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            Process? process = Process.Start(startInfo);
            if (process == null)
            {
                return false;
            }
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        internal static bool IsDockerAvailable()
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "docker",
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            Process? process = Process.Start(startInfo);
            if (process == null)
            {
                return false;
            }
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        internal static string GenerateRandomString()
        {
            Random random = new();
            string chars = "0123456789abcdef";
            char[] stringChars = new char[64];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(stringChars);
        }
    }
}
