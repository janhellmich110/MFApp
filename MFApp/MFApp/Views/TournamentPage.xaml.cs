using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MFApp.Models;
using MFApp.Services;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using MFApp.Interfaces;
using System.IO;

namespace MFApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TournamentPage : TabbedPage
    {
        public TournamentPageData TournamentPageData { get; set; }

        Label[] InSumLabels = null;
        Label[] OutSumLabels = null;
        Label[] TotalSumLabels = null;

        Label[] InSumPuttsLabels = null;
        Label[] OutSumPuttsLabels = null;
        Label[] TotalSumPuttsLabels = null;

        List<Entry> AllEntryFields = new List<Entry>();

        int LastPlayerId = 0;

        IDataStore<Result> DataStoreResults = DependencyService.Get<IDataStore<Result>>();
        IDataStore<Flight> DataStoreFlight = DependencyService.Get<IDataStore<Flight>>();
        IDataStore<Flight2Player> DataStoreFlight2Player = DependencyService.Get<IDataStore<Flight2Player>>();

        public TournamentPage(Tournament tournament)
        {
            InitializeComponent();

            TournamentPageData PageData = new TournamentPageData(tournament);
            InSumLabels = new Label[4];
            OutSumLabels = new Label[4];
            TotalSumLabels = new Label[4];

            InSumPuttsLabels = new Label[4];
            OutSumPuttsLabels = new Label[4];
            TotalSumPuttsLabels = new Label[4];

            BindingContext = this.TournamentPageData = PageData;
        }

        private void ContentPage_Scorecard_Appearing(object sender, EventArgs e)
        {
            AllEntryFields = new List<Entry>();
            // first clean scorecard
            ScoreKarte.RowDefinitions.Clear();
            ScoreKarte.ColumnDefinitions.Clear();
            ScoreKarte.Children.Clear();

            if (TournamentPageData.SelectedPlayers.Count() == 0)
                return;

            int playerCount = this.TournamentPageData.SelectedPlayers.Count();
            LastPlayerId = TournamentPageData.SelectedPlayers[playerCount - 1].Id;
            int teeCount = this.TournamentPageData.TeeList.Count();

            int rowcount = teeCount == 9 ? 11 : 22;

            // get saved results
            List<Result> SavedResults = GetTournamentResults(TournamentPageData.Tournament.Id); 

            for (int columnIndex = 0; columnIndex < (playerCount + 1); columnIndex++)
            {
                if (columnIndex == 0)
                    ScoreKarte.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) });
                else
                    ScoreKarte.ColumnDefinitions.Add(new ColumnDefinition());

                for (int rowIndex = 0; rowIndex < rowcount; rowIndex++)
                {
                    if (columnIndex == 0)
                    {
                        ScoreKarte.RowDefinitions.Add(new RowDefinition());
                    }

                    if ((rowIndex == 0) && (columnIndex > 0))
                    {
                        // first row header row with player intials
                        string HeaderText = TournamentPageData.SelectedPlayers[columnIndex - 1].Initials + " (" + TournamentPageData.SelectedPlayers[columnIndex - 1].CourseHandicap + ')';
                        AddHeaderPuttsLabelCell(HeaderText, columnIndex, rowIndex);
                    }
                    else if ((rowIndex > 0) && (rowIndex < 10))
                    {
                        // erste 9 Löcher
                        if (columnIndex == 0)
                        {
                            // get tee
                            Tee tee = TournamentPageData.TeeList[rowIndex - 1];
                            AddHoleLabelCell(tee, columnIndex, rowIndex);
                        }
                        else
                        {
                            // get saved value
                            Result SavedResult = SavedResults.Where(x => x.PlayerId == TournamentPageData.SelectedPlayers[columnIndex - 1].Id).Where(y => y.TeeId == TournamentPageData.TeeList[rowIndex - 1].Id).FirstOrDefault();
                            string savedScore = "";
                            string savedPutts = "";
                            bool cellReadOnly = false;
                            if (SavedResult != null)
                            {
                                savedScore = SavedResult.Score.ToString();
                                savedPutts = SavedResult.Putts.ToString();
                                cellReadOnly = SavedResult.Final;
                            }
                            string entryParameter = rowIndex.ToString() + "_" + columnIndex.ToString();
                            AddEntryCell(entryParameter, columnIndex, rowIndex, savedScore, savedPutts, cellReadOnly);
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
                            // insum with putts
                            var StackLayout = new StackLayout
                            {
                                BackgroundColor = Color.LightGray,
                                Orientation = StackOrientation.Horizontal,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                            };
                            var subScoreStackLayout = new StackLayout
                            {
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand
                            };

                            Label ScoreLabel = AddLabelCell("", subScoreStackLayout);

                            StackLayout.Children.Add(subScoreStackLayout);

                            if (TournamentPageData.Tournament.WithPutts)
                            {
                                var subPuttsStackLayout = new StackLayout
                                {
                                    BackgroundColor = Color.White,
                                    VerticalOptions = LayoutOptions.FillAndExpand,
                                    HorizontalOptions = LayoutOptions.FillAndExpand
                                };
                                Label PuttsLabel = AddLabelCell("", subPuttsStackLayout);
                                StackLayout.Children.Add(subPuttsStackLayout);
                                InSumPuttsLabels[columnIndex - 1] = PuttsLabel;
                            }

                            ScoreKarte.Children.Add(StackLayout, columnIndex, rowIndex);

                            InSumLabels[columnIndex - 1] = ScoreLabel;
                        }
                    }
                    else if ((teeCount > 9) && (rowIndex > 10) && (rowIndex < 20))
                    {
                        // zweite 9 Löcher
                        if (columnIndex == 0)
                        {
                            Tee tee = TournamentPageData.TeeList[rowIndex - 2];
                            AddHoleLabelCell(tee, columnIndex, rowIndex);
                        }
                        else
                        {
                            // get saved value
                            Result SavedResult = SavedResults.Where(x => x.PlayerId == TournamentPageData.SelectedPlayers[columnIndex - 1].Id).Where(y => y.TeeId == TournamentPageData.TeeList[rowIndex - 2].Id).FirstOrDefault();
                            string savedScore = "";
                            string savedPutts = "";
                            bool cellReadOnly = false;
                            if (SavedResult != null)
                            {
                                savedScore = SavedResult.Score.ToString();
                                savedPutts = SavedResult.Putts.ToString();
                                cellReadOnly = SavedResult.Final;
                            }

                            string entryParameter = (rowIndex - 1).ToString() + "_" + columnIndex.ToString();
                            AddEntryCell(entryParameter, columnIndex, rowIndex, savedScore, savedPutts, cellReadOnly);
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
                            // insum with putts
                            var StackLayout = new StackLayout
                            {
                                BackgroundColor = Color.LightGray,
                                Orientation = StackOrientation.Horizontal,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                            };
                            var subScoreStackLayout = new StackLayout
                            {
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand
                            };

                            Label ScoreLabel = AddLabelCell("", subScoreStackLayout);
                            StackLayout.Children.Add(subScoreStackLayout);

                            if (TournamentPageData.Tournament.WithPutts)
                            {
                                var subPuttsStackLayout = new StackLayout
                                {
                                    BackgroundColor = Color.White,
                                    VerticalOptions = LayoutOptions.FillAndExpand,
                                    HorizontalOptions = LayoutOptions.FillAndExpand
                                };
                                Label PuttsLabel = AddLabelCell("", subPuttsStackLayout);
                                StackLayout.Children.Add(subPuttsStackLayout);
                                OutSumPuttsLabels[columnIndex - 1] = PuttsLabel;
                            }

                            ScoreKarte.Children.Add(StackLayout, columnIndex, rowIndex);

                            OutSumLabels[columnIndex - 1] = ScoreLabel;
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
                            // insum with putts
                            var StackLayout = new StackLayout
                            {
                                BackgroundColor = Color.LightGray,
                                Orientation = StackOrientation.Horizontal,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                            };
                            var subScoreStackLayout = new StackLayout
                            {
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand
                            };

                            StackLayout.Children.Add(subScoreStackLayout);
                            Label ScoreLabel = AddLabelCell("", subScoreStackLayout);

                            if (TournamentPageData.Tournament.WithPutts)
                            {
                                var subPuttsStackLayout = new StackLayout
                                {
                                    BackgroundColor = Color.White,
                                    VerticalOptions = LayoutOptions.FillAndExpand,
                                    HorizontalOptions = LayoutOptions.FillAndExpand
                                };

                                Label PuttsLabel = AddLabelCell("", subPuttsStackLayout);
                                StackLayout.Children.Add(subPuttsStackLayout);

                                TotalSumPuttsLabels[columnIndex - 1] = PuttsLabel;
                            }


                            ScoreKarte.Children.Add(StackLayout, columnIndex, rowIndex);

                            TotalSumLabels[columnIndex - 1] = ScoreLabel;
                        }
                    }

                }
            }

            WriteSumsFromDB(SavedResults);
            SetCloseTournamentButtonEnabled();
        }

        private void ContentPageResults_Appearing(object sender, EventArgs e)
        {
            this.BindingContext = null;
            CollectionView AllResults = (CollectionView)this.FindByName("PlayerAllResultCollectionView");
            AllResults.IsVisible = false;

            if (TournamentPageData.TournamentEvent.EventType == EventTypeEnum.Tournament)
            {
                Button allResultsButton = (Button)this.FindByName("LoadAllResults");
                allResultsButton.IsVisible = true;
            }
            else
            {
                Button allResultsButton = (Button)this.FindByName("LoadAllResults");
                allResultsButton.IsVisible = false;
            }
            if (TournamentPageData.TournamentEvent.EventType == EventTypeEnum.AppEvent)
            {
                Button b = (Button)this.FindByName("FinishTournament");
                b.IsVisible = false;
            }

            TournamentPageData.PlayerResults.Clear();
            TournamentPageData.PlayerResults = new System.Collections.ObjectModel.ObservableCollection<TournamentResultSummary>();

            List<Result> SavedResults = GetTournamentResults(TournamentPageData.Tournament.Id);

            foreach (TournamentPlayer tp in TournamentPageData.SelectedPlayers)
            {
                TournamentResultSummary trs = new TournamentResultSummary();
                trs.PlayerId = tp.Id;
                trs.PlayerName = tp.Name;

                int BruttoScore = 0;
                int NettoScore = 0;
                int BruttoPoints = 0;
                int NettoPoints = 0;
                int Putts = 0;

                foreach (Tee t in TournamentPageData.TeeList)
                {
                    // get result by player and tee
                    Result PlayerResult = SavedResults.Where(x => x.PlayerId == tp.Id).Where(y => y.TeeId == t.Id).FirstOrDefault();
                    {
                        if (PlayerResult != null)
                        {
                            // result found
                            BruttoScore = BruttoScore + PlayerResult.Score;
                            Putts = Putts + PlayerResult.Putts;

                            //Bruttopunkte
                            int points = 0;
                            points = t.Par - PlayerResult.Score + 2;

                            if (points > 0)
                            {
                                BruttoPoints += points;
                            }

                            //netto Par erstellen
                            int nPar = t.Par;

                            int Teeanzahl = TournamentPageData.TeeList.Count;
                            int Handicap = tp.CourseHandicap;

                            int Lochvorgabe = 0;
                            //spielvorgabe zugreifen
                            Lochvorgabe = Handicap / 18;

                            nPar += Lochvorgabe;

                            int evtlLochvorgabe = 0;
                            evtlLochvorgabe = Handicap % 18;

                            if (t.Hcp <= evtlLochvorgabe)
                            {
                                nPar += 1;
                            }

                            //NettoZählspiel
                            NettoScore += PlayerResult.Score - (nPar - t.Par);

                            //Nettopunkte
                            int npoints = 0;
                            npoints = nPar - PlayerResult.Score + 2;

                            if (npoints > 0)
                            {
                                NettoPoints += npoints;
                            }
                        }
                    }

                }

                trs.ScoreBrutto = BruttoScore;
                trs.ScoreNetto = NettoScore;
                trs.BruttoPoints = BruttoPoints;
                trs.NettoPoints = NettoPoints;
                trs.Putts = Putts;

                TournamentPageData.PlayerResults.Add(trs);
            }
            this.BindingContext = TournamentPageData;
        }

        private void ContentPageTurnier_Appearing(object sender, EventArgs e)
        {
            this.BindingContext = null;
            this.BindingContext = TournamentPageData;
        }

        #region event handler
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            Xamarin.Forms.Switch CurrentSwitch = (Xamarin.Forms.Switch)sender;
            TournamentPlayer p = (TournamentPlayer)CurrentSwitch.BindingContext;

            if (e.Value)
            {
                bool found = false;
                foreach (TournamentPlayer tp in TournamentPageData.SelectedPlayers)
                {
                    if (p.Id == tp.Id)
                    {
                        found = true;
                        break;
                    }
                }

                if ((!found) && (TournamentPageData.SelectedPlayers.Count() < 4))
                {
                    TournamentPageData.SelectedPlayers.Add(p);
                }
                else if (!found)
                {
                    CurrentSwitch.IsToggled = false;
                }
            }
            else
            {
                TournamentPageData.SelectedPlayers.Remove(p);
            }
            SaveFlight();
        }

        private async void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Debug.Print("Start enry changed: " + DateTime.Now.ToString("hh:mm:ss.fff"));
            // save result
            var oldText = e.OldTextValue;
            var newText = e.NewTextValue;

            if (string.IsNullOrEmpty(newText))
            {
                return;
            }

            string currentCommand = ((Entry)sender).ReturnCommandParameter.ToString();
            bool isScore = true;
            if (currentCommand.StartsWith("putts"))
            {
                isScore = false;
                currentCommand = currentCommand.Replace("putts_", "");
            }


            string[] currentCommandParameter = currentCommand.Split('_');
            int currentrowIndex = Convert.ToInt32(currentCommandParameter[0].ToString());
            int currentColumnIndex = Convert.ToInt32(currentCommandParameter[1].ToString());
            int currentPlayerId = TournamentPageData.SelectedPlayers[currentColumnIndex - 1].Id;

            Debug.Print("Start save local: " + DateTime.Now.ToString("hh:mm:ss.fff"));
            SaveResultLocal(newText, currentColumnIndex, currentrowIndex, isScore);
            Debug.Print("End save local: " + DateTime.Now.ToString("hh:mm:ss.fff"));

            // calculate sum fields
            int[] InSum = new int[4] { 0, 0, 0, 0 };
            int[] OutSum = new int[4] { 0, 0, 0, 0 };
            int[] TotalSum = new int[4] { 0, 0, 0, 0 };

            int[] InSumPutts = new int[4] { 0, 0, 0, 0 };
            int[] OutSumPutts = new int[4] { 0, 0, 0, 0 };
            int[] TotalSumPutts = new int[4] { 0, 0, 0, 0 };

            List<TournamentResult> TournamentResultList = new List<TournamentResult>();

            foreach (View v in ScoreKarte.Children)
            {
                if ((v is StackLayout) && (((StackLayout)v).Children.Count > 0) && (((StackLayout)v).Children[0] is StackLayout))
                {
                    StackLayout subStack = (StackLayout)((StackLayout)v).Children[0];
                    int score = 0;
                    int putts = 0;

                    int rowIndex = 0;
                    int columnIndex = 0;

                    if ((subStack.Children.Count() > 0) && (subStack.Children[0] is StackLayout))
                    {
                        StackLayout subsubScoreStack = (StackLayout)subStack.Children[0];

                        if ((subsubScoreStack.Children.Count() > 0) && (subsubScoreStack.Children[0] is StackLayout))
                        {
                            StackLayout scoreStack = (StackLayout)subsubScoreStack.Children[0];

                            if (scoreStack.Children[0] is Entry)
                            {
                                // entry
                                Entry tmpEntry = ((Entry)(scoreStack.Children[0]));
                                string[] commandParameter = tmpEntry.ReturnCommandParameter.ToString().Split('_');

                                rowIndex = Convert.ToInt32(commandParameter[0].ToString());
                                columnIndex = Convert.ToInt32(commandParameter[1].ToString());
                                string entryText = tmpEntry.Text;

                                if (!string.IsNullOrEmpty(entryText))
                                {
                                    try
                                    {
                                        score = Convert.ToInt32(entryText);
                                    }
                                    catch (Exception exp) { }

                                    if (rowIndex < 10)
                                    {
                                        InSum[columnIndex - 1] = InSum[columnIndex - 1] + score;
                                    }
                                    else
                                    {
                                        OutSum[columnIndex - 1] = OutSum[columnIndex - 1] + score;
                                    }
                                    TotalSum[columnIndex - 1] = TotalSum[columnIndex - 1] + score;
                                }
                            }
                        }
                    }

                    if ((subStack.Children.Count() > 1) && (subStack.Children[1] is StackLayout))
                    {
                        StackLayout subsubPuttsStack = (StackLayout)subStack.Children[1];

                        if ((subsubPuttsStack.Children.Count() > 0) && (subsubPuttsStack.Children[0] is StackLayout))
                        {
                            StackLayout puttStack = (StackLayout)subsubPuttsStack.Children[0];

                            if (puttStack.Children[0] is Entry)
                            {
                                // entry
                                Entry tmpEntry = ((Entry)(puttStack.Children[0]));
                                string[] commandParameter = tmpEntry.ReturnCommandParameter.ToString().Replace("putts_", "").Split('_');

                                rowIndex = Convert.ToInt32(commandParameter[0].ToString());
                                columnIndex = Convert.ToInt32(commandParameter[1].ToString());
                                string entryText = tmpEntry.Text;

                                if (!string.IsNullOrEmpty(entryText))
                                {
                                    try
                                    {
                                        putts = Convert.ToInt32(entryText);
                                    }
                                    catch (Exception exp) { }

                                    if (rowIndex < 10)
                                    {
                                        InSumPutts[columnIndex - 1] = InSumPutts[columnIndex - 1] + putts;
                                    }
                                    else
                                    {
                                        OutSumPutts[columnIndex - 1] = OutSumPutts[columnIndex - 1] + putts;
                                    }
                                    TotalSumPutts[columnIndex - 1] = TotalSumPutts[columnIndex - 1] + putts;
                                }
                            }
                        }
                    }

                    if ((columnIndex > 0) && (rowIndex > 0) && ((score > 0) || (putts > 0)))
                    {
                        // add result
                        int playerId = TournamentPageData.SelectedPlayers[columnIndex - 1].Id;
                        int tournamentId = TournamentPageData.Tournament.Id;
                        int teeId = TournamentPageData.TeeList[rowIndex - 1].Id;
                        TournamentResult tr = new TournamentResult
                        {
                            UserId = TournamentPageData.CurrentPlayer.Id,
                            PlayerId = playerId,
                            TournamentId = tournamentId,
                            TeeId = teeId,
                            Score = score,
                            Putts = putts,
                            Final = false
                        };
                        TournamentResultList.Add(tr);
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

                if (InSumPuttsLabels[i] != null)
                {
                    InSumPuttsLabels[i].Text = InSumPutts[i].ToString();
                }
                if (OutSumPuttsLabels[i] != null)
                {
                    OutSumPuttsLabels[i].Text = OutSumPutts[i].ToString();
                }
                if (TotalSumPuttsLabels[i] != null)
                {
                    TotalSumPuttsLabels[i].Text = TotalSumPutts[i].ToString();
                }
            }

            if (TournamentPageData.Tournament.Id > 0)
            {
                Debug.Print("Start save web: " + DateTime.Now.ToString("hh:mm:ss.fff"));
                // send results to webapp
                SaveResultsWeb(TournamentResultList);
                Debug.Print("End save web: " + DateTime.Now.ToString("hh:mm:ss.fff"));
            }

            SetCloseTournamentButtonEnabled();
        }

        private async void FinishTournament_Clicked(object sender, EventArgs e)
        {
            List<Result> SavedResults = GetTournamentResults(TournamentPageData.Tournament.Id);
            List<TournamentResult> TournamentResultList = new List<TournamentResult>();

            foreach (TournamentPlayer tp in TournamentPageData.SelectedPlayers)
            {
                foreach (Tee t in TournamentPageData.TeeList)
                {
                    // get result by player and tee
                    Result PlayerResult = SavedResults.Where(x => x.PlayerId == tp.Id).Where(y => y.TeeId == t.Id).FirstOrDefault();
                    {
                        if (PlayerResult != null)
                        {
                            TournamentResult tr = new TournamentResult
                            {
                                UserId = TournamentPageData.CurrentPlayer.Id,
                                PlayerId = tp.Id,
                                TournamentId = TournamentPageData.Tournament.Id,
                                TeeId = t.Id,
                                Score = PlayerResult.Score,
                                Putts = PlayerResult.Putts,
                                Final = true
                            };
                            TournamentResultList.Add(tr);
                        }
                    }

                }

            }

            // send results to web
            MFWebDataSync DataSync = new MFWebDataSync();
            //var ResultTaskDataSync = DataSync.SendResults(TournamentResultList);
            bool DataSyncResult = await DataSync.SendResults(TournamentResultList); // ResultTaskDataSync.Result;

            Button button = sender as Button;
            if (DataSyncResult)
            {
                button.Text = "Turnier wurde abgeschlossen!";

                // set final flag for all results
                foreach (TournamentPlayer tp in TournamentPageData.SelectedPlayers)
                {
                    foreach (Tee t in TournamentPageData.TeeList)
                    {
                        // get result by player and tee
                        Result PlayerResult = SavedResults.Where(x => x.PlayerId == tp.Id).Where(y => y.TeeId == t.Id).FirstOrDefault();
                        {
                            if (PlayerResult != null)
                            {
                                PlayerResult.Final = true;
                                DataStoreResults.UpdateItemAsync(PlayerResult);
                            }
                        }
                    }

                }

                // disable all entry fields
                try
                {
                    foreach(Entry entry in AllEntryFields)
                    {
                        entry.IsEnabled = false;
                    }
                }
                catch(Exception)
                { }
            }
            else
            {
                button.Text = "Fehler. Bitte später noch einmal versuchen!";
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                var screenShotService = Xamarin.Forms.DependencyService.Get<IScreenshotService>();

                var viewImage = screenShotService.GetScreenshot(ScoreKarte, new Size(((StackLayout)ScoreKarte.Parent).Width, ((StackLayout)ScoreKarte.Parent).Height));

                if (viewImage == null)
                    return;

                // write file to temp because share wants it
                var file = Path.Combine(FileSystem.CacheDirectory, "shareimg.jpeg");
                File.WriteAllBytes(file, viewImage);

                string shareTitle = TournamentPageData.TournamentEvent.Name + " - " + TournamentPageData.Tournament.Name + ": " + DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();


                await Share.RequestAsync(new ShareFileRequest
                {
                    File = new ShareFile(file),
                    Title = shareTitle,
                    PresentationSourceBounds = Xamarin.Essentials.DeviceInfo.Platform == DevicePlatform.iOS && Xamarin.Essentials.DeviceInfo.Idiom == DeviceIdiom.Tablet
                            ? new System.Drawing.Rectangle(0, 20, 0, 0)
                            : System.Drawing.Rectangle.Empty
                });
            }
            catch (Exception exc)
            {

            }
        }

        private void LoadAllResults_Clicked(object sender, EventArgs e)
        {
            CollectionView AllResults = (CollectionView)this.FindByName("PlayerAllResultCollectionView");
            AllResults.IsVisible = true;
            AllResults.BindingContext = null;

            TournamentPageData.IsRefreshingAllResults = true;
            AllResults.BindingContext = TournamentPageData;
        }
        #endregion

        #region helper functions
        private Label AddLabelCell(string LabelText, int ColumnIndex, int RowIndex)
        {
            var stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.LightGray
            };
            var subStackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            stackLayout.BackgroundColor = Color.LightGray;

            var label = new Label
            {
                Text = LabelText,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold
            };
            subStackLayout.Children.Add(label);
            stackLayout.Children.Add(subStackLayout);
            ScoreKarte.Children.Add(stackLayout, ColumnIndex, RowIndex);

            return label;
        }

        private Label AddHoleLabelCell(Tee tee, int ColumnIndex, int RowIndex)
        {
            var stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.LightGray
            };
            var subStackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            stackLayout.BackgroundColor = Color.LightGray;

            var label = new Label
            {
                Text = tee.Name.ToString(),
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold
            };
            subStackLayout.Children.Add(label);
            stackLayout.Children.Add(subStackLayout);

            #region add tee info header
            var subTeeHeaderStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };
            var subTeeParHeaderStackLayout = new StackLayout
            {
                Padding = new Thickness(2),
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            var subTeeHdcpHeaderStackLayout = new StackLayout
            {
                Padding = new Thickness(2),
                HorizontalOptions = LayoutOptions.EndAndExpand
            };

            var parLabel = new Label
            {
                Text = tee.Par.ToString(),
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 16,
                FontAttributes = FontAttributes.None
            };
            var hdcpLabel = new Label
            {
                Text = tee.Hcp.ToString(),
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 16,
                FontAttributes = FontAttributes.None
            };

            subTeeParHeaderStackLayout.Children.Add(parLabel);
            subTeeHdcpHeaderStackLayout.Children.Add(hdcpLabel);

            subTeeHeaderStackLayout.Children.Add(subTeeParHeaderStackLayout);
            subTeeHeaderStackLayout.Children.Add(subTeeHdcpHeaderStackLayout);

            stackLayout.Children.Add(subTeeHeaderStackLayout);
            #endregion

            ScoreKarte.Children.Add(stackLayout, ColumnIndex, RowIndex);

            // add tapped event
            var teeInfo_tap = new TapGestureRecognizer();
            teeInfo_tap.Tapped += (s, e) =>
            {
                Navigation.PushModalAsync(new NavigationPage(new TeeInfoPage(TournamentPageData.TournamentEvent.GolfclubId, tee.Name)));
            };
            stackLayout.GestureRecognizers.Add(teeInfo_tap);

            return label;
        }

        private Label AddHeaderPuttsLabelCell(string LabelText, int ColumnIndex, int RowIndex)
        {
            var stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.LightGray
            };
            var subStackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            stackLayout.BackgroundColor = Color.LightGray;

            var label = new Label
            {
                Text = LabelText,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold
            };
            subStackLayout.Children.Add(label);
            stackLayout.Children.Add(subStackLayout);

            #region add score putts header
            if (TournamentPageData.Tournament.WithPutts)
            {
                var subScorePuttHeaderStackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal
                };
                var subScoreHeaderStackLayout = new StackLayout
                {
                    Padding = new Thickness(1),
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };
                var subPuttsHeaderStackLayout = new StackLayout
                {
                    Padding = new Thickness(1),
                    HorizontalOptions = LayoutOptions.EndAndExpand
                };

                var scoreLabel = new Label
                {
                    Text = "Score",
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    FontAttributes = FontAttributes.None
                };
                var puttsLabel = new Label
                {
                    Text = "Putts",
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    FontAttributes = FontAttributes.None
                };

                subScoreHeaderStackLayout.Children.Add(scoreLabel);
                subPuttsHeaderStackLayout.Children.Add(puttsLabel);

                subScorePuttHeaderStackLayout.Children.Add(subScoreHeaderStackLayout);
                subScorePuttHeaderStackLayout.Children.Add(subPuttsHeaderStackLayout);

                stackLayout.Children.Add(subScorePuttHeaderStackLayout);
            }
            #endregion

            ScoreKarte.Children.Add(stackLayout, ColumnIndex, RowIndex);

            return label;
        }

        private Label AddLabelCell(string LabelText, StackLayout parent)
        {
            var stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.LightGray
            };
            var subStackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            stackLayout.BackgroundColor = Color.LightGray;

            var label = new Label
            {
                Text = LabelText,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold
            };
            subStackLayout.Children.Add(label);
            stackLayout.Children.Add(subStackLayout);
            parent.Children.Add(stackLayout);

            return label;
        }

        private void AddEntryCell(string CommandParameter, int ColumnIndex, int RowIndex, string EntryText, string EntryText1, bool readOnly = false)
        {
            var stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            var subStackLayout = new StackLayout
            {
                BackgroundColor = Color.Gray,
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            var subsubScoreStackLayout = new StackLayout
            {
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            var subsubPuttsStackLayout = new StackLayout
            {
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };


            var stackLayoutScore = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            var entry = new Entry
            {
                Text = EntryText,
                HorizontalTextAlignment = TextAlignment.Center,
                ReturnCommandParameter = CommandParameter,
                FontSize = 20,
                WidthRequest = 40,
                Keyboard = Keyboard.Numeric,
                BackgroundColor = Color.White,
                TextColor = Color.Black,
                IsReadOnly = readOnly
            };

            entry.TextChanged += Entry_TextChanged;

            if (TournamentPageData.Tournament.WithPutts)
            {
                var stackLayoutPutts = new StackLayout
                {
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };
                CommandParameter = "putts_" + CommandParameter;
                var entryPutts = new Entry
                {
                    Text = EntryText1,
                    HorizontalTextAlignment = TextAlignment.Center,
                    ReturnCommandParameter = CommandParameter,
                    FontSize = 20,
                    WidthRequest = 30,
                    Keyboard = Keyboard.Numeric,
                    BackgroundColor = Color.White,
                    TextColor = Color.Black,
                    IsReadOnly = readOnly
                };

                entryPutts.TextChanged += Entry_TextChanged;

                stackLayoutPutts.Children.Add(entryPutts);

                subsubPuttsStackLayout.Children.Add(stackLayoutPutts);
            }

            stackLayoutScore.Children.Add(entry);
            subsubScoreStackLayout.Children.Add(stackLayoutScore);

            subStackLayout.Children.Add(subsubScoreStackLayout);

            if (TournamentPageData.Tournament.WithPutts)
                subStackLayout.Children.Add(subsubPuttsStackLayout);

            stackLayout.Children.Add(subStackLayout);
            ScoreKarte.Children.Add(stackLayout, ColumnIndex, RowIndex);

            AllEntryFields.Add(entry);
        }
        #endregion


        private void SaveResultLocal(string NewText, int ColumnIndex, int RowIndex, bool score)
        {
            int playerId = TournamentPageData.SelectedPlayers[ColumnIndex - 1].Id;
            int teeId = TournamentPageData.TeeList[RowIndex - 1].Id;

            // local saving
            List<Result> SavedResults = GetTournamentResults(TournamentPageData.Tournament.Id);

            Result res = SavedResults.Where(y => y.PlayerId == playerId).Where(z => z.TeeId == teeId).FirstOrDefault();
            if(res != null)
            {
                if (score)
                {
                    res.Score = Convert.ToInt32(NewText);
                }
                else
                {
                    res.Putts = Convert.ToInt32(NewText);
                }
                
                DataStoreResults.UpdateItemAsync(res);
            }
            else
            {
                int newScore = 0;
                int newPutts = 0;
                if(score)
                {
                    newScore = Convert.ToInt32(NewText);
                }
                else
                {
                    newPutts = Convert.ToInt32(NewText);
                }

                Result newRes = new Result
                {
                    Score = newScore,
                    Putts = newPutts,
                    TournamentId = TournamentPageData.Tournament.Id,
                    PlayerId = playerId,
                    TeeId = teeId,
                    Final=false
                };
                newRes.LastModified = DateTime.Now;
                DataStoreResults.AddItemAsync(newRes);
            }
        }

        private async void SaveResultsWeb(List<TournamentResult> TournamentResultList)
        {
            // send results to web
            MFWebDataSync DataSync = new MFWebDataSync();

            var t = Task.Run(() => DataSync.SendResults(TournamentResultList));      
        }

        private async void SaveFlight()
        {
            int tournamentId = TournamentPageData.Tournament.Id;

            List<Flight> Flights = (await DataStoreFlight.GetItemsAsync()).ToList();

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
                    FlightName = "Mein Flight",
                    TournamentId = tournamentId
                };

                await DataStoreFlight.AddItemAsync(f);
            }
            else
            {
                // remove all players from flight
                List<Flight2Player> Flight2Players = (await DataStoreFlight2Player.GetItemsAsync()).ToList();

                List<Flight2Player> CurrentFlightPlayers = Flight2Players.Where(x => x.FlightId == flightNumber).ToList();
                foreach(Flight2Player f2p in CurrentFlightPlayers)
                {
                    await DataStoreFlight2Player.DeleteItemAsync(f2p.Id);
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
                await DataStoreFlight2Player.AddItemAsync(f2p);
            }
        }

        private void WriteSumsFromDB(List<Result> SavedResults)
        {

            for (int i = 0; i < 4; i++)
            {
                if (InSumLabels[i] != null)
                {
                    // calculate sum for first 9
                    int inSumScore = 0;
                    int inSumPutts = 0;
                    for (int teeIndex =0; teeIndex < 9; teeIndex++)
                    {
                        var teeResult = SavedResults.Where(x => x.PlayerId == TournamentPageData.SelectedPlayers[i].Id).Where(y => y.TeeId == TournamentPageData.TeeList[teeIndex].Id).FirstOrDefault(); 
                        if(teeResult != null)
                        {
                            inSumScore = inSumScore + teeResult.Score;
                            inSumPutts = inSumPutts + teeResult.Putts;
                        }
                    }

                    InSumLabels[i].Text = inSumScore.ToString();
                    if (InSumPuttsLabels[i] != null)
                        InSumPuttsLabels[i].Text = inSumPutts.ToString();
                }
                if (OutSumLabels[i] != null)
                {
                    // calculate sum for last 9
                    int outSumScore = 0;
                    int outSumPutts = 0;
                    for (int teeIndex = 9; teeIndex < 18; teeIndex++)
                    {
                        var teeResult = SavedResults.Where(x => x.PlayerId == TournamentPageData.SelectedPlayers[i].Id).Where(y => y.TeeId == TournamentPageData.TeeList[teeIndex].Id).FirstOrDefault();
                        if (teeResult != null)
                        {
                            outSumScore = outSumScore + teeResult.Score;
                            outSumPutts = outSumPutts + teeResult.Putts;
                        }
                    }

                    OutSumLabels[i].Text = outSumScore.ToString();
                    if (OutSumPuttsLabels[i] != null)
                        OutSumPuttsLabels[i].Text = outSumPutts.ToString();
                }
                if (TotalSumLabels[i] != null)
                {
                    TotalSumLabels[i].Text = SavedResults.Where(x => x.PlayerId == TournamentPageData.SelectedPlayers[i].Id).Select(y => y.Score).Sum().ToString();                 
                }

                if (TotalSumPuttsLabels[i] != null)
                {
                    TotalSumPuttsLabels[i].Text = SavedResults.Where(x => x.PlayerId == TournamentPageData.SelectedPlayers[i].Id).Select(y => y.Putts).Sum().ToString();
                }
            }
        }

        private void SetCloseTournamentButtonEnabled()
        {
            List<Result> SavedResults = GetTournamentResults(TournamentPageData.Tournament.Id);

            bool buttonEnabled = true;

            foreach(TournamentPlayer p in TournamentPageData.SelectedPlayers)
            {
                int resultCount = SavedResults.Where(x => x.PlayerId == p.Id).Count();
                if(resultCount < TournamentPageData.TeeList.Count())
                {
                    buttonEnabled = false;
                    break;
                }
            }
            Button finishButton = (Button)this.FindByName("FinishTournament");
            if(buttonEnabled)
            {
                finishButton.IsEnabled = true;
            }
            else
            {
                finishButton.IsEnabled = false;
            }
        }

        private List<Result> GetTournamentResults(int tournamentId)
        {
            var ResultTask = DataStoreResults.GetItemsAsync();
            List<Result> Results = ResultTask.Result.ToList();
            return Results.Where(x => x.TournamentId == TournamentPageData.Tournament.Id).ToList();
        }
    }
}