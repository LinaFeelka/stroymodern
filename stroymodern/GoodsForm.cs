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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace stroymodern
{
    public partial class GoodsForm : Form
    {
        enum RowState
        {
            Existed,
            New,
            Modfied,
            ModifidedNew,
            Deleted
        }

        int selectedRow;

        private readonly CheckUser _user;
        Database database = new Database();

        string[] good_filter = { "Все типы", "Дверной проём", "Настенное покрытие", "Материалы", "Мебель" };
        string[] good_sort = { "Название (по возрастанию)", "Номер цеха (по возрастанию)", "Минимальная стоимость (по возрастанию)", "Название (по убыванию)", "Номер цеха (по убыванию)", "Минимальная стоимость (по убыванию)" };
        public GoodsForm(CheckUser user)
        {
            _user = user;
            InitializeComponent();
            button5.Visible = false;

            foreach (string filts in good_filter)
            {
                comboBox1.Items.Add(filts);
            }

            foreach (string sorts in good_sort)
            {
                comboBox2.Items.Add(sorts);
            }
        }

        private void GoodsForm_Load(object sender, EventArgs e)
        {
            IsAdmin();
            label2.Text = $"{_user.Login}:{_user.Status}";
            createColumns();
            refreshDataGrid(dataGridView1);
        }

        private void IsAdmin()
        {
            button1.Enabled = _user.IsAdmin;
            button2.Enabled = _user.IsAdmin;
            button3.Enabled = _user.IsAdmin;
            button4.Enabled = _user.IsAdmin;
            button7.Enabled = _user.IsAdmin;
        }

        private void refreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string querry = $"select name_good, number_plant, price, article, photo, type_good, id from goods";
            NpgsqlCommand command = new NpgsqlCommand(querry, database.GetConnection());

            database.OpenConnection();

            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();

            database.CloseConnection();
        }

        private void Edit()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var id = textBox7.Text;
            var good_name = textBox1.Text;
            var number_plant = textBox2.Text;
            var price = textBox3.Text;
            var article = textBox4.Text;
            var photo = textBox5.Text;
            var type_good = textBox6.Text;


            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView1.Rows[selectedRowIndex].SetValues(good_name, number_plant, price, article, photo, type_good, id);
                dataGridView1.Rows[selectedRowIndex].Cells[7].Value = RowState.Modfied;
            }

        }

        private void DeleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[7].Value = RowState.Deleted;
                return;
            }
            dataGridView1.Rows[index].Cells[7].Value = RowState.Deleted;
        }
        private void Update()
        {
            database.OpenConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[7].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id2 = Convert.ToInt32(textBox7.Text);
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[7].Value);
                    var deleteQuery = $"delete from goods where id = {id2}";

                    var comm = new NpgsqlCommand(deleteQuery, database.GetConnection());
                    comm.ExecuteNonQuery();

                }

                if (rowState == RowState.Modfied)
                {
                    var id = dataGridView1.Rows[index].Cells[6].Value.ToString();
                    var name_good = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var number_plant = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var price = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var article = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var photo = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var type_good = dataGridView1.Rows[index].Cells[5].Value.ToString();

                    var changeQuery = $"update goods set name_good = '{name_good}', number_plant = '{number_plant}', price = '{price}', article = '{article}', photo = '{photo}', type_good = '{type_good}' where id = '{id}'";

                    var comm = new NpgsqlCommand(changeQuery, database.GetConnection());
                    comm.ExecuteNonQuery();
                }
            }
            database.CloseConnection();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Edit();
            ClearFields();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeleteRow();
           // ClearFields();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Update();
        }

        private void ClearFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
        }





        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetString(0), record.GetString(1), record.GetInt64(2), record.GetInt64(3), record.GetString(4), record.GetString(5), record.GetInt32(6), RowState.ModifidedNew);
        }

        /*private void ReadSingleRow2(DataGridView gridView, IDataRecord record)
        {
            gridView.Rows.Add(record.GetString(0), record.GetString(1), record.GetInt64(2), record.GetInt64(3), record.GetString(4), record.GetString(5), record.GetInt32(6), RowState.ModifidedNew);
        } */

        private void createColumns()
        {
            dataGridView1.Columns.Add("name_item", "Название товара"); //0
            dataGridView1.Columns.Add("number_plant", "Номер цеха"); //1
            dataGridView1.Columns.Add("price", "Цена товара"); //2
            dataGridView1.Columns.Add("article", "Артикул");       //3
            dataGridView1.Columns.Add("photo", "Изображение");     //4
            dataGridView1.Columns.Add("type_item", "Категория товара"); //5
            dataGridView1.Columns.Add("id_item", "ID товара"); //6
            dataGridView1.Columns.Add("isNew", String.Empty); //7
            dataGridView1.Columns["isNew"].Visible = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string imagePath = row.Cells[4].Value.ToString();

                if (!string.IsNullOrEmpty(imagePath))
                {

                    pictureBox1.ImageLocation = imagePath;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    MessageBox.Show("Изображение не найдено");
                }
            }

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox1.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
                textBox3.Text = row.Cells[2].Value.ToString();
                textBox4.Text = row.Cells[3].Value.ToString();
                textBox5.Text = row.Cells[4].Value.ToString();
                textBox6.Text = row.Cells[5].Value.ToString();
                textBox7.Text = row.Cells[6].Value.ToString();
            }
        }

        private int clickedRowIndex = -1;

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Сохраняем индекс строки, на которую нажал пользователь правой кнопкой мыши
                clickedRowIndex = e.RowIndex;

                // Показываем panel1
                button5.Visible = true;

            }
        }

        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchString = $"select name_good, number_plant, price, article, photo, type_good, id from goods where concat (name_good, type_good) like '%" + textBoxSearch.Text + "%'";

            NpgsqlCommand comm = new NpgsqlCommand(searchString, database.GetConnection());

            database.OpenConnection();

            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }

            read.Close();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == "Название (по возрастанию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            }
            if (comboBox2.SelectedItem == "Номер цеха (по возрастанию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);
            }
            if (comboBox2.SelectedItem == "Минимальная стоимость (по возрастанию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
            }


            if (comboBox2.SelectedItem == "Название (по убыванию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
            }
            if (comboBox2.SelectedItem == "Номер цеха (по убыванию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Descending);
            }
            if (comboBox2.SelectedItem == "Минимальная стоимость (по убыванию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Descending);
            }
        }


        public void AllTypes(DataGridView dgw)
        {
            dgw.Rows.Clear();

            database.OpenConnection();

            string query = $"select name_good, number_plant, price, article, photo, type_good, id from goods";
            NpgsqlCommand comm = new NpgsqlCommand(query, database.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            database.CloseConnection();
        }

        public void Opening(DataGridView dgw)
        {
            dgw.Rows.Clear();

            database.OpenConnection();

            string query = $"select name_good, number_plant, price, article, photo, type_good, id from goods where type_good LIKE 'Проём'";
            NpgsqlCommand comm = new NpgsqlCommand(query, database.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            database.CloseConnection();
        }


        public void Furniture(DataGridView dgw)
        {
            dgw.Rows.Clear();

            database.OpenConnection();

            string query = $"select name_good, number_plant, price, article, photo, type_good, id from goods where type_good LIKE 'Мебель'";
            NpgsqlCommand comm = new NpgsqlCommand(query, database.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            database.CloseConnection();
        }

        public void WallCovering(DataGridView dgw)
        {
            dgw.Rows.Clear();

            database.OpenConnection();

            string query = $"select name_good, number_plant, price, article, photo, type_good, id from goods where type_good LIKE 'Настенное покрытие'";
            NpgsqlCommand comm = new NpgsqlCommand(query, database.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            database.CloseConnection();
        }

        public void Material(DataGridView dgw)
        {
            dgw.Rows.Clear();

            database.OpenConnection();

            string query = $"select name_good, number_plant, price, article, photo, type_good, id from goods where type_good LIKE 'Материал'";
            NpgsqlCommand comm = new NpgsqlCommand(query, database.GetConnection());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            database.CloseConnection();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == "Все категории")
            {
                AllTypes(dataGridView1);
            }
            if (comboBox1.SelectedItem == "Дверной проём")
            {
                Opening(dataGridView1);
            }
            if (comboBox1.SelectedItem == "Мебель")
            {
                Furniture(dataGridView1);
            }
            if (comboBox1.SelectedItem == "Настенное покрытие")
            {
                WallCovering(dataGridView1);
            }
            if (comboBox1.SelectedItem == "Материалы")
            {
                Material(dataGridView1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddGood addgood = new AddGood(_user);
            addgood.Show();
            Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AddUser adduser = new AddUser(_user);
            adduser.Show();
            Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (clickedRowIndex >= 0 && clickedRowIndex < dataGridView1.Rows.Count)
            {
                string name_good = dataGridView1.Rows[clickedRowIndex].Cells[0].Value.ToString();
                string article = dataGridView1.Rows[clickedRowIndex].Cells[3].Value.ToString();
                decimal price = Convert.ToDecimal(dataGridView1.Rows[clickedRowIndex].Cells[2].Value);

                NpgsqlConnection npgsqlConnection = new NpgsqlConnection("Server = localhost; port = 5432;Database = StroyModern; User Id=postgres; Password = 123");

                OrderForm order = new OrderForm(name_good, article, price, npgsqlConnection, _user);
                order.Show();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SpamForm spam = new SpamForm(_user);
            spam.Show();
            this.Close();
        }
    }
}
