using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DeliveryToolkit.Model
{
    public class ScreenInfo : ViewModelBase
    {
        private double _screenWidth = Application.Current.MainWindow.Width;

        public double ScreenWidth
        {
            get { return _screenWidth; }
            set { Set(ref _screenWidth, value); }
        }
        public ScreenInfo()
        {
            App.Current.MainWindow.SizeChanged += MainWindow_SizeChanged;
        }
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScreenWidth = e.NewSize.Width;
        }
        public override void Cleanup()
        {
            base.Cleanup();
            App.Current.MainWindow.SizeChanged -= MainWindow_SizeChanged;
        }
    }
}
