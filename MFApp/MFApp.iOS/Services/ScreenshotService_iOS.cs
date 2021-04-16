using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using MFApp.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(MFApp.iOS.Services.ScreenshotService_iOS))]
namespace MFApp.iOS.Services
{
    class ScreenshotService_iOS : IScreenshotService
    {
        public byte[] GetScreenshot(View[] views)
        {
            try
            {
                if (views == null || views?.Count() == 0)
                    return null;

                // Get each Picture from the given views
                List<UIImage> images = new List<UIImage>();
                foreach (var view in views)
                {
                    var image = ConvertFormsToUIImage(view);
                    images.Add(image);
                }

                //calc the overall pictures size
                List<IVisualElementRenderer> renderers = new List<IVisualElementRenderer>();
                int width = 0;
                int height = 0;
                for (int i = 0; i < views.Length; i++)
                {
                    renderers.Add(Platform.GetRenderer(views[i]));
                    if (i == 0)
                        width += Convert.ToInt32(renderers.Last().Element.Width);

                    height += Convert.ToInt32(renderers.Last().Element.Height);
                }


                CGSize s = new CGSize(width, height);


                byte[] bitmapData;
                nfloat oldViewHeight = 0;
                //create a big picture containing all pictures underneath each other
                using (var context = UIGraphics.GetCurrentContext())
                {
                    UIGraphics.BeginImageContext(s);
                    foreach (var image in images)
                    {
                        image.Draw(new CGRect(0, oldViewHeight, image.Size.Width, image.Size.Height));
                        oldViewHeight = image.Size.Height;
                    }

                    UIImage uIImage = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();

                    var nsData = uIImage.AsPNG();
                    bitmapData = nsData.ToArray();
                }

                return bitmapData;
            }
            catch (Exception ex)
            {
                //CrashTracker.Track(ex);
                return null;
            }
        }

        private UIImage ConvertFormsToUIImage(View view)
        {
            var rect = new CGRect(0, 0, view.Width, view.Height);
            //Converting forms page to native view
            var iOSView = ConvertFormsToNative(view, rect);

            // Converting View to UIImage
            UIImage uiImage = ConvertViewToImage(iOSView, rect);

            return uiImage;
        }

        private UIView ConvertFormsToNative(Xamarin.Forms.View view, CGRect size)
        {
            try
            {
                var renderer = Platform.GetRenderer(view);

                renderer.NativeView.Frame = size;

                renderer.NativeView.AutoresizingMask = UIViewAutoresizing.All;
                renderer.NativeView.ContentMode = UIViewContentMode.ScaleToFill;

                renderer.Element.Layout(size.ToRectangle());

                var nativeView = renderer.NativeView;

                nativeView.SetNeedsLayout();

                return nativeView;
            }
            catch (Exception e)
            {
                //CrashTracker.Track(e);
                return null;
            }

        }

        private UIImage ConvertViewToImage(UIView iOSView, CGRect size)
        {
            try
            {
                CGSize s = new CGSize(size.Width, size.Height);
                UIGraphics.BeginImageContext(s);
                iOSView.Layer.RenderInContext(UIGraphics.GetCurrentContext());
                UIImage image = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
                return image;
            }
            catch (Exception e)
            {
                //CrashTracker.Track(e);
                return null;
            }
        }
    }
}