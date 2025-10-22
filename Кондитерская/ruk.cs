using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
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
    public partial class ruk : Form
    {
        private bool isFullScreen = true;
        private string connectionString = "Host=localhost;Database=kond;Username=postgres;Password=seki";
        private NpgsqlConnection connection;

        private string selectedProductType = null;
        private string selectedEmployeeLogin = null;
        private string selectedProductName = null;
        public ruk()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
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

        private void ruk_Load(object sender, EventArgs e)
        {
            //сотрудник
            label1.Text = $"Авторизован: {user.FullName}";
            //дата и время
            DateTime now = DateTime.Now;
            string shortDay = GetShortDayOfWeek(now.DayOfWeek);
            label9.Text = $"{shortDay} {now:dd.MM.yyyy}";

            //кнопка назад для админа
            if (user.Position == "Администратор")
            {
                button4.Visible = true;
            }
            else if (button4 != null)
            {
                button4.Visible = false;
            }

            LoadEmployees();
            LoadProductTypes();
            LoadProducts();

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.RowHeadersWidth = 15;

            textBox1.TextChanged += textBox1_TextChanged;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox1.TextChanged += comboBox1_TextChanged;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;
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

        private void button4_Click(object sender, EventArgs e)
        {
            adm f3 = new adm();
            f3.Show();
            this.Close();
        }

        //сотрудники
        private void LoadEmployees()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT login, f_e, n_e, o_e FROM emp ORDER BY f_e";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            comboBox1.Items.Clear();
                            comboBox1.Items.Add("Все сотрудники"); // Опция для выбора всех

                            while (reader.Read())
                            {
                                string login = reader["login"].ToString();
                                string lastName = reader["f_e"].ToString();
                                string firstName = reader["n_e"].ToString();
                                string middleName = reader["o_e"].ToString();

                                // Добавляем в формате "Фамилия И.О. (логин)"
                                string displayText = $"{lastName} {firstName.Substring(0, 1)}.{middleName.Substring(0, 1)}. ({login})";
                                comboBox1.Items.Add(displayText);
                            }
                        }
                    }

                    // Настраиваем автодополнение
                    comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;
                    comboBox1.SelectedIndex = 0; // Выбираем "Все сотрудники" по умолчанию
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке сотрудников: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //виды товаров
        private void LoadProductTypes()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT DISTINCT vid FROM vid ORDER BY vid";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            comboBox2.Items.Clear();
                            comboBox2.Items.Add("Все виды"); // Опция для выбора всех

                            while (reader.Read())
                            {
                                string productType = reader["vid"].ToString();
                                comboBox2.Items.Add(productType);
                            }
                        }
                    }

                    comboBox2.SelectedIndex = 0; // Выбираем "Все виды" по умолчанию
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке видов товаров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //товары
        private void LoadProducts(string productType = null)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                SELECT name_iz AS ""Название"", vid AS ""Вид""
                FROM assort";

                    if (!string.IsNullOrEmpty(productType) && productType != "Все виды")
                    {
                        query += " WHERE vid = @productType";
                    }

                    query += " ORDER BY name_iz";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(productType) && productType != "Все виды")
                        {
                            command.Parameters.AddWithValue("@productType", productType);
                        }

                        NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView2.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string selectedText = comboBox1.SelectedItem.ToString();

                if (selectedText == "Все сотрудники")
                {
                    selectedEmployeeLogin = null;
                }
                else
                {
                    // Извлекаем логин из строки "Фамилия И.О. (логин)"
                    int startIndex = selectedText.LastIndexOf('(') + 1;
                    int endIndex = selectedText.LastIndexOf(')');
                    if (startIndex > 0 && endIndex > startIndex)
                    {
                        selectedEmployeeLogin = selectedText.Substring(startIndex, endIndex - startIndex);
                    }
                }
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = comboBox1.Text.ToLower().Trim();

            if (searchText.Length >= 2)
            {
                // Сохраняем текущую позицию курсора
                int cursorPosition = comboBox1.SelectionStart;

                // Если текст не соответствует точно элементу из списка, показываем выпадающий список
                bool exactMatch = false;
                foreach (var item in comboBox1.Items)
                {
                    if (item.ToString().ToLower() == searchText)
                    {
                        exactMatch = true;
                        break;
                    }
                }

                if (!exactMatch)
                {
                    // Показываем выпадающий список
                    comboBox1.DroppedDown = true;

                    // Фильтруем элементы в выпадающем списке
                    List<object> matchingItems = new List<object>();
                    foreach (var item in comboBox1.Items)
                    {
                        string itemText = item.ToString().ToLower();
                        if (itemText.Contains(searchText))
                        {
                            matchingItems.Add(item);
                        }
                    }

                    // Если есть совпадения, обновляем список
                    if (matchingItems.Count > 0)
                    {
                        comboBox1.Items.Clear();
                        comboBox1.Items.Add("Все сотрудники");

                        foreach (var item in matchingItems)
                        {
                            comboBox1.Items.Add(item);
                        }

                        // Восстанавливаем текст и позицию курсора
                        comboBox1.Text = searchText;
                        comboBox1.SelectionStart = cursorPosition;
                        comboBox1.SelectionLength = 0;
                    }
                    else
                    {
                        // Если нет совпадений, перезагружаем полный список
                        LoadEmployees();
                        comboBox1.Text = searchText;
                        comboBox1.SelectionStart = cursorPosition;
                        comboBox1.SelectionLength = 0;
                    }
                }
            }
            else if (searchText.Length == 0)
            {
                // Если поле пустое, загружаем полный список
                LoadEmployees();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                selectedProductType = comboBox2.SelectedItem.ToString();

                // Обновляем список товаров с учетом поиска в TextBox1
                textBox1_TextChanged(textBox1, EventArgs.Empty);
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                if (selectedRow.Cells["Название"].Value != null)
                {
                    selectedProductName = selectedRow.Cells["Название"].Value.ToString();
                }
            }
            else
            {
                selectedProductName = null;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Проверяем, что выбран хотя бы один тип отчета
            if (!checkBox1.Checked && !checkBox2.Checked)
            {
                MessageBox.Show("Выберите тип отчета (Продажи и/или Заказы)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                GenerateReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод генерации отчета
        private void GenerateReport()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string query = "";
                List<string> unions = new List<string>();

                // Если выбраны продажи
                if (checkBox1.Checked)
                {
                    string salesQuery = @"
        SELECT 
            'Продажа' as ""Тип операции"",
            p.id_pr as ""№ операции"",
            p.data_pr as ""Дата"",
            e.f_e || ' ' || e.n_e || ' ' || e.o_e as ""Сотрудник"",
            a.name_iz as ""Товар"",
            a.vid as ""Вид товара"",
            kp.kolvo_pr as ""Количество"",
            a.price as ""Цена за единицу"",
            (kp.kolvo_pr * a.price) as ""Сумма""";

                    if (checkBox2.Checked)
                    {
                        salesQuery += @",
            '' as ""Клиент"",
            '' as ""Адрес доставки"",
            null as ""Дата доставки""";
                    }

                    salesQuery += @"
        FROM prodaji p
        JOIN emp e ON p.login = e.login
        JOIN korzina_pr kp ON p.id_pr = kp.id_pr
        JOIN tovar t ON kp.id_t = t.id_t
        JOIN assort a ON t.name_iz = a.name_iz
        WHERE 1=1";

                    // Фильтр по сотруднику
                    if (!string.IsNullOrEmpty(selectedEmployeeLogin))
                    {
                        salesQuery += " AND p.login = @employeeLogin";
                    }

                    // Фильтр по товару
                    if (!string.IsNullOrEmpty(selectedProductName))
                    {
                        salesQuery += " AND a.name_iz = @productName";
                    }

                    // Фильтр по виду товара
                    if (!string.IsNullOrEmpty(selectedProductType) && selectedProductType != "Все виды")
                    {
                        salesQuery += " AND a.vid = @productType";
                    }

                    unions.Add(salesQuery);
                }

                // Если выбраны заказы
                if (checkBox2.Checked)
                {
                    string ordersQuery = @"
        SELECT 
            'Заказ' as ""Тип операции"",
            z.id_z as ""№ операции"",
            z.data_z as ""Дата"",
            e.f_e || ' ' || e.n_e || ' ' || e.o_e as ""Сотрудник"",
            a.name_iz as ""Товар"",
            a.vid as ""Вид товара"",
            kz.kolvo_z as ""Количество"",
            a.price as ""Цена за единицу"",
            (kz.kolvo_z * a.price) as ""Сумма"",
            c.f_c || ' ' || c.n_c || ' ' || c.o_c as ""Клиент"",
            z.adres as ""Адрес доставки"",
            z.data_d as ""Дата доставки""
        FROM zakaz z
        JOIN emp e ON z.login = e.login
        JOIN client c ON z.email = c.email
        JOIN korzina_z kz ON z.id_z = kz.id_z
        JOIN assort a ON kz.name_iz = a.name_iz
        WHERE 1=1";

                    // Фильтр по сотруднику
                    if (!string.IsNullOrEmpty(selectedEmployeeLogin))
                    {
                        ordersQuery += " AND z.login = @employeeLogin";
                    }

                    // Фильтр по товару
                    if (!string.IsNullOrEmpty(selectedProductName))
                    {
                        ordersQuery += " AND a.name_iz = @productName";
                    }

                    // Фильтр по виду товара
                    if (!string.IsNullOrEmpty(selectedProductType) && selectedProductType != "Все виды")
                    {
                        ordersQuery += " AND a.vid = @productType";
                    }

                    unions.Add(ordersQuery);
                }

                query = string.Join(" UNION ALL ", unions);
                query += @" ORDER BY ""Дата"" DESC, ""№ операции""";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    // Добавляем параметры
                    if (!string.IsNullOrEmpty(selectedEmployeeLogin))
                    {
                        command.Parameters.AddWithValue("@employeeLogin", selectedEmployeeLogin);
                    }

                    if (!string.IsNullOrEmpty(selectedProductName))
                    {
                        command.Parameters.AddWithValue("@productName", selectedProductName);
                    }

                    if (!string.IsNullOrEmpty(selectedProductType) && selectedProductType != "Все виды")
                    {
                        command.Parameters.AddWithValue("@productType", selectedProductType);
                    }

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;

                    // Настраиваем отображение столбцов
                    if (dataGridView1.Columns.Count > 0)
                    {
                        dataGridView1.Columns["Цена за единицу"].DefaultCellStyle.Format = "C2";
                        dataGridView1.Columns["Сумма"].DefaultCellStyle.Format = "C2";
                        dataGridView1.Columns["Дата"].DefaultCellStyle.Format = "dd.MM.yyyy";

                        if (dataGridView1.Columns["Дата доставки"] != null)
                        {
                            dataGridView1.Columns["Дата доставки"].DefaultCellStyle.Format = "dd.MM.yyyy";
                        }
                    }

                    // Показываем количество записей
                    int recordCount = dataTable.Rows.Count;
                    decimal totalSum = 0;

                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (row["Сумма"] != DBNull.Value)
                        {
                            totalSum += Convert.ToDecimal(row["Сумма"]);
                        }
                    }

                    label6.Text = $"Общая сумма: {totalSum:C2}";

                    // Обновленное сообщение с информацией о фильтрах
                    string statisticsMessage = $"Отчет сформирован!\nКоличество записей: {recordCount}\nОбщая сумма: {totalSum:C2}";

                    if (!string.IsNullOrEmpty(selectedEmployeeLogin))
                    {
                        statisticsMessage += $"\n\nФильтр по сотруднику: {comboBox1.Text}";
                    }

                    if (!string.IsNullOrEmpty(selectedProductType) && selectedProductType != "Все виды")
                    {
                        statisticsMessage += $"\nФильтр по виду товара: {selectedProductType}";
                    }

                    if (!string.IsNullOrEmpty(selectedProductName))
                    {
                        statisticsMessage += $"\nФильтр по товару: {selectedProductName}";
                    }

                    MessageBox.Show(statisticsMessage, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Метод для получения названия отчета
        private string GetReportTitle()
        {
            if (checkBox1.Checked && checkBox2.Checked)
            {
                return "ОТЧЕТ ПО ПРОДАЖАМ И ЗАКАЗАМ";
            }
            else if (checkBox1.Checked)
            {
                return "ОТЧЕТ ПО ПРОДАЖАМ";
            }
            else if (checkBox2.Checked)
            {
                return "ОТЧЕТ ПО ЗАКАЗАМ";
            }
            return "ОТЧЕТ";
        }

        //эксель
        private void button12_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string fileName = $"Отчет_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.FileName = fileName;
                saveFileDialog.Title = "Сохранить отчет в Excel";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToExcelOpenXml(saveFileDialog.FileName);
                    MessageBox.Show("Отчет успешно экспортирован в Excel!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToExcelOpenXml(string filePath)
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                // Добавляем стили
                WorkbookStylesPart stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = CreateSimpleStylesheet();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

                DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());
                DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Отчет"
                };
                sheets.Append(sheet);

                DocumentFormat.OpenXml.Spreadsheet.SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.SheetData>();
                uint rowIndex = 1;

                // Заголовок отчета
                AddStyledRow(sheetData, rowIndex++, GetReportTitle(), 1); // Жирный
                AddStyledRow(sheetData, rowIndex++, "", 0); // Пустая строка

                // Информация о фильтрах (над таблицей)
                if (!string.IsNullOrEmpty(selectedEmployeeLogin))
                    AddStyledRow(sheetData, rowIndex++, $"Фильтр по сотруднику: {comboBox1.Text}", 0);

                if (!string.IsNullOrEmpty(selectedProductType) && selectedProductType != "Все виды")
                    AddStyledRow(sheetData, rowIndex++, $"Фильтр по виду товара: {selectedProductType}", 0);

                if (!string.IsNullOrEmpty(selectedProductName))
                    AddStyledRow(sheetData, rowIndex++, $"Фильтр по товару: {selectedProductName}", 0);

                AddStyledRow(sheetData, rowIndex++, "", 0); // Пустая строка перед таблицей

                uint tableStartRow = rowIndex;

                // Заголовки таблицы
                DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row() { RowIndex = rowIndex++ };
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell()
                    {
                        CellReference = GetColumnLetter(i + 1) + (rowIndex - 1),
                        DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.InlineString,
                        StyleIndex = 2 // Стиль заголовка таблицы
                    };
                    cell.InlineString = new DocumentFormat.OpenXml.Spreadsheet.InlineString()
                    {
                        Text = new DocumentFormat.OpenXml.Spreadsheet.Text(dataGridView1.Columns[i].HeaderText)
                    };
                    headerRow.Append(cell);
                }
                sheetData.Append(headerRow);

                // Данные таблицы
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (!dataGridView1.Rows[i].IsNewRow)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row dataRow = new DocumentFormat.OpenXml.Spreadsheet.Row() { RowIndex = rowIndex++ };
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell()
                            {
                                CellReference = GetColumnLetter(j + 1) + (rowIndex - 1),
                                DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.InlineString,
                                StyleIndex = 3 // Стиль данных таблицы
                            };
                            var cellValue = dataGridView1.Rows[i].Cells[j].Value;
                            cell.InlineString = new DocumentFormat.OpenXml.Spreadsheet.InlineString()
                            {
                                Text = new DocumentFormat.OpenXml.Spreadsheet.Text(cellValue?.ToString() ?? "")
                            };
                            dataRow.Append(cell);
                        }
                        sheetData.Append(dataRow);
                    }
                }

                // Информация под таблицей
                AddStyledRow(sheetData, rowIndex++, "", 0); // Пустая строка после таблицы
                AddStyledRow(sheetData, rowIndex++, $"Сформировал: {user.FullName}", 0);
                AddStyledRow(sheetData, rowIndex++, $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}", 0);
                AddStyledRow(sheetData, rowIndex++, label6.Text, 0);

                SetAutoColumnWidths(worksheetPart, dataGridView1.Columns.Count);
                workbookPart.Workbook.Save();
            }
        }

        // Вспомогательный метод для получения буквы столбца Excel
        private string GetColumnLetter(int columnNumber)
        {
            string columnLetter = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnLetter = Convert.ToChar('A' + modulo) + columnLetter;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnLetter;
        }

        // Добавление строки со стилем
        private void AddStyledRow(DocumentFormat.OpenXml.Spreadsheet.SheetData sheetData, uint rowIndex, string text, uint styleIndex)
        {
            DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row() { RowIndex = rowIndex };
            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell()
            {
                CellReference = "A" + rowIndex,
                DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.InlineString,
                StyleIndex = styleIndex
            };
            cell.InlineString = new DocumentFormat.OpenXml.Spreadsheet.InlineString()
            {
                Text = new DocumentFormat.OpenXml.Spreadsheet.Text(text)
            };
            row.Append(cell);
            sheetData.Append(row);
        }

        // Минимальные стили
        private DocumentFormat.OpenXml.Spreadsheet.Stylesheet CreateSimpleStylesheet()
        {
            var stylesheet = new DocumentFormat.OpenXml.Spreadsheet.Stylesheet();

            // Шрифты
            var fonts = new DocumentFormat.OpenXml.Spreadsheet.Fonts() { Count = 2 };
            fonts.Append(new DocumentFormat.OpenXml.Spreadsheet.Font()); // Обычный
            fonts.Append(new DocumentFormat.OpenXml.Spreadsheet.Font(new DocumentFormat.OpenXml.Spreadsheet.Bold())); // Жирный
            stylesheet.Append(fonts);

            // Заливки
            var fills = new DocumentFormat.OpenXml.Spreadsheet.Fills() { Count = 1 };
            fills.Append(new DocumentFormat.OpenXml.Spreadsheet.Fill());
            stylesheet.Append(fills);

            // Границы
            var borders = new DocumentFormat.OpenXml.Spreadsheet.Borders() { Count = 2 };
            borders.Append(new DocumentFormat.OpenXml.Spreadsheet.Border()); // Без границ

            var borderWithLines = new DocumentFormat.OpenXml.Spreadsheet.Border();
            borderWithLines.Append(new DocumentFormat.OpenXml.Spreadsheet.LeftBorder() { Style = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin });
            borderWithLines.Append(new DocumentFormat.OpenXml.Spreadsheet.RightBorder() { Style = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin });
            borderWithLines.Append(new DocumentFormat.OpenXml.Spreadsheet.TopBorder() { Style = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin });
            borderWithLines.Append(new DocumentFormat.OpenXml.Spreadsheet.BottomBorder() { Style = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin });
            borders.Append(borderWithLines); // С границами
            stylesheet.Append(borders);

            // Форматы ячеек
            var cellFormats = new DocumentFormat.OpenXml.Spreadsheet.CellFormats() { Count = 4 };
            cellFormats.Append(new DocumentFormat.OpenXml.Spreadsheet.CellFormat()); // 0 - обычный
            cellFormats.Append(new DocumentFormat.OpenXml.Spreadsheet.CellFormat() { FontId = 1 }); // 1 - жирный
            cellFormats.Append(new DocumentFormat.OpenXml.Spreadsheet.CellFormat() { FontId = 1, BorderId = 1 }); // 2 - заголовок таблицы
            cellFormats.Append(new DocumentFormat.OpenXml.Spreadsheet.CellFormat() { BorderId = 1 }); // 3 - данные таблицы
            stylesheet.Append(cellFormats);

            return stylesheet;
        }

        // Автоподбор ширины (упрощенный)
        private void SetAutoColumnWidths(WorksheetPart worksheetPart, int columnCount)
        {
            var columnWidths = new double[columnCount];

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
                columnWidths[i] = Math.Max(columnWidths[i], dataGridView1.Columns[i].HeaderText.Length * 1.2);

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (!dataGridView1.Rows[i].IsNewRow)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        string cellText = dataGridView1.Rows[i].Cells[j].Value?.ToString() ?? "";
                        columnWidths[j] = Math.Max(columnWidths[j], cellText.Length * 1.2);
                    }
                }
            }

            var columns = new DocumentFormat.OpenXml.Spreadsheet.Columns();
            for (int i = 0; i < columnCount; i++)
            {
                columns.Append(new DocumentFormat.OpenXml.Spreadsheet.Column()
                {
                    Min = (uint)(i + 1),
                    Max = (uint)(i + 1),
                    Width = Math.Min(columnWidths[i] + 2, 50),
                    CustomWidth = true
                });
            }

            worksheetPart.Worksheet.InsertBefore(columns, worksheetPart.Worksheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.SheetData>());
        }

        //ворд
        private void button11_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string fileName = $"Отчет_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.docx";

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Word files (*.docx)|*.docx";
                saveFileDialog.FileName = fileName;
                saveFileDialog.Title = "Сохранить отчет в Word";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToWordOpenXml(saveFileDialog.FileName);
                    MessageBox.Show("Отчет успешно экспортирован в Word!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Word: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод экспорта в Word с использованием OpenXML
        private void ExportToWordOpenXml(string filePath)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                DocumentFormat.OpenXml.Wordprocessing.Body body = mainPart.Document.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Body());

                // Настройка горизонтальной ориентации страницы
                DocumentFormat.OpenXml.Wordprocessing.SectionProperties sectionProperties = new DocumentFormat.OpenXml.Wordprocessing.SectionProperties();
                DocumentFormat.OpenXml.Wordprocessing.PageSize pageSize = new DocumentFormat.OpenXml.Wordprocessing.PageSize()
                {
                    Width = 16838, // A4 горизонтально
                    Height = 11906,
                    Orient = DocumentFormat.OpenXml.Wordprocessing.PageOrientationValues.Landscape
                };
                sectionProperties.Append(pageSize);

                // Заголовок отчета
                DocumentFormat.OpenXml.Wordprocessing.Paragraph titleParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
                DocumentFormat.OpenXml.Wordprocessing.Run titleRun = new DocumentFormat.OpenXml.Wordprocessing.Run();
                DocumentFormat.OpenXml.Wordprocessing.RunProperties titleRunProperties = new DocumentFormat.OpenXml.Wordprocessing.RunProperties();
                titleRunProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.Bold());
                titleRunProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "28" }); // 14pt
                titleRunProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
                titleRun.Append(titleRunProperties);
                titleRun.Append(new DocumentFormat.OpenXml.Wordprocessing.Text(GetReportTitle()));
                titleParagraph.Append(titleRun);

                DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties titleParagraphProperties = new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties();
                titleParagraphProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.Justification() { Val = DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Center });
                titleParagraph.PrependChild(titleParagraphProperties);

                body.Append(titleParagraph);
                body.Append(CreateParagraphWithFont("")); // Пустая строка

                // Информация о фильтрах (над таблицей)
                if (!string.IsNullOrEmpty(selectedEmployeeLogin))
                {
                    body.Append(CreateParagraphWithFont($"Фильтр по сотруднику: {comboBox1.Text}"));
                }

                if (!string.IsNullOrEmpty(selectedProductType) && selectedProductType != "Все виды")
                {
                    body.Append(CreateParagraphWithFont($"Фильтр по виду товара: {selectedProductType}"));
                }

                if (!string.IsNullOrEmpty(selectedProductName))
                {
                    body.Append(CreateParagraphWithFont($"Фильтр по товару: {selectedProductName}"));
                }

                body.Append(CreateParagraphWithFont("")); // Пустая строка перед таблицей

                // Создание таблицы
                DocumentFormat.OpenXml.Wordprocessing.Table table = new DocumentFormat.OpenXml.Wordprocessing.Table();

                // Свойства таблицы
                DocumentFormat.OpenXml.Wordprocessing.TableProperties tableProperties = new DocumentFormat.OpenXml.Wordprocessing.TableProperties(
                    new DocumentFormat.OpenXml.Wordprocessing.TableBorders(
                        new DocumentFormat.OpenXml.Wordprocessing.TopBorder() { Val = new EnumValue<DocumentFormat.OpenXml.Wordprocessing.BorderValues>(DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single), Size = 12 },
                        new DocumentFormat.OpenXml.Wordprocessing.BottomBorder() { Val = new EnumValue<DocumentFormat.OpenXml.Wordprocessing.BorderValues>(DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single), Size = 12 },
                        new DocumentFormat.OpenXml.Wordprocessing.LeftBorder() { Val = new EnumValue<DocumentFormat.OpenXml.Wordprocessing.BorderValues>(DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single), Size = 12 },
                        new DocumentFormat.OpenXml.Wordprocessing.RightBorder() { Val = new EnumValue<DocumentFormat.OpenXml.Wordprocessing.BorderValues>(DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single), Size = 12 },
                        new DocumentFormat.OpenXml.Wordprocessing.InsideHorizontalBorder() { Val = new EnumValue<DocumentFormat.OpenXml.Wordprocessing.BorderValues>(DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single), Size = 12 },
                        new DocumentFormat.OpenXml.Wordprocessing.InsideVerticalBorder() { Val = new EnumValue<DocumentFormat.OpenXml.Wordprocessing.BorderValues>(DocumentFormat.OpenXml.Wordprocessing.BorderValues.Single), Size = 12 }
                    ),
                    new DocumentFormat.OpenXml.Wordprocessing.TableWidth() { Type = DocumentFormat.OpenXml.Wordprocessing.TableWidthUnitValues.Pct, Width = "100%" }
                );
                table.AppendChild(tableProperties);

                // Заголовки столбцов
                DocumentFormat.OpenXml.Wordprocessing.TableRow headerRow = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    DocumentFormat.OpenXml.Wordprocessing.TableCell headerCell = new DocumentFormat.OpenXml.Wordprocessing.TableCell();

                    DocumentFormat.OpenXml.Wordprocessing.Paragraph headerParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
                    DocumentFormat.OpenXml.Wordprocessing.Run headerRun = new DocumentFormat.OpenXml.Wordprocessing.Run();
                    DocumentFormat.OpenXml.Wordprocessing.RunProperties headerRunProperties = new DocumentFormat.OpenXml.Wordprocessing.RunProperties();
                    headerRunProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.Bold());
                    headerRunProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" }); // 12pt
                    headerRunProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
                    headerRun.Append(headerRunProperties);
                    headerRun.Append(new DocumentFormat.OpenXml.Wordprocessing.Text(dataGridView1.Columns[i].HeaderText));
                    headerParagraph.Append(headerRun);

                    headerCell.Append(headerParagraph);
                    headerRow.Append(headerCell);
                }
                table.Append(headerRow);

                // Данные
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (!dataGridView1.Rows[i].IsNewRow)
                    {
                        DocumentFormat.OpenXml.Wordprocessing.TableRow dataRow = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            DocumentFormat.OpenXml.Wordprocessing.TableCell dataCell = new DocumentFormat.OpenXml.Wordprocessing.TableCell();
                            var cellValue = dataGridView1.Rows[i].Cells[j].Value;

                            DocumentFormat.OpenXml.Wordprocessing.Paragraph dataParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
                            DocumentFormat.OpenXml.Wordprocessing.Run dataRun = new DocumentFormat.OpenXml.Wordprocessing.Run();
                            DocumentFormat.OpenXml.Wordprocessing.RunProperties dataRunProperties = new DocumentFormat.OpenXml.Wordprocessing.RunProperties();
                            dataRunProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" }); // 12pt
                            dataRunProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
                            dataRun.Append(dataRunProperties);
                            dataRun.Append(new DocumentFormat.OpenXml.Wordprocessing.Text(cellValue?.ToString() ?? ""));
                            dataParagraph.Append(dataRun);

                            dataCell.Append(dataParagraph);
                            dataRow.Append(dataCell);
                        }
                        table.Append(dataRow);
                    }
                }

                body.Append(table);

                // Информация под таблицей
                body.Append(CreateParagraphWithFont("")); // Пустая строка после таблицы
                body.Append(CreateParagraphWithFont($"Сформировал: {user.FullName}"));
                body.Append(CreateParagraphWithFont($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}"));
                body.Append(CreateParagraphWithFont(label6.Text));

                body.Append(sectionProperties);
                mainPart.Document.Save();
            }
        }

        // Вспомогательный метод для создания параграфа с шрифтом Times New Roman 14pt
        private DocumentFormat.OpenXml.Wordprocessing.Paragraph CreateParagraphWithFont(string text)
        {
            DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
            DocumentFormat.OpenXml.Wordprocessing.Run run = new DocumentFormat.OpenXml.Wordprocessing.Run();
            DocumentFormat.OpenXml.Wordprocessing.RunProperties runProperties = new DocumentFormat.OpenXml.Wordprocessing.RunProperties();
            runProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "28" }); // 14pt
            runProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" });
            run.Append(runProperties);
            run.Append(new DocumentFormat.OpenXml.Wordprocessing.Text(text));
            paragraph.Append(run);
            return paragraph;
        }

        // Вспомогательный метод для создания параграфа
        private DocumentFormat.OpenXml.Wordprocessing.Paragraph CreateParagraph(string text)
        {
            return new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                new DocumentFormat.OpenXml.Wordprocessing.Run(
                    new DocumentFormat.OpenXml.Wordprocessing.Text(text)
                )
            );
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                SELECT name_iz AS ""Название"", vid AS ""Вид""
                FROM assort
                WHERE 1=1";

                    List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

                    // Фильтр по названию товара (если введен текст)
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query += " AND LOWER(name_iz) LIKE @searchText";
                        parameters.Add(new NpgsqlParameter("@searchText", $"%{searchText.ToLower()}%"));
                    }

                    // Фильтр по виду товара (если выбран в ComboBox2)
                    if (!string.IsNullOrEmpty(selectedProductType) && selectedProductType != "Все виды")
                    {
                        query += " AND vid = @productType";
                        parameters.Add(new NpgsqlParameter("@productType", selectedProductType));
                    }

                    query += " ORDER BY name_iz";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());

                        NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView2.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при поиске товаров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                // Очищаем переменные фильтров
                selectedEmployeeLogin = null;
                selectedProductName = null;
                selectedProductType = null;

                // Сбрасываем ComboBox'ы на значения по умолчанию
                comboBox1.SelectedIndex = 0; // "Все сотрудники"
                comboBox2.SelectedIndex = 0; // "Все виды"

                // Очищаем TextBox поиска
                textBox1.Clear();

                // Снимаем галочки с CheckBox'ов
                checkBox1.Checked = false;
                checkBox2.Checked = false;

                // Очищаем DataGridView с отчетом
                dataGridView1.DataSource = null;

                // Перезагружаем все товары в DataGridView2
                LoadProducts();

                // Очищаем label с общей суммой
                label6.Text = "Общая сумма: 0,00 ₽";

                // Снимаем выделение в DataGridView2
                dataGridView2.ClearSelection();

                MessageBox.Show("Все фильтры очищены!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при очистке фильтров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}