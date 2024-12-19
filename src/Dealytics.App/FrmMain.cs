using Dealytics.App.Maganers;
using Dealytics.App.Monitor;
using Dealytics.App.Overlays;
using Dealytics.App.Services;
using Dealytics.Domain.Entities;
using Dealytics.Domain.Mappers;
using Dealytics.Features.Action;
using Dealytics.Features.Action.CreateAll;
using Dealytics.Features.Card;
using Dealytics.Features.Card.CreateAll;
using Dealytics.Features.Regions;
using Dealytics.Features.Regions.CreateAll;
using Dealytics.Features.Regions.Update;
using Marten;
using PokerVisionAI.App.Services;
using SkiaSharp;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Dealytics.App;

public partial class FrmMain : Form
{
    private readonly IDocumentStore _store;
    private OverlayManager overlayManager;
    private WindowMonitor monitor;

    #region UseCases
    private RegionUseCases _regionUseCases;
    private CardUseCases _cardUseCases;
    private ActionUseCases _actionUseCases;
    #endregion

    private List<RegionCategory> _regionCateory;
    private IReadOnlyList<Table> _tables;
    private List<Table> _dataTables;
    private Rectangle currentRectangle;
    private bool shouldDrawRectangle = false;
    private Domain.ValueObjects.Region? _selectedRegion;

    #region Services
    private WindowCaptureService _windowCaptureService;
    private OcrService _ocrService = new();
    private ColorDetectionService _colorDetectionService = new();
    private ImageCropperService _imageCropperService = new();

    #endregion

    #region DirectoryControl
    private string[] imageFiles;
    private int currentImageIndex = -1;
    private string currentDirectory;
    #endregion

    private Dictionary<nint, FrmExternalWindowOverlay> activeOverlays = new Dictionary<nint, FrmExternalWindowOverlay>();

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public FrmMain(IDocumentStore store,
        RegionUseCases regionUseCases,
        CardUseCases cardUseCases,
        ActionUseCases actionUseCases)
    {
        InitializeComponent();
        _store = store;
        overlayManager = new OverlayManager();
        monitor = new WindowMonitor("poker", 1);
        _windowCaptureService = new WindowCaptureService();

        _regionUseCases = regionUseCases;
        _cardUseCases = cardUseCases;
        _actionUseCases = actionUseCases;

        monitor.OnWindowChanged += Monitor_OnWindowChanged;

        Task.FromResult(LoadDataAsync());
    }

