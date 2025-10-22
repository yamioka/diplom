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
    public partial class manager : Form
    {
        private bool isFullScreen = true;
        public manager()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            prod childForm = new prod();

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panel2.Controls.Clear();
            panel2.Controls.Add(childForm);
            childForm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            zakaz childForm = new zakaz();

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panel2.Controls.Clear();
            panel2.Controls.Add(childForm);
            childForm.Show();
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

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Red;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Black;
        }

        private void manager_Load(object sender, EventArgs e)
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
                button6.Visible = true;
            }
            else if (button6 != null)
            {
                button6.Visible = false;
            }
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

        private void button3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            adm f3 = new adm();
            f3.Show();
            this.Close();
        }
    }
}
