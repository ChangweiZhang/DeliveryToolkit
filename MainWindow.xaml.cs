using AduSkin.Controls.Metro;
using AduSkin.Themes;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
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

        }

        private void Theme_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            BorderBrush = e.SelectedBrush;
            App.Current.Resources["DefaultBrush"] = e.SelectedBrush;
        }



        protected override void OnClosed(EventArgs e)
        {
            Theme.SelectedBrushChanged -= Theme_SelectedBrushChanged;
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
