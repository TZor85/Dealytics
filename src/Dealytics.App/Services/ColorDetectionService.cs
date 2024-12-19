using SkiaSharp;

namespace PokerVisionAI.App.Services;

public class ColorDetectionService
{
    public SKColor GetPixelColor(Image image, int x, int y)
    {
        try
        {
            // Convertir System.Drawing.Image a Stream
            using var ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;

            // Cargar imagen en SKBitmap
            using var bitmap = SKBitmap.Decode(ms);

            // Obtener el color del pixel en la posición x,y
            SKColor pixelColor = bitmap.GetPixel(x, y);
            return pixelColor;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener el color: {ex.Message}");
        }
    }

}
