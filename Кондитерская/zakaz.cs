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
using System.IO;

namespace Кондитерская
{
    public partial class zakaz : Form
    {
        private Image defaultImage;
        private string connectionString = "Host=localhost;Database=kond;Username=postgres;Password=seki";
        private NpgsqlConnection connection;
        private string selectedClientPhone = null;
        public zakaz()
        {
            InitializeComponent();
            LoadData();
            LoadClients();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            defaultImage = Image.FromFile("111.png");
            pictureBox1.Image = defaultImage;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        //вывод товаров
        private void LoadData()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                    SELECT name_iz AS ""Название"", vid AS ""Вид"", ves AS ""Вес"", price AS ""Цена"", srok AS ""Срок годности"", image
                    FROM assort";

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
                    var dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;

                    dataGridView1.Columns["image"].Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }


        // Метод для загрузки клиентов в comboBox1
        private void LoadClients()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT phone FROM client WHERE phone IS NOT NULL ORDER BY f_c";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            comboBox1.Items.Clear();

                            while (reader.Read())
                            {
                                string phone = reader["phone"].ToString();
                                if (!string.IsNullOrEmpty(phone))
                                {
                                    comboBox1.Items.Add(phone);
                                }
                            }
                        }
                    }

                    // Настраиваем автодополнение
                    comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Метод для загрузки информации о клиенте по телефону
        private void LoadClientInfo(string phone)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT f_c, n_c, o_c FROM client WHERE phone = @phone";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@phone", phone);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string lastName = reader["f_c"].ToString();
                                string firstName = reader["n_c"].ToString();
                                string middleName = reader["o_c"].ToString();

                                // Сохраняем телефон для использования при оформлении заказа
                                selectedClientPhone = phone;

                                // Отображаем ФИО в textBox3
                                textBox3.Text = $"{lastName} {firstName} {middleName}".Trim();
                            }
                            else
                            {
                                // Клиент не найден
                                textBox3.Text = "Клиент не найден";
                                selectedClientPhone = null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке информации о клиенте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox3.Clear();
                    selectedClientPhone = null;
                }
            }
        }

        //добавить
        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                string name = selectedRow.Cells["Название"].Value?.ToString() ?? "";
                decimal price = Convert.ToDecimal(selectedRow.Cells["Цена"].Value);
                int quantityToAdd = (int)numericUpDown1.Value;

                if (quantityToAdd <= 0)
                {
                    MessageBox.Show("Введите корректное количество товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool itemExists = false;
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells["Название"].Value?.ToString() == name)
                    {
                        itemExists = true;
                        int existingQuantity = Convert.ToInt32(row.Cells["Количество"].Value);
                        row.Cells["Количество"].Value = existingQuantity + quantityToAdd;
                        break;
                    }
                }

                if (!itemExists)
                {
                    dataGridView2.Rows.Add(name, price, quantityToAdd);
                }

                UpdateCartTotal();
            }
            else
            {
                MessageBox.Show("Выберите товар для добавления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //общая цена
        private void UpdateCartTotal()
        {
            decimal total = 0;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells["Цена"].Value != null && row.Cells["Количество"].Value != null)
                {
                    decimal price = Convert.ToDecimal(row.Cells["Цена"].Value);
                    int quantity = Convert.ToInt32(row.Cells["Количество"].Value);
                    total += price * quantity;
                }
            }
            label3.Text = $"Общая:{total}₽";
        }

        //поиск
        private void button3_Click(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();

            if (dataGridView1.DataSource is System.Data.DataTable dataTable)
            {
                if (dataTable.Rows.Count > 0)
                {
                    var dv = new System.Data.DataView(dataTable);
                    dv.RowFilter = $"Название LIKE '%{searchText}%'";
                    dataGridView1.DataSource = dv;
                }
            }
        }

        //отмена поиска
        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            LoadData();
        }
        //+
        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                int currentQuantity = Convert.ToInt32(selectedRow.Cells["Количество"].Value);
                selectedRow.Cells["Количество"].Value = currentQuantity + 1;
                UpdateCartTotal();
            }
            else
            {
                MessageBox.Show("Выберите товар для изменения количества!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //-
        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                int currentQuantity = Convert.ToInt32(selectedRow.Cells["Количество"].Value);

                if (currentQuantity > 1)
                {
                    selectedRow.Cells["Количество"].Value = currentQuantity - 1;
                }
                else
                {
                    dataGridView2.Rows.Remove(selectedRow);
                }

                UpdateCartTotal();
            }
            else
            {
                MessageBox.Show("Выберите товар для изменения количества!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //очистить
        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            UpdateCartTotal();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            LoadData();
        }

        //удалить
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                dataGridView2.Rows.Remove(selectedRow);
                UpdateCartTotal();
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления из заказа!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //оформить заказ
        private void button4_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли товары в корзине
            int validRowsCount = 0;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (!row.IsNewRow && row.Cells["Название"].Value != null && row.Cells["Количество"].Value != null)
                {
                    validRowsCount++;
                }
            }

            if (validRowsCount == 0)
            {
                MessageBox.Show("Корзина пуста! Добавьте товары для оформления заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверяем, выбран ли клиент
            if (string.IsNullOrEmpty(selectedClientPhone))
            {
                MessageBox.Show("Выберите клиента для оформления заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверяем, указан ли адрес доставки
            if (string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                MessageBox.Show("Укажите адрес доставки.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем общую сумму
            decimal totalPrice = 0;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.IsNewRow || row.Cells["Цена"].Value == null || row.Cells["Количество"].Value == null)
                    continue;

                try
                {
                    decimal price = Convert.ToDecimal(row.Cells["Цена"].Value);
                    int quantity = Convert.ToInt32(row.Cells["Количество"].Value);
                    totalPrice += price * quantity;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // Получаем email клиента по телефону для внешнего ключа
            string clientEmail = GetClientEmailByPhone(selectedClientPhone);
            if (string.IsNullOrEmpty(clientEmail))
            {
                MessageBox.Show("Не удалось найти email клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Создаем запись в таблице zakaz
                            int orderId;
                            using (var cmd = new NpgsqlCommand())
                            {
                                cmd.Connection = connection;
                                cmd.Transaction = transaction;
                                cmd.CommandText = @"
                            INSERT INTO zakaz (data_z, data_d, adres, email, price_z, login) 
                            VALUES (@data_z, @data_d, @adres, @email, @price_z, @login) 
                            RETURNING id_z";
                                cmd.Parameters.AddWithValue("@data_z", DateTime.Now.Date);
                                cmd.Parameters.AddWithValue("@data_d", dateTimePicker1.Value.Date);
                                cmd.Parameters.AddWithValue("@adres", textBox2.Text.Trim());
                                cmd.Parameters.AddWithValue("@email", clientEmail);
                                cmd.Parameters.AddWithValue("@price_z", totalPrice);
                                cmd.Parameters.AddWithValue("@login", user.Login);

                                orderId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // 2. Добавляем товары в таблицу korzina_z
                            foreach (DataGridViewRow row in dataGridView2.Rows)
                            {
                                if (row.IsNewRow ||
                                    row.Cells["Название"].Value == null ||
                                    row.Cells["Количество"].Value == null)
                                    continue;

                                string productName;
                                int quantity;

                                try
                                {
                                    productName = row.Cells["Название"].Value.ToString();
                                    quantity = Convert.ToInt32(row.Cells["Количество"].Value);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }

                                // Добавляем запись в корзину заказа
                                using (var cmd = new NpgsqlCommand())
                                {
                                    cmd.Connection = connection;
                                    cmd.Transaction = transaction;
                                    cmd.CommandText = "INSERT INTO korzina_z (name_iz, kolvo_z, id_z) VALUES (@name_iz, @kolvo_z, @id_z)";
                                    cmd.Parameters.AddWithValue("@name_iz", productName);
                                    cmd.Parameters.AddWithValue("@kolvo_z", quantity);
                                    cmd.Parameters.AddWithValue("@id_z", orderId);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // Фиксируем транзакцию
                            transaction.Commit();

                            MessageBox.Show($"Заказ №{orderId} успешно оформлен на сумму {totalPrice}₽\n" +
                                           $"Клиент: {textBox3.Text}\n" +
                                           $"Дата доставки: {dateTimePicker1.Value.ToShortDateString()}",
                                           "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Очищаем форму
                            dataGridView2.Rows.Clear();
                            UpdateCartTotal();
                            textBox1.Clear();
                            textBox2.Clear();
                            textBox3.Clear();
                            comboBox1.Text = "";
                            selectedClientPhone = null;
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GetClientEmailByPhone(string phone)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT email FROM client WHERE phone = @phone";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@phone", phone);
                        object result = command.ExecuteScalar();
                        return result?.ToString() ?? "";
                    }
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        private void zakaz_Load(object sender, EventArgs e)
        {
            //сотрудник
            label1.Text = $"Авторизован: {user.FullName}";
            //дата и время
            DateTime now = DateTime.Now;
            string shortDay = GetShortDayOfWeek(now.DayOfWeek);
            label9.Text = $"{shortDay} {now:dd.MM.yyyy}";


            if (dataGridView2.Columns.Count == 0)
            {
                dataGridView2.Columns.Add("Название", "Название");
                dataGridView2.Columns.Add("Цена", "Цена");
                dataGridView2.Columns.Add("Количество", "Количество");

                dataGridView2.Columns["Название"].Width = 160;     // Название - широкий столбец
                dataGridView2.Columns["Цена"].Width = 140;          // Цена - узкий столбец
                dataGridView2.Columns["Количество"].Width = 40;
            }
            dataGridView2.RowHeadersWidth = 15;
        }

        //короткие дни недели
        private string GetShortDayOfWeek(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Пн.",
                DayOfWeek.Tuesday => "Вт.",
                DayOfWeek.Wednesday => "Ср.",
                DayOfWeek.Thursday => "Чт.",
                DayOfWeek.Friday => "Пт.",
                DayOfWeek.Saturday => "Сб.",
                DayOfWeek.Sunday => "Вс.",
                _ => string.Empty
            };
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string relativePath = row.Cells["image"].Value?.ToString(); // Относительный путь из БД

                if (!string.IsNullOrEmpty(relativePath))
                {
                    string cakePath = FindImagePath(relativePath);

                    if (!string.IsNullOrEmpty(cakePath) && File.Exists(cakePath))
                    {
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // Чтобы изображение корректно вставлялось
                        pictureBox1.Image = Image.FromFile(cakePath);
                    }
                    else
                    {
                        pictureBox1.Image = defaultImage;
                    }
                }
                else
                {
                    pictureBox1.Image = defaultImage;
                }
            }
        }

        private string FindImagePath(string relativePath)
        {
            string[] drives = Environment.GetLogicalDrives(); // Получаем список всех дисков
            string cakePath = null; // Здесь будет итоговый путь

            foreach (string drive in drives)
            {
                string candidate = Path.Combine(drive, relativePath);
                if (File.Exists(candidate))
                {
                    cakePath = candidate; // Нашли файл — сохраняем путь
                    break;
                }
            }

            return cakePath;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string selectedPhone = comboBox1.SelectedItem.ToString();
                LoadClientInfo(selectedPhone);
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            string enteredPhone = comboBox1.Text.Trim();

            // Если введен полный номер, пытаемся найти клиента
            if (enteredPhone.Length >= 10)
            {
                LoadClientInfo(enteredPhone);
            }
            else
            {
                // Очищаем информацию о клиенте, если номер неполный
                textBox3.Clear();
                selectedClientPhone = null;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            client client = new client();
            client.Show();
        }
    }
}
