﻿using StudPracticeAutumn2024.Controls;
using StudPracticeAutumn2024.DATABASE;
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

namespace StudPracticeAutumn2024.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientViewService.xaml
    /// </summary>
    public partial class ClientViewService : Page
    {
        public ClientViewService()
        {
            InitializeComponent();
            UpdatePage();
        }
        public void UpdatePage()
        {
            ClientServiceWpar.Children.Clear();
            foreach (var item in App.db.Service)
            {
                ClientServiceWpar.Children.Add(new ClientServiceUserControl(item));
            }
        }


        private void Button_Click_ViewService(object sender, RoutedEventArgs e)
        {

        }
    }
}
