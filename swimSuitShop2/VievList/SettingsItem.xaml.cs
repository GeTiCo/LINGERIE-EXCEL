using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;
using Page = System.Windows.Controls.Page;

namespace swimSuitShop2.VievList
{
    /// <summary>
    /// Логика взаимодействия для SettingsItem.xaml
    /// </summary>
    public partial class SettingsItem : Page
    {
        List<Classes.Product> listProducts;

        public SettingsItem()
        {
            InitializeComponent();

            this.DataContext = this;

            listCategory.Items.Clear();
            listCategory.ItemsSource = App.makeCategoryList();//(1)
        }

        private void ListCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UidItem.Text = null;
            CostItem.Text = null;
            SizeItem.Text = null;
            NameItem.Text = null;
            InformationItem.Text = null;
            MaterialItem.Text = null;
            StructureItem.Text = null;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Classes.Product product = (sender as Button).DataContext as Classes.Product;

                App.activeProduct = product.Name;
                App.activeProduct = product.Name;

                PhotoItem.Source = App.ShowImageBit(App.pathExe + $@"/photo/{App.activeCategory}/{App.activeProduct}.png");
                UidItem.Text = product.Uid;
                CostItem.Text = Convert.ToString(product.Cost);
                SizeItem.Text = product.Size;
                NameItem.Text = product.Name;
                InformationItem.Text = product.Information;
                MaterialItem.Text = product.Material;
                StructureItem.Text = product.Structure;
            }
            catch
            {
                MessageBox.Show("Ошибка отображения\nинформации о продукте");
            }
            

        }

        private void saveItem(object sender, RoutedEventArgs e)
        {
            App.excelWorkSheet = (Excel.Worksheet)App.excelWorkBook.Worksheets.get_Item(App.activeCategory);
            App.excelRange = App.excelWorkSheet.Cells.Find(App.activeProduct, Type.Missing, Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart);

            App.excelWorkSheet.Cells[App.excelRange.Row, 1] = NameItem.Text;
            App.excelWorkSheet.Cells[App.excelRange.Row, 2] = Convert.ToInt32(CostItem.Text);
            App.excelWorkSheet.Cells[App.excelRange.Row, 3] = UidItem.Text;
            App.excelWorkSheet.Cells[App.excelRange.Row, 4] = SizeItem.Text;
            App.excelWorkSheet.Cells[App.excelRange.Row, 5] = MaterialItem.Text;
            App.excelWorkSheet.Cells[App.excelRange.Row, 6] = StructureItem.Text;
            App.excelWorkSheet.Cells[App.excelRange.Row, 7] = InformationItem.Text;

            string oldurl = App.pathExe + $@"/photo/{App.activeCategory}/{App.activeProduct}.png";
            string newurl = App.pathExe + $@"/photo/{App.activeCategory}/{NameItem.Text}.png";

            MessageBoxResult result = MessageBox.Show($"Вы хотите поменять изображение товара? {App.activeProduct}", "Изменение изображения", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.FileName = $"{NameItem.Text}";
                    dlg.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";

                    if (dlg.ShowDialog() == true)
                    {
                        File.Delete(oldurl);
                        File.Copy(dlg.FileName, newurl);
                        MessageBox.Show("Данные товара изменены");
                    }
                    else
                    {
                        MessageBox.Show("Данные товара\nизменены не полностью");
                    }
                    
                }
                catch
                {
                    MessageBox.Show("Ошибка, фото не изменено");
                }
                
            }
            else
            {
                try
                {
                    newurl = App.pathExe + $@"/photo/{App.activeCategory}/{NameItem.Text}.png";
                    File.Move(oldurl, newurl);
                    MessageBox.Show("Данные товара изменены");
                }
                catch
                {
                    MessageBox.Show("Ошибка изменения имени фото");
                }
                
            }

                
        }
    }
}
