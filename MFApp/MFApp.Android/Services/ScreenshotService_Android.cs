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
        public byte[] GetScreenshot(Xamarin.Forms.View[] views)
        {
            try
            {
                if (views == null || views?.Count() == 0)
                    return null;

                // Get each Picture from the given views
                List<Bitmap> bitmaps = new List<Bitmap>();
                foreach (var view in views)
                {
                    var bitmap = ConvertFormsToBitmap(view);
                    bitmaps.Add(bitmap);
                }

                //calc the overall pictures size
                List<IVisualElementRenderer> renderers = new List<IVisualElementRenderer>();
                int width = 0;
                int height = 0;
                for (int i = 0; i < views.Length; i++)
                {
                    renderers.Add(Platform.GetRenderer(views[i]));

                    if (i == 0)
                        width += renderers.Last().View.Width;

                    height += renderers[i].View.Height;
                }

                byte[] bitmapData = null;
                int viewCount = 0;

                int oldViewHeight = 0;

                //create a big picture containing all pictures underneath each other
                using (var bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888))
                {
                    using (var canvas = new Canvas(bitmap))
                    {
                        //use canvas to store pictures in it
                        foreach (var renderer in renderers)
                        {
                            canvas.DrawBitmap(bitmaps[viewCount],
                                0,
                                oldViewHeight,
                                null);

                            oldViewHeight += renderer.View.Height;
                            viewCount++;
                        }
                        canvas.Save();

                        canvas.SetBitmap(bitmap);
                    }
                    // create usable byte[] of the created picture
                    using (var stream = new MemoryStream())
                    {
                        bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                        bitmapData = stream.ToArray();
                    }
                }

                return bitmapData;

            }
            catch (Exception ex)
            {
                //CrashTracker.Track(ex);
                return null;
            }
        }

        private Bitmap ConvertFormsToBitmap(Xamarin.Forms.View view)
        {
            //Converting forms page to native view
            var androidView = ConvertFormsToNative(view);

            // Converting View to BitMap
            var bitmap = ConvertViewToBitMap(androidView);

            return bitmap;
        }

        private Android.Views.View ConvertFormsToNative(Xamarin.Forms.View view)
        {
            var currentRenderer = Platform.GetRenderer(view);

            var newView = currentRenderer.View;
            currentRenderer.Tracker.UpdateLayout();
            var layoutParams = new ViewGroup.LayoutParams((int)currentRenderer.View.Width, (int)currentRenderer.View.Height);
            newView.LayoutParameters = layoutParams;
            newView.Layout(0, 0, (int)currentRenderer.View.Width, (int)currentRenderer.View.Height);

            return newView;
        }

        private Bitmap ConvertViewToBitMap(Android.Views.View view)
        {
            Bitmap bitmap = Bitmap.CreateBitmap((int)view.Width, (int)view.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            canvas.DrawColor(Android.Graphics.Color.White);
            view.Draw(canvas);
            return bitmap;
        }
    }
}