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

namespace Кондитерская
{
    public partial class adm : Form
    {
        private string connectionString = "Host=localhost;Database=kond;Username=postgres;Password=seki";
        private NpgsqlConnection connection;
        private BindingSource bindingSource = new BindingSource();
        private bool isFullScreen = true;
        public adm()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.DataSource = bindingSource;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            manager f4 = new manager();
            f4.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ruk f2 = new ruk();
            f2.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
            "Вы хотите полностью выйти из приложения?",
            "Выход",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
            else if (result == DialogResult.No)
            {
                avto f1 = new avto();
                f1.Show();
                this.Hide();
            }
        }

        private void LoadData(string tableName)
        {
            textBox1.Clear();
            bindingSource.Filter = string.Empty;
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT * FROM {tableName};";
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
                    var dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);
                    bindingSource.DataSource = dataTable;

                    if (tableName == "vid")
                    {
                        dataGridView1.Columns["vid"].HeaderText = "Вид продукции";
                    }
                    else if (tableName == "assort")
                    {
                        dataGridView1.Columns["name_iz"].HeaderText = "Название";
                        dataGridView1.Columns["vid"].HeaderText = "Вид";
                        dataGridView1.Columns["ves"].HeaderText = "Вес (кг)";
                        dataGridView1.Columns["price"].HeaderText = "Цена (₽)";
                        dataGridView1.Columns["srok"].HeaderText = "Срок годности (дней)";
                    }
                    else if (tableName == "tovar")
                    {
                        dataGridView1.Columns["id_t"].HeaderText = "ID";
                        dataGridView1.Columns["name_iz"].HeaderText = "Название";
                        dataGridView1.Columns["data"].HeaderText = "Дата поступления";
                        dataGridView1.Columns["kolvo"].HeaderText = "Количество";
                    }
                    else if (tableName == "post")
                    {
                        dataGridView1.Columns["name_p"].HeaderText = "Должность";
                        dataGridView1.Columns["zarp"].HeaderText = "Зарплата (₽)";
                    }
                    else if (tableName == "emp")
                    {
                        dataGridView1.Columns["login"].HeaderText = "Логин";
                        dataGridView1.Columns["password"].HeaderText = "Пароль";
                        dataGridView1.Columns["f_e"].HeaderText = "Фамилия";
                        dataGridView1.Columns["n_e"].HeaderText = "Имя";
                        dataGridView1.Columns["o_e"].HeaderText = "Отчество";
                        dataGridView1.Columns["name_p"].HeaderText = "Должность";
                        dataGridView1.Columns["phone"].HeaderText = "Телефон";
                    }
                    else if (tableName == "prodaji")
                    {
                        dataGridView1.Columns["id_pr"].HeaderText = "ID продажи";
                        dataGridView1.Columns["data_pr"].HeaderText = "Дата";
                        dataGridView1.Columns["price_pr"].HeaderText = "Сумма (₽)";
                        dataGridView1.Columns["login"].HeaderText = "Сотрудник";
                    }
                    else if (tableName == "korzina_pr")
                    {
                        dataGridView1.Columns["id_kpr"].HeaderText = "ID корзины";
                        dataGridView1.Columns["id_t"].HeaderText = "ID товара";
                        dataGridView1.Columns["kolvo_pr"].HeaderText = "Количество";
                        dataGridView1.Columns["id_pr"].HeaderText = "ID продажи";
                    }
                    else if (tableName == "client")
                    {
                        dataGridView1.Columns["email"].HeaderText = "Email";
                        dataGridView1.Columns["f_c"].HeaderText = "Фамилия";
                        dataGridView1.Columns["n_c"].HeaderText = "Имя";
                        dataGridView1.Columns["o_c"].HeaderText = "Отчество";
                        dataGridView1.Columns["phone"].HeaderText = "Телефон";
                    }
                    else if (tableName == "zakaz")
                    {
                        dataGridView1.Columns["id_z"].HeaderText = "ID заказа";
                        dataGridView1.Columns["data_z"].HeaderText = "Дата заказа";
                        dataGridView1.Columns["data_d"].HeaderText = "Дата доставки";
                        dataGridView1.Columns["adres"].HeaderText = "Адрес";
                        dataGridView1.Columns["email"].HeaderText = "Email клиента";
                        dataGridView1.Columns["price_z"].HeaderText = "Сумма (₽)";
                        dataGridView1.Columns["login"].HeaderText = "Сотрудник";
                    }
                    else if (tableName == "korzina_z")
                    {
                        dataGridView1.Columns["id_kz"].HeaderText = "ID корзины";
                        dataGridView1.Columns["name_iz"].HeaderText = "Название";
                        dataGridView1.Columns["kolvo_z"].HeaderText = "Количество";
                        dataGridView1.Columns["id_z"].HeaderText = "ID заказа";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
                }
            }
        }


        private void labelVid_Click(object sender, EventArgs e) => LoadData("vid");
        private void labelAssort_Click(object sender, EventArgs e) => LoadData("assort");
        private void labelTovar_Click(object sender, EventArgs e) => LoadData("tovar");
        private void labelPost_Click(object sender, EventArgs e) => LoadData("post");
        private void labelEmp_Click(object sender, EventArgs e) => LoadData("emp");
        private void labelProdaji_Click(object sender, EventArgs e) => LoadData("prodaji");
        private void labelKorzinaPr_Click(object sender, EventArgs e) => LoadData("korzina_pr");
        private void labelClient_Click(object sender, EventArgs e) => LoadData("client");
        private void labelZakaz_Click(object sender, EventArgs e) => LoadData("zakaz");
        private void labelKorzinaZ_Click(object sender, EventArgs e) => LoadData("korzina_z");

        //поиск
        private void button7_Click(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();  // Получаем текст из TextBox
            SearchData(searchText);  // Фильтруем данные
        }
        // Поиск по всем столбцам
        private void SearchData(string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                // Получаем DataTable из BindingSource
                DataTable dataTable = bindingSource.DataSource as DataTable;
                if (dataTable != null)
                {
                    // Строим условие поиска по всем столбцам
                    StringBuilder filter = new StringBuilder();
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        if (filter.Length > 0)
                            filter.Append(" OR "); // Разделяем условия "ИЛИ" для каждого столбца

                        // Для каждого столбца строим фильтр по его значениям
                        filter.Append($"CONVERT([{column.ColumnName}], 'System.String') LIKE '%{searchText}%'");
                    }

                    // Применяем фильтр
                    bindingSource.Filter = filter.ToString();
                }
            }
            else
            {
                bindingSource.Filter = string.Empty;  // Если строка поиска пустая, сбрасываем фильтр
            }
        }

        //отмена поиска
        private void button6_Click(object sender, EventArgs e)
        {
            bindingSource.Filter = string.Empty;
        }

        //определение таблицы
        private string GetCurrentTable()
        {
            // Проверяем, какая таблица загружена, основываясь на DataSource
            DataTable dataTable = bindingSource.DataSource as DataTable;
            if (dataTable == null || dataTable.Columns.Count == 0)
                return "";

            // Определяем таблицу по структуре столбцов
            if (dataTable.Columns.Contains("vid") && dataTable.Columns.Count == 1)
                return "vid";
            else if (dataTable.Columns.Contains("name_iz") && dataTable.Columns.Contains("ves") && dataTable.Columns.Contains("price"))
                return "assort";
            else if (dataTable.Columns.Contains("id_t") && dataTable.Columns.Contains("name_iz") && dataTable.Columns.Contains("data"))
                return "tovar";
            else if (dataTable.Columns.Contains("name_p") && dataTable.Columns.Contains("zarp"))
                return "post";
            else if (dataTable.Columns.Contains("login") && dataTable.Columns.Contains("password") && dataTable.Columns.Contains("f_e"))
                return "emp";
            else if (dataTable.Columns.Contains("id_pr") && dataTable.Columns.Contains("data_pr"))
                return "prodaji";
            else if (dataTable.Columns.Contains("id_kpr") && dataTable.Columns.Contains("kolvo_pr"))
                return "korzina_pr";
            else if (dataTable.Columns.Contains("email") && dataTable.Columns.Contains("f_c"))
                return "client";
            else if (dataTable.Columns.Contains("id_z") && dataTable.Columns.Contains("data_z"))
                return "zakaz";
            else if (dataTable.Columns.Contains("id_kz") && dataTable.Columns.Contains("kolvo_z"))
                return "korzina_z";

            return "";
        }

        //удалить
        private void button9_Click(object sender, EventArgs e)
        {
            string currentTable = GetCurrentTable();
            if (string.IsNullOrEmpty(currentTable))
            {
                MessageBox.Show("Не выбрана таблица.");
                return;
            }
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите строку для удаления.");
                return;
            }

            // Столбец с первичным ключом в зависимости от выбранной таблицы
            string primaryKeyColumn = "";
            switch (currentTable)
            {
                case "vid": primaryKeyColumn = "vid"; break;
                case "client": primaryKeyColumn = "email"; break;
                case "assort": primaryKeyColumn = "name_iz"; break;
                case "tovar": primaryKeyColumn = "id_t"; break;
                case "post": primaryKeyColumn = "name_p"; break;
                case "emp": primaryKeyColumn = "login"; break;
                case "prodaji": primaryKeyColumn = "id_pr"; break;
                case "korzina_pr": primaryKeyColumn = "id_kpr"; break;
                case "zakaz": primaryKeyColumn = "id_z"; break;
                case "korzina_z": primaryKeyColumn = "id_kz"; break;
                default:
                    MessageBox.Show("Удаление для этой таблицы не поддерживается.");
                    return;
            }

            // Получаем значение ключа из выделенной строки
            var selectedRow = dataGridView1.SelectedRows[0];
            var primaryKeyValue = selectedRow.Cells[primaryKeyColumn].Value;

            // Формируем SQL-запрос для удаления
            string deleteQuery = $"DELETE FROM {currentTable} WHERE {primaryKeyColumn} = @primaryKeyValue";

            // Подтверждение удаления
            DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранную строку?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes)
            {
                return;
            }

            // Выполняем запрос на удаление
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@primaryKeyValue", primaryKeyValue);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();

                    MessageBox.Show("Строка успешно удалена.");
                    LoadData(currentTable);  // Перезагружаем данные в таблице после удаления
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при удалении: {ex.Message}");
            }
        }

        //сохранить
        private void button8_Click(object sender, EventArgs e)
        {
            string currentTable = GetCurrentTable();
            if (string.IsNullOrEmpty(currentTable))
            {
                MessageBox.Show("Не выбрана таблица.");
                return;
            }

            try
            {
                SaveChangesToDatabase(currentTable);
                MessageBox.Show("Данные успешно сохранены!");
                LoadData(currentTable); // Перезагружаем данные
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void adm_Load(object sender, EventArgs e)
        {
            //сотрудник
            label1.Text = $"Авторизован: {user.FullName}";
            //дата и время
            DateTime now = DateTime.Now;
            string shortDay = GetShortDayOfWeek(now.DayOfWeek);
            label9.Text = $"{shortDay} {now:dd.MM.yyyy}";
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

        private void button3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isFullScreen)
            {
                // Возвращаем в нормальный размер
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                isFullScreen = false;
            }
            else
            {
                // Делаем окно полноэкранным
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = FormBorderStyle.None;
                isFullScreen = true;
            }
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Red;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Black;
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Blue;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Black;
        }

        private void SaveChangesToDatabase(string tableName)
        {
            DataTable dataTable = bindingSource.DataSource as DataTable;
            if (dataTable == null) return;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                foreach (DataRow row in dataTable.Rows)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        InsertNewRow(connection, tableName, row);
                    }
                    else if (row.RowState == DataRowState.Modified)
                    {
                        UpdateExistingRow(connection, tableName, row);
                    }
                }
            }
        }

        private void InsertNewRow(NpgsqlConnection connection, string tableName, DataRow row)
        {
            string insertQuery = "";
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;

            switch (tableName)
            {
                case "vid":
                    insertQuery = "INSERT INTO vid (vid) VALUES (@vid)";
                    command.Parameters.AddWithValue("@vid", row["vid"] ?? DBNull.Value);
                    break;

                case "assort":
                    insertQuery = "INSERT INTO assort (name_iz, vid, ves, price, srok) VALUES (@name_iz, @vid, @ves, @price, @srok)";
                    command.Parameters.AddWithValue("@name_iz", row["name_iz"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@vid", row["vid"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ves", row["ves"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@price", row["price"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@srok", row["srok"] ?? DBNull.Value);
                    break;

                case "tovar":
                    insertQuery = "INSERT INTO tovar (name_iz, data, kolvo) VALUES (@name_iz, @data, @kolvo)";
                    command.Parameters.AddWithValue("@name_iz", row["name_iz"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@data", row["data"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@kolvo", row["kolvo"] ?? DBNull.Value);
                    break;

                case "post":
                    insertQuery = "INSERT INTO post (name_p, zarp) VALUES (@name_p, @zarp)";
                    command.Parameters.AddWithValue("@name_p", row["name_p"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@zarp", row["zarp"] ?? DBNull.Value);
                    break;

                case "emp":
                    insertQuery = "INSERT INTO emp (login, password, f_e, n_e, o_e, name_p, phone) VALUES (@login, @password, @f_e, @n_e, @o_e, @name_p, @phone)";
                    command.Parameters.AddWithValue("@login", row["login"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@password", row["password"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@f_e", row["f_e"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@n_e", row["n_e"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@o_e", row["o_e"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@name_p", row["name_p"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@phone", row["phone"] ?? DBNull.Value);
                    break;

                case "prodaji":
                    insertQuery = "INSERT INTO prodaji (data_pr, price_pr, login) VALUES (@data_pr, @price_pr, @login)";
                    command.Parameters.AddWithValue("@data_pr", row["data_pr"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@price_pr", row["price_pr"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@login", row["login"] ?? DBNull.Value);
                    break;

                case "korzina_pr":
                    insertQuery = "INSERT INTO korzina_pr (id_t, kolvo_pr, id_pr) VALUES (@id_t, @kolvo_pr, @id_pr)";
                    command.Parameters.AddWithValue("@id_t", row["id_t"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@kolvo_pr", row["kolvo_pr"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id_pr", row["id_pr"] ?? DBNull.Value);
                    break;

                case "client":
                    insertQuery = "INSERT INTO client (email, f_c, n_c, o_c, phone) VALUES (@email, @f_c, @n_c, @o_c, @phone)";
                    command.Parameters.AddWithValue("@email", row["email"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@f_c", row["f_c"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@n_c", row["n_c"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@o_c", row["o_c"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@phone", row["phone"] ?? DBNull.Value);
                    break;

                case "zakaz":
                    insertQuery = "INSERT INTO zakaz (data_z, data_d, adres, email, price_z, login) VALUES (@data_z, @data_d, @adres, @email, @price_z, @login)";
                    command.Parameters.AddWithValue("@data_z", row["data_z"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@data_d", row["data_d"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@adres", row["adres"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@email", row["email"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@price_z", row["price_z"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@login", row["login"] ?? DBNull.Value);
                    break;

                case "korzina_z":
                    insertQuery = "INSERT INTO korzina_z (name_iz, kolvo_z, id_z) VALUES (@name_iz, @kolvo_z, @id_z)";
                    command.Parameters.AddWithValue("@name_iz", row["name_iz"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@kolvo_z", row["kolvo_z"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id_z", row["id_z"] ?? DBNull.Value);
                    break;
            }

            if (!string.IsNullOrEmpty(insertQuery))
            {
                command.CommandText = insertQuery;
                command.ExecuteNonQuery();
            }
        }

        private void UpdateExistingRow(NpgsqlConnection connection, string tableName, DataRow row)
        {
            string updateQuery = "";
            string primaryKeyColumn = "";
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;

            switch (tableName)
            {
                case "vid":
                    primaryKeyColumn = "vid";
                    updateQuery = "UPDATE vid SET vid = @vid WHERE vid = @original_vid";
                    command.Parameters.AddWithValue("@vid", row["vid"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@original_vid", row["vid", DataRowVersion.Original] ?? DBNull.Value);
                    break;

                case "assort":
                    primaryKeyColumn = "name_iz";
                    updateQuery = "UPDATE assort SET name_iz = @name_iz, vid = @vid, ves = @ves, price = @price, srok = @srok WHERE name_iz = @original_name_iz";
                    command.Parameters.AddWithValue("@name_iz", row["name_iz"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@vid", row["vid"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ves", row["ves"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@price", row["price"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@srok", row["srok"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@original_name_iz", row["name_iz", DataRowVersion.Original] ?? DBNull.Value);
                    break;

                case "tovar":
                    primaryKeyColumn = "id_t";
                    updateQuery = "UPDATE tovar SET name_iz = @name_iz, data = @data, kolvo = @kolvo WHERE id_t = @original_id_t";
                    command.Parameters.AddWithValue("@name_iz", row["name_iz"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@data", row["data"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@kolvo", row["kolvo"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@original_id_t", row["id_t", DataRowVersion.Original] ?? DBNull.Value);
                    break;

                case "post":
                    primaryKeyColumn = "name_p";
                    updateQuery = "UPDATE post SET name_p = @name_p, zarp = @zarp WHERE name_p = @original_name_p";
                    command.Parameters.AddWithValue("@name_p", row["name_p"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@zarp", row["zarp"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@original_name_p", row["name_p", DataRowVersion.Original] ?? DBNull.Value);
                    break;

                case "emp":
                    primaryKeyColumn = "login";
                    updateQuery = "UPDATE emp SET login = @login, password = @password, f_e = @f_e, n_e = @n_e, o_e = @o_e, name_p = @name_p, phone = @phone WHERE login = @original_login";
                    command.Parameters.AddWithValue("@login", row["login"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@password", row["password"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@f_e", row["f_e"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@n_e", row["n_e"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@o_e", row["o_e"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@name_p", row["name_p"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@phone", row["phone"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@original_login", row["login", DataRowVersion.Original] ?? DBNull.Value);
                    break;

                case "prodaji":
                    primaryKeyColumn = "id_pr";
                    updateQuery = "UPDATE prodaji SET data_pr = @data_pr, price_pr = @price_pr, login = @login WHERE id_pr = @original_id_pr";
                    command.Parameters.AddWithValue("@data_pr", row["data_pr"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@price_pr", row["price_pr"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@login", row["login"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@original_id_pr", row["id_pr", DataRowVersion.Original] ?? DBNull.Value);
                    break;

                case "korzina_pr":
                    primaryKeyColumn = "id_kpr";
                    updateQuery = "UPDATE korzina_pr SET id_t = @id_t, kolvo_pr = @kolvo_pr, id_pr = @id_pr WHERE id_kpr = @original_id_kpr";
                    command.Parameters.AddWithValue("@id_t", row["id_t"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@kolvo_pr", row["kolvo_pr"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id_pr", row["id_pr"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@original_id_kpr", row["id_kpr", DataRowVersion.Original] ?? DBNull.Value);
                    break;

                case "client":
                    primaryKeyColumn = "email";
                    updateQuery = "UPDATE client SET email = @email, f_c = @f_c, n_c = @n_c, o_c = @o_c, phone = @phone WHERE email = @original_email";
                    command.Parameters.AddWithValue("@email", row["email"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@f_c", row["f_c"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@n_c", row["n_c"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@o_c", row["o_c"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@phone", row["phone"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@original_email", row["email", DataRowVersion.Original] ?? DBNull.Value);
                    break;

                case "zakaz":
                    primaryKeyColumn = "id_z";
                    updateQuery = "UPDATE zakaz SET data_z = @data_z, data_d = @data_d, adres = @adres, email = @email, price_z = @price_z, login = @login WHERE id_z = @original_id_z";
                    command.Parameters.AddWithValue("@data_z", row["data_z"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@data_d", row["data_d"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@adres", row["adres"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@email", row["email"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@price_z", row["price_z"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@login", row["login"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@original_id_z", row["id_z", DataRowVersion.Original] ?? DBNull.Value);
                    break;

                case "korzina_z":
                    primaryKeyColumn = "id_kz";
                    updateQuery = "UPDATE korzina_z SET name_iz = @name_iz, kolvo_z = @kolvo_z, id_z = @id_z WHERE id_kz = @original_id_kz";
                    command.Parameters.AddWithValue("@name_iz", row["name_iz"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@kolvo_z", row["kolvo_z"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@id_z", row["id_z"] ?? DBNull.Value);
                    command.Parameters.AddWithValue("@original_id_kz", row["id_kz", DataRowVersion.Original] ?? DBNull.Value);
                    break;
            }

            if (!string.IsNullOrEmpty(updateQuery))
            {
                command.CommandText = updateQuery;
                command.ExecuteNonQuery();
            }
        }

    }
}
