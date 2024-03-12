using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stroymodern
{
    public partial class Form1 : Form
    {
        Database database = new Database();

        private int failedAttempts = 0;
        private DateTime blockedUntil = DateTime.MinValue;
        private bool closed = false;
        private bool captchaRequired = false;
        private string correctCaptcha = "";

        public Form1()
        {
            InitializeComponent();
            panel2.Visible = false;
        }

        private string GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder captcha = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                captcha.Append(chars[random.Next(chars.Length)]);
            }
            return captcha.ToString();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            database.OpenConnection();

            var login = textBoxLogin.Text;
            var password = textBoxPassword.Text;

            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
            DataTable table = new DataTable();

            string querry = $"select id_user, login, password, id_role from users where login = '{login}' and password = '{password}'";

            NpgsqlCommand command = new NpgsqlCommand(querry, database.GetConnection());

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (closed)
            {
                return;
            }
            else if (DateTime.Now < blockedUntil)
            {
                MessageBox.Show($"Попробуйте ещё раз через {blockedUntil.Subtract(DateTime.Now).TotalSeconds} секунд");

                return;
            }
            else if (captchaRequired)
            {
                if (textBox1.Text != correctCaptcha)
                {
                    MessageBox.Show("Превышено максимально количество попыток");

                    closed = true;
                    this.Close();
                    return;
                }

                failedAttempts = 0;
                captchaRequired = false;
                textBox1.Text = "";
                var user = new CheckUser(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3]));
                GoodsForm goodform = new GoodsForm(user);
                goodform.Show();
                this.Hide();
            }
            else if (table.Rows.Count == 1)
            {
                MessageBox.Show("Вы успешно вошли!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                var user = new CheckUser(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3]));
                GoodsForm goodform = new GoodsForm(user);
                goodform.Show();
                this.Hide();
            }
            else
            {
                failedAttempts++;
                if (failedAttempts == 3)
                {
                    blockedUntil = DateTime.Now.AddSeconds(30);
                    MessageBox.Show($"Вы ввели неправильно в 3 раз. Попробуйте ещё раз через 30 сек");
                }
            }

            if (failedAttempts >= 4)
            {
                failedAttempts++;
                panel2.Visible = true;
                correctCaptcha = GenerateCaptcha();
                captcha.Text = $"Captcha: {correctCaptcha}";
                captchaRequired = true;
                MessageBox.Show($"Вы ввели неправильно в 4 раз. Код капчи: {correctCaptcha}");
            }

            if (failedAttempts >= 6)
            {
                closed = true;
                this.Close();
            }

        database.CloseConnection();
        }
    }
}