    private async Task LoadDataAsync()
    {
        try
        {
            using (var session = _store.LightweightSession())
            {
                var regions = new List<Domain.ValueObjects.Region>();

                var dataRegions = await session.Query<RegionCategory>().ToListAsync();
                var categories = dataRegions.Select(x => x.Regions).Where(x => x != null).Distinct().ToList();

                foreach (var group in categories)
                {
                    foreach (var category in group!)
                    {
                        regions.Add(category);
                    }
                }

                var dataCards = await session.Query<Card>().ToListAsync();

                dgvCartas.DataSource = dataCards;
                dgvRegiones.DataSource = regions;
                _regionCateory = dataRegions.ToList();
                LoadTreeView(dataRegions.ToList());

                _tables = await session.Query<Table>().ToListAsync();
                _dataTables = _tables.ToList();
                LoadTvHands(_dataTables);

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    //TODO: Evento que se ejecuta automáticamente cada vez que se activa una ventana con la palabra clave
    private void Monitor_OnWindowChanged(object? sender, WindowChangedEventArgs e)
    {
        // Este método se ejecutará cada vez que cambie la ventana activa
        // que contenga la palabra clave
        btnAddOverlay.Text = ($"Handle: {e.Handle}");

        var overlay = new FrmExternalWindowOverlay(e.Title ?? string.Empty);

        if (!activeOverlays.ContainsKey(e.Handle))
        {
            if (overlay.IsTargetWindowValid())
            {
                overlay.Show();
                activeOverlays.Add(e.Handle, overlay);

                overlay.FormClosed += (s, args) =>
                {
                    activeOverlays.Remove(e.Handle);
                };
            }
        }

        // Se agrega la imagen de la ventana activa
        var image = _windowCaptureService.CaptureWindow(e.Handle);
        pbImagenOcr.Image = image;

        // Aquí puedes agregar cualquier lógica adicional que necesites
    }

    private async void btnRegiones_Click(object sender, EventArgs e)
    {
        try
        {
            var regions = new List<RegionCategory>();
            var regionsDto = await Task.Run(async () => await _regionUseCases.GetAllRegions.Executesync());

            if (regionsDto != null)
            {
                foreach (var item in regionsDto)
                {
                    regions.Add(item.ToEntity());
                }

                await Task.Run(async () =>
                {
                    await _regionUseCases.CreateAllRegions.ExecuteAsync(new CreateAllRegionsRequest(regions));
                });

                lbMessageRegions.Text = "Regiones inicializadas correctamente";
            }
            else
            {
                lbMessageRegions.Text = "No se encontraron regiones para inicializar.";
            }
        }
        catch (Exception ex)
        {
            lbMessageRegions.Text = $"Error al inicializar regiones: {ex.Message}";
        }
    }

    private async void btnAddCards_Click(object sender, EventArgs e)
    {
        try
        {
            var cards = new List<Card>();
            var cardsDto = await Task.Run(async () => await _cardUseCases.LoadAllCards.ExecuteAsync());

            if (cardsDto != null)
            {
                foreach (var item in cardsDto)
                {
                    cards.Add(item.ToEntity());
                }

                await Task.Run(async () =>
                {
                    await _cardUseCases.CreateAllCards.ExecuteAsync(new CreateAllCardsRequest(cards));
                });

                lbMessageCards.Text = "Cartas inicializadas correctamente";
            }
            else
            {
                lbMessageCards.Text = "No se encontraron cartas para inicializar.";
            }
        }
        catch (Exception ex)
        {
            lbMessageCards.Text = $"Error al inicializar cartas: {ex.Message}";
        }
    }

    private async void btnHands_Click(object sender, EventArgs e)
    {
        try
        {
            var actions = new List<Domain.Entities.Table>();
            var actionsDto = await Task.Run(async () => await _actionUseCases.GetAllActions.ExecuteAsync());

            if (actionsDto != null)
            {
                actions.Add(actionsDto.ToEntity());
                await Task.Run(async () =>
                {
                    await _actionUseCases.CreateAllActions.ExecuteAsync(new CreateAllActionsRequest(actions));
                });

                lbMessageHands.Text = "Manos inicializadas correctamente";
            }
            else
            {
                lbMessageHands.Text = "No se encontraron manos para inicializar.";
            }
        }
        catch (Exception ex)
        {
            lbMessageHands.Text = $"Error al inicializar manos: {ex.Message}";
        }
    }

    private void LoadTreeView(List<RegionCategory> categories)
    {
        tvRegions.Nodes.Clear();

        foreach (var category in categories)
        {
            // Añadir nodo principal (categoría)
            TreeNode categoryNode = tvRegions.Nodes.Add(category.Id);

            // Añadir sub-nodos (regiones)
            if (category.Regions != null)
            {
                foreach (var region in category.Regions)
                {
                    categoryNode.Nodes.Add(region.Name); // Asumiendo que Region tiene una propiedad Name
                }
            }
        }
    }

    private void LoadTvHands(List<Domain.Entities.Table> actions)
    {
        tvTables.Nodes.Clear();

        foreach (var action in actions)
        {
            // Primer nivel - Name de la tabla
            TreeNode actionNode = tvTables.Nodes.Add(action.Id, action.Id);

            if (action.Positions != null && action.Positions.Any())
            {
                // Agrupar por HeroPosition para crear el segundo nivel
                var positionGroups = action.Positions
                    .GroupBy(p => p.HeroPosition)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var positionGroup in positionGroups)
                {
                    // Segundo nivel - HeroPosition
                    TreeNode heroPositionNode = actionNode.Nodes.Add(
                        positionGroup.Key, // Key como identificador
                        positionGroup.Key  // Key como texto a mostrar
                    );

                    // Tercer nivel - Name de cada posición
                    foreach (var position in positionGroup.Value)
                    {
                        heroPositionNode.Nodes.Add(
                            position.Name, // Identificador único (puedes usar un Guid si lo necesitas)
                            position.Name  // Texto a mostrar
                        );
                    }
                }
            }
        }
    }

    private void LoadImagesFromDirectory(string filePath)
    {
        currentDirectory = Path.GetDirectoryName(filePath);
        imageFiles = Directory.GetFiles(currentDirectory, "*.png")  // Añade más extensiones si es necesario
                             .Union(Directory.GetFiles(currentDirectory, "*.jpg"))
                             .Union(Directory.GetFiles(currentDirectory, "*.jpeg"))
                             .ToArray();

        // Encuentra el índice de la imagen actual
        currentImageIndex = Array.IndexOf(imageFiles, filePath);
        UpdateNavigationButtons();
    }

    private void btnPrevious_Click(object sender, EventArgs e)
    {
        if (currentImageIndex > 0)
        {
            currentImageIndex--;
            LoadImage(imageFiles[currentImageIndex]);
            UpdateNavigationButtons();
        }
    }

    private void btnNext_Click(object sender, EventArgs e)
    {
        if (currentImageIndex < imageFiles.Length - 1)
        {
            currentImageIndex++;
            LoadImage(imageFiles[currentImageIndex]);
            UpdateNavigationButtons();
        }
    }

    private void UpdateNavigationButtons()
    {
        btnPrevious.Enabled = currentImageIndex > 0;
        btnNext.Enabled = currentImageIndex < imageFiles.Length - 1;
    }
    private void LoadImage(string filePath)
    {
        if (File.Exists(filePath))
        {
            if (pbImagenOcr.Image != null)
            {
                pbImagenOcr.Image.Dispose();
            }
            pbImagenOcr.Image = Image.FromFile(filePath);
        }
    }

    private void btnCargarImagen_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog.Title = "Seleccionar Imagen";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadImage(openFileDialog.FileName);
                LoadImagesFromDirectory(openFileDialog.FileName);

            }
        }
    }

    // Opcional: Navegación con teclas
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Left && btnPrevious?.Enabled == true)
        {
            btnPrevious_Click(this, EventArgs.Empty);
            return true;
        }
        if (keyData == Keys.Right && btnNext?.Enabled == true)
        {
            btnNext_Click(this, EventArgs.Empty);
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }
    private void tvRegions_DoubleClick(object sender, EventArgs e)
    {
        if (sender is TreeView treeView && treeView.SelectedNode != null) // Si es un nodo hijo (región)
        {
            if (pbImagenOcr.Image != null)
            {
                // Obtener la región correspondiente
                _selectedRegion = ObtenerRegionDelNodo(treeView.SelectedNode);
                if (_selectedRegion?.Name != null)
                {
                    numPosX.Value = _selectedRegion.PosX;
                    numPosY.Value = _selectedRegion.PosY;
                    numWidth.Value = _selectedRegion.Width;
                    numHeight.Value = _selectedRegion.Height;

                    // Calcular escala preservando el aspect ratio de la imagen
                    float ratio = Math.Min(
                        (float)pbImagenOcr.Width / pbImagenOcr.Image.Width,
                        (float)pbImagenOcr.Height / pbImagenOcr.Image.Height
                    );

                    // Actualizar el rectángulo actual
                    currentRectangle = new Rectangle(
                        (int)(_selectedRegion.PosX * ratio),
                        (int)(_selectedRegion.PosY * ratio),
                        (int)(_selectedRegion.Width * ratio),
                        (int)(_selectedRegion.Height * ratio)
                    );

                    shouldDrawRectangle = true;
                    pbImagenOcr.Invalidate(); // Forzar redibujado
                }
            }
        }
    }

    private void tvTables_DoubleClick(object sender, EventArgs e)
    {
        if (sender is TreeView treeView && treeView.SelectedNode != null)
        {
            var manos = ObtenerManosActionNodo(treeView.SelectedNode);
            var manosOrder = manos?.OrderByDescending(o => o.Name).ToList();
            dgvHands.DataSource = manosOrder;
        }
    }

    private List<Domain.ValueObjects.Hand> ObtenerManosActionNodo(TreeNode selectedNode)
    {
        // Solo procesar si es un nodo hoja (último nivel)
        if (selectedNode.Nodes.Count == 0 &&
            selectedNode.Parent != null &&
            selectedNode.Parent.Parent != null)
        {
            // Obtener el nombre de la posición (nivel actual)
            string positionName = selectedNode.Text;

            // Obtener el HeroPosition (nivel padre)
            string heroPosition = selectedNode.Parent.Text;

            // Obtener el nombre de la tabla (nivel raíz)
            string tableName = selectedNode.Parent.Parent.Text;

            // Buscar la tabla correspondiente
            var table = _dataTables.FirstOrDefault(f => f.Id == tableName);
            if (table != null)
            {
                // Buscar la posición específica
                var position = table.Positions?
                    .FirstOrDefault(p =>
                        p.Name == positionName &&
                        p.HeroPosition == heroPosition);

                return position?.Hands?.ToList() ?? new List<Domain.ValueObjects.Hand>();
            }
        }

        return new List<Domain.ValueObjects.Hand>();
    }

    private Domain.ValueObjects.Region? ObtenerRegionDelNodo(TreeNode node)
    {
        // Implementa la lógica para obtener la región basada en el nodo
        return _regionCateory.SelectMany(c => c.Regions ?? new List<Domain.ValueObjects.Region>())
                    .FirstOrDefault(r => r.Name == node.Text);
    }

    private void pbImagenOcr_Paint(object sender, PaintEventArgs e)
    {
        if (shouldDrawRectangle)
        {
            e.Graphics.DrawRectangle(Pens.Red, currentRectangle);
        }
    }

    private void NumericUpDown_ValueChanged(object sender, EventArgs e)
    {
        if (pbImagenOcr.Image != null)
        {
            float ratio = Math.Min(
               (float)pbImagenOcr.Width / pbImagenOcr.Image.Width,
               (float)pbImagenOcr.Height / pbImagenOcr.Image.Height);

            currentRectangle = new Rectangle(
                (int)((float)numPosX.Value * ratio),
                (int)((float)numPosY.Value * ratio),
                (int)((float)numWidth.Value * ratio),
                (int)((float)numHeight.Value * ratio)
            );
            pbImagenOcr.Invalidate();
        }
    }

    //private async void btnUpdateRegion_Click(object sender, EventArgs e)
    //{
    //    if (tvRegions.SelectedNode?.Parent != null)
    //    {
    //        await Task.Run(async () => await _regionUseCases.UpdateRegion.ExecuteAsync(new UpdateRegionRequest(
    //            tvRegions.SelectedNode.Parent.Text,
    //            tvRegions.SelectedNode.Text,
    //            (int)numPosX.Value,
    //            (int)numPosY.Value,
    //            (int)numWidth.Value,
    //            (int)numHeight.Value)));
    //    }
    //    else
    //    {
    //        lbMessageRegions.Text = "Seleccione una región válida para actualizar.";
    //    }
    //}

    private void btnTextoOcr_Click(object sender, EventArgs e)
    {
        if (_selectedRegion != null && pbImagenOcr.Image != null)
        {
            var ocr = new OcrResult();

            ocr = _ocrService.ExtractTextFromRegionAndDebug(
                        pbImagenOcr.Image,
                        _selectedRegion.PosX,
                        _selectedRegion.PosY,
                        _selectedRegion.Width,
                        _selectedRegion.Height,
                        _selectedRegion.Umbral.GetValueOrDefault(), false);

            if (string.IsNullOrEmpty(ocr.Text))
            {
                ocr = _ocrService.ExtractTextFromRegionAndDebug(
                        pbImagenOcr.Image,
                        _selectedRegion.PosX,
                        _selectedRegion.PosY,
                        _selectedRegion.Width,
                        _selectedRegion.Height,
                        _selectedRegion.InactiveUmbral.GetValueOrDefault(), false);
            }

            lbResultOcr.Text = !string.IsNullOrEmpty(ocr.Text) ? ocr.Text : "Sin resultado";
            pbImageDebug.Image = ocr.Image;
        }
    }

    private void btnColor_Click(object sender, EventArgs e)
    {
        SKColor color = _colorDetectionService.GetPixelColor(pbImagenOcr.Image, _selectedRegion.PosX, _selectedRegion.PosY);
        pbColorDebug.BackColor = Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        lbHexadecimal.Text = $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}";
    }

    private async void btnCard_Click(object sender, EventArgs e)
    {
        if (_selectedRegion != null && pbImagenOcr.Image != null)
        {
            var imageToBase64 = _imageCropperService.CropImageToBase64(pbImagenOcr.Image, _selectedRegion.PosX, _selectedRegion.PosY, _selectedRegion.Width, _selectedRegion.Height);

            //_imageCropperService.GuardarEnArchivo(tbCarta.Text, imageToBase64);

            var images = await Task.Run(async () => await _cardUseCases.GetAllCards.ExecuteAsync());

            foreach (var item in images.Value)
            {
                if (!string.IsNullOrEmpty(item.ImageBase64))
                {
                    if (_imageCropperService.CompareCardsBase64(item.ImageBase64, imageToBase64))
                    {
                        pbCartas.Image = _imageCropperService.Base64ToImage(item.ImageBase64);
                        break;
                    }
                }
            }

            tbCarta.Text = string.Empty;
        }
        else
        {
            MessageBox.Show("No se ha seleccionado una región o la imagen es nula.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}


public class WindowInfo
{
    public IntPtr Handle { get; set; }
    public string? Title { get; set; }

    public override string? ToString()
    {
        return Title;
    }
}