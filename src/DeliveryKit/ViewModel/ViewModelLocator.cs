using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using DeliveryToolkit.Interface;
using DeliveryToolkit.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace DeliveryToolkit.ViewModel
{
    public class ViewModelLocator : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<PPTToolViewModel>();
            SimpleIoc.Default.Register<IPdfService, ITextPdfService>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public PPTToolViewModel PPTTool
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PPTToolViewModel>();
            }
        }
        public override void Cleanup()
        {
            base.Cleanup();
        }

    }
}
