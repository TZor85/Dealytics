﻿using SkiaSharp;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PokerVisionAI.App.Services;

public class ColorDetectionService
{
    private byte[]? _pixelData;
    private int _stride;
    private Image? _lastImage;
    private GCHandle _handle;

    //public SKColor GetPixelColor(Image image, int x, int y)
    //{
    //    try
    //    {
    //        // Convertir System.Drawing.Image a Stream
    //        using var ms = new MemoryStream();
    //        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
    //        ms.Position = 0;

    //        // Cargar imagen en SKBitmap
    //        using var bitmap = SKBitmap.Decode(ms);

    //        // Obtener el color del pixel en la posición x,y
    //        SKColor pixelColor = bitmap.GetPixel(x, y);
    //        return pixelColor;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception($"Error al obtener el color: {ex.Message}");
    //    }
    //}

    public SKColor GetPixelColor(Image image, int x, int y)
    {
        try
        {
            if (_lastImage != image || _pixelData == null)
            {
                ReleaseCurrentData();

                using var bitmap = new Bitmap(image);
                var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                _stride = bitmapData.Stride;
                _pixelData = new byte[_stride * bitmap.Height];
                _handle = GCHandle.Alloc(_pixelData, GCHandleType.Pinned);

                Marshal.Copy(bitmapData.Scan0, _pixelData, 0, _pixelData.Length);
                bitmap.UnlockBits(bitmapData);

                _lastImage = image;
            }

            int index = y * _stride + x * 4;
            return new SKColor(
                _pixelData[index + 2], // R
                _pixelData[index + 1], // G
                _pixelData[index],     // B
                _pixelData[index + 3]  // A
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener el color: {ex.Message}");
        }
    }

    private void ReleaseCurrentData()
    {
        if (_handle.IsAllocated)
            _handle.Free();
        _pixelData = null;
        _lastImage = null;
    }

    public void Dispose()
    {
        ReleaseCurrentData();
    }

}
