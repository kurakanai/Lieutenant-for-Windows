using ICSharpCode.SharpZipLib.Zip;
using Lieutenant.API;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using static Lieutenant.API.InternetAPI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Lieutenant.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DownloadProgress : Page
{
    public DownloadProgress()
    {
        InitializeComponent();
    }
    private string downloadURL;
    private string destinationPath = "Bootcamp/bootcamp_package.pkg";
    protected override void OnNavigatedTo(NavigationEventArgs args)
    {
        downloadURL = (string)args.Parameter;
        if(downloadURL != null ){
            PerformDownload();
        }
    }
    private async void PerformDownload()
    {
        if (string.IsNullOrEmpty(downloadURL)) return;

        DownloadProgressBar.Visibility = Visibility.Visible;
        Directory.CreateDirectory("Bootcamp");
        var progress = new Progress<DownloadProgressReport>(report =>
        {
            DownloadProgressBar.Value = report.Percentage;
            if (report.TotalBytes.HasValue)
            {
                double downloadedMB = report.BytesDownloaded / 1024.0 / 1024.0;
                double totalMB = report.TotalBytes.Value / 1024.0 / 1024.0;
                DownloadProgressText.Text = $"Downloading: {downloadedMB:F1} MB / {totalMB:F1} MB ({report.Percentage:F0}%)";
            }
        });

        try
        {
            await InternetAPI.DownloadAsync(downloadURL, destinationPath, progress);
        }
        catch (Exception ex)
        {
            DownloadProgressText.Text = $"Error: {ex.Message}";
        }
        finally
        {
            DownloadingElements.Visibility = Visibility.Collapsed;
            DownloadedBar.Visibility = Visibility.Visible;
        }
    }

    private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
    {
        CommandAPI.PerformCMDCommand("cd Bootcamp & start .");
    }
}