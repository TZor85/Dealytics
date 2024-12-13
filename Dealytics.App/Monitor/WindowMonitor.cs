using System.Runtime.InteropServices;
using System.Text;
using Timer = System.Windows.Forms.Timer;

namespace Dealytics.App.Monitor
{
    public class WindowChangedEventArgs : EventArgs
    {
        public IntPtr Handle { get; set; }
        public string? Title { get; set; }
    }

    public class WindowMonitor
    {
        private Queue<WindowInfo> windowQueue = new Queue<WindowInfo>();
        private Timer monitorTimer;
        private Dictionary<string, DateTime> cooldowns = new Dictionary<string, DateTime>();
        private int cooldownSeconds = 1;
        private string tituloABuscar;
        private IntPtr lastHandle = IntPtr.Zero;

        // Evento para notificar cambios de ventana
        public event EventHandler<WindowChangedEventArgs> OnWindowChanged;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);

        public WindowMonitor(string titulo, int cooldownTime = 5)
        {
            tituloABuscar = titulo;
            cooldownSeconds = cooldownTime;
            monitorTimer = new Timer();
            monitorTimer.Interval = 1500;
            monitorTimer.Tick += CheckActiveWindow;
            monitorTimer.Start();
        }

        private void CheckActiveWindow(object sender, EventArgs e)
        {
            IntPtr handle = GetForegroundWindow();

            if (handle != lastHandle && handle != IntPtr.Zero)
            {
                StringBuilder sb = new StringBuilder(256);
                GetWindowText(handle, sb, 256);
                string title = sb.ToString();

                if (!string.IsNullOrEmpty(title))
                {
                    if (title.Contains(tituloABuscar))
                    {
                        if (cooldowns.ContainsKey(title))
                        {
                            if ((DateTime.Now - cooldowns[title]).TotalSeconds < cooldownSeconds)
                            {
                                return;
                            }
                            cooldowns.Remove(title);
                        }

                        var windowInfo = new WindowInfo
                        {
                            Handle = handle,
                            Title = title
                        };

                        windowQueue.Enqueue(windowInfo);
                        if (windowQueue.Count > 10)
                        {
                            windowQueue.Dequeue();
                        }

                        // Lanzar el evento de cambio de ventana
                        OnWindowChanged?.Invoke(this, new WindowChangedEventArgs
                        {
                            Handle = handle,
                            Title = title
                        });

                        windowQueue = new Queue<WindowInfo>(windowQueue.Where(w => w.Title != title));
                        cooldowns[title] = DateTime.Now;
                    }

                    lastHandle = handle;
                }
            }
        }

        public void SetTituloBuscado(string titulo)
        {
            tituloABuscar = titulo;
        }

        public void SetCooldownTime(int seconds)
        {
            cooldownSeconds = seconds;
        }

        public void StopMonitoring()
        {
            monitorTimer.Stop();
        }

        public Queue<WindowInfo> GetWindowQueue()
        {
            return windowQueue;
        }
    }
}
