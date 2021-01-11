using DeliveryToolkit.View;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DeliveryToolkit.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private UserControl _pptTool = new PPTMarkupTool();

        public UserControl PPTTool
        {
            get { return _pptTool; }
            set
            {
                Set(ref _pptTool, value);
            }
        }

        private UserControl _aboutKit = new AboutToolkit();

        public UserControl AboutKit
        {
            get { return _aboutKit; }
            set
            {
                Set(ref _aboutKit, value);
            }
        }


    }
}
