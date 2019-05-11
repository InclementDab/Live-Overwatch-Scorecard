using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WinLossCounter
{

    public class ViewModel
    {

        private Scorecard _Scorecard;
        public Scorecard Scorecard
        {
            get => _Scorecard;
            set
            {
                _Scorecard = value;
            }
        }

        public ViewModel()
        {
            Scorecard = new Scorecard();
        }
    }
}
