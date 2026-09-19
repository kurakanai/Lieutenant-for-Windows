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
                // Primary check: Win32_ComputerSystem Model property
                using var searcher = new ManagementObjectSearcher("SELECT Model FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string model = obj["Model"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(model) && !model.Equals("System Product Name", StringComparison.OrdinalIgnoreCase))
                    {
                        return model.Trim();
                    }
                }
            }
            catch { /* Fallback to secondary WMI query */ }

            try
            {
                // Fallback check: Win32_ComputerSystemProduct Name property
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_ComputerSystemProduct");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string name = obj["Name"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(name) && !name.Equals("System Product Name", StringComparison.OrdinalIgnoreCase))
                    {
                        return name.Trim();
                    }
                }
            }
            catch { }

            return "Unable to detect Mac.";
        }
    }
}
