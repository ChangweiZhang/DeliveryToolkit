using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryToolkit.Service
{
    public class ThemeManager : ViewModelBase
    {
        private static Lazy<ThemeManager> _lazy = new Lazy<ThemeManager>();
        public static ThemeManager Instance
        {
            get
            {
                return _lazy.Value;
            }
        }
        private string _brush = "#EE796F";
        public string ThemeBrush
        {
            get
            {
                return _brush;
            }
            set
            {
                Set(ref _brush, value);
            }
        }

        public async Task<string> GetThemeColorAsync()
        {
            string themeColor = null;
            try
            {
                var data = await GetConfigDataAsync();
                if (data != null)
                {
                    if (data.ContainsKey(App.THEME_KEY))
                    {
                        themeColor = data[App.THEME_KEY];
                    }
                }
            }
            catch
            {

            }
            if (themeColor != null)
            {
                this.ThemeBrush = themeColor;
            }
            return themeColor;
        }
        async Task<Dictionary<string, string>> GetConfigDataAsync()
        {
            Dictionary<string, string> data = null;
            try
            {
                var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                if (File.Exists(configFile))
                {
                    using (var sr = File.OpenText(configFile))
                    {
                        var json = await sr.ReadToEndAsync();
                        if (!string.IsNullOrEmpty(json))
                        {
                            data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        }
                    }
                }
            }
            catch
            {

            }
            return data;
        }
        public async Task<bool> SetThemeColorAsync(string themeColor)
        {
            bool res = false;
            if (!string.IsNullOrEmpty(themeColor))
            {
                ThemeBrush = themeColor;
            }
            try
            {
                var data = await GetConfigDataAsync();
                if (data == null)
                {
                    data = new Dictionary<string, string>();
                }
                data[App.THEME_KEY] = themeColor;
                var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                var json = JsonConvert.SerializeObject(data);
                if (!string.IsNullOrEmpty(json))
                {
                    using (var fs = File.Open(configFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        var byteData = UTF8Encoding.UTF8.GetBytes(json);
                        await fs.WriteAsync(byteData);
                    }
                }
            }
            catch
            {

            }
            return res;

        }
    }
}
