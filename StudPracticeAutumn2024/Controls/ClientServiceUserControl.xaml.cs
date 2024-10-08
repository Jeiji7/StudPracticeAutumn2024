﻿using StudPracticeAutumn2024.DATABASE;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace StudPracticeAutumn2024.Controls
{
    //public class People
    //{
    //    public int Id { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Patronymic { get; set; }
    //}


    public partial class ClientServiceUserControl : UserControl
    {
        private string text;
        private NavigationService _navigationService;
        private Service ser;
        public ClientServiceUserControl(Service service)
        {
            InitializeComponent();
            ser = service;
            TitleServiceTB.Text = ser.Title.ToString();
            LoadDataComboBox();



            // Получить путь к папке "ресурс" относительно папки, в которой находится исполняемый файл
            var imagesBD = App.db.ServicePhoto.FirstOrDefault(x => x.ID == ser.ServicePhotoID).PhotoPath.ToString();
            string folderName = "StudPracticeAutumn2024/Resource";
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string fullPath = System.IO.Path.Combine(projectDirectory, folderName, imagesBD);

            //Заменяем обратные слеши на прямые слеши
            ImageService.Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute));

            if (ser.Discount != null)
            {
                //Зачёркнутый текст
                textDecorate.Text = $"{ser.Cost.Value.ToString("0.#")}";
                textDecorate.TextDecorations = TextDecorations.Strikethrough;

                CostAndTimeTB.Text = $"{((((double?)ser.Cost) - ((double?)ser.Cost) * ser.Discount / 100)).ToString()} рублей за {ser.DurationInMinutes.ToString()} минут";
                DiscountTB.Text = $"* скидка {ser.Discount.ToString()}%";
            }
            else
            {
                myGrid.Background = new SolidColorBrush(Colors.LightBlue);
                CostAndTimeTB.Text = $"{ser.Cost.Value.ToString("0.#")} рублей за {ser.DurationInMinutes.ToString()} минут";
                DiscountTB.Text = "";
            }
        }
        private void NavigateTo(object content)
        {
            Window window = Window.GetWindow(this);

            if (window == null)
                return;
            Frame mainFrame = LogicalTreeHelper.FindLogicalNode(window, "MainFrame") as Frame;
            mainFrame?.Navigate(content);
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            if (myTextBox.Text == "Время записи")
            {
                myTextBox.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(myTextBox.Text))
            {
                myTextBox.Text = "Время записи";
            }
        }
        //чертовский TextBox TimePicker
        private void myTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            text = myTextBox.Text;

            // Запрещаем ввод более 5 символов
            if (text.Length >= 5)
            {
                e.Handled = true;
                return;
            }

            if (text.Length < 2)
            {
                // Первый символ должен быть от 0 до 2
                if (text.Length == 0 && !Regex.IsMatch(e.Text, @"[0-2]"))
                {
                    e.Handled = true;
                }
                // Второй символ зависит от первого: если 0 или 1, то от 0 до 9, если 2, то от 0 до 3
                else if (text.Length == 1)
                {
                    if (text == "2" && !Regex.IsMatch(e.Text, @"[0-3]"))  // Часы до 23
                    {
                        e.Handled = true;
                    }
                    else if (text != "2" && !Regex.IsMatch(e.Text, @"[0-9]"))
                    {
                        e.Handled = true;
                    }
                }
            }
            // Третий символ должен быть двоеточием
            else if (text.Length == 2)
            {
                // Принудительно добавляем двоеточие и запрещаем передавать введенный символ дальше
                if (e.Text != ":")
                {
                    myTextBox.Text += ":";
                    myTextBox.CaretIndex = myTextBox.Text.Length;  // Устанавливаем курсор в конец текста
                    e.Handled = true;  // Игнорируем вводимый символ
                }
            }
            // Проверка четвертого символа (первые цифры минут)
            else if (text.Length == 3)
            {
                if (!Regex.IsMatch(e.Text, @"[0-5]"))  // Первое число минут от 0 до 5
                {
                    e.Handled = true;
                }
            }
            // Проверка пятого символа (вторые цифры минут)
            else if (text.Length == 4)
            {
                if (!Regex.IsMatch(e.Text, @"[0-9]"))  // Второе число минут от 0 до 9
                {
                    e.Handled = true;
                }
            }

            // Запрещаем ввод любых символов, кроме цифр и двоеточия
            if (!e.Handled)
            {
                e.Handled = !Regex.IsMatch(e.Text, @"[0-9:]");
            }

            text += ":00";
        }

        public void LoadDataComboBox()
        {
            try
            {
                var context = App.db.Client;
                // Создаем список объединенных строк (FirstName, LastName, Patronymic)
                var peopleList = context
                    .Select(p => new
                    {
                        FullName = p.FirstName + " " + p.LastName + " " + (p.Patronymic ?? "")
                    })
                    .ToList();

                // Привязываем список к ComboBox через ItemSource
                ListFIOCB.ItemsSource = peopleList.Select(p => p.FullName).ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message);
            }
        }

        private void Button_Click_Registration(object sender, RoutedEventArgs e)
        {
            ClientService clientService = new ClientService();
            var selectedDate = DateDP.SelectedDate; // Получаем выбранную дату из DatePicker
            try
            {
                clientService.ClientID = ListFIOCB.SelectedIndex + 1;
                clientService.ServiceID = ser.ID;
                if (selectedDate.HasValue) // Проверяем, выбрана ли дата
                {
                    // Объединяем дату и время
                    var dateTimeString = $"{selectedDate.Value.ToString("yyyy-MM-dd")} {text}"; // text = "10:10:00"
                    clientService.StartTime = DateTime.Parse(dateTimeString); // Преобразуем в DateTime
                }

                App.db.ClientService.Add(clientService);
                App.db.SaveChanges();
                MessageBox.Show("Запись прошла успешно!");
                NavigateTo(new Pages.ClientViewService());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Ошибка формата: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
