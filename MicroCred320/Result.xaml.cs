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

namespace MicroCred320
{
    /// <summary>
    /// Логика взаимодействия для Result.xaml
    /// </summary>
    public partial class Result : Window
    {
        private string allRes { get; set; }
        public Result()
        {
            InitializeComponent();
        }

        private void TextSizeChanger(object sender, SizeChangedEventArgs e)
        {
            Size n = e.NewSize;
            Size p = e.PreviousSize;
            double l = n.Width / p.Width;
            if (l != double.PositiveInfinity)
            {
                if (sender is TextBox)
                {
                    (sender as TextBox).FontSize *= l;
                }
                else if (sender is TextBlock)
                {
                    (sender as TextBlock).FontSize *= l;
                }
                else if (sender is Button)
                {
                    (sender as Button).FontSize *= l;
                }
            }
        }

        
    }
}
