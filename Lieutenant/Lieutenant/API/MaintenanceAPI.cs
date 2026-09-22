using Lieutenant.API;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lieutenant.API
{
    class MaintenanceAPI
    {
        public async static void PerformUpdate(bool downloadUpdate)
        {
            string updatePath = "release.zip";
            string chosenPath = Globals.UpdateZip;
            using var s = await InternetAPI.client.GetStreamAsync(chosenPath);
            using var fs = new FileStream("release.zip", FileMode.CreateNew);
            await s.CopyToAsync(fs);
            if (updatePath != "")
            {
                CommandAPI.PerformCMDCommand($"taskkill /f /im Lieutenant.exe & mkdir Update & MOVE * Update/ & cd Update & move \"{updatePath}\" ../release.zip & cd .. & tar -xf release.zip & del /f /q release.zip & rmdir /s /q Update & Lieutenant.exe");
            }
        }
        public static async Task CheckUpdate()
        {
            using HttpClient client = new();
            string reply = await client.GetStringAsync(Globals.UpdateString);
            Globals.IsUpdateAvailable = !(reply == Globals.version);
        }
    }
}
