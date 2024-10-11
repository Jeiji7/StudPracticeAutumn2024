using StudPracticeAutumn2024.Controls;
using StudPracticeAutumn2024.DATABASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace StudPracticeAutumn2024.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientServiseListView.xaml
    /// </summary>
    
    public partial class ClientServiseListView : Page
    {
        private DispatcherTimer dispatcherTimer;

        private List<ClientService> clientSer = new List<ClientService>();
        public bool activeRecord;
        public ClientServiseListView()
        {
            InitializeComponent();
            SortingDateTimeCB.SelectedIndex = 2;
            UpdateData();
            StartAutoUpdate();
        }

        private void StartAutoUpdate()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(30); // Интервал 30 секунд
            dispatcherTimer.Tick += dispatcherTimer_Tick; // Обработка события Tick
            dispatcherTimer.Start(); // Запуск таймера
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Здесь вызываем метод, который нужно обновлять
            UpdateData();
        }

        public void UpdateData()
        {
            ServiceWpar.Children.Clear();
            clientSer = App.db.ClientService.ToList();
            SortingDateTime();
            foreach (var item in clientSer)
            {
                ServiceWpar.Children.Add(new RecordUserControl(item, activeRecord));
            }
        }

        private void SortingDateTime()
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime currentTime = DateTime.Now;
            DateTime tomorrow = currentDate.AddDays(1);

            // Проверяем, что выбрано в ComboBox
            switch (SortingDateTimeCB.SelectedIndex)
            {
                case 0: // "Сегодня"
                    clientSer = clientSer
                        .Where(c => c.StartTime.Date == currentDate)
                        .OrderBy(c => c.StartTime)
                        .ToList();
                    ColorTextBlock();
                    break;
                case 1: // "Завтра"
                    clientSer = clientSer
                        .Where(c => c.StartTime.Date == tomorrow)
                        .OrderBy(c => c.StartTime)
                        .ToList();
                    ColorTextBlock();
                    break;
                case 2: // "Все"
                    clientSer = App.db.ClientService
                        .OrderByDescending(c => c.StartTime)
                        .ToList();
                    ColorTextBlock();
                    break;
            }
        }
        private void SortingDateTimeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void ColorTextBlock()
        {
            DateTime currentTime = DateTime.Now;
            foreach (var client in clientSer)
            {
                if (client.StartTime.Subtract(currentTime).TotalHours <= 1 && client.StartTime > currentTime)
                {
                    activeRecord = true;
                }
                else
                {
                    activeRecord = false;
                }
            }
        }


        private void Button_Click_GoBack(object sender, RoutedEventArgs e)
        {
            if (dispatcherTimer != null && dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
                //MessageBox.Show("Таймер остановлен!");
            }
            NavigationService.Navigate(new Pages.EnterPage());
        }

    }
}
