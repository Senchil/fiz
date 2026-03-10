using System;
using System.Drawing;
using System.Windows.Forms;
using fiz.Data;

namespace fiz.Forms
{
    public partial class StudentForm : Form
    {
        public StudentForm()
        {
            CreateUI();
        }

        private void CreateUI()
        {
            this.Text = "Список студентов";
            this.Size = new Size(1200, 700);
            this.BackColor = Color.White;
            this.WindowState = FormWindowState.Maximized;

            // Шапка
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(32, 178, 170);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 60;

            Label titleLabel = new Label();
            titleLabel.Text = "Спортивные достижения студентов";
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            titleLabel.AutoSize = false;
            titleLabel.Dock = DockStyle.Left;
            titleLabel.Width = 400;
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            titleLabel.Padding = new Padding(20, 0, 0, 0);
            headerPanel.Controls.Add(titleLabel);

            Label adminLabel = new Label();
            adminLabel.Text = $"Админ: {Database.CurrentUser?.Login}";
            adminLabel.ForeColor = Color.White;
            adminLabel.Font = new Font("Microsoft Sans Serif", 11F);
            adminLabel.AutoSize = true;
            adminLabel.Location = new Point(450, 18);
            headerPanel.Controls.Add(adminLabel);

            Button logoutButton = new Button();
            logoutButton.Text = "Выйти";
            logoutButton.BackColor = Color.White;
            logoutButton.ForeColor = Color.FromArgb(32, 178, 170);
            logoutButton.FlatStyle = FlatStyle.Flat;
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            logoutButton.Size = new Size(100, 35);
            logoutButton.Location = new Point(this.Width - 130, 12);
            logoutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logoutButton.Cursor = Cursors.Hand;
            logoutButton.Click += (s, e) => { Database.CurrentUser = null; this.Close(); };
            headerPanel.Controls.Add(logoutButton);

            // Кнопки
            Button addBtn = new Button();
            addBtn.Text = "Добавить студента";
            addBtn.BackColor = Color.FromArgb(32, 178, 170);
            addBtn.ForeColor = Color.White;
            addBtn.FlatStyle = FlatStyle.Flat;
            addBtn.FlatAppearance.BorderSize = 0;
            addBtn.Size = new Size(180, 35);
            addBtn.Location = new Point(10, 70);
            addBtn.Cursor = Cursors.Hand;
            this.Controls.Add(addBtn);

            Button editBtn = new Button();
            editBtn.Text = "Редактировать";
            editBtn.BackColor = Color.FromArgb(32, 178, 170);
            editBtn.ForeColor = Color.White;
            editBtn.FlatStyle = FlatStyle.Flat;
            editBtn.FlatAppearance.BorderSize = 0;
            editBtn.Size = new Size(150, 35);
            editBtn.Location = new Point(200, 70);
            editBtn.Cursor = Cursors.Hand;
            this.Controls.Add(editBtn);

            // Таблица
            DataGridView grid = new DataGridView();
            grid.Location = new Point(10, 115);
            grid.Size = new Size(this.Width - 30, this.Height - 135);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;

            grid.Columns.Add("Id", "№");
            grid.Columns.Add("FullName", "ФИО");
            grid.Columns.Add("Faculty", "Факультет");
            grid.Columns.Add("Group", "Группа");
            grid.Columns.Add("StudentCardNumber", "Номер студ. билета");
            grid.Columns.Add("BirthDate", "Дата рождения");
            grid.Columns.Add("ContactInfo", "Контактные данные");

            foreach (var student in Database.Students)
            {
                grid.Rows.Add(student.Id, student.FullName, student.Faculty, student.Group,
                    student.StudentCardNumber, student.BirthDate, student.ContactInfo);
            }

            this.Controls.Add(grid);
            this.Controls.Add(headerPanel);
        }
    }
}