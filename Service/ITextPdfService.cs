using DeliveryToolkit.Interface;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.IO;

namespace DeliveryToolkit.Service
{
    public class ITextPdfService : IPdfService
    {
        public bool AddWaterMarkerToPdf(string pdfFile, string destFile, string text, string fontFamily, string fontColor, int fontSize = 50)
        {
            if (File.Exists(pdfFile))
            {
                try
                {
                    using (var pr = new PdfReader(pdfFile))
                    {

                        var rgbColor = System.Drawing.ColorTranslator.FromHtml(fontColor);

                        PdfFont font = PdfFontFactory.CreateFont(fontFamily, "UniGB-UCS2-H", true);
                        var textLength = font.GetWidth(text, fontSize);
                        var pw = new PdfWriter(destFile);
                        var pdfDoc = new PdfDocument(pr, pw);
                        Rectangle ps;
                        PdfCanvas canvas;
                        var pageCount = pdfDoc.GetNumberOfPages();
                        for (var i = 1; i < pageCount + 1; i++)
                        {
                            var page = pdfDoc.GetPage(i);
                            ps = page.GetPageSize();
                            float watermarkTrimmingRectangleWidth = textLength; //Math.Min(ps.GetWidth(), ps.GetHeight());
                            float watermarkTrimmingRectangleHeight = watermarkTrimmingRectangleWidth;
                            var rotationInRads = MathF.Atan2(ps.GetHeight(), ps.GetWidth());
                            //var angle = rotationInRads * 180f / MathF.PI;
                            float formWidth = ps.GetWidth();//watermarkTrimmingRectangleWidth;
                            float formHeight = ps.GetHeight();//watermarkTrimmingRectangleWidth;
                            float formXOffset = 0;
                            float formYOffset = 0;

                            float xTranslation = (formWidth - watermarkTrimmingRectangleWidth * MathF.Cos(rotationInRads)) / 2;
                            float yTranslation = (formHeight - watermarkTrimmingRectangleWidth * MathF.Sin(rotationInRads)) / 2;

                            //Center the annotation
                            Rectangle watermarkTrimmingRectangle = new Rectangle(0, 0, formWidth, formHeight);//watermarkTrimmingRectangleWidth, watermarkTrimmingRectangleWidth);

                            PdfWatermarkAnnotation watermark = new PdfWatermarkAnnotation(watermarkTrimmingRectangle);

                            //Apply linear algebra rotation math
                            //Create identity matrix
                            AffineTransform transform = new AffineTransform();//No-args constructor creates the identity transform
                                                                              //Apply translation
                                                                              //transform.Translate(xTranslation, yTranslation);
                                                                              //Apply rotation
                            transform.Translate(xTranslation, yTranslation);
                            transform.Rotate(rotationInRads);

                            PdfFixedPrint fixedPrint = new PdfFixedPrint();
                            watermark.SetFixedPrint(fixedPrint);
                            //Create appearance
                            Rectangle formRectangle = new Rectangle(formXOffset, formYOffset, formWidth, formHeight);

                            //Observation: font XObject will be resized to fit inside the watermark rectangle
                            PdfFormXObject form = new PdfFormXObject(formRectangle);
                            PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(rgbColor.A / 255f);
                            canvas = new PdfCanvas(form, pdfDoc);

                            float[] transformValues = new float[6];
                            transform.GetMatrix(transformValues);
                            canvas.SaveState()
                                .BeginText().SetFillColorRgb(rgbColor.R / 225f, rgbColor.G / 225f, rgbColor.B / 255f).SetExtGState(gs1)
                                .SetTextMatrix(transformValues[0], transformValues[1], transformValues[2], transformValues[3], transformValues[4], transformValues[5])
                                .SetFontAndSize(font, fontSize)
                                .ShowText(text)
                                .EndText()
                                .RestoreState();

                            canvas.Release();

                            watermark.SetAppearance(PdfName.N, new PdfAnnotationAppearance(form.GetPdfObject()));
                            watermark.SetFlags(PdfAnnotation.PRINT);

                            page.AddAnnotation(watermark);
                        }
                        pdfDoc.Close();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Analytics.TrackEvent("PDFError", new Dictionary<string, string>
                    {
                        ["message"] = ex.ToString()
                    }); 
                }
                finally
                {
                    File.Delete(pdfFile);
                }
            }
            return false;
        }
    }
}
