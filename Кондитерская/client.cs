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
    public partial class client : Form
    {
        private string connectionString = "Host=localhost;Database=kond;Username=postgres;Password=seki";
        private NpgsqlConnection connection;
        public client()
        {
            InitializeComponent();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button6_MouseEnter(object sender, EventArgs e)
        {
            button6.ForeColor = Color.Red;
        }

        private void button6_MouseLeave(object sender, EventArgs e)
        {
            button6.ForeColor = Color.Black;
        }

        //новый клиент
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверка заполнения всех обязательных полей
                if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                    string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text) ||
                    string.IsNullOrWhiteSpace(textBox4.Text) ||
                    string.IsNullOrWhiteSpace(textBox5.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля: Email, Фамилия, Имя, Отчество, Телефон");
                    return;
                }

                // Проверка валидности email
                if (!textBox1.Text.Contains("@") || !textBox1.Text.Contains("."))
                {
                    MessageBox.Show("Пожалуйста, введите корректный email адрес");
                    return;
                }

                using (connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"INSERT INTO client (email, f_c, n_c, o_c, phone) 
                          VALUES (@email, @lastName, @firstName, @middleName, @phone)";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", textBox1.Text.Trim());
                        command.Parameters.AddWithValue("@lastName", textBox2.Text.Trim());
                        command.Parameters.AddWithValue("@firstName", textBox3.Text.Trim());
                        command.Parameters.AddWithValue("@middleName", textBox4.Text.Trim());
                        command.Parameters.AddWithValue("@phone", textBox5.Text.Trim());

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Клиент успешно добавлен в базу данных!");
                            // Очистка полей после успешного добавления
                            textBox1.Clear();
                            textBox2.Clear();
                            textBox3.Clear();
                            textBox4.Clear();
                            textBox5.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить клиента");
                        }
                    }
                }
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23505")
            {
                MessageBox.Show("Клиент с таким email уже существует в базе данных");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении клиента: {ex.Message}");
            }
        }
    }
}
