using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MFApp.Services;
using MFApp.Models;

namespace MFApp.Views
{
    [DesignTimeVisible(false)]
    public partial class LoginPage : ContentPage
    {
        public LoginPageData LoginPageData { get; set; }

        IDataStore<Player> DataStore = DependencyService.Get<IDataStore<Player>>();

        public LoginPage()
        {
            InitializeComponent();

            LoginPageData = new LoginPageData
            {
                UserName = "",
                Password = "",
                Message = ""
            };

            BindingContext = this;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Label errorMessage = (Label)this.FindByName("errorMessage");

            var players = DataStore.GetItemsAsync().GetAwaiter().GetResult();
            bool playerExists = false;
            Player currentPlayer = null;
            foreach (Player p in players)
            {
                if (p.UserName.ToLower() == LoginPageData.UserName.ToLower())
                {
                    if (p.UserPassword == LoginPageData.Password)
                    {
                        currentPlayer = p;
                        playerExists = true;
                        break;
                    }
                    else
                    {
                        errorMessage.Text = "Password ist Falsch";
                    }
                }
            }

            if (playerExists)
            {
                IDataStore<Profile> DataStoreProfile = DependencyService.Get<IDataStore<Profile>>();
                Profile p = new Profile();
                p.UserName = currentPlayer.UserName;
                p.Id = currentPlayer.Id;
                p.Initials = currentPlayer.Initials;
                p.Name = currentPlayer.Name;
                p.Handicap = currentPlayer.Handicap;
                p.Birthday = currentPlayer.Birthday;
                p.Gender = currentPlayer.Gender;
                p.Mail = currentPlayer.Mail;
                DataStoreProfile.AddItemAsync(p);

                Navigation.PopModalAsync();
            }
            else
            {       
                if(errorMessage.Text == "")
                    errorMessage.Text = "Benutzername ist Falsch";
            }
        }
    }
}