using Ardalis.Result;
using Dealytics.App.Maganers;
using Dealytics.App.Monitor;
using Dealytics.App.Overlays;
using Dealytics.App.Services;
using Dealytics.Domain.Dtos;
using Dealytics.Domain.Entities;
using Dealytics.Domain.Enum;
using Dealytics.Domain.Mappers;
using Dealytics.Domain.ValueObjects;
using Dealytics.Features.Action.CreateAll;
using Dealytics.Features.Card;
using Dealytics.Features.Card.CreateAll;
using Dealytics.Features.Regions;
using Dealytics.Features.Regions.CreateAll;
using Dealytics.Features.Table;
using Emgu.CV.Dnn;
using Marten;
using Marten.Linq.MatchesSql;
using PokerVisionAI.App.Services;
using SkiaSharp;
using System.Runtime.InteropServices;
using System.Text;

namespace Dealytics.App;

public partial class FrmMain : Form
{
    private readonly IDocumentStore _store;
    private OverlayManager overlayManager;
    private WindowMonitor monitor;

    #region UseCases
    private RegionUseCases _regionUseCases;
    private CardUseCases _cardUseCases;
    private TableUseCases _actionUseCases;
    #endregion


    private IReadOnlyList<Table> _tables;
    private List<Table> _dataTables;
    private Rectangle currentRectangle;
    private bool shouldDrawRectangle = false;
    private Domain.ValueObjects.Region? _selectedRegion;

    #region DataToLoad
    private List<RegionCategory> _regionsCategories;
    private Result<List<Domain.Dtos.CardDTO>?> _cardsImages;

    #endregion

    private BettingRound _bettingRound;
    private DataRegions _dataRegions = new DataRegions();

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


