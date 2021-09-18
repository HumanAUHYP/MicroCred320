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
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Security;

namespace MicroCred320
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> result = new List<string>();
        private string rate;
        private DateTime date = new DateTime();
        private string allRes;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (tbxResult.Text == "")
                    Calculate();
                else
                    MessageBox.Show("Сбросьте значения");
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат ввода");
            }
            
        }

        public void Calculate()
        {
            if (rate == null)
                MessageBox.Show("Загрузите тарифный план");
            else
            {
                double loanSum = double.Parse(tbxCreditSum.Text);
                int term = int.Parse(tbxCreditTerm.Text);
                date = dpDay.SelectedDate.Value;

                //Сделать ввод из текстового файла. Возможно словарь - день:процент. В тхт файле "от до %"

                result.Add("День\tДата\tСтавка\tДолг\tСумма выплаты");
                if (term > 0)
                {
                    string[] persent = rate.Split(';');
                    double[] cumulatively = new double[term];
                    double[] payments = new double[term];
                    double cumu = 0;
                    int day = 0;
                    if (persent.Length == term)
                    {
                        for (int i = 0; i < term; i++)
                        {
                            cumu += ((Convert.ToDouble(persent[i]) / 100) * loanSum);
                            cumulatively[i] = cumu;
                            payments[i] = cumu + loanSum;
                            result.Add($"\n{i+1}\t{date.ToString("dd.MM.yy")}\t{persent[i]}%\t{cumulatively[i]}p.\t{payments[i]}p.");
                            day++;
                            date = date.AddDays(1);
                        }
                    }
                    else if (persent.Length < term)
                    {
                        int days = term;
                        for (int i = 0; i < persent.Length; i++)
                        {
                            cumu += ((Convert.ToDouble(persent[i]) / 100) * loanSum);
                            cumulatively[i] = cumu;
                            payments[i] = cumu + loanSum;
                            result.Add($"\n{i+1}\t{date.ToString("dd.MM.yy")}\t{persent[i]}%\t{cumulatively[i]}p.\t{payments[i]}p.");
                            days--;
                            day++;
                            date = date.AddDays(1);
                        }

                        for (int i = 0; i < days; i++)
                        {
                            cumu += ((Convert.ToDouble(persent[persent.Length - 1]) / 100) * loanSum);
                            cumulatively[day] = cumu;
                            payments[day] = cumu + loanSum;
                            result.Add($"\n{day+1}\t{date.ToString("dd.MM.yy")}\t{persent[persent.Length - 1]}%\t{cumulatively[day]}p.\t{payments[day]}p.");
                            day++;
                            date = date.AddDays(1);
                        }

                    }
                    else if (persent.Length > term)
                    {
                        for(int i = 0; i < term; i++)
                        {
                            cumu += ((Convert.ToDouble(persent[i]) / 100) * loanSum);
                            cumulatively[i] = cumu;
                            payments[i] = cumu + loanSum;
                            result.Add($"\n{i+1}\t{date.ToString("dd.MM.yy")}\t{persent[i]}%\t{cumulatively[i]}p.\t{payments[i]}p.");
                            day++;
                            date = date.AddDays(1);
                        }
                    }
                    
                    tbPaymentSum.Text = $"Общая сумма выплаты: {Convert.ToString(payments[term - 1])} p.";
                    tbCreditSum.Text = $"Сумма долга: {Convert.ToString(cumulatively[term - 1])} p.";
                    tbEffRate.Text = $"Эффективная ставка: {(((cumulatively[term - 1] / loanSum) / term) * 100)} %";
                    tbxResult.Text = string.Join(Environment.NewLine, result);
                    allRes = $"\n{string.Join(Environment.NewLine, result)}\t{tbPaymentSum}\t{tbCreditSum}\t{tbEffRate}";
                    if (cumulatively[term - 1] >= loanSum * 1.5)
                    {
                        MessageBox.Show("Размер выплаты по микрозайму не может превышать 2,5-кратного размера суммы займа");
                        return;
                    }
                    if (loanSum + cumulatively[term - 1] >= 500000)
                    {
                        MessageBox.Show("Предельный размер долговой нагрузки на одно физическое лицо не может превышать 500 тыс. руб");
                        return;
                    }
                }
            }

        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            result = null;
            result = new List<string>();
            tbxResult.Text = "";
            tbxCreditSum.Text = "";
            tbxCreditTerm.Text = "";
            tbPaymentSum.Text = "Общая сумма выплаты: ";
            tbCreditSum.Text = "Сумма процентов: ";
            tbEffRate.Text = "Эффективная ставка: ";
            allRes = "";
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlgSave = new SaveFileDialog();

            dlgSave.Filter = "Текст (*.txt)|*.txt";

            if (dlgSave.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(dlgSave.OpenFile(), Encoding.Default))
                {
                    sw.WriteLine(tbxResult.Text);
                    sw.WriteLine(tbPaymentSum.Text);
                    sw.WriteLine(tbCreditSum.Text);
                    sw.WriteLine(tbEffRate.Text);
                    sw.Close();
                }
            }
            Result taskWindow = new Result();
            taskWindow.Show();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            string name = Environment.UserName;
            string path = String.Concat("C:\\Users\\", name, "\\Documents\\plane\\");


            OpenFileDialog openFileDialog = new OpenFileDialog();
            
                openFileDialog.InitialDirectory = path;
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
            }
            rate = fileContent;
        }

        private void tbxCreditSum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

        private void tbxCreditTerm_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

        private void dpDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
