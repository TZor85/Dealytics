using Dealytics.Domain.Dtos;
using System.Text.Json;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Dealytics.Features.InitialConfig.Card;

public class GetAllCards
{
    private readonly IFileDialogService _fileDialogService;

    public GetAllCards(IFileDialogService fileDialogService)
    {
        _fileDialogService = fileDialogService;
    }

    public async Task<List<CardDTO>?> ExecuteAsync()
    {
        try
        {
            var fileName = _fileDialogService.ShowOpenFileDialog(
                "Por favor seleccione un archivo JSON",
                "Archivos JSON (*.json)|*.json");

            if (fileName == null) return null;

            if (!fileName.EndsWith("json", StringComparison.OrdinalIgnoreCase))
            {
                _fileDialogService.ShowError("Por favor seleccione un archivo JSON válido", "Error");
                return null;
            }

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                var jsonContent = await reader.ReadToEndAsync();
                var cards = JsonSerializer.Deserialize<List<CardDTO>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return cards ?? new List<CardDTO>();
            }
        }
        catch (Exception ex)
        {
            _fileDialogService.ShowError($"Error al cargar el archivo: {ex.Message}", "Error");
            return null;
        }
    }
}
