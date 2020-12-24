using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RSA
{
    public partial class Form1 : Form
    {
            char[] characters = new char[] { '#', 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И',
                                                'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С',
                                                'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ь', 'Ы', 'Ъ',
                                                'Э', 'Ю', 'Я', ' ', '1', '2', '3', '4', '5', '6', '7',
                                                '8', '9', '0' };
        public Form1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Кнопка, отвечающая за зашифровку методом RSA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            if ((this.textBox_p.Text.Length > 0) && (this.textBox_q.Text.Length > 0)
                && (this.textBox_p2.Text.Length > 0) && (this.textBox_q2.Text.Length > 0))
            {
                long p = Convert.ToInt64(this.textBox_p.Text);
                long q = Convert.ToInt64(this.textBox_q.Text);
                long p2 = long.Parse(this.textBox_p2.Text);
                long q2 = long.Parse(this.textBox_q2.Text);

                if (this.IsTheNumberSimple(p) && this.IsTheNumberSimple(q)
                    && this.IsTheNumberSimple(p2) && this.IsTheNumberSimple(q2))
                {
                    string s = "";
                    var sr = new StreamReader("in.txt");

                    while (!sr.EndOfStream)
                    {
                        s += sr.ReadLine();
                    }

                    sr.Close();

                    s = s.ToUpper();

                    long n = p * q;
                    long n2 = p2 * q2;
                    long m = (p - 1) * (q - 1);
                    long m2 = (p2 - 1) * (q2 - 1);
                    long d = this.Calculate_d(m);
                    long d2 = this.Calculate_d(m2);
                    long e_ = this.Calculate_e(d, m);
                    long e_2 = this.Calculate_e(d2, m2);

                    List<string> result = this.RSA_Encode(s, e_, e_2, n, n2);

                    var sw = new StreamWriter("out1.txt");
                    foreach (string item in result)
                        sw.WriteLine(item);
                    sw.Close();

                    this.textBox_d.Text = d.ToString();
                    this.textBox_n.Text = d.ToString();

                    Process.Start("out1.txt");
                }
                else
                    MessageBox.Show("p или q - не простые числа","Информация",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Введите p и q","Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Метод, выполняющий шифрование строки алгоритмом RSA
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e_"></param>
        /// <param name="e_2"></param>
        /// <param name="n"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        private List<string> RSA_Encode(string s, long e_, long e_2, long n, long n2)
        {
            List<string> result = new List<string>();

            BigInteger bi;

            for(int i = 0; i < s.Length; i++)
            {
                int index = Array.IndexOf(this.characters, s[i]);

                bi = new BigInteger(index);
                bi = BigInteger.ModPow(bi, (int)e_, (int)e_2);

                var n_ = new BigInteger((int)n);
                var n_2 = new BigInteger((int)n2);

                bi = bi % n_;
                bi = bi % n_2;

                result.Add(bi.ToString());
            }
            return result;
        }

        /// <summary>
        /// Метод, вычисляющий значение параметра e
        /// </summary>
        /// <param name="d"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private long Calculate_e(long d, long m)
        {
            long e = 10;

            while (true)
            {
                if ((e * d) % m == 1)
                    break;
                else
                    e++;
            }
            return e;
        }

        /// <summary>
        /// Вычисление параметра d
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private long Calculate_d(long m)
        {
            long d = m - 1;

            for(long i = 2; i <= m; i++)
                if ((m % i == 0) && (d % i == 0))//если имеют общие детали
                {
                    d--;
                    i = 1;
                }
            return d;
        }

        /// <summary>
        /// Тест Миллера - Рабина на простоту числа
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private bool IsTheNumberSimple(long n)
        {
            if (n < 2)
                return false;

            if (n == 2)
                return true;

            for (long i = 2; i < n; i++)
                if (n % i == 0)
                    return false;

            return true;
        }

        /// <summary>
        /// Кнопка, отвечающая за дешифровку методом RSA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDecipher_Click(object sender, EventArgs e)
        {
            if ((this.textBox_p.Text.Length > 0) && (this.textBox_q.Text.Length > 0)
                && (this.textBox_p2.Text.Length > 0) && (this.textBox_q2.Text.Length > 0))
            {
                long d = Convert.ToInt64(this.textBox_d.Text);
                long n = Convert.ToInt64(this.textBox_n.Text);

                List<string> input = new List<string>();

                var sr = new StreamReader("out1.txt");

                while (!sr.EndOfStream)
                {
                    input.Add(sr.ReadLine());
                }

                sr.Close();

                string result = this.RSA_Decode(input, d, n);

                var sw = new StreamWriter("out2.txt");
                sw.WriteLine(result);
                sw.Close();

                Process.Start("out2.txt");
            }
            else
                MessageBox.Show("Введите секретный ключ!","Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Метод, выполняющий расшифровку строки алгоритмом RSA
        /// </summary>
        /// <param name="input"></param>
        /// <param name="d"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private string RSA_Decode(List<string> input, long d, long n)
        {
            string result = "";

            BigInteger bi;

            foreach (string item in input)
            {
                bi = new BigInteger(Convert.ToDouble(item));
                bi = BigInteger.Pow(bi, (int)d);

                var n_ = new BigInteger((int)n);

                bi = bi % n_;
                 
                int index = Convert.ToInt32(bi.ToString());

                result += this.characters[index].ToString();
            }

            return result;
        }

        /// <summary>
        /// Открытие файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFile_Click(object sender, EventArgs e)
        {
            var o = new OpenFileDialog();
            o.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (o.ShowDialog() == DialogResult.OK)
            {
                this.Ext();
            }
        }

        private void Ext()
        {
            this.textBox_p2.Text = File.ReadLines("in.txt").Skip(0).First();
            this.textBox_q2.Text = File.ReadLines("in.txt").Skip(1).First();
        }

        /// <summary>
        /// Построение графика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_p_TextChanged(object sender, EventArgs e)
        {
            this.chart1.Series.Clear();
            this.chart1.Series.Add("TextBox");
            this.chart1.Series[0].ChartType = SeriesChartType.Line;
            this.chart1.Series[0].Color = Color.Red;
            //double[] data=this.textBox_p2.Text.Split(",".ToArray(),StringSplitOptions.RemoveEmptyEntries).Select(x=>double.Parse(x)).ToArray();
            //double[] data2 = this.textBox_q2.Text.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x)).ToArray();
            double[] data3 = this.textBox_p.Text.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x)).ToArray();
            double[] data4 = this.textBox_q.Text.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x)).ToArray();
            //foreach (double s in data)
            //{
            //    this.chart1.Series[0].Points.AddY(s);
            //}
            //foreach (double s in data2)
            //{
            //    this.chart1.Series[0].Points.AddY(s);
            //}
            foreach (double s in data3)
            {
                this.chart1.Series[0].Points.AddY(s);
            }
            foreach (double s in data4)
            {
                this.chart1.Series[0].Points.AddY(s);
            }
        }

        private void textBox_p_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar) | (e.KeyChar == Convert.ToChar(",")) | e.KeyChar == '\b') return;
            else
                e.Handled = true;
        }
    }
}
