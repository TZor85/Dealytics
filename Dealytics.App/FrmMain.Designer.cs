namespace Dealytics.App
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            tcMain = new TabControl();
            tpGame = new TabPage();
            btnAddOverlay = new Button();
            tpConfig = new TabPage();
            tcConfiguration = new TabControl();
            tpRegion = new TabPage();
            dgvRegiones = new DataGridView();
            tpCards = new TabPage();
            dgvCartas = new DataGridView();
            tpOcr = new TabPage();
            tpInit = new TabPage();
            button1 = new Button();
            button3 = new Button();
            button2 = new Button();
            tcMain.SuspendLayout();
            tpGame.SuspendLayout();
            tpConfig.SuspendLayout();
            tcConfiguration.SuspendLayout();
            tpRegion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRegiones).BeginInit();
            tpCards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCartas).BeginInit();
            tpInit.SuspendLayout();
            SuspendLayout();
            // 
            // tcMain
            // 
            tcMain.Controls.Add(tpGame);
            tcMain.Controls.Add(tpConfig);
            tcMain.Dock = DockStyle.Fill;
            tcMain.Location = new Point(0, 0);
            tcMain.Name = "tcMain";
            tcMain.SelectedIndex = 0;
            tcMain.Size = new Size(1051, 684);
            tcMain.TabIndex = 0;
            // 
            // tpGame
            // 
            tpGame.Controls.Add(btnAddOverlay);
            tpGame.Location = new Point(4, 24);
            tpGame.Name = "tpGame";
            tpGame.Padding = new Padding(3);
            tpGame.Size = new Size(1043, 656);
            tpGame.TabIndex = 0;
            tpGame.Text = "Juego";
            tpGame.UseVisualStyleBackColor = true;
            // 
            // btnAddOverlay
            // 
            btnAddOverlay.Location = new Point(8, 94);
            btnAddOverlay.Name = "btnAddOverlay";
            btnAddOverlay.Size = new Size(165, 23);
            btnAddOverlay.TabIndex = 0;
            btnAddOverlay.Text = "button4";
            btnAddOverlay.UseVisualStyleBackColor = true;
            // 
            // tpConfig
            // 
            tpConfig.Controls.Add(tcConfiguration);
            tpConfig.Location = new Point(4, 24);
            tpConfig.Name = "tpConfig";
            tpConfig.Padding = new Padding(3);
            tpConfig.Size = new Size(1043, 656);
            tpConfig.TabIndex = 1;
            tpConfig.Text = "Configuración";
            tpConfig.UseVisualStyleBackColor = true;
            // 
            // tcConfiguration
            // 
            tcConfiguration.Controls.Add(tpRegion);
            tcConfiguration.Controls.Add(tpCards);
            tcConfiguration.Controls.Add(tpOcr);
            tcConfiguration.Controls.Add(tpInit);
            tcConfiguration.Dock = DockStyle.Fill;
            tcConfiguration.Location = new Point(3, 3);
            tcConfiguration.Name = "tcConfiguration";
            tcConfiguration.SelectedIndex = 0;
            tcConfiguration.Size = new Size(1037, 650);
            tcConfiguration.TabIndex = 0;
            // 
            // tpRegion
            // 
            tpRegion.Controls.Add(dgvRegiones);
            tpRegion.Location = new Point(4, 24);
            tpRegion.Name = "tpRegion";
            tpRegion.Padding = new Padding(3);
            tpRegion.Size = new Size(1029, 622);
            tpRegion.TabIndex = 0;
            tpRegion.Text = "Regiones";
            tpRegion.UseVisualStyleBackColor = true;
            // 
            // dgvRegiones
            // 
            dgvRegiones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRegiones.Dock = DockStyle.Fill;
            dgvRegiones.Location = new Point(3, 3);
            dgvRegiones.Name = "dgvRegiones";
            dgvRegiones.Size = new Size(1023, 616);
            dgvRegiones.TabIndex = 0;
            // 
            // tpCards
            // 
            tpCards.Controls.Add(dgvCartas);
            tpCards.Location = new Point(4, 24);
            tpCards.Name = "tpCards";
            tpCards.Padding = new Padding(3);
            tpCards.Size = new Size(1029, 622);
            tpCards.TabIndex = 1;
            tpCards.Text = "Cartas";
            tpCards.UseVisualStyleBackColor = true;
            // 
            // dgvCartas
            // 
            dgvCartas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCartas.Dock = DockStyle.Fill;
            dgvCartas.Location = new Point(3, 3);
            dgvCartas.Name = "dgvCartas";
            dgvCartas.Size = new Size(1023, 616);
            dgvCartas.TabIndex = 0;
            // 
            // tpOcr
            // 
            tpOcr.Location = new Point(4, 24);
            tpOcr.Name = "tpOcr";
            tpOcr.Size = new Size(1029, 622);
            tpOcr.TabIndex = 2;
            tpOcr.Text = "OCR";
            tpOcr.UseVisualStyleBackColor = true;
            // 
            // tpInit
            // 
            tpInit.Controls.Add(button1);
            tpInit.Controls.Add(button3);
            tpInit.Controls.Add(button2);
            tpInit.Location = new Point(4, 24);
            tpInit.Name = "tpInit";
            tpInit.Size = new Size(1029, 622);
            tpInit.TabIndex = 3;
            tpInit.Text = "Init Data";
            tpInit.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(269, 209);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 3;
            button1.Text = "Regiones";
            button1.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(225, 65);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 2;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(111, 65);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 1;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1051, 684);
            Controls.Add(tcMain);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmMain";
            Text = "DEALYTICS";
            tcMain.ResumeLayout(false);
            tpGame.ResumeLayout(false);
            tpConfig.ResumeLayout(false);
            tcConfiguration.ResumeLayout(false);
            tpRegion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRegiones).EndInit();
            tpCards.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvCartas).EndInit();
            tpInit.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tcMain;
        private TabPage tpGame;
        private TabPage tpConfig;
        private TabControl tcConfiguration;
        private TabPage tpRegion;
        private TabPage tpCards;
        private TabPage tpOcr;
        private TabPage tpInit;
        private Button button3;
        private Button button2;
        private DataGridView dgvRegiones;
        private DataGridView dgvCartas;
        private Button button1;
        private Button btnAddOverlay;
    }
}
