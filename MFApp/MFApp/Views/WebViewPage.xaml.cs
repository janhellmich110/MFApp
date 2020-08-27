using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MFApp.ViewModels;
using MFApp.Models;

namespace MFApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewPage : ContentPage
    {
        public UrlWebViewSource WebSource { get; set; }

        public WebViewPage()
        {
            InitializeComponent();
        }

        public WebViewPage(string PageTitle, string Url)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(Url))
            {
                Title = PageTitle;

                var url = Url.StartsWith("https") ? Url : $"https://{Url}";

                webView.Source = new UrlWebViewSource()
                {
                    Url = url
                };
            }

        }

        public WebViewPage(HomePageEvent webEvent)
        {
            InitializeComponent();

            if (webEvent != null)
            {
                Title = webEvent.EventTitle;

                switch (webEvent.EventType)
                {
                    case EventType.Notification:
                        if (!string.IsNullOrEmpty(webEvent.Url))
                        {
                            var url = webEvent.Url.StartsWith("https") ? webEvent.Url : $"https://{webEvent.Url}";

                            webView.Source = new UrlWebViewSource()
                            {
                                Url = url
                            };
                        }
                        break;
                    case EventType.Tournament:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}