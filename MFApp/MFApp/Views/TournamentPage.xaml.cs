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
        }

        public TournamentPage()
        {
            InitializeComponent();
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            Switch CurrentSwitch = (Switch)sender;
            TournamentPlayer p = (TournamentPlayer)CurrentSwitch.BindingContext;

            if (e.Value)
            {
                TournamentPageData.SelectedPlayers.Add(p);
            }
            else
            {
                TournamentPageData.SelectedPlayers.Remove(p);
            }
        }
        void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var oldText = e.OldTextValue;
            var newText = e.NewTextValue;

            

        }


        private void ContentPage_Scorecard_Appearing(object sender, EventArgs e)
        {
            // first clean scorecard
            ScoreKarte.RowDefinitions.Clear();
            ScoreKarte.ColumnDefinitions.Clear();
            ScoreKarte.Children.Clear();

            int playerCount = this.TournamentPageData.SelectedPlayers.Count();
            int teeCount = this.TournamentPageData.TeeList.Count();



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
            if (teeCount == 9)
            {

                for (int i = 0; i < (teeCount + 2); i++)
                {
                    ScoreKarte.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
                }


                for (int i = 0; i < (playerCount + 1); i++)
                {
                    ScoreKarte.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int rowIndex = 1; rowIndex < (teeCount + 2); rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < (playerCount + 1); columnIndex++)
                    {
                        var stackLayout = new StackLayout();

                        if ((columnIndex == 0) || (rowIndex == 0) || (rowIndex == 10) )
                            stackLayout.BackgroundColor = Color.Gray;

                        if (!((rowIndex == 0) || (columnIndex == 0) || (rowIndex == 10)))
                            stackLayout.BackgroundColor = Color.White;


                        if ((columnIndex == 0) && !(rowIndex == 10))
                        {
                            var label = new Label
                            {
                                Text = rowIndex.ToString(),
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                            };
                            stackLayout.Children.Add(label);
                            ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
                        }

                        if ((columnIndex == 0) && (rowIndex == 10))
                        {
                            var label = new Label
                            {
                                Text = "Total",
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                            };
                            stackLayout.Children.Add(label);
                            ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
                        }


                        if ((columnIndex > 0) && (rowIndex == 10))
                        {
                            var label = new Label
                            {
                                Text = "",
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                            };
                            stackLayout.Children.Add(label);
                            ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
                        }


                        if (columnIndex > 0)
                        {
                            for (int i = 0; i < rowIndex; i++)
                            {
                                if (!(rowIndex == 10) )
                                {
                                    Entry entry = new Entry
                                    {
                                        Text = "",
                                        VerticalOptions = LayoutOptions.Center,
                                        HorizontalOptions = LayoutOptions.Center,
                                        Keyboard = Keyboard.Numeric,
                                        ReturnCommandParameter = columnIndex.ToString() + "_" + rowIndex.ToString()
                                        
                                    };
                                    entry.TextChanged += Entry_TextChanged;

                                    stackLayout.Children.Add(entry);
                                    ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);

                                }
                            }
                        }
                    }
                }
            }
            if (teeCount == 18)
            {

                for (int i = 0; i < (teeCount + 4); i++)
                {
                    ScoreKarte.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
                }


                for (int i = 0; i < (playerCount + 1); i++)
                {
                    ScoreKarte.ColumnDefinitions.Add(new ColumnDefinition ());
                }

                for (int rowIndex = 1; rowIndex < (teeCount + 4); rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < (playerCount + 1); columnIndex++)
                    {
                        var stackLayout = new StackLayout();

                        if ((columnIndex == 0) || (rowIndex == 0) || (rowIndex == 10) || (rowIndex == 21) || (rowIndex == 20))
                            stackLayout.BackgroundColor = Color.Gray;

                        if (!((rowIndex == 0) || (columnIndex == 0) || (rowIndex == 10) || (rowIndex == 21) || (rowIndex == 20)))
                            stackLayout.BackgroundColor = Color.White;


                        if((columnIndex == 0) && !((rowIndex == 10) || (rowIndex == 20) || (rowIndex == 21)))
                        {
                            var label = new Label
                            {
                                Text = rowIndex.ToString(),
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                            };
                            stackLayout.Children.Add(label);
                            ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
                        }

                        if ((columnIndex == 0) && (rowIndex == 10) ) 
                        {
                            var label = new Label
                            {
                                Text = "Out",
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                            };
                            stackLayout.Children.Add(label);
                            ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
                        }

                        if ((columnIndex == 0) && (rowIndex == 20))
                        {
                            var label = new Label
                            {
                                Text = "In",
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                            };
                            stackLayout.Children.Add(label);
                            ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
                        }

                        if ((columnIndex == 0) && (rowIndex == 21))
                        {
                            var label = new Label
                            {
                                Text = "Total",
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                            };
                            stackLayout.Children.Add(label);
                            ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
                        }

                        if ((columnIndex == 1) && (rowIndex == 10))
                        {
                            var label = new Label
                            {
                                Text = "",
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                            };
                            stackLayout.Children.Add(label);
                            ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
                        }

                        if ((columnIndex == 1) && (rowIndex == 20))
                        {
                            var label = new Label
                            {
                                Text = "",
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                            };
                            stackLayout.Children.Add(label);
                            ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
                        }

                        if ((columnIndex == 1) && (rowIndex == 21))
                        {
                            var label = new Label
                            {
                                Text = "",
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                            };
                            stackLayout.Children.Add(label);
                            ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);
                        }

                        if (columnIndex > 0)
                        {
                            for (int i = 0; i < rowIndex; i++)
                            {
                                if (!((rowIndex == 10) || (rowIndex == 20) || (rowIndex == 21)))
                                {
                                    Entry entry = new Entry
                                    {
                                        Text ="",
                                        VerticalOptions = LayoutOptions.Center,
                                        HorizontalOptions = LayoutOptions.Center,
                                        Keyboard = Keyboard.Numeric,
                                        ReturnCommandParameter = columnIndex.ToString() + "_" + rowIndex.ToString()

                                    };
                                    entry.TextChanged += Entry_TextChanged;

                                    stackLayout.Children.Add(entry);
                                    ScoreKarte.Children.Add(stackLayout, columnIndex, rowIndex);

                                }
                            }
                        }
                    }
                }
            }    
            
        }
    }
}