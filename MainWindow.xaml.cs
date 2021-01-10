using AduSkin.Controls.Metro;
using AduSkin.Themes;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void Theme_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties.Contains(App.THEME_KEY))
            {

                BorderBrush = new SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(
                        App.Current.Properties[App.THEME_KEY].ToString()
                        )
                    );
            }
        }

        private void Theme_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            BorderBrush = e.SelectedBrush;
            App.Current.Resources["DefaultBrush"] = e.SelectedBrush;
            App.Current.Properties[App.THEME_KEY] = e.SelectedBrush.ToColor().ToHexString(); ;
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
