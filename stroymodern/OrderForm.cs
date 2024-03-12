using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using Npgsql;
using NpgsqlTypes;
using System.Drawing.Printing;
using Npgsql.Internal;

namespace stroymodern
{
    public partial class OrderForm : Form
    {
        private readonly CheckUser _user;
        Database database = new Database();

        private decimal price;
        private string name_good;
        private string article;
        
        public OrderForm(string name_good, string article, decimal price, Npgsql.NpgsqlConnection npgsqlConnection, CheckUser user)
        {
            InitializeComponent();
            _user = user;

            this.name_good = name_good;
            this.article = article;
            npgsqlConnection = npgsqlConnection;

            richTextBox1.Text =
                                $"Наименование товара: {this.name_good}\n\n" +
                                $"Артикул товара: {this.article}\n\n" +
                                $"Количество: {numericUpDown1.Value}\n\n" +
                                $"Цена: {price:C}\n\n" +
                                $"ФИО: {textBox3.Text:C}\n\n" +
                                $"Место ремонтa: {textBox3.Text:C}\n\n";

            numericUpDown1.Value = 1;

            this.price = price;
        }

        private void OrderForm_Load(object sender, EventArgs e)
        {
            label2.Text = $"{_user.Login}:{_user.Status}";
        }

        private NpgsqlConnection npgsqlConnection = new NpgsqlConnection("Server = localhost; Port = 5432; Database = stroyModern; User Id = postgres; Password = assaq123;");
        private void button4_Click(object sender, EventArgs e)
        {
            // Получаем значения из richTextBox1
            string names = this.name_good;
            string article = this.article;
            decimal price = this.price;
            decimal totalAmount = decimal.Parse(textBox1.Text); // Общая сумма заказа
            decimal quantity = numericUpDown1.Value;

            // Сохраняем данные в базу данных
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                npgsqlConnection.Open();
                cmd.Connection = npgsqlConnection;
          
                string query = $"INSERT INTO orders (name_good,article,price,total_amount,quantity, status) VALUES ('{names}', '{article}', '{price}', '{totalAmount}', '{quantity}', 'Новый')";
                cmd.CommandText = query ;
                // Параметры запроса
                cmd.Parameters.Add(new NpgsqlParameter("@order_date", NpgsqlDbType.Date)).Value = DateTime.Now.Date;

                cmd.Parameters.AddWithValue("@names", names); // Предполагается, что название товара берется из поля name
                cmd.Parameters.AddWithValue("@article", article);
                cmd.Parameters.AddWithValue("@price_item", price);
                cmd.Parameters.AddWithValue("@totalAmount", totalAmount);
                cmd.Parameters.AddWithValue("@quantity", quantity);

                // Выполняем запрос
                cmd.ExecuteNonQuery();

                MessageBox.Show("Заказ успешно оформлен и сохранен в базе данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                GoodsForm goods = new GoodsForm(_user);
                goods.Show();
                this.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            UpdateRichTextBox();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            decimal quantity = numericUpDown1.Value;
            UpdateRichTextBox();
        }

        private void UpdateRichTextBox()
        {
            decimal quantity = numericUpDown1.Value;

            if (quantity <= 0)
            {
                MessageBox.Show("Заказ не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GoodsForm goods = new GoodsForm(_user);
                goods.Show();
                this.Close();
            }
            else
            {
                decimal totalAmount = (this.price * quantity);

                // Обновляем содержимое richTextBox1 на основе сохраненных данных
                richTextBox1.Text =
                                $"Наименование товара: {this.name_good}\n\n" +
                                $"Артикул товара: {this.article}\n\n" +
                                $"Количество: {quantity}\n\n" +
                                $"Начальная цена: {price:C}\n\n" +
                                $"Сумма: {totalAmount:C}\n\n" +
                                $"ФИО: {textBox3.Text:C}\n\n" +
                                $"Место, где будет производиться ремонт: {textBox3.Text:C}\n\n";


                textBox1.Text = totalAmount.ToString();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            GoodsForm goods = new GoodsForm(_user);
            goods.Show();
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DrawBarcode(article);
        }

        private void DrawBarcode(string code, int resolution = 20) // resolution - пикселей на миллиметр
        {
            int numberCount = 15; // количество цифр
            float height = 25.93f * resolution; // высота штрих кода
            float lineHeight = 22.85f * resolution; // высота штриха
            float leftOffset = 3.63f * resolution; // свободная зона слева
            float rightOffset = 2.31f * resolution; // свободная зона справа
                                                    //штрихи, которые образуют правый и левый ограничивающие знаки,
                                                    //а также центральный ограничивающий знак должны быть удлинены вниз на 1,65мм
            float longLineHeight = lineHeight + 1.65f * resolution;
            float fontHeight = 2.75f * resolution; // высота цифр
            float lineToFontOffset = 0.165f * resolution; // минимальный размер от верхнего края цифр до нижнего края штрихов
            float lineWidthDelta = 0.15f * resolution; // ширина 0.15*{цифра}
            float lineWidthFull = 1.35f * resolution; // ширина белой полоски при 0 или 0.15*9
            float lineOffset = 0.2f * resolution; // между штрихами должно быть расстояние в 0.2мм

            float width = leftOffset + rightOffset + 6 * (lineWidthDelta + lineOffset) + numberCount * (lineWidthFull + lineOffset); // ширина штрих-кода

            Bitmap bitmap = new Bitmap((int)width, (int)height); // создание картинки нужных размеров
            Graphics g = Graphics.FromImage(bitmap); // создание графики

            Font font = new Font("Arial", fontHeight, FontStyle.Regular, GraphicsUnit.Pixel); // создание шрифта

            StringFormat fontFormat = new StringFormat(); // Центрирование текста
            fontFormat.Alignment = StringAlignment.Center;
            fontFormat.LineAlignment = StringAlignment.Center;

            float x = leftOffset; // позиция рисования по x
            for (int i = 0; i < numberCount; i++)
            {
                int number = Convert.ToInt32(code[i].ToString()); // число из кода
                if (number != 0)
                {
                    g.FillRectangle(Brushes.Black, x, 0, number * lineWidthDelta, lineHeight); // рисуем штрих
                }
                RectangleF fontRect = new RectangleF(x, lineHeight + lineToFontOffset, lineWidthFull, fontHeight); // рамки для буквы
                g.DrawString(code[i].ToString(), font, Brushes.Black, fontRect, fontFormat); // рисуем букву
                x += lineWidthFull + lineOffset; // смещаем позицию рисования по x
                if (i == 0 && i == numberCount / 2 && i == numberCount - 1) // если это начало, середина или конец кода рисуем разделители
                {
                    for (int j = 0; j < 2; j++) // рисуем 2 линии разделителя
                    {
                        g.FillRectangle(Brushes.Black, x, 0, lineWidthDelta, longLineHeight); // рисуем длинный штрих
                        x += lineWidthDelta + lineOffset; // смещаем позицию рисования по x
                    }
                }
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // делаем чтобы картинка помещалась в pictureBox
            pictureBox1.Image = bitmap; // устанавливаем картинку
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(richTextBox1.Text, new Font("Times New Roman", 16, FontStyle.Regular), Brushes.Black, new Point(10, 10));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            printPreviewDialog2.Document = printDocument2;
            printPreviewDialog2.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG|*.png|JPEG|*.jpg|GIF|*.gif|BMP|*.bmp";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog.FileName);
            }
        }

        private void printDocument2_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(pictureBox1.Image, 20, 20, pictureBox1.Width, pictureBox1.Height);
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }
    }
}
