namespace Dealytics.App.Helper;

public static class ImageHelper
{
    public static Color GetPixelColor(Bitmap image, int posX, int posY)
    {
        return image.GetPixel(posX, posY);
    }
}
