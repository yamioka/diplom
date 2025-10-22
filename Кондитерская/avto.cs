using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Кондитерская
{
    public partial class avto : Form
    {
        private string connectionString = "Host=localhost;Database=kond;Username=postgres;Password=seki";
        private NpgsqlConnection connection;
        private bool isPasswordShown = false;
        public avto()
        {
            InitializeComponent();
            textBox2.UseSystemPasswordChar = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox2.Text;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT e.login, e.f_e || ' ' || e.n_e || ' ' || e.o_e AS full_name, e.name_p " +
                               "FROM emp e WHERE e.login = @login AND e.password = @password";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", password);

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Сохраняем данные пользователя в классе CurrentUser
                            user.Login = reader.GetString(0);
                            user.FullName = reader.GetString(1);
                            user.Position = reader.GetString(2);

                            // Переход на форму в зависимости от должности
                            if (user.Position == "Руководитель")
                            {
                                ruk f2 = new ruk();
                                f2.Show();
                                this.Hide();
                            }
                            else if (user.Position == "Администратор")
                            {
                                adm f3 = new adm();
                                f3.Show();
                                this.Hide();
                            }
                            else if (user.Position == "Продавец")
                            {
                                manager f4 = new manager();
                                f4.Show();
                                this.Hide();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль.");
                        }
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            info f5 = new info();
            f5.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (isPasswordShown)
            {
                // Скрыть пароль
                textBox2.UseSystemPasswordChar = true;
                pictureBox1.Image = Image.FromFile("donut.png");
                isPasswordShown = false;
            }
            else
            {
                // Показать пароль
                textBox2.UseSystemPasswordChar = false;
                pictureBox1.Image = Image.FromFile("keks.png");
                isPasswordShown = true;
            }
        }

        private void button6_MouseEnter(object sender, EventArgs e)
        {
            button6.ForeColor = Color.Red;
        }

        private void button6_MouseLeave(object sender, EventArgs e)
        {
            button6.ForeColor = Color.Black;
        }
    }
}
