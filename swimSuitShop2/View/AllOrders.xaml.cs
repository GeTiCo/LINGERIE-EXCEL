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
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;
using Window = System.Windows.Window;
using Button = System.Windows.Controls.Button;
using Microsoft.Office.Interop.Word;

namespace swimSuitShop2.View
{

    /// <summary>
    /// Логика взаимодействия для AllOrders.xaml
    /// </summary>
    public partial class AllOrders : Window
    {
        public int SummaBankForOrder { get; set; }

        List<Classes.ProductsInOrder> listProductsInOrders = MakeOrder.listProductsInOrders;

        public AllOrders(int bank)
        {
            InitializeComponent();

            this.SummaBankForOrder = bank;

            SecretOrder.ItemsSource = MakeOrder.listProductsInOrders;
            order.ItemsSource = MakeOrder.listProductsInOrders;
            wallet.Text = $"Сумма заказа: {MakeOrder.SummaOrder}";
        }

        /*Изменение состава заказа:*/
        private void Button_update(object sender, RoutedEventArgs e)
        {
            int index, newcosting;
            string name, doing = (sender as Button).Name;

            Classes.ProductsInOrder product = (sender as Button).DataContext as Classes.ProductsInOrder;

            switch (doing)
            {
                case "plus":
                    if (MakeOrder.SummaOrder + product.Cost < SummaBankForOrder)
                    {
                        newcosting = (product.Count + 1) * product.Cost;
                        product.Costing = newcosting;
                        product.Count = product.Count + 1;
                        MakeOrder.SummaOrder += product.Cost;
                    }
                    else
                    {
                        MessageBox.Show("Недостаточно средств");
                    }
                    break;
                case "minus":
                    if (product.Count == 1)
                    {
                        name = product.Name;
                        index = MakeOrder.listProductsInOrders.FindIndex(x => x.Name == name);
                        MakeOrder.listProductsInOrders.RemoveAt(index);
                    }
                    else
                    {
                        newcosting = (product.Count - 1) * product.Cost;
                        product.Costing = newcosting;
                        product.Count = product.Count - 1;
                        MakeOrder.SummaOrder -= product.Cost;
                    }
                    break;
                case "delete":
                    index = MakeOrder.listProductsInOrders.FindIndex(x => x.Name == product.Name);
                    MakeOrder.listProductsInOrders.RemoveAt(index);
                    MakeOrder.SummaOrder -= (product.Cost * product.Count);
                    break;
            }
            order.Items.Refresh();
            wallet.Text = $"Сумма заказа: {MakeOrder.SummaOrder}";
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                App.wordApp = new Word.Application();
                App.wordApp.Visible = false;
            }
            catch
            {
                MessageBox.Show("Ошибка создания товарного чека");
                return;
            }
            //Добавить новый документ
            App.wordDoc = App.wordApp.Documents.Add();
            //Ориентация страницы - книжная
            App.wordDoc.PageSetup.BottomMargin = 20;
            App.wordDoc.PageSetup.TopMargin = 20;
            App.wordDoc.PageSetup.LeftMargin = 20;
            App.wordDoc.PageSetup.RightMargin = 20;
            App.wordDoc.PageSetup.Orientation = Word.WdOrientation.wdOrientPortrait;
            App.wordDoc.Content.ParagraphFormat.LeftIndent = App.wordDoc.Content.Application.CentimetersToPoints((float)0);
            App.wordDoc.Content.ParagraphFormat.RightIndent = App.wordDoc.Content.Application.CentimetersToPoints((float)0);
            //Выравнивание текста в абзацах
            App.wordDoc.Content.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            //Доступ к 1-му существующему параграфу
            App.wordPar = (Word.Paragraph)App.wordDoc.Paragraphs[1];
            //Добавление нового параграфа  после существующего
            //Настройки параграфа
            App.wordPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            //------------------------------------------------------------------------------------
            App.wordRange = App.wordPar.Range;      //Его содержимое
            //Добавление новой картинки
            App.wordShape = App.wordDoc.InlineShapes.AddPicture(App.pathExe + @"\SwimWordLog.png", Type.Missing, Type.Missing, App.wordRange);
            //Настройка картинки
            App.wordShape.Width = 250;
            App.wordShape.Height = 250;
            //------------------------------------------------------------------------------------
            App.wordRange.InsertParagraphAfter();

            App.wordPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            App.wordRange = App.wordPar.Range;
            App.wordRange.Font.Size = 20;
            App.wordRange.Font.Color = Word.WdColor.wdColorBlue;
            App.wordRange.Font.Name = "Sylfaen";
            Random rnd = new Random();
            App.wordRange.Text = "ЗАКАЗ #" + rnd.Next(1,1000);

