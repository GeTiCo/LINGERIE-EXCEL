using swimSuitShop2.Classes;
using swimSuitShop2.VievList;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
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

namespace swimSuitShop2.View
{
    public partial class MakeOrder : Window
    {
        //Глобальные параметры------------------------------------------------------------
        /*Листы: категорий товаров, товаров из категорий и выбранных товаров*/
        public static List<Classes.Product> listProducts;
        public static List<Classes.ProductsInOrder> listProductsInOrders;

        /*Публичные финансовые переменные (Баланс / Корзина)*/
        public int SummaBankCard { get; set; }
        public static int SummaOrder { get; set; }

        //Основные функции------------------------------------------------------------
        public MakeOrder(int summaBankCard)
        {
            InitializeComponent();

            listCategory.Items.Clear();
            listCategory.ItemsSource = App.makeCategoryList();

            listProductsInOrders = new List<Classes.ProductsInOrder>();

            AllOrders money = new AllOrders(SummaBankCard);
            SummaOrder = 0;
            this.SummaBankCard = summaBankCard;
            wallet.Text = $"Сумма на кошельке: {SummaBankCard}";
            limit.Text = "Сумма товаров: " + SummaOrder;
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

        private void Button_Click_plus(object sender, RoutedEventArgs e)
        {
            Classes.ProductsInOrder productInOrder = null;

            Classes.Product product = (sender as Button).DataContext as Classes.Product;

            try
            {
                if (SummaOrder + product.Cost <= SummaBankCard)
                {

                    int index = listProductsInOrders.FindIndex(x => x.Name == product.Name);

                    if (index < 0)
                    {
                        productInOrder = new Classes.ProductsInOrder();

                        productInOrder.Photo = product.Photo;
                        productInOrder.Name = product.Name;
                        productInOrder.Uid = product.Uid;
                        productInOrder.Cost = product.Cost;
                        productInOrder.Size = product.Size;
                        productInOrder.Structure = product.Structure;
                        productInOrder.Count = 1;
                        productInOrder.Costing = product.Cost;

                        listProductsInOrders.Add(productInOrder);
                    }
                    else
                    {
                        listProductsInOrders[index].Count++;
                        listProductsInOrders[index].Costing = listProductsInOrders[index].Cost * listProductsInOrders[index].Count;
                    }

                    SummaOrder += product.Cost;
                    limit.Text = "Сумма товаров: " + SummaOrder;
                }
                else
                {
                    MessageBox.Show("У вас недостаточно средств");
                }
            }
            catch
            {
                MessageBox.Show("Нам не удалось добавить товар в корзину");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            View.AllOrders newOrder = new View.AllOrders(SummaBankCard);
            this.Hide();
            newOrder.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
