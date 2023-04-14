using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace swimSuitShop2.Classes
{   //Класс для выбранных товаров
    public class ProductsInOrder : Product
    {
        public int Count { get; set; } //Кол-во товаров
        public int Costing { get; set; }//Общая стоимость
    }
    //Класс для формирования товаров + путь к дирриктории
    public class Product
    {
        public BitmapImage Photo { get; set; }	//Изображение блюда
        public string Name { get; set; }	//Название блюда
        public int Cost { get; set; }	//Цена блюда
        public string Uid { get; set; }	//номер блюда
        public string Size { get; set; }	//Размеры блюда
        public string Material { get; set; }	//Материалы блюда
        public string Structure { get; set; }	//Состав блюда
        public string Information { get; set; }	//Состав блюда

    }
    public class activeProduct
    {

    }

}
