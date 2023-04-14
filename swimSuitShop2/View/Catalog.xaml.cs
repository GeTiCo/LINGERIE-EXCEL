using swimSuitShop2.VievList;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace swimSuitShop2.View
{
    /// <summary>
    /// Логика взаимодействия для Catalog.xaml
    /// </summary>
    public partial class Catalog : Window
    {
        //Глобальные параметры------------------------------------------------------------
        /*Листы: категорий товаров, товаров из категорий и выбранных товаров*/
        public static List<Classes.Product> listProducts;
        //Основные функции------------------------------------------------------------
        public Catalog()
        {
            InitializeComponent();

            listCategory.Items.Clear();
            listCategory.ItemsSource = App.makeCategoryList();
        }

        private void listCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.activeCategory = listCategory.SelectedItem.ToString();

            listProducts = new List<Classes.Product>();
            try
            {
                App.excelWorkSheet = (Excel.Worksheet)App.excelWorkBook.Worksheets.get_Item(App.activeCategory);
                App.excelRange = App.excelWorkSheet.UsedRange;
            }
            catch
            {
                MessageBox.Show("Ошибка на стороне Excel");
            }

            for (int row = 1; row <= App.excelRange.Rows.Count; row++)
            {
                Classes.Product product = new Classes.Product();

                product.Name = Convert.ToString(App.excelRange.Cells[row, 1].value2);
                product.Cost = Convert.ToUInt16(App.excelRange.Cells[row, 2].value2);
                product.Uid = Convert.ToString(App.excelRange.Cells[row, 3].value2);
                product.Size = Convert.ToString(App.excelRange.Cells[row, 4].value2);
                product.Material = Convert.ToString(App.excelRange.Cells[row, 5].value2);
                product.Structure = Convert.ToString(App.excelRange.Cells[row, 6].value2);
                product.Information = Convert.ToString(App.excelRange.Cells[row, 7].value2);

                string url = App.pathExe + $@"/photo/{App.activeCategory}/{product.Name}.png";
                string def = App.pathExe + @"/default.png";

                try
                {
                    if (File.Exists(url))
                    {
                        product.Photo = App.ShowImageBit(url);
                    }
                    else
                    {
                        product.Photo = App.ShowImageBit(def);
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка обработки URL изображений");
                }


                listProducts.Add(product);
            }

            listViewProducts.ItemsSource = listProducts;
        }

        private void MoreInfo(object sender, RoutedEventArgs e)
        {
            try
            {
                Classes.Product product = (sender as Hyperlink).DataContext as Classes.Product;

                newFrame.Content = new InfoFrame(product.Name, Convert.ToString(product.Cost), product.Photo,
                    product.Uid, product.Size, product.Material, product.Structure, product.Information);
            }
            catch
            {
                MessageBox.Show("Продукт временно недоступен");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
