using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ekzamen
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int[] temperatures;
        string FilePath;
        public MainWindow()
        {
            InitializeComponent();
            FishKindSel.Items.Add("Сёмга");
            FishKindSel.Items.Add("Минтай");
        }

        private void CheckBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MaxTempTxt.Text))
            {
                MessageBox.Show("Введите максимальную температуру!");
                return;
            }
            if (string.IsNullOrEmpty(MinTempTxt.Text))
            {
                MessageBox.Show("Введите минимальную температуру!");
                return;
            }
            if (string.IsNullOrEmpty(OverheatTimeTxt.Text))
            {
                MessageBox.Show("Введите лимит времени перегрева!");
                return;
            }
            if (string.IsNullOrEmpty(OvercoldTimeTxt.Text))
            {
                MessageBox.Show("Введите лимит времени переохлаждения!");
                return;
            }
            if (string.IsNullOrEmpty(TempChangeTxt.Text))
            {
                MessageBox.Show("Введите график изменения температур!");
                return;
            }
            if (string.IsNullOrEmpty(ShipmentDateTxt.Text))
            {
                MessageBox.Show("Введите дату и время отгрузки!");
                return;
            }
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
            if (temperatures.Length < 18)
            {
                MessageBox.Show("Минимальное время транспортировки 3 часа!");
                return;
            }
            if (temperatures.Length > 54)
            {
                MessageBox.Show("Максимальное время транспортировки 9 часа!");
                return;
            }
            int maxTemp = int.Parse(MaxTempTxt.Text);
            int minTemp = int.Parse(MinTempTxt.Text);
            int maxTempTime = int.Parse(OverheatTimeTxt.Text);
            int minTempTime = int.Parse(OvercoldTimeTxt.Text);
            DateTime shipmentDt = DateTime.Parse(ShipmentDateTxt.Text);
            int OverheatTime = 0;
            int OvercoldTime = 0;
            DateTime CurTime = shipmentDt;
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
                CurTime = CurTime.AddMinutes(10);
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
            }
            if (OvercoldTime > minTempTime)
            {
                res += $"Текущее время: {CurTime}; Рыба была переохлаждена более {OvercoldTime} минут!\n";
                res += $"Максимально допустимое время переохлаждения - {minTempTime} минут!\n";
            }
            if (OverheatTime > maxTempTime)
            {
                res += $"Текущее время: {CurTime}; Рыба была перегрета более {OverheatTime} минут!\n";
                res += $"Максимально допустимое время перегрева - {maxTempTime} минут!\n";
            }
            ReportResTxt.Text = res;
        }

        private void LoadDataBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл",
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
            }
            ReadData();
        }
        public void ReadData()
        {
            DateTime startDateTime;
            if (!File.Exists(FilePath))
            {
                MessageBox.Show("Ошибка: файл не найден.");
                return;
            }
            string[] fileLines = File.ReadAllLines(FilePath);
            if (fileLines.Length != 2)
            {
                MessageBox.Show("Ошибка: файл должен содержать ровно две строки.");
                return;
            }
            string[] temperatureStrings = fileLines[1].Split(' ');
            try
            {
                temperatures = temperatureStrings.Select(temp => int.Parse(temp.Trim())).ToArray();
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка: вторая строка должна содержать целые числа");
                return;
            }
            startDateTime = DateTime.Parse(fileLines[0]);
            ShipmentDateTxt.Text = startDateTime.ToString();
            TempChangeTxt.Text = string.Join(" ", temperatures);

        }
        private void SaveReport_Click(object sender, RoutedEventArgs e)
        {
            SaveResultToFile(ReportResTxt.Text);
        }

        public void SaveResultToFile(string res)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Сохранить результат",
                FileName = "Result.txt"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, res);
                MessageBox.Show("Результат успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void FishKindSel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FishKindSel.SelectedIndex == 0)
            {
                MaxTempTxt.Text = "5";
                MinTempTxt.Text = "-3";
                OvercoldTimeTxt.Text = "60";
                OverheatTimeTxt.Text = "20";
            }
            if (FishKindSel.SelectedIndex == 1)
            {
                MaxTempTxt.Text = "-5";
                MinTempTxt.Text = "-180";
                OvercoldTimeTxt.Text = "5";
                OverheatTimeTxt.Text = "120";
            }
        }

        private void MinTempTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void MaxTempTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void OvercoldTimeTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void OverheatTimeTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void TempChangeTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
