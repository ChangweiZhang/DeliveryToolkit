﻿using CommonServiceLocator;
using DeliveryToolkit.Interface;
using DeliveryToolkit.Model;
using GalaSoft.MvvmLight.Command;
using Microsoft.AppCenter.Analytics;
using Microsoft.Office.Core;
using Microsoft.VisualBasic;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing.Text;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using iText.Layout.Font;
using FontInfo = DeliveryToolkit.Model.FontInfo;
using iText.IO.Font;
using iText.IO.Font.Constants;

namespace DeliveryToolkit.ViewModel
{
    public class PPTToolViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        #region 水印
        public RelayCommand SelectFontCommand => new RelayCommand(SelectFont);
        void SelectFont()
        {
            var dialog = new FontDialog()
            {
                FontMustExist = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CurrentFont = new FontInfo
                {
                    FamilyName = dialog.Font.SystemFontName
                };
            }
        }
        static List<FontInfo> GetSystemFonts()
        {
            List<FontInfo> fonts = new List<FontInfo>();

            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                FontFamily[] fontFamilies = fontsCollection.Families;

                foreach (FontFamily font in fontFamilies)
                {

                    var tf = new Font(font.Name, 20, FontStyle.Regular, GraphicsUnit.World);
                    fonts.Add(new FontInfo
                    {
                        FamilyName = font.Name,
                        Name = font.Name
                    });

                }
            }
            return fonts;
        }

        private string _fontColor = "#ff4d4d";

        public string FontColor
        {
            get { return _fontColor; }
            set { Set(ref _fontColor, value); }
        }
        private FontInfo _currentFont;

        public FontInfo CurrentFont
        {
            get { return _currentFont; }
            set { Set(ref _currentFont, value ?? FontList.First()); }
        }
        private int _fontSize = 50;

        public int FontSize
        {
            get { return _fontSize; }
            set { Set(ref _fontSize, value); }
        }
        private List<FontInfo> _fontList = new List<FontInfo> {
            new FontInfo{ Name="中文字体",FamilyName="STSong-Light"
                //Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Font","Segoe UI Bold.ttf")
            },
            new FontInfo{ Name=StandardFonts.TIMES_BOLD,FamilyName=StandardFonts.TIMES_BOLD},
            new FontInfo{ Name=StandardFonts.COURIER_BOLD,FamilyName=StandardFonts.COURIER_BOLD},
            new FontInfo{ Name=StandardFonts.HELVETICA_BOLD,FamilyName=StandardFonts.HELVETICA_BOLD},
            new FontInfo{ Name=StandardFonts.SYMBOL,FamilyName=StandardFonts.SYMBOL},
            new FontInfo{ Name=StandardFonts.ZAPFDINGBATS,FamilyName=StandardFonts.ZAPFDINGBATS},
        };
        //private List<FontInfo> _fontList = GetSystemFonts();
        public List<FontInfo> FontList
        {
            get { return _fontList; }
            set { Set(ref _fontList, value); }
        }

        #endregion
        private string _pptFolder;

        public string PPTFolder
        {
            get { return _pptFolder; }
            set { Set(ref _pptFolder, value); }
        }

        private List<PPTFile> _pptFiles;

        public List<PPTFile> PPTFiles
        {
            get { return _pptFiles; }
            set
            {
                Set(ref _pptFiles, value);
                IsProcessEnable = _pptFiles?.Count > 0 && IsNoProcessRunning && !string.IsNullOrEmpty(WaterMarker);
            }
        }
        private string _waterMarker;

        public string WaterMarker
        {
            get { return _waterMarker; }
            set
            {
                Set(ref _waterMarker, value);
                if (string.IsNullOrWhiteSpace(value))
                {
                    CheckMessage = "水印内容不能为空";
                }
                else
                {
                    CheckMessage = null;
                }
                IsProcessEnable = _pptFiles?.Count > 0 && IsNoProcessRunning && !string.IsNullOrEmpty(WaterMarker);
            }
        }

        private double _progressVal;

        public double ProgressVal
        {
            get { return _progressVal; }
            set { Set(ref _progressVal, value); }
        }

        private bool _isProcessEnable;

        public bool IsProcessEnable
        {
            get { return _isProcessEnable; }
            set { Set(ref _isProcessEnable, value); }
        }

        private bool _isNoProcessRunning = true;

        public bool IsNoProcessRunning
        {
            get { return _isNoProcessRunning; }
            set
            {
                Set(ref _isNoProcessRunning, value);
                IsProcessEnable = _pptFiles?.Count > 0 && IsNoProcessRunning && !string.IsNullOrEmpty(WaterMarker);
            }
        }

        private string _checkMessage;

        public string CheckMessage
        {
            get { return _checkMessage; }
            set { Set(ref _checkMessage, value); }
        }



        public PPTToolViewModel()
        {
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public RelayCommand SelectPPTFolderCommand => new RelayCommand(SelectPPTFolder);

        private void SelectPPTFolder()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                PPTFolder = dialog.SelectedPath;
                var files = Directory.GetFiles(PPTFolder).Where(f => f.ToLower().EndsWith(".ppt")
                || f.ToLower().EndsWith(".pptx")
                || f.ToLower().EndsWith(".pdf")).ToArray();
                var pptFileList = new List<PPTFile>();
                for (var i = 0; i < files.Length; i++)
                {
                    var fileInfo = new FileInfo(files[i]);
                    pptFileList.Add(new PPTFile
                    {
                        FullName = Path.GetFileName(fileInfo.FullName),
                        Name = Path.GetFileNameWithoutExtension(fileInfo.Name),
                        Path = fileInfo.FullName
                    });
                }
                PPTFiles = pptFileList;
            }
        }

        public RelayCommand RunPPTProcessCommand => new RelayCommand(RunPPTProcess);

        private async void RunPPTProcess()
        {
            foreach (var file in PPTFiles)
            {
                file.State = ProcessState.Unstarted;
            }
            IsNoProcessRunning = false;
            IsProcessEnable = false;
            ProgressVal = 0;
            int okCount = 0, errorCount = 0;
            try
            {
                for (var i = 0; i < PPTFiles.Count; i++)
                {
                    var pptFile = PPTFiles[i];
                    pptFile.State = ProcessState.Processing;
                    var result = false;
                    var format = Path.GetExtension(pptFile.Path).ToLower();
                    var pdfFile = string.Empty;
                    var fileName = Path.GetFileNameWithoutExtension(pptFile.Path);
                    var tempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
                    if (!Directory.Exists(tempFolder))
                    {
                        Directory.CreateDirectory(tempFolder);
                    }
                    var outputFile = Path.Combine(tempFolder, $"{fileName}.pdf");
                    if (format == ".pdf")
                    {
                        File.Copy(pptFile.Path, outputFile);
                        pdfFile = outputFile;
                    }
                    else
                    {
                        pdfFile = await ConvertPPTToPDFAsync(pptFile.Path, outputFile);
                    }
                    if (!string.IsNullOrEmpty(pdfFile))
                    {
                        pptFile.Pdf = pdfFile;
                        var dp = ServiceLocator.Current.GetInstance<IPdfService>();
                        var outputFolder = Path.Combine(Path.GetDirectoryName(pptFile.Path), "output");
                        if (!Directory.Exists(outputFolder))
                        {
                            Directory.CreateDirectory(outputFolder);
                        }
                        var destFile = Path.Combine(outputFolder, $"{ pptFile.Name}.pdf");
                        if (dp?.AddWaterMarkerToPdf(pptFile.Pdf, destFile, WaterMarker, CurrentFont.FamilyName, FontColor, FontSize) == true)
                        {
                            result = true;
                            pptFile.Pdf = destFile;
                        }
                    }
                    pptFile.State = result ? ProcessState.Success : ProcessState.Failure;
                    if (result)
                    {
                        okCount++;
                    }
                    else
                    {
                        errorCount++;
                    }
                    await App.Current.Dispatcher.InvokeAsync(() =>
                     {
                         ProgressVal = ((i + 1d) / PPTFiles.Count) * 100;
                     });
                }
            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    MessageBoxX.Show($"添加水印失败，请检查文件后再试\nerror:{ex.ToString()}", "错误");
                });
                return;
            }
            finally
            {
                IsNoProcessRunning = true;
                IsProcessEnable = true;
            }
            Notice.Show($"文件处理结果：\n成功 {okCount} 失败 {errorCount}\n 输出文件夹位于PPT文件目录", "任务结束", Panuon.UI.Silver.MessageBoxIcon.Success);
        }

        private async Task<string> ConvertPPTToPDFAsync(string inputFile, string outputFile)
        {
            try
            {

                await Task.Delay(500);
                await Task.Factory.StartNew(() =>
                {
                    var app = new Microsoft.Office.Interop.PowerPoint.Application();
                    var retryCount = 3;
                    while (retryCount <= 3 && retryCount > 0)
                    {
                        try
                        {
                            var presentations = app.Presentations;
                            var presentation = presentations.Open(inputFile, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
                            presentation.ExportAsFixedFormat(outputFile, Microsoft.Office.Interop.PowerPoint.PpFixedFormatType.ppFixedFormatTypePDF, Microsoft.Office.Interop.PowerPoint.PpFixedFormatIntent.ppFixedFormatIntentScreen);
                            retryCount++;

                            app.Quit();
                        }
                        catch (Exception error)
                        {
                            retryCount--;
                            //App.Current.Dispatcher.Invoke(() =>
                            // {
                            //     MessageBoxX.Show("Office转换文件失败，请检查Office是否正确安装", "错误");
                            // });

                            Analytics.TrackEvent("OfficeError", new Dictionary<string, string>
                            {
                                ["message"] = error.ToString()
                            });
                        }

                    }

                });
                if (File.Exists(outputFile))
                {
                    return outputFile;
                }
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("PDFError", new Dictionary<string, string>
                {
                    ["message"] = ex.ToString()
                });
            }

            return string.Empty;
        }
        //private bool AddWaterMarkerToPdf(string pdfFile)
        //{
        //    try
        //    {
        //        var font = new XFont("Arial", 20, XFontStyle.Regular);
        //        var pdfDoc = PdfReader.Open(pdfFile, PdfDocumentOpenMode.Modify);
        //        foreach (var page in pdfDoc.Pages)
        //        {
        //            using (XGraphics gfx = XGraphics.FromPdfPage(page))
        //            {

        //                // 获取文本的大小（以点为单位）
        //                XSize size = gfx.MeasureString(WaterMarker, font);
        //                // 在页面中心定义旋转转换
        //                //gfx.TranslateTransform(page.Width / 2, page.Height / 2);
        //                //gfx.RotateTransform(-Math.Atan(page.Height / page.Width) * 180 / Math.PI);
        //                //gfx.TranslateTransform(-page.Width / 2, -page.Height / 2);

        //                // 创建字符串格式
        //                XStringFormat format = new XStringFormat();
        //                format.Alignment = XStringAlignment.Near;
        //                format.LineAlignment = XLineAlignment.Near;

        //                // 创建暗红色画笔
        //                XBrush brush = new XSolidBrush(XColor.FromArgb(128, 255, 0, 0));

        //                // 画弦
        //                gfx.DrawString(WaterMarker, font, brush,
        //                  new XPoint(0, 0),//(page.Width - size.Width) / 2, (page.Height - size.Height) / 2),
        //                  format);
        //                // gfx.Save();

        //            }
        //        }
        //        pdfDoc.Save(pdfFile);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return false;
        //}
    }
}
