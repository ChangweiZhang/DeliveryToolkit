using AduSkin.Controls.Metro;
using DeliveryToolkit.Service;
using Newtonsoft.Json;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DeliveryToolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Theme.SelectedBrushChanged += Theme_SelectedBrushChanged;
            Theme.Loaded += Theme_Loaded;
        }

        private async void Theme_Loaded(object sender, RoutedEventArgs e)
        {
            var brush = await ThemeManager.Instance.GetThemeColorAsync();
            //if (brush != null)
            //{
            //    BorderBrush = brush;
            //}
        }

        private async void Theme_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            BorderBrush = e.SelectedBrush;
            App.Current.Resources["DefaultBrush"] = e.SelectedBrush;
            await ThemeManager.Instance.SetThemeColorAsync(e.SelectedBrush.ToColor().ToHexString());
        }



        protected override void OnClosed(EventArgs e)
        {
            Theme.Loaded -= Theme_Loaded;
            Theme.SelectedBrushChanged -= Theme_SelectedBrushChanged;
            base.OnClosed(e);
            Application.Current.Shutdown();
        }


    }
}
