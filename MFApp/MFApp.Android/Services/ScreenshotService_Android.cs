using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MFApp.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.Dependency(typeof(MFApp.Droid.Services.ScreenshotService_Android))]
namespace MFApp.Droid.Services
{
    class ScreenshotService_Android : IScreenshotService
    {
        public byte[] GetScreenshot(Xamarin.Forms.View view, Size size)
        {
            try
            {
                if (size == null)
                    size = new Size(view.Width, view.Height);

                var rect = new Rectangle(0, 0, size.Width, size.Height);

                //Converting forms page to native view
                var androidView = ConvertFormsToNative(view, rect);

                // Converting View to BitMap
                var bitmap = ConvertViewToBitMap(androidView, rect);

                byte[] bitmapData;

                using (var stream = new MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                    bitmapData = stream.ToArray();
                }

                return bitmapData;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Android.Views.View ConvertFormsToNative(Xamarin.Forms.View view, Rectangle size)
        {
            var currentRenderer = Platform.GetRenderer(view);

            // Platform.CreateRendererWithContext(view, Android.App.Application.Context) ; //, CrossCurrentActivity.Current.AppContext);

            var newView = currentRenderer.View;
            currentRenderer.Tracker.UpdateLayout();
            var layoutParams = new ViewGroup.LayoutParams((int)size.Width, (int)size.Height);
            newView.LayoutParameters = layoutParams;
            newView.Layout(0, 0, (int)size.Width, (int)size.Height);

            //Platform.SetRenderer(view, currentRenderer);

            return newView;
        }

        private Bitmap ConvertViewToBitMap(Android.Views.View view, Rectangle size)
        {
            Bitmap bitmap = Bitmap.CreateBitmap((int)size.Width, (int)size.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            canvas.DrawColor(Android.Graphics.Color.White);
            view.Draw(canvas);
            return bitmap;
        }
    }
}