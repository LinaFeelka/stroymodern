using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace stroymodern
{
    public partial class AddUser : Form
    {
        private readonly CheckUser _user;
        Database database = new Database();
        string[] roles = { "Менеджер", "Администратор" };
        public AddUser(CheckUser user)
        {
            _user = user;
            InitializeComponent();

            foreach (string types in roles)
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

        private void button6_Click(object sender, EventArgs e)
        {
            var login = textBox1.Text;
            var password = textBox2.Text;
            var role = 0;
            if (comboBox1.SelectedItem == "Администратор")
            {
                role = 1;
            } else if (comboBox1.SelectedItem == "Менеджер")
            {
                role = 0;
            }

            database.OpenConnection();
            string query = $"insert into users (login, password, id_role) values ('{login}','{password}','{role}')";

            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, database.GetConnection());
            npgsqlCommand.ExecuteNonQuery();

            MessageBox.Show("Данные успешно добавлены!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            database.CloseConnection();

            GoodsForm goods = new GoodsForm(_user);
            goods.ShowDialog();
            this.Close();

        }
    }
}
