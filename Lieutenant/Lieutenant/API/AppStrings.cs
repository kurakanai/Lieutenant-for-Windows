using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieutenant.API
{
    public static class AppStrings
    {
        private static readonly ResourceLoader resourceLoader = new();
        // Developer info
        public static readonly string devGithubUrl = "https://github.com/kurakanai";
        public static readonly string devPaypalUrl = "https://www.paypal.com/paypalme/dinisp25";
        public static readonly string reportUrl = "https://github.com/kurakanai/Lieutenant-for-Windows/issues/new";
        // Update Strings
        public static string updateUpdate = resourceLoader.GetString("Update2");
        public static string updateUpToDate = resourceLoader.GetString("UpToDate");
        public static string updateUpdating = resourceLoader.GetString("Updating");
        public static string UpdateVersion(string version)
        {
            return (resourceLoader.GetString("UpdateString") + " " + version);
        }
    }
}
