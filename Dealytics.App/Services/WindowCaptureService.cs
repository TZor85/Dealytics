using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Dealytics.App.Services
{
    public class WindowCaptureService
    {
        // Importaciones de Windows API necesarias
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, int nFlags);

        // Estructura para almacenar las coordenadas de la ventana
        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public Bitmap CaptureWindow(IntPtr handle)
        {
            try
            {
                // Obtener las dimensiones de la ventana
                var rect = new Rect();
                GetWindowRect(handle, ref rect);

                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                // Crear un bitmap con las dimensiones de la ventana
                var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    // Obtener el handle del dispositivo de contexto (DC) para el graphics
                    IntPtr hdcBitmap = graphics.GetHdc();

                    // Capturar la ventana
                    PrintWindow(handle, hdcBitmap, 0);

                    // Liberar el DC
                    graphics.ReleaseHdc(hdcBitmap);
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al capturar la ventana: {ex.Message}", ex);
            }
        }
    }
}
