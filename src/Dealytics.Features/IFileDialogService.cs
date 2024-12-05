namespace Dealytics.Features;

public interface IFileDialogService
{
    string? ShowOpenFileDialog(string title, string filter);
    void ShowError(string message, string title);
}
