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

        Label[] InSumPuttsLabels = null;
        Label[] OutSumPuttsLabels = null;
        Label[] TotalSumPuttsLabels = null;

        int LastPlayerId = 0;

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

            if (TournamentPageData.SelectedPlayers.Count() == 0)
                return;

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
                if(columnIndex==0)
                    ScoreKarte.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) });
                else
                    ScoreKarte.ColumnDefinitions.Add(new ColumnDefinition());

                for (int rowIndex = 0; rowIndex < rowcount; rowIndex++)
                {
                    if (columnIndex == 0)
                    { 
                        //if (teeCount == 9)
                            ScoreKarte.RowDefinitions.Add(new RowDefinition());
                        //else
                        //    ScoreKarte.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
                    }

                    if ((rowIndex == 0) && (columnIndex > 0))
                    {
                        // first row header row with player intials
                        AddHeaderPuttsLabelCell(TournamentPageData.SelectedPlayers[columnIndex - 1].Initials, columnIndex, rowIndex);
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
                            if (SavedResult!=null)
                            {
                                savedScore = SavedResult.Score.ToString();
                                savedPutts = SavedResult.Putts.ToString();
                            }
                            string entryParameter = rowIndex.ToString() + "_" + columnIndex.ToString();
                            AddEntryCell(entryParameter, columnIndex, rowIndex, savedScore, savedPutts);
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
                            var subPuttsStackLayout = new StackLayout
                            {
                                BackgroundColor = Color.White,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand
                            };

                            Label ScoreLabel = AddLabelCell("", subScoreStackLayout);
                            Label PuttsLabel = AddLabelCell("", subPuttsStackLayout);

                            StackLayout.Children.Add(subScoreStackLayout);
                            StackLayout.Children.Add(subPuttsStackLayout);

                            ScoreKarte.Children.Add(StackLayout, columnIndex, rowIndex);

                            InSumLabels[columnIndex - 1] = ScoreLabel;
                            InSumPuttsLabels[columnIndex - 1] = PuttsLabel;
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
                            if (SavedResult != null)
                            {
                                savedScore = SavedResult.Score.ToString();
                                savedPutts = SavedResult.Putts.ToString();
                            }

                            string entryParameter = (rowIndex - 1).ToString() + "_" + columnIndex.ToString();
                            AddEntryCell(entryParameter, columnIndex, rowIndex, savedScore, savedPutts);
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
                            var subPuttsStackLayout = new StackLayout
                            {
                                BackgroundColor = Color.White,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand
                            };

                            Label ScoreLabel = AddLabelCell("", subScoreStackLayout);
                            Label PuttsLabel = AddLabelCell("", subPuttsStackLayout);

                            StackLayout.Children.Add(subScoreStackLayout);
                            StackLayout.Children.Add(subPuttsStackLayout);

                            ScoreKarte.Children.Add(StackLayout, columnIndex, rowIndex);

                            OutSumLabels[columnIndex - 1] = ScoreLabel;
                            OutSumPuttsLabels[columnIndex - 1] = PuttsLabel;
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
                            var subPuttsStackLayout = new StackLayout
                            {
                                BackgroundColor = Color.White,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand
                            };

                            Label ScoreLabel = AddLabelCell("", subScoreStackLayout);
                            Label PuttsLabel = AddLabelCell("", subPuttsStackLayout);

                            StackLayout.Children.Add(subScoreStackLayout);
                            StackLayout.Children.Add(subPuttsStackLayout);

                            ScoreKarte.Children.Add(StackLayout, columnIndex, rowIndex);

                            TotalSumLabels[columnIndex - 1] = ScoreLabel;
                            TotalSumPuttsLabels[columnIndex - 1] = PuttsLabel;
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
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor=Color.LightGray
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
            var subScorePuttHeaderStackLayout = new StackLayout
            {
                Orientation= StackOrientation.Horizontal
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
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.None
            };
            var puttsLabel = new Label
            {
                Text = "Putts",
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.None
            };

            subScoreHeaderStackLayout.Children.Add(scoreLabel);
            subPuttsHeaderStackLayout.Children.Add(puttsLabel);

            subScorePuttHeaderStackLayout.Children.Add(subScoreHeaderStackLayout);
            subScorePuttHeaderStackLayout.Children.Add(subPuttsHeaderStackLayout);

            stackLayout.Children.Add(subScorePuttHeaderStackLayout);
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

        private void AddEntryCell(string CommandParameter, int ColumnIndex, int RowIndex, string EntryText, string EntryText1)
        {
            var stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            var subStackLayout = new StackLayout
            {
                BackgroundColor=Color.Gray,
                Orientation=StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            var subsubScoreStackLayout = new StackLayout
            {
                BackgroundColor=Color.White,
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
                Text= EntryText,
                HorizontalTextAlignment=TextAlignment.Center,
                ReturnCommandParameter = CommandParameter,
                FontSize=20,
                MinimumWidthRequest = 100,
                Keyboard= Keyboard.Numeric
            };

            entry.TextChanged += Entry_TextChanged;

            stackLayoutScore.Children.Add(entry);

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
                MinimumWidthRequest = 100,
                Keyboard = Keyboard.Numeric
            };

            entryPutts.TextChanged += Entry_TextChanged;

            stackLayoutScore.Children.Add(entry);
            stackLayoutPutts.Children.Add(entryPutts);

            subsubScoreStackLayout.Children.Add(stackLayoutScore);
            subsubPuttsStackLayout.Children.Add(stackLayoutPutts);

            subStackLayout.Children.Add(subsubScoreStackLayout);
            subStackLayout.Children.Add(subsubPuttsStackLayout);

            stackLayout.Children.Add(subStackLayout);
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

            string currentCommand = ((Entry)sender).ReturnCommandParameter.ToString();
            bool isScore = true;
            if(currentCommand.StartsWith("putts"))
            {
                isScore = false;
                currentCommand = currentCommand.Replace("putts_", "");
            }


            string[] currentCommandParameter = currentCommand.Split('_');
            int currentrowIndex = Convert.ToInt32(currentCommandParameter[0].ToString());
            int currentColumnIndex = Convert.ToInt32(currentCommandParameter[1].ToString());
            int currentPlayerId = TournamentPageData.SelectedPlayers[currentColumnIndex - 1].Id;

            SaveResultLocal(newText, currentColumnIndex, currentrowIndex, isScore);

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
                            PlayerId = playerId,
                            TournamentId = tournamentId,
                            TeeId = teeId,
                            Score = score,
                            Putts = putts
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

            // send results to webapp
            SaveResultsWeb(TournamentResultList);
        }

        private void SaveResultLocal(string NewText, int ColumnIndex, int RowIndex, bool score)
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
                if (score)
                {
                    res.Score = Convert.ToInt32(NewText);
                }
                else
                {
                    res.Putts = Convert.ToInt32(NewText);
                }
                
                DataStore.UpdateItemAsync(res);
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

        private void ContentPageResults_Appearing(object sender, EventArgs e)
        {
            TournamentPageData.PlayerResults.Clear();
            IDataStore<Result> DataStore = DependencyService.Get<IDataStore<Result>>();
            var ResultTask = DataStore.GetItemsAsync();
            List<Result> Results = ResultTask.Result.ToList();
            List<Result> SavedResults = Results.Where(x => x.TournamentId == TournamentPageData.Tournament.Id).ToList();

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
                        if(PlayerResult != null)
                        {
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
                            int Handicap = Convert.ToInt32(tp.Handicap);


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

                            NettoScore += PlayerResult.Score - (nPar - t.Par) ;


                            //Nettopunkte

                            int npoints = 0;
                            npoints = nPar - PlayerResult.Score + 2;

                            if (points > 0)
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
        }
    }
}