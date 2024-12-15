using Dealytics.Features;

namespace Dealytics.App.Services;

public class WindowsFileDialogService : IFileDialogService
{
    public string? ShowOpenFileDialog(string title, string filter)
    {
        if (Application.OpenForms.Count > 0 && Application.OpenForms[0]?.InvokeRequired == true)
        {
            string? result = null;
            Application.OpenForms[0]?.Invoke(new Action(() =>
            {
                result = ShowOpenFileDialogInternal(title, filter);
            }));
            return result;
        }
        return ShowOpenFileDialogInternal(title, filter);
    }

    private string? ShowOpenFileDialogInternal(string title, string filter)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Title = title;
            openFileDialog.Filter = filter;
            return openFileDialog.ShowDialog() == DialogResult.OK
                ? openFileDialog.FileName
                : null;
        }
    }

    public void ShowError(string message, string title)
    {
        if (Application.OpenForms.Count > 0 && Application.OpenForms[0]?.InvokeRequired == true)
        {
            Application.OpenForms[0]?.Invoke(new Action(() =>
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
            ));
            return;
        }
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
