using Microsoft.Office.Interop.Excel;
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
using Button = System.Windows.Controls.Button;
using Excel = Microsoft.Office.Interop.Excel;
using Page = System.Windows.Controls.Page;

namespace swimSuitShop2.VievList
{
    /// <summary>
    /// Логика взаимодействия для delItem.xaml
    /// </summary>
    public partial class delItem : Page
    {
        List<Classes.Product> listProducts;

        public delItem()
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

                NameItem.Text = product.Name;
                App.activeProduct = product.Name;

                PhotoItem.Source = App.ShowImageBit(App.pathExe + $@"/photo/{App.activeCategory}/{App.activeProduct}.png");

                UidItem.Text = product.Uid;
                CostItem.Text = Convert.ToString(product.Cost);
                SizeItem.Text = product.Size;
                InformationItem.Text = product.Information;
                MaterialItem.Text = product.Material;
                StructureItem.Text = product.Structure;
            }
            catch
            {
                MessageBox.Show("Ошибка отображения\nинформации о продукте");
            }
            

        }

        private void delProduct(object sender, RoutedEventArgs e)
        {
            try
            {
                App.excelWorkSheet = (Excel.Worksheet)App.excelWorkBook.Worksheets.get_Item(App.activeCategory);
                App.excelRange = App.excelWorkSheet.Cells.Find(App.activeProduct, Type.Missing, Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart);
                System.IO.File.Delete(App.pathExe + $@"/photo/{App.activeCategory}/{App.activeProduct}.png");
                App.excelWorkSheet.Rows[App.excelRange.Row].Delete();
                MessageBox.Show("Товар успешно удален");
            }
            catch
            {
                MessageBox.Show("Ошибка удаления товара");
            }
        }
        
    }
}
