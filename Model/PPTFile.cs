using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryToolkit.Model
{
    public enum ProcessState
    {
        [Description("未开始")]
        Unstarted,
        [Description("处理中")]
        Processing,
        [Description("成功")]
        Success,
        [Description("失败")]
        Failure
    }
    public class PPTFile : ViewModelBase
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public string FullName { get; set; }
        private ProcessState _state = ProcessState.Unstarted;

        public ProcessState State
        {
            get { return _state; }
            set { Set(ref _state, value); }
        }
        public string Pdf { get; set; }
    }
}
