using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using StudPracticeAutumn2024.DATABASE;

namespace StudPracticeAutumn2024
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SchollEntities db = new SchollEntities();
    }
}
