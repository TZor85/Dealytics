using Dealytics.App.Overlays;

namespace Dealytics.App.Maganers;

public class OverlayManager
{
    private List<ExternalWindowOverlay> overlays;

    public OverlayManager()
    {
        overlays = new List<ExternalWindowOverlay>();
    }

    public void AddOverlay(string windowTitle)
    {
        //System.Threading.Thread.Sleep(2000);
        var overlay = new ExternalWindowOverlay(windowTitle);
        if (overlay.IsTargetWindowValid())
        {
            overlays.Add(overlay);
            overlay.Show();
        }
    }

    public void RemoveOverlay(string windowTitle)
    {
        var overlay = overlays.Find(o => o.WindowTitle == windowTitle);
        if (overlay != null)
        {
            overlay.Close();
            overlays.Remove(overlay);
        }
    }

    public void CloseAllOverlays()
    {
        foreach (var overlay in overlays)
        {
            overlay.Close();
        }
        overlays.Clear();
    }
}
