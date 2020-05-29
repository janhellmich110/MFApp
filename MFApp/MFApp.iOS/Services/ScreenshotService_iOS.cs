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
        public byte[] GetScreenshot(View view, Size size)
        {
            try
            {
                if (size == null)
                {
                    size = new Size(view.Width, view.Height);
                }

                var rect = new CGRect(0, 0, size.Width, size.Height);
                //Converting forms page to native view
                var iOSView = ConvertFormsToNative(view, rect);

                // Converting View to UIImage
                UIImage uiImage = ConvertViewToImage(iOSView, rect);

                byte[] bitmapData;

                var png = uiImage.AsPNG();
                bitmapData = png.ToArray();

                UIGraphics.EndImageContext();

                return bitmapData;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static UIView ConvertFormsToNative(Xamarin.Forms.View view, CGRect size)
        {
            try
            {
                var renderer = Platform.CreateRenderer(view);

                renderer.NativeView.Frame = size;

                renderer.NativeView.AutoresizingMask = UIViewAutoresizing.All;
                renderer.NativeView.ContentMode = UIViewContentMode.ScaleToFill;

                renderer.Element.Layout(size.ToRectangle());

                var nativeView = renderer.NativeView;

                nativeView.SetNeedsLayout();

                return nativeView;
            }
            catch (Exception)
            {
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
                return image;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}