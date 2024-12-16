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

namespace Practika
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Переход на окно "Категории продукции"
        private void CategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            CategoriesWindow categoriesWindow = new CategoriesWindow();
            categoriesWindow.Show();
        }

        // Переход на окно "Продукция"
        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow productsWindow = new ProductsWindow();
            productsWindow.Show();
        }

        // Переход на окно "Платежи пользователей за продукцию"
        private void PaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            PaymentWindow paymentsWindow = new PaymentWindow();
            paymentsWindow.Show();
        }

        // Переход на окно "Отчёт"
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            OrderWindow ordertWindow = new OrderWindow();
            ordertWindow.Show();
        }
    }
}
