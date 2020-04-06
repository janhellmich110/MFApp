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

        public TournamentPage(Tournament tournament)
        {
            InitializeComponent();

            TournamentPageData PageData = new TournamentPageData(tournament);

            BindingContext = this.TournamentPageData = PageData;

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
                    var stackLayout = new StackLayout();

                    if ((rowIndex == 0) || (columnIndex == 0))
                        stackLayout.BackgroundColor = Color.Gray;

                    var label = new Label
                    {
                        Text = rowIndex.ToString() + "-" + columnIndex.ToString(),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                    };
                    stackLayout.Children.Add(label);
                    ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
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
            TournamentPlayer p = (TournamentPlayer)CurrentSwitch.BindingContext;            

            if(e.Value)
            {
                TournamentPageData.SelectedPlayers.Add(p);
            }
            else
            {
                TournamentPageData.SelectedPlayers.Remove(p);
            }
        }

        private void ContentPage_Scorecard_Appearing(object sender, EventArgs e)
        {
            // first clean scorecard
            ScoreKarte.RowDefinitions.Clear();
            ScoreKarte.ColumnDefinitions.Clear();
            ScoreKarte.Children.Clear();

            int playerCount = this.TournamentPageData.SelectedPlayers.Count();

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

            for(int i = 0; i < (playerCount + 1); i++)
            {
                ScoreKarte.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // header row
            for (int columnIndex = 0; columnIndex < (playerCount + 1); columnIndex++)
            {
                var stackLayout = new StackLayout();
                stackLayout.BackgroundColor = Color.Gray;

                var label = new Label
                {
                    Text = "",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                };

                if (columnIndex > 0)
                {
                    label.Text = this.TournamentPageData.SelectedPlayers[columnIndex - 1].Initials;
                }
                stackLayout.Children.Add(label);
                ScoreKarte.Children.Add(stackLayout, columnIndex, 0);
            }

            // add tee rows
            for (int rowIndex = 1; rowIndex < 11; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < (playerCount + 1); columnIndex++)
                {
                    var stackLayout = new StackLayout();

                    if ((rowIndex == 0) || (columnIndex == 0))
                        stackLayout.BackgroundColor = Color.Gray;

                    var label = new Label
                    {
                        Text = rowIndex.ToString() + "-" + columnIndex.ToString(),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                    };
                    stackLayout.Children.Add(label);
                    ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
                }
            }
        }
    }
}