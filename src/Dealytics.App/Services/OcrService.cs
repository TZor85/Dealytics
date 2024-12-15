using Tesseract;

namespace Dealytics.App.Services;

public class OcrService
{
    public string ExtractTextFromRegion(Bitmap originalImage, int posX, int posY, int width, int height, int threshold)
    {
        try
        {
            // Crear un rectángulo con las coordenadas especificadas
            Rectangle cropArea = new Rectangle(posX, posY, width, height);

            // Recortar la región especificada
            using (Bitmap croppedImage = originalImage.Clone(cropArea, originalImage.PixelFormat))
            {
                // Aplicar umbral a la imagen recortada
                using (Bitmap thresholdImage = ApplyThreshold(croppedImage, threshold))
                {
                    // Guardar temporalmente la imagen procesada
                    string tempFile = Path.GetTempFileName() + ".png";
                    thresholdImage.Save(tempFile, System.Drawing.Imaging.ImageFormat.Png);

                    // Realizar OCR en la imagen procesada
                    using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                    {
                        using (var img = Pix.LoadFromFile(tempFile))
                        {
                            using (var page = engine.Process(img))
                            {
                                // Limpiar archivo temporal
                                File.Delete(tempFile);
                                return page.GetText().Trim();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    private Bitmap ApplyThreshold(Bitmap original, int threshold)
    {
        Bitmap result = new Bitmap(original.Width, original.Height);

        for (int i = 0; i < original.Width; i++)
        {
            for (int j = 0; j < original.Height; j++)
            {
                Color c = original.GetPixel(i, j);
                int gray = (int)((c.R * 0.3) + (c.G * 0.59) + (c.B * 0.11));
                Color newColor = gray > threshold ? Color.White : Color.Black;
                result.SetPixel(i, j, newColor);
            }
        }

        return result;
    }
}
