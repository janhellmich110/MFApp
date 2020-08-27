using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MFApp.Services;
using MFApp.Models;
using Xamarin.Essentials;
using System.Collections.Generic;
using MFApp.ViewModels;

namespace MFApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void PhoneNumer_Tapped(object sender, EventArgs e)
        {
            // get phone numer
            Label lbl = sender as Label;
            try
            {
                PhoneDialer.Open(lbl.Text);
            }
            catch (ArgumentNullException anEx)
            {
                CrashTracker.Track(anEx);
                // Number was null or white space
            }
            catch (FeatureNotSupportedException ex)
            {
                CrashTracker.Track(ex);
                // Phone Dialer is not supported on this device.
            }
            catch (Exception ex)
            {
                CrashTracker.Track(ex);
                // Other error has occurred.
            }
        }

        private async void SendMail_Tapped(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            List<string> toList = new List<string>();
            toList.Add(lbl.Text);
            try
            {
                var message = new EmailMessage
                {
                    Subject = "Nachricht aus Golfclub App",
                    Body = "",
                    To = toList
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                CrashTracker.Track(fbsEx);
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                CrashTracker.Track(ex);
                // Some other exception occurred
            }
        }

        private async void Web_Tapped(object sender, EventArgs e)
        {
            Label lbl = sender as Label;

            var homepageEvent = new HomePageEvent()
            {
                Url = "https://www.golfclub-sittensen.de/startseite.html",
                EventTitle = "Hompage GCKS",
                EventType = EventType.Notification
            };

            if (!string.IsNullOrEmpty(homepageEvent.Url))
                await Navigation.PushAsync(new WebViewPage(homepageEvent));
        }
    }
}