            App.wordRange.InsertParagraphAfter();
            App.wordPar = App.wordDoc.Paragraphs.Add();
            App.wordPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            App.wordRange = App.wordPar.Range;
            App.wordRange.Font.Size = 16;
            App.wordRange.Font.Color = Word.WdColor.wdColorBlue;
            App.wordRange.Font.Name = "Sylfaen";
            App.wordRange.Text = "Дата заказа: " + DateTime.Now.ToLongDateString();
            //------------------------------------------------------------------------------------
            App.wordRange.InsertParagraphAfter();
            App.wordPar = App.wordDoc.Paragraphs.Add();     //Абзац для таблицы
            App.wordRange = App.wordPar.Range;      //Диапазон абзаца		
            App.wordTable = App.wordDoc.Tables.Add(App.wordRange, SecretOrder.Items.Count + 1, 7);
            App.wordTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            App.wordTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            App.wordTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle; //Бордюр
            Word.Range cellRange;           //Отдельная ячейка таблицы
            for (int col = 1; col <= 7; col++)
            {
                cellRange = App.wordTable.Cell(1, col).Range;   //Ссылка к нужной ячейке
                cellRange.Text = SecretOrder.Columns[col - 1].Header.ToString();    //Значение из ЭУ
            }
            App.wordTable.Rows[1].Shading.ForegroundPatternColor = Word.WdColor.wdColorLightBlue;
            App.wordTable.Rows[1].Range.Font.Color = Word.WdColor.wdColorWhite;
            App.wordTable.Rows[1].Range.Font.Name = "Sylfaen";
            App.wordTable.Rows[1].Range.Font.Size = 15;
            App.wordTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            //------------------------------------------------------------------------------------
            App.wordPar.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            App.wordPar.set_Style("Заголовок 2");               //Стиль, взятый из Word
            for (int row = 2; row <= listProductsInOrders.Count + 1; row++)
            {
                App.wordTable.Rows[row].Shading.ForegroundPatternColor = Word.WdColor.wdColorGray05;
                App.wordTable.Rows[row].Range.Font.Color = Word.WdColor.wdColorBlack;
                App.wordTable.Rows[row].Range.Font.Name = "Sylfaen";
                App.wordTable.Rows[row].Range.Font.Size = 15;
                App.wordRange.Font.Size = 14;
                App.wordRange.Font.Color = Word.WdColor.wdColorBlack;
                App.wordRange.Font.Name = "Time New Roman";
                cellRange = App.wordTable.Cell(row, 1).Range;
                App.wordTable.Columns.SetWidth(70, Microsoft.Office.Interop.Word.WdRulerStyle.wdAdjustProportional);
                cellRange.Text = listProductsInOrders[row - 2].Uid.ToString();
                cellRange = App.wordTable.Cell(row, 2).Range;
                cellRange.Text = listProductsInOrders[row - 2].Name.ToString();
                cellRange = App.wordTable.Cell(row, 3).Range;
                cellRange.Text = listProductsInOrders[row - 2].Size.ToString();
                cellRange = App.wordTable.Cell(row, 4).Range;
                cellRange.Text = listProductsInOrders[row - 2].Structure.ToString();
                cellRange = App.wordTable.Cell(row, 5).Range;
                cellRange.Text = listProductsInOrders[row - 2].Cost.ToString();
                cellRange = App.wordTable.Cell(row, 6).Range;
                cellRange.Text = listProductsInOrders[row - 2].Count.ToString();
                cellRange = App.wordTable.Cell(row, 7).Range;
                cellRange.Text = listProductsInOrders[row - 2].Costing.ToString();
            }

            App.wordRange.InsertParagraphAfter();
            App.wordPar = App.wordDoc.Paragraphs.Add();
            App.wordRange = App.wordPar.Range;               //Стиль, взятый из Word
            App.wordRange.Font.Color = Word.WdColor.wdColorBlue;
            App.wordRange.Font.Size = 20;
            App.wordRange.Font.Name = "Sylfaen";
            App.wordRange.Text = "Стоимость заказа: " + MakeOrder.SummaOrder.ToString() + " рублей";

            //Сохранить документ в двух форматах: docx и pdf 
            App.wordDoc.Saved = true;
            //Полный путь к документу с именем – текущей даты
            string pathDoc = App.pathExe + @"\checks" + "test";
            App.wordDoc.SaveAs(pathDoc + ".docx");
            //Сохранить в формате pdf
            App.wordDoc.SaveAs(pathDoc + ".pdf", Word.WdExportFormat.wdExportFormatPDF);
            App.wordDoc.Close(true, null, null);
            App.wordApp.Quit();                     //Выход из Word
                                                //Вызвать свою подпрограмму убивания процессов
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(App.wordApp);
            //Заставляет сборщик мусора провести сборку мусора
            GC.Collect();

        }
    }
}
