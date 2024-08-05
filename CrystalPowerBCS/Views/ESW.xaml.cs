using CrystalPowerBCS.DbFunctions.EventSinglePhaseFunctions;
using CrystalPowerBCS.DbFunctions.EventThreePhaseFunctions;
using CrystalPowerBCS.Helpers;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CrystalPowerBCS.Views
{
    /// <summary>
    /// Interaction logic for ESW.xaml
    /// </summary>
    public partial class ESW : UserControl
    {
        private int _numValue = 0;

        public int NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                txtNum.Text = value.ToString();
            }
        }
        public bool IsGraph;
        public bool IsFilterEnabled;
        public ObservableCollection<ESWSinglePhaseDto> ESWSinglePhase;
        public ESWSinglePhaseCommand ESWSinglePhaseCommand;
        public ObservableCollection<ESWThreePhaseDto> ESWThreePhase;
        public ESWThreePhaseCommand ESWThreePhaseCommand;
        public string MeterType;
        public int pageSize = 10;
        public ESW()
        {
            InitializeComponent();

            string MeterTypess = MeterPhaseType.Text;

            DataTableGrid.Height = 667;

            MainGraphView.Height = 667;

            //If Meter is single Phase Disply Single Phase Grid else three phase grid
            ESWSingleGrid.Visibility = Visibility.Visible;
            ESWThreeGrid.Visibility = Visibility.Collapsed;

            graphView.Visibility = Visibility.Collapsed;
            graphViewDisabled.Visibility = Visibility.Visible;
            listView.Visibility = Visibility.Visible;
            listView.IsHitTestVisible = false;
            listViewDisabled.Visibility = Visibility.Collapsed;

            downarrow.Visibility = Visibility.Visible;
            uparrow.Visibility = Visibility.Collapsed;

            Filters.Visibility = Visibility.Collapsed;

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ChangeView(object sender, RoutedEventArgs e)
        {
            IsGraph = !IsGraph;
            if (IsGraph == true)
            {
                TableView.Visibility = Visibility.Collapsed;
                listView.Visibility = Visibility.Collapsed;
                listViewDisabled.Visibility = Visibility.Visible;
                GraphView.Visibility = Visibility.Visible;
                graphView.Visibility = Visibility.Visible;
                graphView.IsHitTestVisible = false;
                graphViewDisabled.Visibility = Visibility.Collapsed;

                double[] dataX1 = new double[] { 0, 2, 3, 4, 5 };
                double[] dataY1 = new double[] { 0, 4, 9, 16, 25 };

                double[] dataX2 = new double[] { 0, 4, 6, 7, 8 };
                double[] dataY2 = new double[] { 0, 12, 19, 26, 35 };

                IPGraph.Plot.AddScatter(dataX1, dataY1);
                IPGraph.Plot.AddScatter(dataX2, dataY2);
                IPGraph.Plot.Palette = ScottPlot.Palette.Frost;
                IPGraph.Refresh();
            }
            else
            {
                TableView.Visibility = Visibility.Visible;
                listView.Visibility = Visibility.Visible;
                listView.IsHitTestVisible = false;
                listViewDisabled.Visibility = Visibility.Collapsed;
                GraphView.Visibility = Visibility.Collapsed;
                graphView.Visibility = Visibility.Collapsed;
                graphViewDisabled.Visibility = Visibility.Visible;
            }
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbNumberOfRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string startDate = FilterDatePicker != null ? FilterDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string endDate = FilterToDatePicker != null ? FilterToDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;
            this.Dispatcher.Invoke(async () =>
            {
                await Filter(startDate, endDate, pageSize);;
            });
        }

        private void CmdUp_Click(object sender, RoutedEventArgs e)
        {
            NumValue++;
        }

        private void CmdDown_Click(object sender, RoutedEventArgs e)
        {
            NumValue--;
        }

        private void TxtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum == null)
            {
                return;
            }

            if (!int.TryParse(txtNum.Text, out _numValue))
                txtNum.Text = _numValue.ToString();
        }

        private void TxtNum_TextChanged1(object sender, TextChangedEventArgs e)
        {
            if (txtNum1 == null)
            {
                return;
            }

            if (!int.TryParse(txtNum1.Text, out _numValue))
                txtNum1.Text = _numValue.ToString();
        }

        private void ShowHideFilter(object sender, MouseButtonEventArgs e)
        {
            IsFilterEnabled = !IsFilterEnabled;
            if (IsFilterEnabled)
            {
                downarrow.Visibility = Visibility.Collapsed;
                uparrow.Visibility = Visibility.Visible;

                Filters.Visibility = Visibility.Visible;

                DataTableGrid.Height = 374;

                MainGraphView.Height = 374;
            }
            else
            {
                downarrow.Visibility = Visibility.Visible;
                uparrow.Visibility = Visibility.Collapsed;

                Filters.Visibility = Visibility.Collapsed;

                DataTableGrid.Height = 667;

                MainGraphView.Height = 667;
            }
        }

        private async void FilterByDate(object sender, RoutedEventArgs e)
        {
            string startDate = FilterDatePicker != null ? FilterDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string endDate = FilterToDatePicker != null ? FilterToDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                pageSize = gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;

                await Filter(startDate,endDate, pageSize);
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    FilterByPageSize(null, null);
                });

            }
        }

        private void FilterByPageSize(object sender, SelectionChangedEventArgs e)
        {
            string startDate = FilterDatePicker.SelectedDate != null ? FilterDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            string endDate = FilterToDatePicker.SelectedDate != null ? FilterToDatePicker.SelectedDate.Value.ToString("dd-MM-yyyy") : null;
            pageSize = gridPageSize != null && gridPageSize.SelectedValue.ToString() != "All" ? Convert.ToInt32(gridPageSize.SelectedValue.ToString()) : 99999;

            this.Dispatcher.Invoke(async () =>
            {
                await Filter(startDate, endDate, pageSize);;
            });
        }

        private async Task Filter(string startDate, string endDate, int page)
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                ESWSinglePhaseCommand = new ESWSinglePhaseCommand();
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                ESWThreePhaseCommand = new ESWThreePhaseCommand();
            }

            return;
        }

        private async Task PopullateGrid()
        {
            if (MeterType == Constants.SinglePhaseMeter)
            {
                ESWSinglePhaseCommand = new ESWSinglePhaseCommand();
            }
            else if (MeterType == Constants.ThreePhaseMeter)
            {
                ESWThreePhaseCommand = new ESWThreePhaseCommand();
            }
        }

        private async Task BindESWSinglePhaseDto(List<ESWSinglePhaseDto> gridData)
        {
            try
            {
                ESWSinglePhase = new ObservableCollection<ESWSinglePhaseDto>();

                int index = 1;
                foreach (var item in gridData)
                {
                    ESWSinglePhase.Add(new ESWSinglePhaseDto
                    {
                        //Number = index,
                        OverVoltage = item.OverVoltage,
                        LowVoltage = item.LowVoltage,
                        OverCurrent = item.OverCurrent,
                        VerylowPF = item.VerylowPF,
                        EarthLoading = item.EarthLoading,
                        InfluenceOfPermanetMagnetOorAcDc = item.InfluenceOfPermanetMagnetOorAcDc,
                        NeutralDisturbance = item.NeutralDisturbance,
                        MeterCoverOpen = item.MeterCoverOpen,
                        MeterLoadDisconnectConnected = item.MeterLoadDisconnectConnected,
                        LastGasp = item.LastGasp,
                        FirstBreath = item.FirstBreath,
                        IncrementInBillingCounterMRI = item.IncrementInBillingCounterMRI,

                    });
                    index++;
                }

                ESWSingleGrid.ItemsSource = ESWSinglePhase;
            }
            catch (Exception ex)
            {

            }
            
        }

        private async Task BindESWThreePhaseDto(List<ESWThreePhaseDto> gridData)
        {
            try
            {
                ESWThreePhase = new ObservableCollection<ESWThreePhaseDto>();

                int index = 1;
                foreach (var item in gridData)
                {
                    ESWThreePhase.Add(new ESWThreePhaseDto
                    {
                        //Number = index,
                        RPhaseVoltageMissing = item.RPhaseVoltageMissing,
                        YPhaseVoltageMissing = item.YPhaseVoltageMissing,
                        BPhaseVoltageMissing = item.BPhaseVoltageMissing,
                        OverVoltage = item.OverVoltage,
                        LowVoltage = item.LowVoltage,
                        VoltagUnbalance = item.VoltagUnbalance,
                        RPhaseCurrentReverse = item.RPhaseCurrentReverse,
                        YPhaseCurrentReverse = item.YPhaseCurrentReverse,
                        BPhaseCurrentReverse = item.BPhaseCurrentReverse,
                        CurrentUnbalance = item.CurrentUnbalance,
                        CurrentBypass = item.CurrentBypass,
                        OverCurrent = item.OverCurrent,
                        VerylowPF = item.VerylowPF,
                        InfluenceOfPermanetMagnetOorAcDc = item.InfluenceOfPermanetMagnetOorAcDc,
                        NeutralDisturbance = item.NeutralDisturbance,
                        MeterCoverOpen = item.MeterCoverOpen,
                        MeterLoadDisconnectConnected = item.MeterLoadDisconnectConnected,
                        LastGasp = item.LastGasp,
                        FirstBreath = item.FirstBreath,
                        IncrementInBillingCounterMRI = item.IncrementInBillingCounterMRI,

                    });
                    index++;
                }

                ESWThreeGrid.ItemsSource = ESWThreePhase;
            }
            catch (Exception ex)
            {

            }
           
        }

        private void ChangeGridType(object sender, TextChangedEventArgs e)
        {
            MeterType = MeterPhaseType.Text.ToString();
            if (MeterType == Constants.SinglePhaseMeter)
            {
                ESWThreeGrid.Visibility = Visibility.Collapsed;
                ESWSingleGrid.Visibility = Visibility.Visible;
            }
            else
            {
                ESWSingleGrid.Visibility = Visibility.Collapsed;
                ESWThreeGrid.Visibility = Visibility.Visible;
            }

            this.Dispatcher.Invoke(async () =>
            {
                await PopullateGrid();
            });

        }
    }
}
