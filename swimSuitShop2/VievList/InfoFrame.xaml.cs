using swimSuitShop2.Classes;
using swimSuitShop2.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
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

namespace swimSuitShop2.VievList
{
    /// <summary>
    /// Логика взаимодействия для InfoFrame.xaml
    /// </summary>
    public partial class InfoFrame : Page
    {
        public InfoFrame(string name, string cost, BitmapImage photo, string uid, string size, string material, string structure, string information)
        {
            InitializeComponent();

            ItemName.Text = name;
            ItemCost.Text = $"Цена: {cost}";
            ItemPhoto.Source = photo;
            ItemUid.Text = $"UID: {uid}";
            ItemSize.Text = $"Размеры: {size}";
            ItemMaterial.Text = $"Состав: {material}";
            ItemStructure.Text = $"В комплекте {structure}";
            ItemInformation.Text = $"Информация: {information}";
        }

        private void closeInfo(object sender, RoutedEventArgs e)
        {
            Content = null;
        }
    }
}
