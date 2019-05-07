using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OverwatchAPI;
using WinLossCounter.Properties;

namespace WinLossCounter
{

    public class Scorecard : ScorecardSettings, INotifyPropertyChanged
    {

        private Player _Player;
        public Player Player
        {
            get
            {
                using (var owClient = new OverwatchClient())
                    owClient.UpdatePlayerAsync(_Player);
                return _Player;
            }
            set
            {
                _Player = value;
            }
        }

        private int _StartSR;
        public int StartSR
        {
            get => _StartSR;
            set
            {
                _StartSR = value;
                NotifyPropertyChanged();
            }
        }


        private int _CurrentSR;
        public int CurrentSR
        {
            get => _CurrentSR;
            set
            {
                _CurrentSR = value;
                NotifyPropertyChanged();
            }
        }

        private int _WinCount;
        public int WinCount
        {
            get => _WinCount;
            set
            {
                if (value < 0 && _WinCount <= 0) return;
                WinStreak += value > 0 ? 1 : WinStreak <= 0 ? 0 : -1;
                LossStreak = 0;

                _WinCount = value;
                NotifyPropertyChanged();
            }
        }

        private int _DrawCount;
        public int DrawCount
        {
            get => _DrawCount;
            set
            {
                if (value < 0 && _DrawCount <= 0)
                {
                    _DrawCount = 0;
                }
                else
                {
                    _DrawCount = value;
                    if (DrawResetsWinStreak) WinStreak = 0;
                    if (DrawResetsLossStreak) LossStreak = 0;
                }


                NotifyPropertyChanged();
            }
        }

        private int _LossCount;
        public int LossCount
        {
            get => _LossCount;
            set
            {
                if (value < 0 && _LossCount <= 0) return;
                LossStreak += value > 0 ? 1 : LossStreak <= 0 ? 0 : -1;
                WinStreak = 0;

                _LossCount = value;
                NotifyPropertyChanged();
            }
        }

        private int _Winstreak;
        public int WinStreak
        {
            get => _Winstreak;
            set
            {
                if (value < 0 && _Winstreak <= 0)
                    _Winstreak = 0;
                else
                    _Winstreak = value;
                NotifyPropertyChanged();
            }
        }

        private int _LossStreak;
        public int LossStreak
        {
            get => _LossStreak;
            set
            {
                if (value < 0 && _LossStreak <= 0)
                    _LossStreak = 0;
                else
                    _LossStreak = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            MainWindow.Update_Content(this);
        }
    }

    public class ScorecardSettings
    {
        public bool DrawResetsWinStreak = false;
        public bool DrawResetsLossStreak = false;
    }
}
