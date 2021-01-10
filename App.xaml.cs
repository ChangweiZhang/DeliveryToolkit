using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;

namespace DeliveryToolkit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string THEME_KEY = "theme_key";
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            AppCenter.Start("13d39608-01a8-411b-a28c-269337d7e654",
                   typeof(Analytics), typeof(Crashes));
            var countryCode = RegionInfo.CurrentRegion.TwoLetterISORegionName;
            AppCenter.SetCountryCode(countryCode);
            Analytics.SetEnabledAsync(true);
            Crashes.SetEnabledAsync(true);
            Analytics.TrackEvent("AppStart", new Dictionary<string, string>
            {
                ["message"] = "App start"
            });

        }
    }
}
