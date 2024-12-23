using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ekzamen
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int[] temperatures;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CheckBtn_Click(object sender, RoutedEventArgs e)
        {
            int maxTemp = int.Parse(MaxTempTxt.Text);
            int minTemp = int.Parse(MinTempTxt.Text);
            int maxTempTime = int.Parse(OverheatTimeTxt.Text);
            int minTempTime = int.Parse(OvercoldTimeTxt.Text);
            DateTime shipmentDt = DateTime.Parse(ShipmentDateTxt.Text);
            int OverheatTime = 0;
            int OvercoldTime = 0;
            DateTime CurTime = shipmentDt;
            string[] temperatureStrings = TempChangeTxt.Text.Split(' ');
            try
            {
                temperatures = temperatureStrings.Select(temp => int.Parse(temp.Trim())).ToArray();
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка: вторая строка должна содержать целые числа");
                return;
            }
            if (maxTemp < minTemp)
            {
                int buffer = maxTemp;
                maxTemp = minTemp;
                minTemp = buffer;
                buffer = maxTempTime;
                maxTempTime = minTempTime;
                minTempTime = buffer;
            }
            string res = $"Дата отгрузки: {shipmentDt}\n";
            foreach (int t in temperatures)
            {
                CurTime.AddMinutes(10);
                if (t > maxTemp)
                {
                    OverheatTime += 10;
                    res += $"Текущее время: {CurTime}; Текущее отклонение: {t - maxTemp}\n";
                }
                if (t < minTemp)
                {
                    OvercoldTime += 10;
                    res += $"Текущее время: {CurTime}; Текущее отклонение: {t - minTemp}\n";
                }
                if (OvercoldTime > minTempTime)
                {
                    res += $"Текущее время: {CurTime}; Рыба была переохлаждена более {minTempTime} минут!\n";
                }
                if (OverheatTime > maxTempTime)
                {
                    res += $"Текущее время: {CurTime}; Рыба была перегрета более {maxTempTime} минут!\n";
                }
            }
            ReportResTxt.Text = res;
        }

        private void LoadDataBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FishKindSel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
