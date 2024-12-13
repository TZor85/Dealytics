using Dealytics.Domain.Dtos;
using System.Text.Json;

namespace Dealytics.Features.InitialConfig.Action
{
    public class GetAllActions
    {
        private readonly IFileDialogService _fileDialogService;

        public GetAllActions(IFileDialogService fileDialogService)
        {
            _fileDialogService = fileDialogService;
        }

        public async Task<ActionDTO?>? ExecuteAsync()
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
                    var actions = JsonSerializer.Deserialize<ActionDTO>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return actions ?? new ActionDTO { Name = string.Empty };
                }
            }
            catch (Exception ex)
            {
                _fileDialogService.ShowError($"Error al cargar el archivo: {ex.Message}", "Error");
                return null;
            }
        }
    }
}
