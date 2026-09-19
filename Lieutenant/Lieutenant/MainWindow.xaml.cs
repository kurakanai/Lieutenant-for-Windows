using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Lieutenant.Pages;
using Lieutenant.API;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Lieutenant
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CommonlyAccessedInstances.mainFrame = contentFrame;
            CommonlyAccessedInstances.nvView = MainNavView;
            this.ExtendsContentIntoTitleBar = true;
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (sender.IsPaneToggleButtonVisible && sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Auto || sender.PaneDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal && !sender.IsPaneToggleButtonVisible)
            {
                try
                {
                    switch (args.SelectedItemContainer.Tag.ToString())
                    {
                        case "Home":
                            contentFrame.Navigate(typeof(HomePage), null, args.RecommendedNavigationTransitionInfo);
                            break;
                        case "Download":
                            contentFrame.Navigate(typeof(DownloadBootcamp), null, args.RecommendedNavigationTransitionInfo);
                            break;
                        case "Settings":
                            contentFrame.Navigate(typeof(Settings), null, args.RecommendedNavigationTransitionInfo);
                            break;
                    }
                }
                catch { }
            }
        }

    }
}
