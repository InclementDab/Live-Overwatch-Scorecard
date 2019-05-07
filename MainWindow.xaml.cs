using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WinLossCounter.Properties;
using OverwatchAPI;
using System.Threading.Tasks;
using System.Windows.Media;
using System.ComponentModel;
using WinLossCounter;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace WinLossCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Scorecard _Scorecard;

        public MainWindow()
        {
            InitializeComponent();


            if (string.IsNullOrEmpty(Settings.Default.OutputFile)) Browse_Click(null, null);

            try
            {
                if (!File.Exists(Settings.Default.OutputFile)) File.Create(Settings.Default.OutputFile);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }

            if (Settings.Default.PreviousCard != null)
                if (MessageBox.Show("Load Previous SR Values?", "Previous SR Values Found", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _Scorecard.StartSR = Settings.Default.PreviousCard.StartSR;
                    _Scorecard.CurrentSR = Settings.Default.PreviousCard.CurrentSR;
                }

            DataContext = _Scorecard = new Scorecard();

        }

        private void Win_Add(object sender, RoutedEventArgs e) => _Scorecard.WinCount += 1;
        private void Win_Remove(object sender, RoutedEventArgs e) => _Scorecard.WinCount -= 1;

        private void Draw_Add(object sender, RoutedEventArgs e) => _Scorecard.DrawCount += 1;
        private void Draw_Remove(object sender, RoutedEventArgs e) => _Scorecard.DrawCount -= 1;

        private void Loss_Add(object sender, RoutedEventArgs e) => _Scorecard.LossCount += 1;
        private void Loss_Remove(object sender, RoutedEventArgs e) => _Scorecard.LossCount -= 1;

        private void Load_Click(object sender, RoutedEventArgs e) => _Scorecard = Settings.Default.PreviousCard;
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _Scorecard.WinCount = 0;
            _Scorecard.LossCount = 0;
            _Scorecard.DrawCount = 0;
        }





        public static void Update_Content(Scorecard scorecard)
        {
            File.WriteAllText(Settings.Default.OutputFile, ParseLine(Settings.Default.QueryData, scorecard));
        }


        private static string ParseLine(string inputString, Scorecard scorecard)
        {
            inputString = Parse(inputString, @"%w", scorecard.WinCount);
            inputString = Parse(inputString, @"%ws", scorecard.WinStreak);
            inputString = Parse(inputString, @"%l", scorecard.LossCount);
            inputString = Parse(inputString, @"%ls", scorecard.LossStreak);
            inputString = Parse(inputString, @"%d", scorecard.DrawCount);
            inputString = Parse(inputString, @"%sr", scorecard.StartSR);
            inputString = Parse(inputString, @"%csr", scorecard.CurrentSR);
            inputString = Parse(inputString, @"%chsr", (scorecard.CurrentSR - scorecard.StartSR).ToString("+#;-#;0"));
            return inputString;
        }

        public string[] HelpData =
        {
            "%w = Total Wins",
            "%l = Total Losses",
            "%d = Total Draws",
            "%ws = Current Win Streak",
            "%ls = Current Loss Streak",
            "%sr = Start SR",
            "%csr = Current SR",
            "%chsr = Difference in SR"
        };


        private static string Parse(string inputString, string pattern, object input)
        {
            return Regex.Replace(inputString, string.Format(@"{0}\b", pattern), input.ToString(), RegexOptions.IgnoreCase) ?? inputString;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Select Output File",
                Filter = "Text Files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if ((bool)dialog.ShowDialog())
                Settings.Default.OutputFile = dialog.FileName;

        }


        private bool UserLoggedIn = false;
        private async void Login_Click(object sender, RoutedEventArgs e) // this code is kinda spaghetti but it handles the login pretty well so imma leave it
        {
            var btn = sender as Button;
            if (UserLoggedIn)
            {
                _Scorecard.Player = null;
                UserLoggedIn = false;

                InfoBox_Modify("Logged Out");
                btn.Content = "Login";
                return;
            }

            try
            {
                OverwatchClient owClient = new OverwatchClient();
                InfoBox_Modify("Logging in...");
                _Scorecard.Player = await owClient.GetPlayerAsync(Settings.Default.BattleTag, Platform.Pc);
            }
            catch (Exception ex)
            {
                InfoBox_Modify("Error: " + ex.Message, Brushes.Red);
                return;
            }


            if (_Scorecard.Player.IsProfilePrivate)
            {
                InfoBox_Modify("Error: Private Profile", Brushes.Red);
            }
            else
            {
                InfoBox_Modify("Login Successful", Brushes.Green);
                _Scorecard.StartSR = _Scorecard.Player.CompetitiveRank;
                _Scorecard.CurrentSR = _Scorecard.StartSR;
                UserLoggedIn = true;
                btn.Content = "Logout";
            }
        }



        private void InfoBox_Modify(string text, Brush color = null)
        {
            InfoBox.Text = text;
            InfoBox.Foreground = color ?? Brushes.Black;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Settings.Default.PreviousCard = _Scorecard;
            Settings.Default.Save();
            base.OnClosing(e);
        }

        private void StartSR_Block_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentSR_Block.Foreground =
                _Scorecard.CurrentSR > _Scorecard.StartSR ? Brushes.Green :
                _Scorecard.CurrentSR < _Scorecard.StartSR ? Brushes.Red :
                Brushes.Black;
        }

        private bool Details_Opened;
        private DetailsAddin Details_Page = new DetailsAddin();
        private void Details_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            Details_Opened = !Details_Opened; // Toggle Bool

            btn.Content = Details_Opened ? 5 : 6;
            Height = Details_Opened ? 500 : 200; // Set Height

            if (Details_Opened)
                MainStackPanel.Children.Add(Details_Page);
            else
                MainStackPanel.Children.Remove(Details_Page);
        }
    }
}
