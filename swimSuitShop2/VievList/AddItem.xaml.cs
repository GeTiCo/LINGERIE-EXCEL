using swimSuitShop2.Classes;
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
using Microsoft.VisualBasic;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using Button = System.Windows.Controls.Button;
using Page = System.Windows.Controls.Page;
using Microsoft.Win32;
using Microsoft.Office.Interop.Word;

namespace swimSuitShop2.VievList
{
    /// <summary>
    /// Логика взаимодействия для AddItem.xaml
    /// </summary>
    public partial class AddItem : Page
    {
        List<Classes.Product> listProducts;

        public AddItem()
        {
            InitializeComponent();

            this.DataContext = this;

            listCategory.Items.Clear();
            listCategory.ItemsSource = App.makeCategoryList();
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

                listProducts.Add(product);
            }
            listViewProducts.ItemsSource = listProducts;
        }


        private void NewItemClick(object sender, RoutedEventArgs e)
        {

            if (App.activeCategory != "" && UidItem.Text != "Uid" && CostItem.Text != "Cost" && SizeItem.Text != "Size" && NameItem.Text != "Name" && MaterialItem.Text != "Material" && StructureItem.Text != "Structure" && InformationItem.Text != "Information")
            {
                if (UidItem.Text != "" || CostItem.Text != "" || SizeItem.Text != "" || NameItem.Text != "" || MaterialItem.Text != "" || StructureItem.Text != "" || InformationItem.Text != "")
                {
                    try
                    {
                        var countRows = App.excelWorkSheet.UsedRange.Rows.Count + 1;

                        App.excelWorkSheet = (Excel.Worksheet)App.excelWorkBook.Worksheets.get_Item(App.activeCategory);
                        App.excelRange = App.excelWorkSheet.Cells;

                        App.excelWorkSheet.Cells[countRows, 1] = NameItem.Text;
                        App.excelWorkSheet.Cells[countRows, 2] = Convert.ToInt32(CostItem.Text);
                        App.excelWorkSheet.Cells[countRows, 3] = UidItem.Text;
                        App.excelWorkSheet.Cells[countRows, 4] = SizeItem.Text;
                        App.excelWorkSheet.Cells[countRows, 5] = MaterialItem.Text;
                        App.excelWorkSheet.Cells[countRows, 6] = StructureItem.Text;
                        App.excelWorkSheet.Cells[countRows, 7] = InformationItem.Text;

                        OpenFileDialog dlg = new OpenFileDialog();
                        dlg.FileName = $"{NameItem.Text}";
                        dlg.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";

                        string url = App.pathExe + $@"/photo/{App.activeCategory}/{dlg.FileName}.png";

                        if (dlg.ShowDialog() == true && File.Exists(NameItem.Text) == false)
                        {
                            File.Copy(dlg.FileName, url);
                        }
                        else
                        {
                            File.Copy(App.pathExe + @"/default.png", App.pathExe + $@"/photo/{App.activeCategory}/{dlg.FileName}.png");
                        }
                        App.excelWorkBook.Save();

                        MessageBox.Show("Товар успешно добавлен");
                }
                    catch
                {
                    MessageBox.Show("Ошибка добавления товара");
                }
            }
                else
                {
                    MessageBox.Show("Присутствуют пустые строки");
                }
            }
            else
            {
                MessageBox.Show("Не все данные введены корректно\nИли не выбрана категория");
            }
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
            Classes.Product product = (sender as Button).DataContext as Classes.Product;//(2)
            string name = $"Наименование:\n{product.Name}\n\nИдентификатор:\n{product.Uid}\n\nЦена:\n{product.Cost}\n\nРазмеры:\n{product.Size}\n\nМатериалы:\n{product.Material}\n\nСостав комплекта:\n{product.Structure}\n\nДополнительная информация:\n{product.Information}";
            MessageBox.Show(name);
        }

        private void Button_Click_NewList(object sender, RoutedEventArgs e)
        {
            string input = Interaction.InputBox("Введите наименование новой категории", "Добавление категории");
            try
            {

                App.excelApp.DisplayAlerts = false;
                Excel.Worksheet sheet;
                sheet = App.excelWorkBook.Worksheets.Add(Type.Missing);
                sheet.Name = input;
                App.excelWorkBook.Save();

                Directory.CreateDirectory(App.pathExe + $@"/photo/{input}");

                if (UidItem.Text != "Uid" && CostItem.Text != "Cost" && SizeItem.Text != "Size" && NameItem.Text != "Name" && MaterialItem.Text != "Material" && StructureItem.Text != "Structure" && InformationItem.Text != "Information")
                {
                    if (UidItem.Text != "" || CostItem.Text != "" || SizeItem.Text != "" || NameItem.Text != "" || MaterialItem.Text != "" || StructureItem.Text != "" || InformationItem.Text != "")
                    {
                        //try
                        //{
                            App.excelWorkSheet = (Excel.Worksheet)App.excelWorkBook.Worksheets.get_Item(App.activeCategory);
                            var countRows = App.excelWorkSheet.UsedRange.Rows.Count;
                            App.excelRange = App.excelWorkSheet.Cells;

                            App.excelWorkSheet.Cells[countRows, 1] = NameItem.Text;
                            App.excelWorkSheet.Cells[countRows, 2] = Convert.ToInt32(CostItem.Text);
                            App.excelWorkSheet.Cells[countRows, 3] = UidItem.Text;
                            App.excelWorkSheet.Cells[countRows, 4] = SizeItem.Text;
                            App.excelWorkSheet.Cells[countRows, 5] = MaterialItem.Text;
                            App.excelWorkSheet.Cells[countRows, 6] = StructureItem.Text;
                            App.excelWorkSheet.Cells[countRows, 7] = InformationItem.Text;

                            OpenFileDialog dlg = new OpenFileDialog();
                            dlg.FileName = $"{NameItem.Text}";
                            dlg.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";

                            string url = App.pathExe + $@"/photo/{App.activeCategory}/{dlg.FileName}.png";

                            if (dlg.ShowDialog() == true && File.Exists(NameItem.Text) == false)
                            {
                                File.Copy(dlg.FileName, url);
                            }
                            else
                            {
                                File.Copy(App.pathExe + @"/default.png", App.pathExe + $@"/photo/{App.activeCategory}/{dlg.FileName}.png");
                            }
                            App.excelWorkBook.Save();
                        //}   
                        //catch
                        //{
                        //    MessageBox.Show("Ошибка добавления товара");
                        //}
                    }
                    else
                    {
                        MessageBox.Show("Присутствуют пустые строки");
                    }
                }
                else
                {
                    MessageBox.Show("Не все данные введены корректно\nИли не выбрана категория");
                }
                MessageBox.Show("Категория c 1 товаром\nуспешно добавлена");
            }
            catch
            {
                MessageBox.Show("Проблема на стороне экселя\nНе удалось создать категорию");
            }
}

        private void Button_Click_DelList(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Вы действительно хотите\nудалить категорию {App.activeCategory}", "Удаление категории", MessageBoxButton.YesNo);

            try
            {
                if (App.activeCategory != "" && result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Directory.Delete(App.pathExe + $@"/photo/{App.activeCategory}", true);
                        App.excelApp.DisplayAlerts = false;
                        App.excelWorkBook.Worksheets[$"{App.activeCategory}"].Delete();
                        App.excelWorkBook.Save();

                        MessageBox.Show("Категория Удалена");
                    }
                    catch
                    {
                        MessageBox.Show("Такой категории нет");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка удаления категории");
            }
        }
    }
}
