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
    public partial class prod : Form
    {
        private Image defaultImage;
        private string connectionString = "Host=localhost;Database=kond;Username=postgres;Password=seki";
        private NpgsqlConnection connection;
        public prod()
        {
            InitializeComponent();
            LoadData();
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
                    SELECT 
                        t.id_t,
                        a.name_iz AS ""Название"",
                        t.data AS ""Дата"",
                        t.kolvo AS ""Количество"",
                        a.ves AS ""Вес"",
                        a.price AS ""Цена"", a.image,
                        a.srok AS ""Срок годности""
                    FROM tovar t
                    JOIN assort a ON t.name_iz = a.name_iz
                    ORDER BY t.data DESC;";

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
                    var dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;

                    if (dataGridView1.Columns["id_t"] != null)
                        dataGridView1.Columns["id_t"].Visible = false;
                    dataGridView1.Columns["image"].Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }

        //добавить в корзину
        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                string name = selectedRow.Cells["Название"].Value?.ToString() ?? "";
                decimal price = Convert.ToDecimal(selectedRow.Cells["Цена"].Value);
                int availableQuantity = Convert.ToInt32(selectedRow.Cells["Количество"].Value);
                int quantityToAdd = (int)numericUpDown1.Value;
                int productId = Convert.ToInt32(selectedRow.Cells["id_t"].Value);

                if (quantityToAdd <= 0)
                {
                    MessageBox.Show("Введите корректное количество товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (quantityToAdd > availableQuantity)
                {
                    MessageBox.Show("Выбранное количество превышает доступное!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    if (dataGridView2.Columns.Count == 0)
                    {
                        dataGridView2.Columns.Add("Название", "Название");
                        dataGridView2.Columns.Add("Цена", "Цена");
                        dataGridView2.Columns.Add("Количество", "Количество");
                    }

                    dataGridView2.Rows.Add(name, price, quantityToAdd);
                }

                // Уменьшаем количество в таблице товаров
                selectedRow.Cells["Количество"].Value = availableQuantity - quantityToAdd;

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
                // Пропускаем пустые строки и новые строки
                if (row.IsNewRow) continue;

                // Проверяем, что ячейки содержат данные
                if (row.Cells["Цена"].Value != null && row.Cells["Количество"].Value != null)
                {
                    try
                    {
                        decimal price = Convert.ToDecimal(row.Cells["Цена"].Value);
                        int quantity = Convert.ToInt32(row.Cells["Количество"].Value);
                        total += price * quantity;
                    }
                    catch (Exception)
                    {
                        // Если не удается преобразовать значения, пропускаем эту строку
                        continue;
                    }
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
                DataGridViewRow selectedRowCart = dataGridView2.SelectedRows[0];
                string name = selectedRowCart.Cells["Название"].Value.ToString();

                // Найти товар в таблице товаров
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Название"].Value.ToString() == name)
                    {
                        int availableQuantity = Convert.ToInt32(row.Cells["Количество"].Value);

                        if (availableQuantity > 0)
                        {
                            // Увеличиваем количество в корзине
                            int currentQuantityCart = Convert.ToInt32(selectedRowCart.Cells["Количество"].Value);
                            selectedRowCart.Cells["Количество"].Value = currentQuantityCart + 1;

                            // Уменьшаем количество в таблице товаров
                            row.Cells["Количество"].Value = availableQuantity - 1;
                            UpdateCartTotal();
                        }
                        else
                        {
                            MessageBox.Show("Товара больше нет в наличии!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        return;
                    }
                }
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
                DataGridViewRow selectedRowCart = dataGridView2.SelectedRows[0];
                string name = selectedRowCart.Cells["Название"].Value.ToString();
                int currentQuantityCart = Convert.ToInt32(selectedRowCart.Cells["Количество"].Value);

                // Найти товар в таблице товаров
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Название"].Value.ToString() == name)
                    {
                        int availableQuantity = Convert.ToInt32(row.Cells["Количество"].Value);

                        if (currentQuantityCart > 1)
                        {
                            // Уменьшаем количество в корзине
                            selectedRowCart.Cells["Количество"].Value = currentQuantityCart - 1;

                            // Увеличиваем количество в таблице товаров
                            row.Cells["Количество"].Value = availableQuantity + 1;
                        }
                        else
                        {
                            // Если количество становится 0, убираем товар из корзины
                            dataGridView2.Rows.Remove(selectedRowCart);

                            // Полностью возвращаем количество в таблицу товаров
                            row.Cells["Количество"].Value = availableQuantity + currentQuantityCart;
                        }
                        UpdateCartTotal();
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для изменения количества!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //очистить
        private void button2_Click(object sender, EventArgs e)
        {
            // Возвращаем количество всех товаров в таблицу товаров
            foreach (DataGridViewRow cartRow in dataGridView2.Rows)
            {
                // Проверяем, что строка не пустая и не является новой строкой
                if (cartRow.IsNewRow) continue;

                // Проверяем, что все необходимые ячейки содержат данные
                if (cartRow.Cells["Название"].Value == null ||
                    cartRow.Cells["Количество"].Value == null)
                    continue;

                string productName = cartRow.Cells["Название"].Value.ToString();
                int quantity = Convert.ToInt32(cartRow.Cells["Количество"].Value);

                // Ищем товар в таблице товаров и возвращаем количество
                foreach (DataGridViewRow productRow in dataGridView1.Rows)
                {
                    // Проверяем, что строка товара не пустая
                    if (productRow.IsNewRow || productRow.Cells["Название"].Value == null)
                        continue;

                    if (productRow.Cells["Название"].Value.ToString() == productName)
                    {
                        int currentQuantity = Convert.ToInt32(productRow.Cells["Количество"].Value ?? 0);
                        productRow.Cells["Количество"].Value = currentQuantity + quantity;
                        break;
                    }
                }
            }

            // Очищаем корзину
            dataGridView2.Rows.Clear();
            UpdateCartTotal();
        }

        //удалить
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];

                // Проверяем, что выбранная строка содержит данные
                if (selectedRow.IsNewRow ||
                    selectedRow.Cells["Название"].Value == null ||
                    selectedRow.Cells["Количество"].Value == null)
                {
                    MessageBox.Show("Выбрана пустая строка!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string productName = selectedRow.Cells["Название"].Value.ToString();
                int quantity = Convert.ToInt32(selectedRow.Cells["Количество"].Value);

                // Возвращаем количество товара в таблицу товаров
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow || row.Cells["Название"].Value == null)
                        continue;

                    if (row.Cells["Название"].Value.ToString() == productName)
                    {
                        int currentQuantity = Convert.ToInt32(row.Cells["Количество"].Value ?? 0);
                        row.Cells["Количество"].Value = currentQuantity + quantity;
                        break;
                    }
                }

                // Удаляем строку из корзины
                dataGridView2.Rows.Remove(selectedRow);
                UpdateCartTotal();
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления из корзины!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //оформить
        private void button4_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли товары в корзине (исключаем пустые строки)
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
                MessageBox.Show("Корзина пуста! Добавьте товары для оформления продажи.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверяем наличие товаров перед оформлением
            if (!CheckProductAvailability())
            {
                return;
            }

            // Получаем общую сумму
            decimal totalPrice = 0;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                // Пропускаем пустые строки и строки с null-значениями
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
                    continue; // Пропускаем строки с некорректными данными
                }
            }

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Начинаем транзакцию
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Создаем запись в таблице prodaji
                            int saleId;
                            using (var cmd = new NpgsqlCommand())
                            {
                                cmd.Connection = connection;
                                cmd.Transaction = transaction;
                                cmd.CommandText = "INSERT INTO prodaji (data_pr, price_pr, login) VALUES (@data_pr, @price_pr, @login) RETURNING id_pr";
                                cmd.Parameters.AddWithValue("@data_pr", DateTime.Now.Date);
                                cmd.Parameters.AddWithValue("@price_pr", totalPrice);
                                cmd.Parameters.AddWithValue("@login", user.Login);

                                saleId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // 2. Добавляем товары в таблицу korzina_pr и обновляем количество в таблице tovar
                            foreach (DataGridViewRow row in dataGridView2.Rows)
                            {
                                // Пропускаем пустые строки и строки с null-значениями
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
                                    continue; // Пропускаем строки с некорректными данными
                                }

                                // Получаем id_t товара по его названию
                                int productId = 0;
                                foreach (DataGridViewRow productRow in dataGridView1.Rows)
                                {
                                    if (productRow.IsNewRow || productRow.Cells["Название"].Value == null)
                                        continue;

                                    if (productRow.Cells["Название"].Value.ToString() == productName)
                                    {
                                        productId = Convert.ToInt32(productRow.Cells["id_t"].Value);
                                        break;
                                    }
                                }

                                if (productId == 0)
                                {
                                    throw new Exception($"Не удалось найти ID товара '{productName}'");
                                }

                                // Добавляем запись в корзину продажи
                                using (var cmd = new NpgsqlCommand())
                                {
                                    cmd.Connection = connection;
                                    cmd.Transaction = transaction;
                                    cmd.CommandText = "INSERT INTO korzina_pr (id_t, kolvo_pr, id_pr) VALUES (@id_t, @kolvo_pr, @id_pr)";
                                    cmd.Parameters.AddWithValue("@id_t", productId);
                                    cmd.Parameters.AddWithValue("@kolvo_pr", quantity);
                                    cmd.Parameters.AddWithValue("@id_pr", saleId);
                                    cmd.ExecuteNonQuery();
                                }

                                // Обновляем количество товара в таблице tovar
                                using (var cmd = new NpgsqlCommand())
                                {
                                    cmd.Connection = connection;
                                    cmd.Transaction = transaction;
                                    cmd.CommandText = "UPDATE tovar SET kolvo = kolvo - @quantity WHERE id_t = @id_t";
                                    cmd.Parameters.AddWithValue("@quantity", quantity);
                                    cmd.Parameters.AddWithValue("@id_t", productId);
                                    int rowsAffected = cmd.ExecuteNonQuery();

                                    if (rowsAffected == 0)
                                    {
                                        throw new Exception($"Не удалось обновить количество товара '{productName}'");
                                    }
                                }
                            }

                            // Фиксируем транзакцию
                            transaction.Commit();

                            MessageBox.Show($"Продажа успешно оформлена на сумму {totalPrice}₽", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Очищаем корзину и обновляем данные
                            dataGridView2.Rows.Clear();
                            UpdateCartTotal();
                            LoadData(); // Перезагружаем данные о товарах
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка при оформлении продажи: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            dataGridView2.Rows.Clear();
            UpdateCartTotal();
            textBox1.Clear();
            LoadData();
            textBox1.Clear();
            textBox1.Clear();
            textBox1.Clear();
        }

        // Проверка наличия товаров перед оформлением продажи
        private bool CheckProductAvailability()
        {
            // Создаем словарь для хранения общего количества каждого товара в корзине
            Dictionary<string, int> cartQuantities = new Dictionary<string, int>();

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                // Пропускаем пустые строки и новые строки
                if (row.IsNewRow)
                    continue;

                // Проверяем, что ячейки содержат данные
                if (row.Cells["Название"].Value == null || row.Cells["Количество"].Value == null)
                    continue;

                try
                {
                    string productName = row.Cells["Название"].Value.ToString();
                    int quantity = Convert.ToInt32(row.Cells["Количество"].Value);

                    if (cartQuantities.ContainsKey(productName))
                        cartQuantities[productName] += quantity;
                    else
                        cartQuantities[productName] = quantity;
                }
                catch (Exception ex)
                {
                    // Логируем ошибку или просто пропускаем проблемную строку
                    continue;
                }
            }

            // Если корзина пуста после фильтрации, возвращаем true
            if (cartQuantities.Count == 0)
                return true;

            // Проверяем наличие товаров в базе данных
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    foreach (var item in cartQuantities)
                    {
                        string productName = item.Key;
                        int requiredQuantity = item.Value;

                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = connection;
                            cmd.CommandText = "SELECT SUM(kolvo) FROM tovar WHERE name_iz = @name_iz";
                            cmd.Parameters.AddWithValue("@name_iz", productName);

                            object result = cmd.ExecuteScalar();
                            int availableQuantity = result != DBNull.Value ? Convert.ToInt32(result) : 0;

                            if (availableQuantity < requiredQuantity)
                            {
                                MessageBox.Show($"Недостаточное количество товара '{productName}' в наличии.\n" +
                                               $"Требуется: {requiredQuantity}, Доступно: {availableQuantity}",
                                               "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при проверке наличия товаров: {ex.Message}",
                                   "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        private void prod_Load_1(object sender, EventArgs e)
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
                string imagePath = row.Cells["image"].Value?.ToString();

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // Чтобы изображение корректно вставлялось
                    pictureBox1.Image = Image.FromFile(imagePath);
                }
                else
                {
                    pictureBox1.Image = defaultImage; // Если изображения нет, вернуть стандартное
                }
            }
        }
    }
}
