using Xamarin.Forms;

namespace MFApp.Interfaces
{
    public interface IScreenshotService
    {
        byte[] GetScreenshot(Xamarin.Forms.View view, Size size);
    }
}
