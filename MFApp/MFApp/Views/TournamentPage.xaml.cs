using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MFApp.Models;

namespace MFApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TournamentPage : TabbedPage
    {
        public TournamentPageData TournamentPageData { get; set; }

        public TournamentPage(TournamentPageData t)
        {
            InitializeComponent();
            BindingContext = this.TournamentPageData = t;

            ScoreKarte.RowDefinitions.Add(new RowDefinition());
            ScoreKarte.RowDefinitions.Add(new RowDefinition());
            ScoreKarte.RowDefinitions.Add(new RowDefinition());
            ScoreKarte.RowDefinitions.Add(new RowDefinition());
            ScoreKarte.RowDefinitions.Add(new RowDefinition());
            ScoreKarte.RowDefinitions.Add(new RowDefinition());
            ScoreKarte.RowDefinitions.Add(new RowDefinition());
            ScoreKarte.RowDefinitions.Add(new RowDefinition());
            ScoreKarte.RowDefinitions.Add(new RowDefinition());
            ScoreKarte.RowDefinitions.Add(new RowDefinition());
            ScoreKarte.RowDefinitions.Add(new RowDefinition());

            ScoreKarte.ColumnDefinitions.Add(new ColumnDefinition());
            ScoreKarte.ColumnDefinitions.Add(new ColumnDefinition());
            ScoreKarte.ColumnDefinitions.Add(new ColumnDefinition());

            for (int rowIndex = 0; rowIndex < 11; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < 3; columnIndex++)
                {
                    var label = new Label
                    {
                        Text = rowIndex.ToString() + "-" + columnIndex.ToString(),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        BackgroundColor = Color.Gray
                    };
                    ScoreKarte.Children.Add(label, columnIndex, rowIndex);
                }
            }

        }
        public TournamentPage()
        {
            InitializeComponent();
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            Switch CurrentSwitch = (Switch)sender;
            Player p = (Player)CurrentSwitch.BindingContext;            

            if(e.Value)
            {
                TournamentPageData.SelectedPlayers.Add(p);
            }
            else
            {
                TournamentPageData.SelectedPlayers.Remove(p);
            }
        }
    }
}