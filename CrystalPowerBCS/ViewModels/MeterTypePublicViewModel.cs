using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CrystalPowerBCS.ViewModels
{
    public class MeterTypePublicViewModel : INotifyPropertyChanged
    {
        //Meter Type
        private string meterType;

        public string MeterType
        {
            get { return meterType; }
            set
            {
                meterType = value;
                NotifyPropertyChanged();
            }
        }


        // Current Window Height
        private string currentHeight = "0";

        public string CurrentHeight
        {
            get { return currentHeight; }
            set
            {
                currentHeight = value;
                NotifyPropertyChanged();
            }
        }

        //Global date
        private string selectedDate = "";

        public string SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                NotifyPropertyChanged();
            }
        }

        //Command To Notify

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
    }
}
