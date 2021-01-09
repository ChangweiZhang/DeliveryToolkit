namespace DeliveryToolkit.Interface
{
    public interface IPdfService
    {
        bool AddWaterMarkerToPdf(string pdfFile, string destFile,string text, string fontFamily, string fontColor, int fontSize);
    }
}
