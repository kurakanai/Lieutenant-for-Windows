using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Lieutenant.API
{
    public static class MacSMBIOSAPI
    {
        public static string GetModelIdentifier()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS");
                string model = key?.GetValue("SystemProductName")?.ToString();

                return !string.IsNullOrWhiteSpace(model) ? model : "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
