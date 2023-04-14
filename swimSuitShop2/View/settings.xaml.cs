using swimSuitShop2.VievList;
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

namespace swimSuitShop2.View
{
    /// <summary>
    /// Логика взаимодействия для settings.xaml
    /// </summary>
    public partial class settings : Window
    {
        public settings()
        {
            InitializeComponent();
            SettingsFrame.Content = new AddItem();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Windows[0].Title = "MainWindow";
            foreach (Window window in App.Current.Windows)
            {
                if (!(window is MainWindow))
                    window.Close();
            }
        }

        private void AddList(object sender, RoutedEventArgs e)
        {
            SettingsFrame.Content = new AddItem();
        }

        private void SettingList(object sender, RoutedEventArgs e)
        {
            SettingsFrame.Content = new SettingsItem();
        }

        private void DelList(object sender, RoutedEventArgs e)
        {
            SettingsFrame.Content = new delItem();
        }
    }
}
