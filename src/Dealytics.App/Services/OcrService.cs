//using Android.Icu.Number;
using SkiaSharp;
using Tesseract;

namespace Dealytics.App.Services;

public class OcrService
{
    private readonly string _tessdataPath;
    public event Action<string> OnDebugImageGenerated;

    public OcrService()
    {
        // Usamos el directorio de la aplicación
        _tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

        try
        {
            // Crear directorio tessdata si no existe
            var tessdataDir = Path.Combine(_tessdataPath, "tessdata");
            if (!Directory.Exists(tessdataDir))
            {
                Directory.CreateDirectory(tessdataDir);
            }

            // Copiar archivo traineddata si no existe
            var trainedDataPath = Path.Combine(tessdataDir, "eng.traineddata");
            if (!File.Exists(trainedDataPath))
            {
                using var stream = GetType().Assembly.GetManifestResourceStream("Dealytics.App.Resources.tessdata.eng.traineddata");
                if (stream == null)
                {
                    throw new Exception("No se pudo encontrar el archivo eng.traineddata en los recursos.");
                }
                using var fileStream = File.Create(trainedDataPath);
                stream.CopyTo(fileStream);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error inicializando OCR: {ex}");
            throw;
        }
    }

    // Método general para texto normal
    public string ExtractTextFromRegion(string imagePath, int x, int y, int width, int height)
    {
        return ExtractTextFromRegion(imagePath, x, y, width, height, OcrMode.Normal);
    }

    // Método específico para detectar BB (Big Blinds)
    public string ExtractBBFromRegion(string imagePath, int x, int y, int width, int height)
    {
        return ExtractTextFromRegion(imagePath, x, y, width, height, OcrMode.BB);
    }

    private enum OcrMode
    {
        Normal,
        BB
    }

    private string ExtractTextFromRegion(string imagePath, int x, int y, int width, int height, OcrMode mode)
    {
        try
        {
            using var originalBitmap = SKBitmap.Decode(imagePath);
            using var croppedBitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(croppedBitmap);

            var sourceRect = new SKRectI(x, y, x + width, y + height);
            canvas.DrawBitmap(originalBitmap, sourceRect, new SKRect(0, 0, width, height));

            // Aplicar efectos según el modo
            if (mode == OcrMode.BB)
            {
                using var paint = new SKPaint();
                paint.ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
                {
                        2.0f, 0, 0, 0, -0.2f,
                        0, 2.0f, 0, 0, -0.2f,
                        0, 0, 2.0f, 0, -0.2f,
                        0, 0, 0, 1.0f, 0
                });
                canvas.DrawBitmap(croppedBitmap, new SKPoint(0, 0), paint);
            }

            using var processedMs = new MemoryStream();
            using var image = SKImage.FromBitmap(croppedBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            data.SaveTo(processedMs);
            processedMs.Position = 0;

            using var engine = new TesseractEngine(Path.Combine(_tessdataPath, "tessdata"), "eng", EngineMode.Default);

            // Configurar Tesseract según el modo
            if (mode == OcrMode.BB)
            {
                engine.SetVariable("tessedit_char_whitelist", "0123456789.BB");
                engine.SetVariable("classify_bln_numeric_mode", "1");
            }
            else
            {
                engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.-/ ");
            }

            using var img = Pix.LoadFromMemory(processedMs.ToArray());
            using var page = engine.Process(img);

            return page.GetText().Trim();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error en OCR: {ex.Message}");
        }
    }

    // ...

    public OcrResult ExtractTextFromRegionAndDebug(Image sourceImage, int x, int y, int width, int height, double porcentaje = 0, bool onlyNumber = false)
    {
        try
        {
            // Convertir System.Drawing.Image a SKBitmap
            using var ms = new MemoryStream();
            sourceImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;

            using var originalBitmap = SKBitmap.Decode(ms);
            using var croppedBitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(croppedBitmap);

            // Recortar la región
            var sourceRect = new SKRectI(x, y, x + width, y + height);
            canvas.DrawBitmap(originalBitmap, sourceRect, new SKRect(0, 0, width, height));

            // Procesar la imagen
            using var processedBitmap = new SKBitmap(width, height);

            //porcentaje = 0.33;

            // Umbralización pixel por pixel
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var pixel = croppedBitmap.GetPixel(i, j);
                    var brightness = (pixel.Red + pixel.Green + pixel.Blue) / 3;
                    processedBitmap.SetPixel(i, j, brightness > porcentaje * 255 ? SKColors.White : SKColors.Black);
                }
            }

            // Convertir SKBitmap a System.Drawing.Bitmap para Windows Forms
            using var debugMs = new MemoryStream();
            using var debugImage = SKImage.FromBitmap(processedBitmap);
            debugImage.Encode(SKEncodedImageFormat.Png, 100).SaveTo(debugMs);
            var debugBitmap = new Bitmap(debugMs);

            // Procesar con Tesseract
            using var engine = new TesseractEngine(Path.Combine(_tessdataPath, "tessdata"), "eng", EngineMode.Default);
            var charWhiteList = onlyNumber ? "0123456789." : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.-/";

            engine.SetVariable("tessedit_char_whitelist", charWhiteList);
            engine.SetVariable("classify_bln_numeric_mode", "1");
            engine.SetVariable("debug_file", "tessDebug.txt");
            engine.SetVariable("tessedit_pageseg_mode", "7");

            using var img = Pix.LoadFromMemory(debugMs.ToArray());
            using var page = engine.Process(img);

            var text = page.GetText().Trim();
            var response = new OcrResult();

            response.Text = text;
            response.Image = debugBitmap;

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error en OCR: {ex.Message}");
        }
    }
       
}

public class OcrResult
{
    public string? Text { get; set; }
    public Bitmap? Image { get; set; }
}
