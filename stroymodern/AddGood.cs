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
    public partial class AddGood : Form
    {
        private readonly CheckUser _user;
        Database database = new Database();
        string[] good_type = { "Проём", "Настенное покрытие", "Материалы", "Мебель" };
        public AddGood(CheckUser user)
        {
            _user = user;
            InitializeComponent();

            foreach (string types in good_type)
            {
                comboBox1.Items.Add(types);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            GoodsForm goods = new GoodsForm(_user);
            goods.ShowDialog();
            this.Close();
        }

        private void AddGood_Load(object sender, EventArgs e)
        {
            label2.Text = $"{_user.Login}:{_user.Status}";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            database.OpenConnection();

            var name_good= textBox1.Text;
            var number_plant = textBox2.Text;
            var price = textBox3.Text;
            var acticle = textBox4.Text;
            var photo = textBox5.Text;
            var type_good = comboBox1.Text;

            string query = $"insert into goods (name_good, number_plant, price, article, photo, type_good) values ('{name_good}','{number_plant}','{price}','{acticle}','{photo}','{type_good}')";

            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, database.GetConnection());
            npgsqlCommand.ExecuteNonQuery();

            MessageBox.Show("Данные успешно добавлены!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            database.CloseConnection();

            GoodsForm goods = new GoodsForm(_user);
            goods.ShowDialog();
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
