using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Dealytics.App.Overlays
{
    public class FrmExternalWindowOverlay : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);

        private System.Windows.Forms.Timer positionTimer;
        private IntPtr targetWindowHandle;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string WindowTitle { get; private set; }
        private bool isDragging = false;
        private Point dragOffset;
        private bool autoPosition = true;  // Controla si la posición es automática

        private Label label1;
        private Label label2;
        private Label label3;

        private struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        public FrmExternalWindowOverlay(string windowTitle)
        {
            WindowTitle = windowTitle;
            targetWindowHandle = FindWindow(null, windowTitle);
            if (targetWindowHandle != IntPtr.Zero)
            {
                InitializeForm();
                InitializeLabels();
                SetupMovement();
            }
        }       

        private void InitializeForm()
        {
            
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = Color.White;
            WindowTitle = "Dealytics";
            TransparencyKey = Color.White;
            Opacity = 0.7;

            Size = new Size(200, 150);

            positionTimer = new System.Windows.Forms.Timer
            {
                Interval = 50
            };
            positionTimer.Tick += UpdatePosition;
            positionTimer.Start();
        }

        private void InitializeLabels()
        {
            // Label 1
            label1 = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(180, 20),
                Text = "Label 1",
                BackColor = Color.LightBlue,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Label 2
            label2 = new Label
            {
                Location = new Point(10, 40),
                Size = new Size(180, 20),
                Text = "Label 2",
                BackColor = Color.LightGreen,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Label 3
            label3 = new Label
            {
                Location = new Point(10, 70),
                Size = new Size(180, 20),
                Text = "Label 3",
                BackColor = Color.LightPink,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Agregar los labels al formulario
            Controls.AddRange(new Control[] { label1, label2, label3 });
        }

        private void SetupMovement()
        {
            // Movimiento con ratón
            this.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    isDragging = true;
                    autoPosition = false;  // Desactivar el posicionamiento automático
                    dragOffset = new Point(e.X, e.Y);
                }
            };

            this.MouseMove += (s, e) =>
            {
                if (isDragging)
                {
                    Point currentScreenPos = PointToScreen(new Point(e.X, e.Y));
                    Location = new Point(
                        currentScreenPos.X - dragOffset.X,
                        currentScreenPos.Y - dragOffset.Y
                    );
                }
            };

            this.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    isDragging = false;
                }
            };

            // Movimiento con teclas
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                autoPosition = false;  // Desactivar posicionamiento automático
                int moveStep = 5;  // Pixels a mover

                switch (e.KeyCode)
                {
                    case Keys.Left:
                        this.Left -= moveStep;
                        break;
                    case Keys.Right:
                        this.Left += moveStep;
                        break;
                    case Keys.Up:
                        this.Top -= moveStep;
                        break;
                    case Keys.Down:
                        this.Top += moveStep;
                        break;
                    case Keys.P:  // Restaurar posición automática
                        autoPosition = true;
                        break;
                }
            };
        }

        // Métodos para actualizar el texto de los labels
        public void UpdateLabel1(string text)
        {
            if (label1.InvokeRequired)
            {
                label1.Invoke(new Action(() => label1.Text = text));
            }
            else
            {
                label1.Text = "texto1";
            }
        }

        public void UpdateLabel2(string text)
        {
            if (label2.InvokeRequired)
            {
                label2.Invoke(new Action(() => label2.Text = text));
            }
            else
            {
                label2.Text = "texto2";
            }
        }

        public void UpdateLabel3(string text)
        {
            if (label3.InvokeRequired)
            {
                label3.Invoke(new Action(() => label3.Text = text));
            }
            else
            {
                label3.Text = "texto3";
            }
        }

        private void UpdatePosition(object sender, EventArgs e)
        {
            if (!IsTargetWindowValid())
            {
                Close();
                return;
            }

            // Solo actualizar la posición si autoPosition es true
            if (autoPosition)
            {
                Rect rect = new Rect();
                if (GetWindowRect(targetWindowHandle, ref rect))
                {
                    int targetWidth = rect.Right - rect.Left;
                    int targetHeight = rect.Bottom - rect.Top;

                    Location = new Point(
                        rect.Left + targetWidth - this.Width - 20,
                        rect.Top + 20
                    );
                }
            }
        }

        public bool IsTargetWindowValid()
        {
            return targetWindowHandle != IntPtr.Zero && IsWindow(targetWindowHandle);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            positionTimer?.Stop();
            base.OnFormClosing(e);
        }
    }
}
