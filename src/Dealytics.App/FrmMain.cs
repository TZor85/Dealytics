using Dealytics.App.Maganers;
using Dealytics.App.Monitor;
using Dealytics.App.Overlays;
using Dealytics.App.Services;
using Dealytics.Domain.Entities;
using Dealytics.Domain.Mappers;
using Dealytics.Features.Action;
using Dealytics.Features.Action.Create;
using Dealytics.Features.Action.CreateAll;
using Dealytics.Features.Card;
using Dealytics.Features.Card.CreateAll;
using Dealytics.Features.InitialConfig.Card;
using Dealytics.Features.InitialConfig.Region;
using Dealytics.Features.Regions;
using Dealytics.Features.Regions.CreateAll;
using Dealytics.Features.Regions.Update;
using Marten;
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
    private ActionUseCases _actionUseCases;
    #endregion

    private List<RegionCategory> _regionCateory;
    private Rectangle currentRectangle;
    private bool shouldDrawRectangle = false;
    private WindowCaptureService windowCaptureService;

    private Dictionary<nint, FrmExternalWindowOverlay> activeOverlays = new Dictionary<nint, FrmExternalWindowOverlay>();

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public FrmMain(IDocumentStore store, RegionUseCases regionUseCases, CardUseCases cardUseCases, ActionUseCases actionUseCases)
    {
        InitializeComponent();
        _store = store;
        overlayManager = new OverlayManager();
        monitor = new WindowMonitor("poker", 1);
        windowCaptureService = new WindowCaptureService();
        
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

                var dataActions = await session.Query<Domain.Entities.Action>().ToListAsync();
                LoadTvHands(dataActions.ToList());

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
        var image = windowCaptureService.CaptureWindow(e.Handle);
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

    private async void btnCards_Click(object sender, EventArgs e)
    {
        try
        {
            var cards = new List<Card>();
            var cardsDto = await Task.Run(async () => await _cardUseCases.GetAllCards.ExecuteAsync());

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
            var actions = new List<Domain.Entities.Action>();
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

    //TODO: Método que carga las manos en el TreeView
    private void LoadTvHands(List<Domain.Entities.Action> actions)
    {
        tvTables.Nodes.Clear();

        foreach (var action in actions)
        { 
            
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
                pbImagenOcr.Image = Image.FromFile(openFileDialog.FileName);
            }
        }
    }

    private void tvRegions_DoubleClick(object sender, EventArgs e)
    {
        if (sender is TreeView treeView && treeView.SelectedNode != null) // Si es un nodo hijo (región)
        {
            // Obtener la región correspondiente
            var region = ObtenerRegionDelNodo(treeView.SelectedNode);
            numPosX.Value = region.PosX;
            numPosY.Value = region.PosY;
            numWidth.Value = region.Width;
            numHeight.Value = region.Height;

            // Calcular escala preservando el aspect ratio de la imagen
            float ratio = Math.Min(
                (float)pbImagenOcr.Width / pbImagenOcr.Image.Width,
                (float)pbImagenOcr.Height / pbImagenOcr.Image.Height
            );

            // Actualizar el rectángulo actual
            currentRectangle = new Rectangle(
                (int)(region.PosX * ratio),
                (int)(region.PosY * ratio),
                (int)(region.Width * ratio),
                (int)(region.Height * ratio)
            );

            shouldDrawRectangle = true;
            pbImagenOcr.Invalidate(); // Forzar redibujado
        }
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

    private async void btnUpdateRegion_Click(object sender, EventArgs e)
    {
        if (tvRegions.SelectedNode?.Parent != null)
        {
            await Task.Run(async () => await _regionUseCases.UpdateRegion.ExecuteAsync(new UpdateRegionRequest(
                tvRegions.SelectedNode.Parent.Text,
                tvRegions.SelectedNode.Text,
                (int)numPosX.Value,
                (int)numPosY.Value,
                (int)numWidth.Value,
                (int)numHeight.Value)));
        }
        else
        {
            lbMessageRegions.Text = "Seleccione una región válida para actualizar.";
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