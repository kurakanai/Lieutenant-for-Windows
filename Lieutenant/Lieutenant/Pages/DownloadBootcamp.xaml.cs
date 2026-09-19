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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static Lieutenant.API.BootcampAPI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Lieutenant.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadBootcamp : Page
    {
        private BootCampCatalog _catalog;
        private string _resolvedUrl;
        public DownloadBootcamp()
        {
            InitializeComponent();
            LoadCatalogAsync();
        }
        private async Task LoadCatalogAsync()
        {
            try
            {
                string path = Path.Combine(AppContext.BaseDirectory, "API", "BootcampLinks.json");

                if (!File.Exists(path))
                {
                    System.Diagnostics.Debug.WriteLine($"Catalog file missing at: {path}");
                    return;
                }

                string json = await File.ReadAllTextAsync(path);

                var options = new JsonSerializerOptions();
                options.Converters.Add(new BootCampCatalogConverter());

                _catalog = JsonSerializer.Deserialize<BootCampCatalog>(json, options);

                if (_catalog?.Families != null)
                {
                    CboFamily.ItemsSource = _catalog.Families;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to deserialize catalog: {ex.Message}");
            }
        }

        private void CboFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CboSubfamily.ItemsSource = null;
            CboSubfamily.IsEnabled = false;

            if (CboFamily.SelectedItem is ProductFamily selectedFamily)
            {
                CboSubfamily.ItemsSource = selectedFamily.Subfamilies;
                CboSubfamily.IsEnabled = true;
            }
            ResetSelectionState();
        }

        private void CboSubfamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CboModel.ItemsSource = null;
            CboModel.IsEnabled = false;

            if (CboSubfamily.SelectedItem is Subfamily selectedSubfamily)
            {
                CboModel.ItemsSource = selectedSubfamily.Models;
                CboModel.IsEnabled = true;
            }
            ResetSelectionState();
        }

        private void CboModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CboYear.ItemsSource = null;
            CboYear.IsEnabled = false;

            if (CboModel.SelectedItem is Model selectedModel)
            {
                CboYear.ItemsSource = selectedModel.Years;
                CboYear.IsEnabled = true;
            }
            ResetSelectionState();
        }
        private void UpdateModel(string modelID)
        {
            TxtDetectedModel.Text = $"Target Model Identifier: {modelID}";
            if (_catalog.ModelLinkIndex.TryGetValue(modelID, out int linkIdx) &&
                    linkIdx >= 0 && linkIdx < _catalog.DownloadLinks.Count)
            {
                _resolvedUrl = _catalog.DownloadLinks[linkIdx];
                TxtDownloadUrl.Text = $"Download URL: {_resolvedUrl}";
                BtnDownload.IsEnabled = true;
            }
            else
            {
                TxtDownloadUrl.Text = "Download URL: Package link unavailable.";
            }
        }
        private void CboYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetSelectionState();

            if (CboYear.SelectedItem is ModelYear selectedYear && selectedYear.ModelIds.Count > 0)
            {
                // Grab the primary model identifier (e.g. "MacBookAir9,1")
                string primaryModelId = selectedYear.ModelIds[0];

                // Resolve URL index from catalog lookup dictionary[cite: 2]
                UpdateModel(primaryModelId);
            }
        }

        private void ResetSelectionState()
        {
            TxtDetectedModel.Text = "Target Model Identifier: None";
            TxtDownloadUrl.Text = "Download URL: Select a model year";
            BtnDownload.IsEnabled = false;
            _resolvedUrl = null;
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_resolvedUrl))
            {
                CommonlyAccessedInstances.mainFrame.Navigate(typeof(DownloadProgress), _resolvedUrl);
            }
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UpdateModel(MacSMBIOSAPI.GetModelIdentifier());
        }
    }
}
