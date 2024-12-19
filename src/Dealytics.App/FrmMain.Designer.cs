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
            tbOcr = new TabPage();
            tbCarta = new TextBox();
            pbCartas = new PictureBox();
            btnCard = new Button();
            lbHexadecimal = new Label();
            pbColorDebug = new PictureBox();
            btnColor = new Button();
            btnNext = new Button();
            btnPrevious = new Button();
            pbImageDebug = new PictureBox();
            lbResultOcr = new Label();
            btnTextoOcr = new Button();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            numHeight = new NumericUpDown();
            numWidth = new NumericUpDown();
            numPosY = new NumericUpDown();
            numPosX = new NumericUpDown();
            pbImagenOcr = new PictureBox();
            btnCargarImagen = new Button();
            tvRegions = new TreeView();
            tbRegion = new TabPage();
            lbMessageRegions = new Label();
            btnRegiones = new Button();
            dgvRegiones = new DataGridView();
            tbCards = new TabPage();
            lbMessageCards = new Label();
            btnAddCards = new Button();
            dgvCartas = new DataGridView();
            tbTables = new TabPage();
            lbMessageHands = new Label();
            btnHands = new Button();
            panel1 = new Panel();
            tvTables = new TreeView();
            dgvHands = new DataGridView();
            tbInit = new TabPage();
            button3 = new Button();
            button2 = new Button();
            tcMain.SuspendLayout();
            tpGame.SuspendLayout();
            tpConfig.SuspendLayout();
            tcConfiguration.SuspendLayout();
            tbOcr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbCartas).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbColorDebug).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbImageDebug).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPosY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPosX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbImagenOcr).BeginInit();
            tbRegion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRegiones).BeginInit();
            tbCards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCartas).BeginInit();
            tbTables.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHands).BeginInit();
            tbInit.SuspendLayout();
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
            tcMain.Size = new Size(1233, 841);
            tcMain.TabIndex = 0;
            // 
            // tpGame
            // 
            tpGame.Controls.Add(btnAddOverlay);
            tpGame.Location = new Point(4, 24);
            tpGame.Name = "tpGame";
            tpGame.Padding = new Padding(3);
            tpGame.Size = new Size(1225, 813);
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
            tpConfig.Size = new Size(1225, 813);
            tpConfig.TabIndex = 1;
            tpConfig.Text = "Configuración";
            tpConfig.UseVisualStyleBackColor = true;
            // 
            // tcConfiguration
            // 
            tcConfiguration.Controls.Add(tbOcr);
            tcConfiguration.Controls.Add(tbRegion);
            tcConfiguration.Controls.Add(tbCards);
            tcConfiguration.Controls.Add(tbTables);
            tcConfiguration.Controls.Add(tbInit);
            tcConfiguration.Dock = DockStyle.Fill;
            tcConfiguration.Location = new Point(3, 3);
            tcConfiguration.Name = "tcConfiguration";
            tcConfiguration.SelectedIndex = 0;
            tcConfiguration.Size = new Size(1219, 807);
            tcConfiguration.TabIndex = 0;
            // 
            // tbOcr
            // 
            tbOcr.Controls.Add(tbCarta);
            tbOcr.Controls.Add(pbCartas);
            tbOcr.Controls.Add(btnCard);
            tbOcr.Controls.Add(lbHexadecimal);
            tbOcr.Controls.Add(pbColorDebug);
            tbOcr.Controls.Add(btnColor);
            tbOcr.Controls.Add(btnNext);
            tbOcr.Controls.Add(btnPrevious);
            tbOcr.Controls.Add(pbImageDebug);
            tbOcr.Controls.Add(lbResultOcr);
            tbOcr.Controls.Add(btnTextoOcr);
            tbOcr.Controls.Add(label4);
            tbOcr.Controls.Add(label3);
            tbOcr.Controls.Add(label2);
            tbOcr.Controls.Add(label1);
            tbOcr.Controls.Add(numHeight);
            tbOcr.Controls.Add(numWidth);
            tbOcr.Controls.Add(numPosY);
            tbOcr.Controls.Add(numPosX);
            tbOcr.Controls.Add(pbImagenOcr);
            tbOcr.Controls.Add(btnCargarImagen);
            tbOcr.Controls.Add(tvRegions);
            tbOcr.Location = new Point(4, 24);
            tbOcr.Name = "tbOcr";
            tbOcr.Size = new Size(1211, 779);
            tbOcr.TabIndex = 2;
            tbOcr.Text = "OCR";
            tbOcr.UseVisualStyleBackColor = true;
            // 
            // tbCarta
            // 
            tbCarta.Location = new Point(609, 53);
            tbCarta.Name = "tbCarta";
            tbCarta.Size = new Size(100, 23);
            tbCarta.TabIndex = 21;
            // 
            // pbCartas
            // 
            pbCartas.Location = new Point(720, 8);
            pbCartas.Name = "pbCartas";
            pbCartas.Size = new Size(20, 35);
            pbCartas.TabIndex = 20;
            pbCartas.TabStop = false;
            // 
            // btnCard
            // 
            btnCard.Location = new Point(634, 14);
            btnCard.Name = "btnCard";
            btnCard.Size = new Size(75, 23);
            btnCard.TabIndex = 19;
            btnCard.Text = "Carta";
            btnCard.UseVisualStyleBackColor = true;
            btnCard.Click += btnCard_Click;
            // 
            // lbHexadecimal
            // 
            lbHexadecimal.AutoSize = true;
            lbHexadecimal.Location = new Point(645, 18);
            lbHexadecimal.Name = "lbHexadecimal";
            lbHexadecimal.Size = new Size(0, 15);
            lbHexadecimal.TabIndex = 18;
            // 
            // pbColorDebug
            // 
            pbColorDebug.Location = new Point(595, 15);
            pbColorDebug.Name = "pbColorDebug";
            pbColorDebug.Size = new Size(20, 20);
            pbColorDebug.TabIndex = 17;
            pbColorDebug.TabStop = false;
            // 
            // btnColor
            // 
            btnColor.Location = new Point(515, 14);
            btnColor.Name = "btnColor";
            btnColor.Size = new Size(75, 23);
            btnColor.TabIndex = 16;
            btnColor.Text = "Color";
            btnColor.UseVisualStyleBackColor = true;
            btnColor.Click += btnColor_Click;
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.Transparent;
            btnNext.Cursor = Cursors.Hand;
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnNext.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Font = new Font("Segoe UI", 20F);
            btnNext.ForeColor = Color.Black;
            btnNext.Location = new Point(1179, 242);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(25, 300);
            btnNext.TabIndex = 15;
            btnNext.Text = "→";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += btnNext_Click;
            // 
            // btnPrevious
            // 
            btnPrevious.BackColor = Color.Transparent;
            btnPrevious.Cursor = Cursors.Hand;
            btnPrevious.FlatAppearance.BorderSize = 0;
            btnPrevious.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnPrevious.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnPrevious.FlatStyle = FlatStyle.Flat;
            btnPrevious.Font = new Font("Segoe UI", 20F);
            btnPrevious.ForeColor = Color.Black;
            btnPrevious.Location = new Point(182, 242);
            btnPrevious.Name = "btnPrevious";
            btnPrevious.Size = new Size(25, 300);
            btnPrevious.TabIndex = 14;
            btnPrevious.Text = "←";
            btnPrevious.UseVisualStyleBackColor = true;
            btnPrevious.Click += btnPrevious_Click;
            // 
            // pbImageDebug
            // 
            pbImageDebug.Location = new Point(357, 14);
            pbImageDebug.Name = "pbImageDebug";
            pbImageDebug.Size = new Size(150, 25);
            pbImageDebug.SizeMode = PictureBoxSizeMode.AutoSize;
            pbImageDebug.TabIndex = 13;
            pbImageDebug.TabStop = false;
            // 
            // lbResultOcr
            // 
            lbResultOcr.AutoSize = true;
            lbResultOcr.Location = new Point(261, 18);
            lbResultOcr.Name = "lbResultOcr";
            lbResultOcr.Size = new Size(0, 15);
            lbResultOcr.TabIndex = 12;
            // 
            // btnTextoOcr
            // 
            btnTextoOcr.Location = new Point(184, 14);
            btnTextoOcr.Name = "btnTextoOcr";
            btnTextoOcr.Size = new Size(75, 23);
            btnTextoOcr.TabIndex = 2;
            btnTextoOcr.Text = "Texto";
            btnTextoOcr.UseVisualStyleBackColor = true;
            btnTextoOcr.Click += btnTextoOcr_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(1092, 18);
            label4.Name = "label4";
            label4.Size = new Size(46, 15);
            label4.TabIndex = 11;
            label4.Text = "Height:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(981, 18);
            label3.Name = "label3";
            label3.Size = new Size(42, 15);
            label3.TabIndex = 10;
            label3.Text = "Width:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(893, 18);
            label2.Name = "label2";
            label2.Size = new Size(17, 15);
            label2.TabIndex = 9;
            label2.Text = "Y:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(805, 18);
            label1.Name = "label1";
            label1.Size = new Size(17, 15);
            label1.TabIndex = 8;
            label1.Text = "X:";
            // 
            // numHeight
            // 
            numHeight.Location = new Point(1142, 14);
            numHeight.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numHeight.Name = "numHeight";
            numHeight.Size = new Size(60, 23);
            numHeight.TabIndex = 6;
            numHeight.ValueChanged += NumericUpDown_ValueChanged;
            // 
            // numWidth
            // 
            numWidth.Location = new Point(1026, 14);
            numWidth.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numWidth.Name = "numWidth";
            numWidth.Size = new Size(60, 23);
            numWidth.TabIndex = 5;
            numWidth.ValueChanged += NumericUpDown_ValueChanged;
            // 
            // numPosY
            // 
            numPosY.Location = new Point(913, 14);
            numPosY.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numPosY.Name = "numPosY";
            numPosY.Size = new Size(60, 23);
            numPosY.TabIndex = 4;
            numPosY.ValueChanged += NumericUpDown_ValueChanged;
            // 
            // numPosX
            // 
            numPosX.Location = new Point(825, 14);
            numPosX.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numPosX.Name = "numPosX";
            numPosX.Size = new Size(60, 23);
            numPosX.TabIndex = 3;
            numPosX.ValueChanged += NumericUpDown_ValueChanged;
            // 
            // pbImagenOcr
            // 
            pbImagenOcr.Location = new Point(181, 49);
            pbImagenOcr.Name = "pbImagenOcr";
            pbImagenOcr.Size = new Size(1025, 727);
            pbImagenOcr.SizeMode = PictureBoxSizeMode.AutoSize;
            pbImagenOcr.TabIndex = 2;
            pbImagenOcr.TabStop = false;
            pbImagenOcr.Paint += pbImagenOcr_Paint;
            // 
            // btnCargarImagen
            // 
            btnCargarImagen.Location = new Point(3, 14);
            btnCargarImagen.Name = "btnCargarImagen";
            btnCargarImagen.Size = new Size(75, 23);
            btnCargarImagen.TabIndex = 1;
            btnCargarImagen.Text = "Cargar";
            btnCargarImagen.UseVisualStyleBackColor = true;
            btnCargarImagen.Click += btnCargarImagen_Click;
            // 
            // tvRegions
            // 
            tvRegions.Location = new Point(0, 49);
            tvRegions.Name = "tvRegions";
            tvRegions.Size = new Size(175, 727);
            tvRegions.TabIndex = 0;
            tvRegions.DoubleClick += tvRegions_DoubleClick;
            // 
            // tbRegion
            // 
            tbRegion.Controls.Add(lbMessageRegions);
            tbRegion.Controls.Add(btnRegiones);
            tbRegion.Controls.Add(dgvRegiones);
            tbRegion.Location = new Point(4, 24);
            tbRegion.Name = "tbRegion";
            tbRegion.Padding = new Padding(3);
            tbRegion.Size = new Size(1211, 779);
            tbRegion.TabIndex = 0;
            tbRegion.Text = "Regiones";
            tbRegion.UseVisualStyleBackColor = true;
            // 
            // lbMessageRegions
            // 
            lbMessageRegions.AutoSize = true;
            lbMessageRegions.Location = new Point(87, 12);
            lbMessageRegions.Name = "lbMessageRegions";
            lbMessageRegions.Size = new Size(0, 15);
            lbMessageRegions.TabIndex = 5;
            // 
            // btnRegiones
            // 
            btnRegiones.Location = new Point(6, 8);
            btnRegiones.Name = "btnRegiones";
            btnRegiones.Size = new Size(75, 23);
            btnRegiones.TabIndex = 4;
            btnRegiones.Text = "Regiones";
            btnRegiones.UseVisualStyleBackColor = true;
            btnRegiones.Click += btnRegiones_Click;
            // 
            // dgvRegiones
            // 
            dgvRegiones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRegiones.Dock = DockStyle.Bottom;
            dgvRegiones.Location = new Point(3, 37);
            dgvRegiones.Name = "dgvRegiones";
            dgvRegiones.Size = new Size(1205, 739);
            dgvRegiones.TabIndex = 0;
            // 
            // tbCards
            // 
            tbCards.Controls.Add(lbMessageCards);
            tbCards.Controls.Add(btnAddCards);
            tbCards.Controls.Add(dgvCartas);
            tbCards.Location = new Point(4, 24);
            tbCards.Name = "tbCards";
            tbCards.Padding = new Padding(3);
            tbCards.Size = new Size(1211, 779);
            tbCards.TabIndex = 1;
            tbCards.Text = "Cartas";
            tbCards.UseVisualStyleBackColor = true;
            // 
            // lbMessageCards
            // 
            lbMessageCards.AutoSize = true;
            lbMessageCards.Location = new Point(87, 12);
            lbMessageCards.Name = "lbMessageCards";
            lbMessageCards.Size = new Size(0, 15);
            lbMessageCards.TabIndex = 7;
            // 
            // btnAddCards
            // 
            btnAddCards.Location = new Point(6, 8);
            btnAddCards.Name = "btnAddCards";
            btnAddCards.Size = new Size(75, 23);
            btnAddCards.TabIndex = 6;
            btnAddCards.Text = "Cartas";
            btnAddCards.UseVisualStyleBackColor = true;
            btnAddCards.Click += btnAddCards_Click;
            // 
            // dgvCartas
            // 
            dgvCartas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCartas.Dock = DockStyle.Bottom;
            dgvCartas.Location = new Point(3, 37);
            dgvCartas.Name = "dgvCartas";
            dgvCartas.Size = new Size(1205, 739);
            dgvCartas.TabIndex = 0;
            // 
            // tbTables
            // 
            tbTables.Controls.Add(lbMessageHands);
            tbTables.Controls.Add(btnHands);
            tbTables.Controls.Add(panel1);
            tbTables.Location = new Point(4, 24);
            tbTables.Name = "tbTables";
            tbTables.Size = new Size(1211, 779);
            tbTables.TabIndex = 4;
            tbTables.Text = "Manos";
            tbTables.UseVisualStyleBackColor = true;
            // 
            // lbMessageHands
            // 
            lbMessageHands.AutoSize = true;
            lbMessageHands.Location = new Point(84, 15);
            lbMessageHands.Name = "lbMessageHands";
            lbMessageHands.Size = new Size(0, 15);
            lbMessageHands.TabIndex = 9;
            // 
            // btnHands
            // 
            btnHands.Location = new Point(3, 11);
            btnHands.Name = "btnHands";
            btnHands.Size = new Size(75, 23);
            btnHands.TabIndex = 8;
            btnHands.Text = "Manos";
            btnHands.UseVisualStyleBackColor = true;
            btnHands.Click += btnHands_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(tvTables);
            panel1.Controls.Add(dgvHands);
            panel1.Location = new Point(1, 40);
            panel1.Name = "panel1";
            panel1.Size = new Size(1210, 738);
            panel1.TabIndex = 3;
            // 
            // tvTables
            // 
            tvTables.Dock = DockStyle.Left;
            tvTables.Location = new Point(0, 0);
            tvTables.Name = "tvTables";
            tvTables.Size = new Size(262, 738);
            tvTables.TabIndex = 4;
            tvTables.DoubleClick += tvTables_DoubleClick;
            // 
            // dgvHands
            // 
            dgvHands.AllowUserToAddRows = false;
            dgvHands.AllowUserToDeleteRows = false;
            dgvHands.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHands.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHands.Dock = DockStyle.Right;
            dgvHands.EnableHeadersVisualStyles = false;
            dgvHands.Location = new Point(260, 0);
            dgvHands.MultiSelect = false;
            dgvHands.Name = "dgvHands";
            dgvHands.ReadOnly = true;
            dgvHands.RowHeadersVisible = false;
            dgvHands.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHands.Size = new Size(950, 738);
            dgvHands.TabIndex = 3;
            // 
            // tbInit
            // 
            tbInit.Controls.Add(button3);
            tbInit.Controls.Add(button2);
            tbInit.Location = new Point(4, 24);
            tbInit.Name = "tbInit";
            tbInit.Size = new Size(1211, 779);
            tbInit.TabIndex = 3;
            tbInit.Text = "Init Data";
            tbInit.UseVisualStyleBackColor = true;
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
            ClientSize = new Size(1233, 841);
            Controls.Add(tcMain);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmMain";
            Text = "DEALYTICS";
            tcMain.ResumeLayout(false);
            tpGame.ResumeLayout(false);
            tpConfig.ResumeLayout(false);
            tcConfiguration.ResumeLayout(false);
            tbOcr.ResumeLayout(false);
            tbOcr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbCartas).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbColorDebug).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbImageDebug).EndInit();
            ((System.ComponentModel.ISupportInitialize)numHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)numWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPosY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPosX).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbImagenOcr).EndInit();
            tbRegion.ResumeLayout(false);
            tbRegion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRegiones).EndInit();
            tbCards.ResumeLayout(false);
            tbCards.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCartas).EndInit();
            tbTables.ResumeLayout(false);
            tbTables.PerformLayout();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvHands).EndInit();
            tbInit.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tcMain;
        private TabPage tpGame;
        private TabPage tpConfig;
        private TabControl tcConfiguration;
        private TabPage tbRegion;
        private TabPage tbCards;
        private TabPage tbOcr;
        private TabPage tbInit;
        private Button button3;
        private Button button2;
        private DataGridView dgvRegiones;
        private DataGridView dgvCartas;
        private Button btnAddOverlay;
        private Button btnRegiones;
        private Label lbMessageRegions;
        private Label lbMessageCards;
        private Button btnAddCards;
        private TreeView tvRegions;
        private Button btnCargarImagen;
        private PictureBox pbImagenOcr;
        private NumericUpDown numHeight;
        private NumericUpDown numWidth;
        private NumericUpDown numPosY;
        private NumericUpDown numPosX;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TabPage tbTables;
        private Panel panel1;
        private TreeView tvTables;
        private DataGridView dgvHands;
        private Label lbMessageHands;
        private Button btnHands;
        private Button btnTextoOcr;
        private Label lbResultOcr;
        private PictureBox pbImageDebug;
        private Button btnPrevious;
        private Button btnNext;
        private Button btnColor;
        private PictureBox pbColorDebug;
        private Label lbHexadecimal;
        private Button btnCard;
        private PictureBox pbCartas;
        private TextBox tbCarta;
    }
}
