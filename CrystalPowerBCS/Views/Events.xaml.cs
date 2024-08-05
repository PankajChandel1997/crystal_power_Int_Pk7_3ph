using CrystalPowerBCS.Helpers;
using CrystalPowerBCS.Views.Event;
using System.Reflection.Metadata;
using System.Windows.Controls;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for Event.xaml
    /// </summary>
    public partial class Events : UserControl
    {
        public Events()
        {
            InitializeComponent();
            EventHostGrid.Children.Add(new EventControlRealted());
            EventGridType.SelectedIndex = 1;
        }
      
        private void cbNumberOfRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EventHostGrid.Children.Clear();

            ComboBox typeItem = EventGridType;
            string value = typeItem.SelectedItem.ToString();

            switch (value)
            {
                case Constants.All:
                    EventHostGrid.Children.Add(new AllEventsRelated());
                    break;

                case Constants.ConnectDisconnect:
                    EventHostGrid.Children.Add(new EventControlRealted());
                    break;

                case Constants.CurrentRelated:
                    EventHostGrid.Children.Add(new EventCurrentrelated());
                    break;

                case Constants.NonRollOver:
                    EventHostGrid.Children.Add(new EventNonRollOver());
                    break;

                case Constants.OthersRelated:
                    EventHostGrid.Children.Add(new EventOtherRelated());
                    break;

                case Constants.PowerRelated:
                    EventHostGrid.Children.Add(new EventPowerrelated());
                    break;

                case Constants.TransactionRelated:
                    EventHostGrid.Children.Add(new EventtransactionRelated());
                    break;

                case Constants.VoltageRelated:
                    EventHostGrid.Children.Add(new Eventvoltagerelated());
                    break;

                case Constants.DIEvent:
                    EventHostGrid.Children.Add(new EventDIRelated());
                    break;

                default:
                    break;
            }
        }
    }
}
