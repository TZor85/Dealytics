using SkiaSharp;

namespace PokerVisionAI.App.Services;

public class ImageCropperService
{
    public string CropImageToBase64(Image sourceImage, int x, int y, int width, int height)
    {
        try
        {
            // Convertir System.Drawing.Image a Stream
            using var ms = new MemoryStream();
            sourceImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;

            // Cargar imagen con SkiaSharp
            using var originalBitmap = SKBitmap.Decode(ms);

            // Crear un nuevo bitmap para la región recortada
            using var croppedBitmap = new SKBitmap(width, height);

            // Crear un canvas para dibujar
            using var canvas = new SKCanvas(croppedBitmap);

            // Definir el área a recortar
            var sourceRect = new SKRectI(x, y, x + width, y + height);
            var destRect = new SKRectI(0, 0, width, height);

            // Dibujar la región recortada
            canvas.DrawBitmap(originalBitmap, sourceRect, destRect);

            // Convertir a base64
            using var image = SKImage.FromBitmap(croppedBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var croppedMs = new MemoryStream();
            data.SaveTo(croppedMs);

            return $"{Convert.ToBase64String(croppedMs.ToArray())}";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al recortar la imagen: {ex.Message}");
        }
    }

    public Image Base64ToImage(string base64String)
    {
        try
        {
            // Remover el encabezado de data URL si existe
            if (base64String.Contains(","))
            {
                base64String = base64String.Split(',')[1];
            }

            // Convertir base64 a byte array
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // Crear un MemoryStream con los bytes
            using (var ms = new MemoryStream(imageBytes))
            {
                // Crear la imagen desde el MemoryStream
                return Image.FromStream(ms);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al convertir base64 a imagen: {ex.Message}");
        }
    }

    public void GuardarEnArchivo(string campo1, string campo2)
    {
        try
        {
            // Ruta del archivo en la raíz del proyecto
            string rutaArchivo = Path.Combine(Application.StartupPath, "datos.txt");

            // Crear la línea a escribir
            string linea = $"\n {campo1} \n {campo2}";  // Incluyo la fecha como ejemplo

            // Añadir la línea al archivo (si no existe, lo crea)
            File.AppendAllText(rutaArchivo, linea + Environment.NewLine);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar en el archivo: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public bool CompareCardsBase64(string base64Image1, string base64Image2)
    {
        try
        {
            using (Image img1 = Base64ToImage(base64Image1))
            using (Image img2 = Base64ToImage(base64Image2))
            {
                // Verificar dimensiones
                if (img1.Width != img2.Width || img1.Height != img2.Height)
                    return false;

                using (Bitmap bmp1 = new Bitmap(img1))
                using (Bitmap bmp2 = new Bitmap(img2))
                {
                    int pixelesSimilares = 0;
                    int totalPixeles = bmp1.Width * bmp1.Height;
                    int tolerancia = 10; // Aumentamos la tolerancia para cartas de póker

                    for (int i = 0; i < bmp1.Width; i++)
                    {
                        for (int j = 0; j < bmp1.Height; j++)
                        {
                            Color pixel1 = bmp1.GetPixel(i, j);
                            Color pixel2 = bmp2.GetPixel(i, j);

                            // Comprobar similitud con mayor tolerancia
                            if (Math.Abs(pixel1.R - pixel2.R) <= tolerancia &&
                                Math.Abs(pixel1.G - pixel2.G) <= tolerancia &&
                                Math.Abs(pixel1.B - pixel2.B) <= tolerancia)
                            {
                                pixelesSimilares++;
                            }
                        }
                    }

                    // Calculamos el porcentaje de similitud
                    double porcentajeSimilitud = (double)pixelesSimilares / totalPixeles * 100;

                    // Para cartas de póker, podemos usar un umbral más bajo
                    return porcentajeSimilitud >= 90; // 90% de similitud es suficiente
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al comparar imágenes de cartas: {ex.Message}");
        }
    }
}
