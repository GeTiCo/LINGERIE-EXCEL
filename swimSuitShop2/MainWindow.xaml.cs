using swimSuitShop2.View;
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

namespace swimSuitShop2
{
    public partial class MainWindow : Window
    {
        /*Конструктор: Попытка создания экземпляра класса приложения эксель со скрытием экселя(1), условие
          наличия файла(2), определение книги со скрытием экселя(3), условие закрытия приложения при 
          отсутствии файла экселя(4), альтернативное условие выхода из приложения при отсутствии экселя(5)*/
        public MainWindow()
        {
            InitializeComponent();
            try//(1)
            {
                App.excelApp = new Excel.Application();
                App.excelApp.Visible = false;
                if (File.Exists(App.fileMenu))//(2)
                {
                    App.excelWorkBook = App.excelApp.Workbooks.Open(App.fileMenu);//(3)
                    App.excelApp.Visible = false;
                }
                else//(4)
                {
                    MessageBox.Show("Файла с БД не обнаружено");
                    this.Close();
                }
            }
            catch//(5)
            {
                MessageBox.Show("Dowload MS Office");
                this.Close();
            }
        }
        //Кнопка выхода из приложения с отчисткой экселя
    private void ExitClick(object sender, RoutedEventArgs e)
        {   //Выйти из Excel
            App.excelApp.Quit();
            //Уничтожить все COM-объекты
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(App.excelApp);
            //Заставляет сборщик мусора провести сборку мусора
            GC.Collect();
            this.Close();
            System.Windows.Application.Current.Shutdown();
        }
        //Кнопка Каталог (Этап: Разработка)
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            View.Catalog newOrder = new View.Catalog();

            this.Hide();
            newOrder.ShowDialog();
            this.Show();
        }
        //Кнопка Оформления заказа (Этап: Список товаров - выполнен, Оформление заказа - разработка)
        //+Лимит средств отправляется в Оформление заказа
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            View.MakeOrder newOrder = new View.MakeOrder(rnd.Next(10000, 20000));

            this.Hide();
            newOrder.ShowDialog();
            this.Show();
        }
        //Кнопка Редактирования (Этап: Проверка клиента - выполнен, Настройки приложения - разработка)
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            View.AdminPanel newOrder = new View.AdminPanel();

            this.Hide();
            newOrder.ShowDialog();
            this.Show();
        }
    }
}