    private double _umbral = 0;
    private double _inactiveUmbral = 0;


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
        TableUseCases actionUseCases)
    {

        InitializeComponent();
        _store = store;
        overlayManager = new OverlayManager();
        monitor = new WindowMonitor("Hold", 1);
        _windowCaptureService = new WindowCaptureService();

        _regionUseCases = regionUseCases;
        _cardUseCases = cardUseCases;
        _actionUseCases = actionUseCases;

        _bettingRound = new BettingRound(Domain.Enum.BettingRoundType.PreFlop, new List<BettingAction>(), new List<Card>());


        monitor.OnWindowChanged += Monitor_OnWindowChanged;

        Task.FromResult(LoadDataAsync());
    }

    private async Task LoadDataAsync()
    {
        try
        {
            using (var session = _store.LightweightSession())
            {
                _cardsImages = await _cardUseCases.GetAllCards.ExecuteAsync();
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

                //_cardsImages.Value
                var dataCards = await session.Query<Card>().ToListAsync();

                dgvCartas.DataSource = dataCards;
                dgvRegiones.DataSource = regions;
                _regionsCategories = dataRegions.ToList();
                LoadTreeView(dataRegions.ToList());

                _tables = await session.Query<Table>().ToListAsync();
                _dataTables = _tables.ToList();
                LoadTvHands(_dataTables);

                //TODO: Quitar este código
                //LoadNames();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    //TODO: Evento que se ejecuta automáticamente cada vez que se activa una ventana con la palabra clave
    private async void Monitor_OnWindowChanged(object? sender, WindowChangedEventArgs e)
    {
        // Este método se ejecutará cada vez que cambie la ventana activa
        // que contenga la palabra clave
        var handle = ($"Handle: {e.Handle}");

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
        SaveImage(image);
        pbImageGame.Image = image;

        await Task.Run(async () => await ObtainDataTable());
    }



    private void SaveImage(Image image)
    {
        try
        {
            string rutaGuardado = @$"..\..\..\Games\Game_{new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString().Replace("/", "_")}"; // Modifica esta ruta según tus necesidades
            string nombreArchivo = $"game_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string rutaCompleta = Path.Combine(rutaGuardado, nombreArchivo);

            // Asegurarse de que el directorio existe
            Directory.CreateDirectory(rutaGuardado);

            // Guardar la imagen
            if (image != null)
            {
                image.Save(rutaCompleta, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        catch (Exception ex)
        {
            // Manejo de errores
            MessageBox.Show($"Error al guardar la imagen: {ex.Message}");
        }
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
                    await _actionUseCases.CreateAllActions.ExecuteAsync(new CreateAllTablesRequest(actions));
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
        currentDirectory = Path.GetDirectoryName(filePath) ?? string.Empty;
        imageFiles = Directory.GetFiles(currentDirectory, "*.png")
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
        btnPreviousDebug.Enabled = currentImageIndex > 0;
        btnNextDebug.Enabled = currentImageIndex < imageFiles.Length - 1;
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
            pbImageGame.Image = Image.FromFile(filePath);
            pbImageDebug.Image = Image.FromFile(filePath);
        }
    }

    private void LoadImageDebug(string filePath)
    {
        if (File.Exists(filePath))
        {
            if (pbImageDebug.Image != null)
            {
                pbImageDebug.Image.Dispose();
            }
            pbImageDebug.Image = Image.FromFile(filePath);
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

                    if (_selectedRegion.Color != null)
                    {
                        pbColorRegionConfig.BackColor = ColorTranslator.FromHtml(_selectedRegion!.Color);
                        txtColorConfig.Text = (_selectedRegion!.Color).ToUpper();
                    }

                    _umbral = _selectedRegion?.Umbral.GetValueOrDefault() ?? 0f;
                    _inactiveUmbral = _selectedRegion?.InactiveUmbral.GetValueOrDefault() ?? 0;
                    numUmbralActive.Value = (decimal)(_selectedRegion?.Umbral ?? 0);
                    numUmbralInactive.Value = (decimal)(_selectedRegion?.InactiveUmbral ?? 0);

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

    private void txtColorConfig_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string hexColor = txtColorConfig.Text;
            if (!hexColor.StartsWith("#"))
            {
                hexColor = "#" + hexColor;
            }

            if (hexColor.Length == 7) // #RRGGBB
            {
                pbColorRegionConfig.BackColor = ColorTranslator.FromHtml(hexColor);
            }
        }
        catch
        {
            // Si el color es inválido, simplemente no hacemos nada
            // y esperamos a que el usuario termine de escribir
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
        return _regionsCategories.SelectMany(c => c.Regions ?? new List<Domain.ValueObjects.Region>())
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
                        _selectedRegion.Umbral.GetValueOrDefault(),
                        _selectedRegion.IsOnlyNumber.GetValueOrDefault());

            if (string.IsNullOrEmpty(ocr.Text))
            {
                ocr = _ocrService.ExtractTextFromRegionAndDebug(
                        pbImagenOcr.Image,
                        _selectedRegion.PosX,
                        _selectedRegion.PosY,
                        _selectedRegion.Width,
                        _selectedRegion.Height,
                        _selectedRegion.InactiveUmbral.GetValueOrDefault(),
                        _selectedRegion.IsOnlyNumber.GetValueOrDefault());
            }

            lbResultOcr.Text = !string.IsNullOrEmpty(ocr.Text) ? ocr.Text : "Sin resultado";
            pbImageTextDebug.Image = ocr.Image;
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

            if (_cardsImages == null)
                _cardsImages = await Task.Run(async () => await _cardUseCases.GetAllCards.ExecuteAsync());

            if (_cardsImages?.Value != null)
            {
                var maxPorcentaje = 0.0;

                foreach (var item in _cardsImages.Value)
                {

                    if (!string.IsNullOrEmpty(item.ImageBase64))
                    {
                        var pocentaje = _imageCropperService.CompareCardsBase64(item.ImageBase64, imageToBase64);

                        if (pocentaje > maxPorcentaje)
                        {
                            maxPorcentaje = pocentaje;
                            pbCartas.Image = _imageCropperService.Base64ToImage(item.ImageBase64);
                        }
                    }
                }
            }
        }
        else
        {
            MessageBox.Show("No se ha seleccionado una región o la imagen es nula.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static List<WindowInfo> ObtenerVentanasAbiertas()
    {
        List<WindowInfo> ventanas = new List<WindowInfo>();

        EnumWindows((hWnd, lParam) =>
        {
            if (IsWindowVisible(hWnd))
            {
                StringBuilder titulo = new StringBuilder(256);
                GetWindowText(hWnd, titulo, 256);

                if (!string.IsNullOrWhiteSpace(titulo.ToString()))
                {
                    ventanas.Add(new WindowInfo
                    {
                        Handle = hWnd,
                        Title = titulo.ToString()
                    });
                }
            }
            return true;
        }, IntPtr.Zero);

        return ventanas;
    }

    private void btnLoadImageDebug_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog.Title = "Seleccionar Imagen";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadImageDebug(openFileDialog.FileName);
                LoadImagesFromDirectory(openFileDialog.FileName);
            }
        }
    }

    private async Task ObtainDataTable(bool debug = false)
    {
        _dataRegions = new DataRegions();
        LoadNames(debug);
        await LoadBoard(debug);
        LoadDealer(debug);
        LoadBets(debug);
        LoadPlaying(debug);
        LoadEmpty(debug);
        LoadSitout(debug);
        LoadTable(debug);
        await LoadUser(debug);

        if (debug)
        {
            ObtainNames();
            ObtainBoard();
            ObtainDealer();
            ObtainBets();
            ObtainPlaying();
            ObtainEmpty();
            ObtainSitOut();
            ObtainTable();
            ObtainUser();
        }
    }

    #region Debug


    private async void btnDebugManual_Click(object sender, EventArgs e)
    {
        await Task.Run(async () => await ObtainDataTable(true));
    }

    private void btnNamesDebug_Click(object sender, EventArgs e)
    {
        LoadNames(true);
        ObtainNames();
    }

    private async void btnBoardDebug_Click(object sender, EventArgs e)
    {
        await Task.Run(async () => await LoadBoard(true));
        ObtainBoard();
    }

    private void btnDealerDebug_Click(object sender, EventArgs e)
    {
        LoadDealer(true);
        ObtainDealer();
    }

    private void btnBetsDebug_Click(object sender, EventArgs e)
    {
        LoadBets(true);
        ObtainBets();
    }

    private void btnPlayingDebug_Click(object sender, EventArgs e)
    {
        LoadPlaying(true);
        ObtainPlaying();
    }

    private void btnEmptyDebug_Click(object sender, EventArgs e)
    {
        LoadEmpty(true);
        ObtainEmpty();
    }

    private void btnSitOutDebug_Click(object sender, EventArgs e)
    {
        LoadSitout(true);
        ObtainSitOut();
    }

    private void btnTableDebug_Click(object sender, EventArgs e)
    {
        LoadTable(true);
        ObtainTable();
    }
    private async void btnUserDebug_Click(object sender, EventArgs e)
    {
        await Task.Run(async () => await LoadUser(true));
        ObtainUser();
    }

    private void ObtainNames()
    {
        lbP1Name.Text = $"P1: {_dataRegions.Players?[0].Name} ";
        lbP2Name.Text = $"P2: {_dataRegions.Players?[1].Name}";
        lbP3Name.Text = $"P3: {_dataRegions.Players?[2].Name}";
        lbP4Name.Text = $"P4: {_dataRegions.Players?[3].Name}";
        lbP5Name.Text = $"P5: {_dataRegions.Players?[4].Name}";
    }

    private void ObtainBoard()
    {
        if (!string.IsNullOrEmpty(_dataRegions.BoardCards?[0].ImageBase64))
            pbCard1.Image = _imageCropperService.Base64ToImage(_dataRegions.BoardCards?[0].ImageBase64 ?? string.Empty);

        if (!string.IsNullOrEmpty(_dataRegions.BoardCards?[1]?.ImageBase64))
            pbCard2.Image = _imageCropperService.Base64ToImage(_dataRegions.BoardCards?[1].ImageBase64 ?? string.Empty);

        if (!string.IsNullOrEmpty(_dataRegions.BoardCards?[2]?.ImageBase64))
            pbCard3.Image = _imageCropperService.Base64ToImage(_dataRegions.BoardCards?[2].ImageBase64 ?? string.Empty);

        if (!string.IsNullOrEmpty(_dataRegions.BoardCards?[3]?.ImageBase64))
            pbCard4.Image = _imageCropperService.Base64ToImage(_dataRegions.BoardCards?[3].ImageBase64 ?? string.Empty);

        if (!string.IsNullOrEmpty(_dataRegions.BoardCards?[4]?.ImageBase64))
            pbCard5.Image = _imageCropperService.Base64ToImage(_dataRegions.BoardCards?[4].ImageBase64 ?? string.Empty);
    }

    private void ObtainDealer()
    {
        lbDealerDebug.Text = GetActiveDealer() ?? string.Empty;
    }

    private void ObtainBets()
    {
        lbP1Bet.Text = $"P1: {_dataRegions.Players?[0].Bet.ToString()}";
        lbP2Bet.Text = $"P2: {_dataRegions.Players?[1].Bet.ToString()}";
        lbP3Bet.Text = $"P3: {_dataRegions.Players?[2].Bet.ToString()}";
        lbP4Bet.Text = $"P4: {_dataRegions.Players?[3].Bet.ToString()}";
        lbP5Bet.Text = $"P5: {_dataRegions.Players?[4].Bet.ToString()}";
    }

    private void ObtainPlaying()
    {
        lbP1Playing.Text = $"P1: {(_dataRegions.Players?[0].IsPlaying == true ? "Playing" : "Folded")}";
        lbP1Playing.ForeColor = _dataRegions.Players?[0].IsPlaying == true ? Color.Green : Color.Red;

        lbP2Playing.Text = $"P2: {(_dataRegions.Players?[1].IsPlaying == true ? "Playing" : "Folded")}";
        lbP2Playing.ForeColor = _dataRegions.Players?[1].IsPlaying == true ? Color.Green : Color.Red;

        lbP3Playing.Text = $"P3: {(_dataRegions.Players?[2].IsPlaying == true ? "Playing" : "Folded")}";
        lbP3Playing.ForeColor = _dataRegions.Players?[2].IsPlaying == true ? Color.Green : Color.Red;

        lbP4Playing.Text = $"P4: {(_dataRegions.Players?[3].IsPlaying == true ? "Playing" : "Folded")}";
        lbP4Playing.ForeColor = _dataRegions.Players?[3].IsPlaying == true ? Color.Green : Color.Red;


        lbP5Playing.Text = $"P5: {(_dataRegions.Players?[4].IsPlaying == true ? "Playing" : "Folded")}";
        lbP5Playing.ForeColor = _dataRegions.Players?[4].IsPlaying == true ? Color.Green : Color.Red;
    }

    private void ObtainEmpty()
    {
        lbP1Empty.Text = $"P1: {(_dataRegions.Players?[0].IsEmpty == true ? "Empty" : "Not Empty")}";
        lbP1Empty.ForeColor = _dataRegions.Players?[0].IsPlaying == true ? Color.Green : Color.Red;

        lbP2Empty.Text = $"P2: {(_dataRegions.Players?[1].IsPlaying == true ? "Empty" : "Not Empty")}";
        lbP2Empty.ForeColor = _dataRegions.Players?[1].IsPlaying == true ? Color.Green : Color.Red;

        lbP3Empty.Text = $"P3: {(_dataRegions.Players?[2].IsPlaying == true ? "Empty" : "Not Empty")}";
        lbP3Empty.ForeColor = _dataRegions.Players?[2].IsPlaying == true ? Color.Green : Color.Red;

        lbP4Empty.Text = $"P4: {(_dataRegions.Players?[3].IsPlaying == true ? "Empty" : "Not Empty")}";
        lbP4Empty.ForeColor = _dataRegions.Players?[3].IsPlaying == true ? Color.Green : Color.Red;

        lbP5Empty.Text = $"P5: {(_dataRegions.Players?[4].IsPlaying == true ? "Empty" : "Not Empty")}";
        lbP5Empty.ForeColor = _dataRegions.Players?[4].IsPlaying == true ? Color.Green : Color.Red;

    }

    private void ObtainSitOut()
    {
        lbP1Sitout.Text = $"P1: {(_dataRegions.Players?[0].IsSitOut == true ? "SitOut" : "Not SitOut")}";
        lbP1Sitout.ForeColor = _dataRegions.Players?[0].IsSitOut == true ? Color.Green : Color.Red;

        lbP2Sitout.Text = $"P2: {(_dataRegions.Players?[1].IsSitOut == true ? "SitOut" : "Not SitOut")}";
        lbP2Sitout.ForeColor = _dataRegions.Players?[1].IsSitOut == true ? Color.Green : Color.Red;

        lbP3Sitout.Text = $"P3: {(_dataRegions.Players?[2].IsSitOut == true ? "SitOut" : "Not SitOut")}";
        lbP3Sitout.ForeColor = _dataRegions.Players?[2].IsSitOut == true ? Color.Green : Color.Red;

        lbP4Sitout.Text = $"P4: {(_dataRegions.Players?[3].IsSitOut == true ? "SitOut" : "Not SitOut")}";
        lbP4Sitout.ForeColor = _dataRegions.Players?[3].IsSitOut == true ? Color.Green : Color.Red;

        lbP5Sitout.Text = $"P5: {(_dataRegions.Players?[4].IsSitOut == true ? "SitOut" : "Not SitOut")}";
        lbP5Sitout.ForeColor = _dataRegions.Players?[4].IsSitOut == true ? Color.Green : Color.Red;
    }

    private void ObtainTable()
    {
        lbTableName.Text = new string(_dataRegions.Table?.Name.Where(char.IsLetter).ToArray());
        lbTableHand.Text = _dataRegions.Table?.Hand;
        lbPot.Text = _dataRegions.Table?.Pot.ToString();
        lbIsFlop.Text = _dataRegions.Table?.IsFlop == true ? "Flop" : "PreFlop";
    }

    private void ObtainUser()
    {
        lbUserBet.Text = $"Bet: {_dataRegions.User?.Bet.ToString()}";
        lbUserAction.Text = _dataRegions.User?.Action == true ? "Action" : "No Action";
        if (!string.IsNullOrEmpty(_dataRegions.User?.CardFace0.ImageBase64))
            pbUserCard0.Image = _imageCropperService.Base64ToImage(_dataRegions.User.CardFace0.ImageBase64);
        if (!string.IsNullOrEmpty(_dataRegions.User?.CardFace1.ImageBase64))
            pbUserCard1.Image = _imageCropperService.Base64ToImage(_dataRegions.User.CardFace1.ImageBase64);
    }

    private async Task LoadUser(bool debug = false)
    {
        var image = debug ? pbImageDebug.Image : pbImageGame.Image;
        var userCategory = _regionsCategories.FirstOrDefault(f => f.Id == "User");
        if (userCategory?.Regions == null) return;

        var userPropertyMapBet = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "u0bet", text => _dataRegions.User!.Bet = TryParseDecimal(text)}
        };

        foreach (var region in userCategory.Regions)
        {
            var text = ExtractTextWithFallback(image, region);

            if (userPropertyMapBet.TryGetValue(region.Name, out var setter))
            {
                setter(text);
            }
        }

        var userPropertyMapAction = new Dictionary<string, Action<bool>>(StringComparer.OrdinalIgnoreCase)
        {
            { "uAction", action => _dataRegions.User!.Action = action },
            { "dealer", action => _dataRegions.User!.IsDealer = action }
        };

        foreach (var region in userCategory.Regions)
        {
            if (userPropertyMapAction.TryGetValue(region.Name, out var setter))
            {
                var color = _colorDetectionService.GetPixelColor(image, region.PosX, region.PosY);
                var hexColor = FormatColorToHex(color);

                if (string.Equals(region.Color, hexColor, StringComparison.OrdinalIgnoreCase))
                {
                    setter(true);
                }
            }
        }

        var userPropertyMapCards = new Dictionary<string, Action<Card>>(StringComparer.OrdinalIgnoreCase)
        {
            { "u0cardface0", card => _dataRegions.User!.CardFace0 = card },
            { "u0cardface1", card => _dataRegions.User!.CardFace1 = card }
        };

        foreach (var region in userCategory.Regions)
        {
            var croppedImage = _imageCropperService.CropImageToBase64(image, region.PosX, region.PosY, region.Width, region.Height);
            var bestMatch = await FindBestMatchingCard(croppedImage, _cardsImages.Value, debug);

            if (bestMatch != null && userPropertyMapCards.TryGetValue(region.Name, out var setter))
            {
                setter(new Card
                {
                    Id = bestMatch.Name ?? string.Empty,
                    ImageBase64 = bestMatch.ImageBase64 ?? string.Empty,
                    Force = bestMatch.Force,
                    Suit = bestMatch.Suit
                });
            }
        }
    }

    private void LoadSitout(bool debug = false)
    {
        var image = debug ? pbImageDebug.Image : pbImageGame.Image;
        var sitOutCategory = _regionsCategories.FirstOrDefault(f => f.Id == "SitOut");

        if (sitOutCategory?.Regions == null) return;

        // Diccionario para mapear nombres de región a propiedades
        var sitOutPropertyMap = new Dictionary<string, Action<bool>>(StringComparer.OrdinalIgnoreCase)
        {
            { "p1sitout", value => _dataRegions.Players![0].IsSitOut = value },
            { "p2sitout", value => _dataRegions.Players![1].IsSitOut = value },
            { "p3sitout", value => _dataRegions.Players![2].IsSitOut = value },
            { "p4sitout", value => _dataRegions.Players![3].IsSitOut = value },
            { "p5sitout", value => _dataRegions.Players![4].IsSitOut = value }
        };

        foreach (var region in sitOutCategory.Regions)
        {
            var text = ExtractTextWithFallback(image, region);
            var isSitOut = !string.IsNullOrEmpty(text) && text.Contains("sit", StringComparison.OrdinalIgnoreCase);

            if (sitOutPropertyMap.TryGetValue(region.Name, out var setter))
            {
                setter(isSitOut);
            }
        }
    }

    private void LoadEmpty(bool debug = false)
    {
        var image = debug ? pbImageDebug.Image : pbImageGame.Image;
        ResetEmptyFlags();

        var emptyCategory = _regionsCategories.FirstOrDefault(f => f.Id == "Empty");
        if (emptyCategory?.Regions == null) return;

        var emptyPropertyMap = new Dictionary<string, Action<bool>>(StringComparer.OrdinalIgnoreCase)
        {
            { "p1empty", value => _dataRegions.Players![0].IsEmpty = value },
            { "p2empty", value => _dataRegions.Players![1].IsEmpty = value },
            { "p3empty", value => _dataRegions.Players![2].IsEmpty = value },
            { "p4empty", value => _dataRegions.Players![3].IsEmpty = value },
            { "p5empty", value => _dataRegions.Players![4].IsEmpty = value }
        };

        foreach (var region in emptyCategory.Regions)
        {
            if (emptyPropertyMap.TryGetValue(region.Name, out var setter))
            {
                var color = _colorDetectionService.GetPixelColor(image, region.PosX, region.PosY);
                var hexColor = FormatColorToHex(color);

                if (string.Equals(region.Color, hexColor, StringComparison.OrdinalIgnoreCase))
                {
                    setter(true);
                }
            }
        }
    }

    private void LoadPlaying(bool debug = false)
    {
        var image = debug ? pbImageDebug.Image : pbImageGame.Image;
        //ResetDealerFlags();

        var dealerCategory = _regionsCategories.FirstOrDefault(f => f.Id == "Playing");
        if (dealerCategory?.Regions == null) return;

        var dealerPropertyMap = new Dictionary<string, Action<bool>>(StringComparer.OrdinalIgnoreCase)
        {
            { "p1playing", value => _dataRegions.Players![0].IsPlaying = value },
            { "p2playing", value => _dataRegions.Players![1].IsPlaying = value },
            { "p3playing", value => _dataRegions.Players![2].IsPlaying = value },
            { "p4playing", value => _dataRegions.Players![3].IsPlaying = value },
            { "p5playing", value => _dataRegions.Players![4].IsPlaying = value }
        };

        foreach (var region in dealerCategory.Regions)
        {
            if (dealerPropertyMap.TryGetValue(region.Name, out var setter))
            {
                var color = _colorDetectionService.GetPixelColor(image, region.PosX, region.PosY);
                var hexColor = FormatColorToHex(color);

                if (string.Equals(region.Color, hexColor, StringComparison.OrdinalIgnoreCase))
                {
                    setter(true);
                }
            }
        }
    }

    private void LoadBets(bool debug = false)
    {
        var image = debug ? pbImageDebug.Image : pbImageGame.Image;
        var namesCategory = _regionsCategories.FirstOrDefault(f => f.Id == "Bets");

        if (namesCategory?.Regions == null) return;

        // Diccionario para mapear nombres de región a propiedades
        var namePropertyMap = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "p1bet", text => _dataRegions.Players![0].Bet = TryParseDecimal(text) },
            { "p2bet", text => _dataRegions.Players![1].Bet = TryParseDecimal(text) },
            { "p3bet", text => _dataRegions.Players![2].Bet = TryParseDecimal(text) },
            { "p4bet", text => _dataRegions.Players![3].Bet = TryParseDecimal(text) },
            { "p5bet", text => _dataRegions.Players![4].Bet = TryParseDecimal(text) }
        };

        foreach (var region in namesCategory.Regions)
        {
            var text = ExtractTextWithFallback(image, region);

            if (namePropertyMap.TryGetValue(region.Name, out var setter))
            {
                setter(text);
            }
        }
    }

    private void LoadDealer(bool debug = false)
    {
        var image = debug ? pbImageDebug.Image : pbImageGame.Image;
        //ResetDealerFlags();

        var dealerCategory = _regionsCategories.FirstOrDefault(f => f.Id == "Dealer");
        if (dealerCategory?.Regions == null) return;

        var dealerPropertyMap = new Dictionary<string, Action<bool>>(StringComparer.OrdinalIgnoreCase)
        {
            { "p1dealer", value => _dataRegions.Players![0].IsDealer = value },
            { "p2dealer", value => _dataRegions.Players![1].IsDealer = value },
            { "p3dealer", value => _dataRegions.Players![2].IsDealer = value },
            { "p4dealer", value => _dataRegions.Players![3].IsDealer = value },
            { "p5dealer", value => _dataRegions.Players![4].IsDealer = value }
        };

        foreach (var region in dealerCategory.Regions)
        {
            if (dealerPropertyMap.TryGetValue(region.Name, out var setter))
            {
                var color = _colorDetectionService.GetPixelColor(image, region.PosX, region.PosY);
                var hexColor = FormatColorToHex(color);

                if (string.Equals(region.Color, hexColor, StringComparison.OrdinalIgnoreCase))
                {
                    setter(true);
                }
            }
        }
    }

    private void LoadNames(bool debug = false)
    {
        var image = debug ? pbImageDebug.Image : pbImageGame.Image;
        var namesCategory = _regionsCategories.FirstOrDefault(f => f.Id == "Names");

        if (namesCategory?.Regions == null) return;

        // Diccionario para mapear nombres de región a propiedades
        var namePropertyMap = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "p1name", text => _dataRegions.Players![0].Name = text },
            { "p2name", text => _dataRegions.Players![1].Name = text },
            { "p3name", text => _dataRegions.Players![2].Name = text },
            { "p4name", text => _dataRegions.Players![3].Name = text },
            { "p5name", text => _dataRegions.Players![4].Name = text }
        };

        foreach (var region in namesCategory.Regions)
        {
            var text = ExtractTextWithFallback(image, region);

            if (namePropertyMap.TryGetValue(region.Name, out var setter))
            {
                setter(text);
            }
        }
    }

    private async Task LoadBoard(bool debug = false)
    {
        var image = debug ? pbImageDebug.Image : pbImageGame.Image;
        if (image == null)
        {
            MessageBox.Show("No se ha seleccionado una región o la imagen es nula.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Cargar cartas una sola vez si es necesario
        if (_cardsImages == null)
        {
            _cardsImages = await _cardUseCases.GetAllCards.ExecuteAsync();
            if (_cardsImages?.Value == null) return;
        }

        var boardCategory = _regionsCategories.FirstOrDefault(w => w.Id == "Board");
        if (boardCategory?.Regions == null) return;

        // Diccionario para mapear nombres de regiones a propiedades de _dataRegions
        var cardPropertyMap = new Dictionary<string, Action<Card>>(StringComparer.OrdinalIgnoreCase)
        {
            { "card1", card => _dataRegions.BoardCards![0] = new Board { Id = card.Id, BinaryValue = card.BinaryValue, Force = card.Force, ImageBase64 = card.ImageBase64, Suit = card.Suit, Hall = card.Hall, BettingRoundType = BettingRoundType.Flop } },
            { "card2", card => _dataRegions.BoardCards![1] = new Board { Id = card.Id, BinaryValue = card.BinaryValue, Force = card.Force, ImageBase64 = card.ImageBase64, Suit = card.Suit, Hall = card.Hall, BettingRoundType = BettingRoundType.Flop } },
            { "card3", card => _dataRegions.BoardCards![2] = new Board { Id = card.Id, BinaryValue = card.BinaryValue, Force = card.Force, ImageBase64 = card.ImageBase64, Suit = card.Suit, Hall = card.Hall, BettingRoundType = BettingRoundType.Flop } },
            { "card4", card => _dataRegions.BoardCards![3] = new Board { Id = card.Id, BinaryValue = card.BinaryValue, Force = card.Force, ImageBase64 = card.ImageBase64, Suit = card.Suit, Hall = card.Hall, BettingRoundType = BettingRoundType.Turn } },
            { "card5", card => _dataRegions.BoardCards![4] = new Board { Id = card.Id, BinaryValue = card.BinaryValue, Force = card.Force, ImageBase64 = card.ImageBase64, Suit = card.Suit, Hall = card.Hall, BettingRoundType = BettingRoundType.River } }
        };

        // Procesar cada región del tablero
        foreach (var region in boardCategory.Regions)
        {
            var croppedImage = _imageCropperService.CropImageToBase64(image, region.PosX, region.PosY, region.Width, region.Height);
            var bestMatch = await FindBestMatchingCard(croppedImage, _cardsImages.Value, debug);

            if (bestMatch != null && cardPropertyMap.TryGetValue(region.Name, out var setter))
            {
                setter(new Card
                {
                    Id = bestMatch.Name ?? string.Empty,
                    ImageBase64 = bestMatch.ImageBase64 ?? string.Empty,
                    Force = bestMatch.Force,
                    Suit = bestMatch.Suit
                });
            }
        }
    }

    private void LoadTable(bool debug = false)
    {
        var image = debug ? pbImageDebug.Image : pbImageGame.Image;
        var namesCategory = _regionsCategories.FirstOrDefault(f => f.Id == "Table");

        if (namesCategory?.Regions == null) return;

        // Diccionario para mapear nombres de región a propiedades
        var namePropertyMap = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "tablehand", text => _dataRegions.Table!.Hand = text },
            { "tablename", text => _dataRegions.Table!.Name = text },
            { "pot", text => _dataRegions.Table!.Pot = TryParseDecimal(text) }
        };

        var namePropertyMapFlop = new Dictionary<string, Action<bool>>(StringComparer.OrdinalIgnoreCase)
        {
            { "isflop", value => _dataRegions.Table!.IsFlop = value },
        };

        foreach (var region in namesCategory.Regions)
        {
            var text = ExtractTextWithFallback(image, region);

            if (namePropertyMap.TryGetValue(region.Name, out var setter))
            {
                setter(text);
            }
        }

        foreach (var region in namesCategory.Regions)
        {
            if (namePropertyMapFlop.TryGetValue(region.Name, out var setter))
            {
                var color = _colorDetectionService.GetPixelColor(image, region.PosX, region.PosY);
                var hexColor = FormatColorToHex(color);

                if (string.Equals(region.Color, hexColor, StringComparison.OrdinalIgnoreCase))
                {
                    setter(true);
                }
            }
        }

    }

    private void ResetEmptyFlags()
    {
        _dataRegions.Players![0].IsEmpty = false;
        _dataRegions.Players![1].IsEmpty = false;
        _dataRegions.Players![2].IsEmpty = false;
        _dataRegions.Players![3].IsEmpty = false;
        _dataRegions.Players![4].IsEmpty = false;
    }

    private static string FormatColorToHex(SKColor color)
    {
        return $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}".ToLower();
    }

    private string ExtractTextWithFallback(Image image, Domain.ValueObjects.Region region)
    {
        var primaryOcr = _ocrService.ExtractTextFromRegionAndDebug(
            image,
            region.PosX,
            region.PosY,
            region.Width,
            region.Height,
            region.Umbral ?? 0,
            region.IsOnlyNumber ?? false);

        if (!string.IsNullOrEmpty(primaryOcr.Text))
        {
            return primaryOcr.Text;
        }

        var fallbackOcr = _ocrService.ExtractTextFromRegionAndDebug(
            image,
            region.PosX,
            region.PosY,
            region.Width,
            region.Height,
            region.InactiveUmbral ?? 0,
            region.IsOnlyNumber ?? false);

        return fallbackOcr.Text;
    }

    

    private async Task<CardDTO?> FindBestMatchingCard(string imageToCompare, IEnumerable<CardDTO> cards, bool debug)
    {
        CardDTO? bestMatch = null;
        double maxPercentage = 0;

        // Paralelizar la comparación de imágenes
        var comparisons = cards
            .Where(card => !string.IsNullOrEmpty(card.ImageBase64))
            .AsParallel()
            .Select(card =>
            {
                var percentage = _imageCropperService.CompareCardsBase64(card.ImageBase64, imageToCompare);
                return (card, percentage);
            });

        foreach (var (card, percentage) in comparisons)
        {
            if (percentage > maxPercentage)
            {
                maxPercentage = percentage;
                bestMatch = card;

                if (!debug && bestMatch.ImageBase64 != null)
                {
                    UpdatePreviewImage(bestMatch.ImageBase64);
                }
            }
        }

        return bestMatch;
    }

    private void UpdatePreviewImage(string base64Image)
    {
        // Actualizar la UI en el thread correcto
        if (pbCartas.InvokeRequired)
        {
            pbCartas.BeginInvoke(() => pbCartas.Image = _imageCropperService.Base64ToImage(base64Image));
        }
        else
        {
            pbCartas.Image = _imageCropperService.Base64ToImage(base64Image);
        }
    }

    #endregion


    private string GetActiveDealer()
    {
        var dealers = new[]
        {
            (Value: _dataRegions.User!.IsDealer, Name: "Hero"),
            (Value: _dataRegions.Players![0].IsDealer, Name: "P1"),
            (Value: _dataRegions.Players![1].IsDealer, Name: "P2"),
            (Value: _dataRegions.Players![2].IsDealer, Name: "P3"),
            (Value: _dataRegions.Players![3].IsDealer, Name: "P4"),
            (Value: _dataRegions.Players![4].IsDealer, Name: "P5")
        };

        return dealers.FirstOrDefault(d => d.Value).Name ?? string.Empty;
    }

    private decimal TryParseDecimal(string input)
    {
        if (decimal.TryParse(input, out var result))
        {
            return result;
        }
        return 0; // Default value if parsing fails
    }

    private void btnImageDebug_Click(object sender, EventArgs e)
    {
        var image = _windowCaptureService.CaptureWindow(activeOverlays.FirstOrDefault().Key);
        SaveImage(image);
    }

    private void numUmbralActive_ValueChanged(object sender, EventArgs e)
    {
        _umbral = (double)numUmbralActive.Value;

        if (_selectedRegion != null)
            _selectedRegion.Umbral = _umbral;
    }

    private void numUmbralInactive_ValueChanged(object sender, EventArgs e)
    {
        _inactiveUmbral = (double)numUmbralInactive.Value;

        if (_selectedRegion != null)
            _selectedRegion.InactiveUmbral = _inactiveUmbral;
    }

    private async void btnUpdateConfig_Click(object sender, EventArgs e)
    {
        if (_selectedRegion != null)
        {
            _selectedRegion.PosX = (int)numPosX.Value;
            _selectedRegion.PosY = (int)numPosY.Value;
            _selectedRegion.Width = (int)numWidth.Value;
            _selectedRegion.Height = (int)numHeight.Value;
            _selectedRegion.Umbral = _umbral;
            _selectedRegion.InactiveUmbral = _inactiveUmbral;
            _selectedRegion.Color = txtColorConfig.Text;

            await _regionUseCases.UpdateRegion.ExecuteAsync(
                new Features.Regions.Update.UpdateRegionRequest(
                    _selectedRegion?.Category ?? string.Empty,
                    _selectedRegion?.Name ?? string.Empty,
                    (int)numPosX.Value,
                    (int)numPosY.Value,
                    (int)numWidth.Value,
                    (int)numHeight.Value,
                    _umbral,
                    _inactiveUmbral,
                    txtColorConfig.Text));
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