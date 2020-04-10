using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MFApp.Models;
using MFApp.Services;

namespace MFApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TournamentPage : TabbedPage
    {
        public TournamentPageData TournamentPageData { get; set; }

        Label[] InSumLabels = null;
        Label[] OutSumLabels = null;
        Label[] TotalSumLabels = null;

        int LastPlayerId = 0;

        public TournamentPage(Tournament tournament)
        {
            InitializeComponent();

            TournamentPageData PageData = new TournamentPageData(tournament);
            InSumLabels = new Label[4];
            OutSumLabels = new Label[4];
            TotalSumLabels = new Label[4];            

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
                bool found = false;
                foreach(TournamentPlayer tp in TournamentPageData.SelectedPlayers)
                {
                    if(p.Id == tp.Id)
                    {
                        found = true;
                        break;
                    }
                }

                if(!found)
                    TournamentPageData.SelectedPlayers.Add(p);
            }
            else
            {
                TournamentPageData.SelectedPlayers.Remove(p);
            }
            SaveFlight();
        }

        private void ContentPage_Scorecard_Appearing(object sender, EventArgs e)
        {
            // first clean scorecard
            ScoreKarte.RowDefinitions.Clear();
            ScoreKarte.ColumnDefinitions.Clear();
            ScoreKarte.Children.Clear();

            int playerCount = this.TournamentPageData.SelectedPlayers.Count();
            LastPlayerId = TournamentPageData.SelectedPlayers[playerCount - 1].Id;
            int teeCount = this.TournamentPageData.TeeList.Count();

            int rowcount = teeCount == 9 ? 11 : 22;

            // get saved results
            IDataStore<Result> DataStore = DependencyService.Get<IDataStore<Result>>();
            var ResultTask = DataStore.GetItemsAsync();
            List<Result> Results = ResultTask.Result.ToList();
            List<Result> SavedResults = Results.Where(x => x.TournamentId == TournamentPageData.Tournament.Id).ToList();

            for (int columnIndex = 0; columnIndex < (playerCount + 1); columnIndex++)
            {
                ScoreKarte.ColumnDefinitions.Add(new ColumnDefinition());
                for (int rowIndex = 0; rowIndex < rowcount; rowIndex++)
                {
                    if (columnIndex == 0)
                    { 
                        if (teeCount == 9)
                            ScoreKarte.RowDefinitions.Add(new RowDefinition());
                        else
                            ScoreKarte.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
                    }

                    if ((rowIndex == 0) && (columnIndex > 0))
                    {
                        // first row header row with player intials
                        AddLabelCell(TournamentPageData.SelectedPlayers[columnIndex - 1].Initials, columnIndex, rowIndex);
                    }
                    else if ((rowIndex > 0) && (rowIndex < 10))
                    {
                        // erste 9 Löcher
                        if (columnIndex == 0)
                        {
                            AddLabelCell(rowIndex.ToString(), columnIndex, rowIndex);
                        }
                        else
                        {
                            // get saved value
                            Result SavedResult = SavedResults.Where(x => x.PlayerId == TournamentPageData.SelectedPlayers[columnIndex - 1].Id).Where(y => y.TeeId == TournamentPageData.TeeList[rowIndex - 1].Id).FirstOrDefault();
                            string savedScore = "";
                            if(SavedResult!=null)
                            {
                                savedScore = SavedResult.Score.ToString();
                            }
                            string entryParameter = rowIndex.ToString() + "_" + columnIndex.ToString();
                            AddEntryCell(entryParameter, columnIndex, rowIndex, savedScore);
                        }
                    }
                    else if (rowIndex == 10)
                    {
                        // 9 Loch Summenzeile
                        if ((columnIndex == 0) && (teeCount == 9))
                        {
                            AddLabelCell("Total", columnIndex, rowIndex);
                        }
                        else if ((columnIndex == 0) && (teeCount > 9))
                        {
                            AddLabelCell("In", columnIndex, rowIndex);
                        }
                        else
                        {
                            Label label = AddLabelCell("", columnIndex, rowIndex);
                            InSumLabels[columnIndex - 1] = label;
                        }
                    }
                    else if ((teeCount > 9) && (rowIndex > 10) && (rowIndex < 20))
                    {
                        // zweite 9 Löcher
                        if (columnIndex == 0)
                        {
                            AddLabelCell((rowIndex - 1).ToString(), columnIndex, rowIndex);
                        }
                        else
                        {
                            // get saved value
                            Result SavedResult = SavedResults.Where(x => x.PlayerId == TournamentPageData.SelectedPlayers[columnIndex - 1].Id).Where(y => y.TeeId == TournamentPageData.TeeList[rowIndex - 2].Id).FirstOrDefault();
                            string savedScore = "";
                            if (SavedResult != null)
                            {
                                savedScore = SavedResult.Score.ToString();
                            }
                            string entryParameter = (rowIndex - 1).ToString() + "_" + columnIndex.ToString();
                            AddEntryCell(entryParameter, columnIndex, rowIndex, savedScore);
                        }
                    }
                    else if ((teeCount > 9) && (rowIndex == 20))
                    {
                        // 9 Loch Summenzeile
                        if (columnIndex == 0)
                        {
                            AddLabelCell("Out", columnIndex, rowIndex);
                        }
                        else
                        {
                            Label label = AddLabelCell("", columnIndex, rowIndex);
                            OutSumLabels[columnIndex - 1] = label;
                        }
                    }
                    else if ((teeCount > 9) && (rowIndex == 21))
                    {
                        // 18 Loch Summenzeile
                        if (columnIndex == 0)
                        {
                            AddLabelCell("Total", columnIndex, rowIndex);
                        }
                        else
                        {
                            Label label = AddLabelCell("", columnIndex, rowIndex);
                            TotalSumLabels[columnIndex - 1] = label;
                        }
                    }

                }
            }
        }

        private Label AddLabelCell(string LabelText, int ColumnIndex, int RowIndex)
        {
            var stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            stackLayout.BackgroundColor = Color.LightGray;

            var label = new Label
            {
                Text = LabelText,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold
            };
            stackLayout.Children.Add(label);
            ScoreKarte.Children.Add(stackLayout, ColumnIndex, RowIndex);

            return label;
        }

        private void AddEntryCell(string CommandParameter, int ColumnIndex, int RowIndex, string EntryText = "")
        {
            var stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            stackLayout.BackgroundColor = Color.White;

            var entry = new Entry
            {
                Text= EntryText,
                VerticalTextAlignment= TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Keyboard = Keyboard.Numeric,
                ReturnCommandParameter = CommandParameter,
                MinimumWidthRequest = 100,
                WidthRequest=100
            };

            entry.TextChanged += Entry_TextChanged;

            stackLayout.Children.Add(entry);
            ScoreKarte.Children.Add(stackLayout, ColumnIndex, RowIndex);
        }

        private async void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            // save result
            var oldText = e.OldTextValue;
            var newText = e.NewTextValue;

            if(string.IsNullOrEmpty(newText))
            {
                return;
            }

            string[] currentCommandParameter = ((Entry)sender).ReturnCommandParameter.ToString().Split('_');
            int currentrowIndex = Convert.ToInt32(currentCommandParameter[0].ToString());
            int currentColumnIndex = Convert.ToInt32(currentCommandParameter[1].ToString());
            int currentPlayerId = TournamentPageData.SelectedPlayers[currentColumnIndex - 1].Id;

            SaveResultLocal(newText, currentColumnIndex, currentrowIndex);

            // calculate sum fields
            int[] InSum = new int[4] { 0, 0, 0, 0 };
            int[] OutSum = new int[4] { 0, 0, 0, 0 };
            int[] TotalSum = new int[4] { 0, 0, 0, 0 };

            List<TournamentResult> TournamentResultList = new List<TournamentResult>();

            foreach (View v in ScoreKarte.Children)
            {
                if ((v is StackLayout) && (((StackLayout)v).Children.Count > 0))
                {
                    if (((StackLayout)v).Children[0] is Entry)
                    {
                        // entry
                        Entry tmpEntry = ((Entry)(((StackLayout)v).Children[0]));
                        string[] commandParameter = tmpEntry.ReturnCommandParameter.ToString().Split('_');

                        int rowIndex = Convert.ToInt32(commandParameter[0].ToString());
                        int columnIndex = Convert.ToInt32(commandParameter[1].ToString());
                        string entryText = tmpEntry.Text;

                        if (!string.IsNullOrEmpty(entryText))
                        {
                            int score = 0;
                            try
                            {
                                score = Convert.ToInt32(entryText);
                            }
                            catch(Exception exp) { }

                            if (rowIndex < 10)
                            {
                                InSum[columnIndex - 1] = InSum[columnIndex - 1] + score;
                            }
                            else
                            {
                                OutSum[columnIndex - 1] = OutSum[columnIndex - 1] + score;
                            }
                            TotalSum[columnIndex - 1] = TotalSum[columnIndex - 1] + score;

                            // add result
                            int playerId = TournamentPageData.SelectedPlayers[columnIndex - 1].Id;
                            int tournamentId = TournamentPageData.Tournament.Id;
                            int teeId = TournamentPageData.TeeList[rowIndex - 1].Id;
                            TournamentResult tr = new TournamentResult
                            {
                                PlayerId = playerId,
                                TournamentId = tournamentId,
                                TeeId = teeId,
                                Score = score
                            };
                            TournamentResultList.Add(tr);
                        }
                    }
                }
            }

            //write sums
            for (int i = 0; i < 4; i++)
            {
                if (InSumLabels[i] != null)
                {
                    InSumLabels[i].Text = InSum[i].ToString();
                }
                if (OutSumLabels[i] != null)
                {
                    OutSumLabels[i].Text = OutSum[i].ToString();
                }
                if (TotalSumLabels[i] != null)
                {
                    TotalSumLabels[i].Text = TotalSum[i].ToString();
                }
            }

            // send results to webapp
            SaveResultsWeb(TournamentResultList);
        }

        private void SaveResultLocal(string NewText, int ColumnIndex, int RowIndex)
        {
            int playerId = TournamentPageData.SelectedPlayers[ColumnIndex - 1].Id;
            int tournamentId = TournamentPageData.Tournament.Id;
            int teeId = TournamentPageData.TeeList[RowIndex - 1].Id;

            // local saving
            IDataStore<Result> DataStore = DependencyService.Get<IDataStore<Result>>();
            var ResultTask = DataStore.GetItemsAsync();
            List<Result> Results = ResultTask.Result.ToList();

            Result res = Results.Where(x => x.TournamentId == tournamentId).Where(y => y.PlayerId == playerId).Where(z => z.TeeId == teeId).FirstOrDefault();
            if(res != null)
            {
                res.Score = Convert.ToInt32(NewText);
                DataStore.UpdateItemAsync(res);
            }
            else
            {
                int newScore = Convert.ToInt32(NewText);
                Result newRes = new Result
                {
                    Score = newScore,
                    TournamentId = tournamentId,
                    PlayerId = playerId,
                    TeeId = teeId
                };
                DataStore.AddItemAsync(newRes);
            }
        }

        private async void SaveResultsWeb(List<TournamentResult> TournamentResultList)
        {
            // send results to web
            MFWebDataSync DataSync = new MFWebDataSync();
            await DataSync.SendResults(TournamentResultList);
        }

        private void SaveFlight()
        {
            int tournamentId = TournamentPageData.Tournament.Id;

            IDataStore<Flight> DataStoreFlight = DependencyService.Get<IDataStore<Flight>>();
            IDataStore<Flight2Player> DataStoreFlight2Player = DependencyService.Get<IDataStore<Flight2Player>>();

            var FlightTask = DataStoreFlight.GetItemsAsync();
            List<Flight> Flights = FlightTask.Result.ToList();

            // check if flight exists
            int flightNumber = TournamentPageData.Tournament.Id * 10000 + TournamentPageData.CurrentPlayer.Id;
            Flight f = Flights.Where(x => x.TournamentId == tournamentId).Where(y => y.FlightNumber == flightNumber).FirstOrDefault();
            if(f == null)
            {
                // create flight
                f = new Flight()
                {
                    Id = flightNumber,
                    FlightNumber = flightNumber,
                    TournamentId = tournamentId
                };

                DataStoreFlight.AddItemAsync(f);
            }
            else
            {
                // remove all players from flight
                var Flight2PlayerTask = DataStoreFlight2Player.GetItemsAsync();
                List<Flight2Player> Flight2Players = Flight2PlayerTask.Result.ToList();

                List<Flight2Player> CurrentFlightPlayers = Flight2Players.Where(x => x.FlightId == flightNumber).ToList();
                foreach(Flight2Player f2p in CurrentFlightPlayers)
                {
                    DataStoreFlight2Player.DeleteItemAsync(f2p.Id);
                }
            }

            // add players to flight
            foreach(TournamentPlayer p in TournamentPageData.SelectedPlayers)
            {
                Flight2Player f2p = new Flight2Player
                {
                    Id = flightNumber + (p.Id * 100000),
                    FlightId = flightNumber,
                    PlayerId = p.Id
                };
                DataStoreFlight2Player.AddItemAsync(f2p);
            }
        }
    }
}