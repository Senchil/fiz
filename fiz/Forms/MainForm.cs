using System;
using System.Drawing;
using System.Windows.Forms;
using fiz.Data;

namespace fiz
{
    public partial class MainForm : Form
    {
        private TabControl mainTabControl;
        private Panel headerPanel;
        private Label welcomeLabel;
        private Button logoutButton;

        public MainForm()
        {
            //InitializeComponent();
            InitializeCustomControls();
        }

        private void InitializeCustomControls()
        {
            this.Text = "Спортивные достижения студентов";
            this.Size = new Size(1200, 700);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            // Шапка
            headerPanel = new Panel();
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

            welcomeLabel = new Label();
            welcomeLabel.Text = $"Админ: {Database.CurrentUser?.Login}";
            welcomeLabel.ForeColor = Color.White;
            welcomeLabel.Font = new Font("Microsoft Sans Serif", 11F);
            welcomeLabel.AutoSize = true;
            welcomeLabel.Location = new Point(450, 18);
            headerPanel.Controls.Add(welcomeLabel);

            logoutButton = new Button();
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
            logoutButton.Click += LogoutButton_Click;
            headerPanel.Controls.Add(logoutButton);

            // Табы
            mainTabControl = new TabControl();
            mainTabControl.Dock = DockStyle.Fill;
            mainTabControl.Font = new Font("Microsoft Sans Serif", 11F);
            mainTabControl.ItemSize = new Size(120, 40);

            // Страница Студенты
            TabPage studentsTab = new TabPage("Студенты");
            studentsTab.Padding = new Padding(10);
            mainTabControl.TabPages.Add(studentsTab);

            // Страница Мероприятия
            TabPage eventsTab = new TabPage("Мероприятия");
            eventsTab.Padding = new Padding(10);
            mainTabControl.TabPages.Add(eventsTab);

            // Страница Участия
            TabPage participationsTab = new TabPage("Участия");
            participationsTab.Padding = new Padding(10);
            mainTabControl.TabPages.Add(participationsTab);

            // Добавляем контролы на страницы
            InitializeStudentsTab(studentsTab);
            InitializeEventsTab(eventsTab);
            InitializeParticipationsTab(participationsTab);

            this.Controls.Add(mainTabControl);
            this.Controls.Add(headerPanel);
        }

        private void InitializeStudentsTab(TabPage tab)
        {
            Button addStudentBtn = new Button();
            addStudentBtn.Text = "Добавить студента";
            addStudentBtn.BackColor = Color.FromArgb(32, 178, 170);
            addStudentBtn.ForeColor = Color.White;
            addStudentBtn.FlatStyle = FlatStyle.Flat;
            addStudentBtn.FlatAppearance.BorderSize = 0;
            addStudentBtn.Size = new Size(180, 35);
            addStudentBtn.Location = new Point(10, 10);
            addStudentBtn.Cursor = Cursors.Hand;
            addStudentBtn.Click += (s, e) => OpenStudentForm();
            tab.Controls.Add(addStudentBtn);

            Button editStudentBtn = new Button();
            editStudentBtn.Text = "Редактировать";
            editStudentBtn.BackColor = Color.FromArgb(32, 178, 170);
            editStudentBtn.ForeColor = Color.White;
            editStudentBtn.FlatStyle = FlatStyle.Flat;
            editStudentBtn.FlatAppearance.BorderSize = 0;
            editStudentBtn.Size = new Size(150, 35);
            editStudentBtn.Location = new Point(200, 10);
            editStudentBtn.Cursor = Cursors.Hand;
            tab.Controls.Add(editStudentBtn);

            DataGridView grid = new DataGridView();
            grid.Location = new Point(10, 55);
            grid.Size = new Size(tab.Width - 30, tab.Height - 75);
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

            RefreshStudentGrid(grid);
            tab.Controls.Add(grid);
        }

        private void RefreshStudentGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            var students = Database.GetStudents();
            foreach (var student in students)
            {
                grid.Rows.Add(student.Id, student.FullName, student.Faculty, student.Group,
                    student.StudentCardNumber, student.BirthDate, student.ContactInfo);
            }
        }

        private void RefreshEventGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            var events = Database.GetEvents();
            foreach (var ev in events)
            {
                grid.Rows.Add(ev.Id, ev.Name, ev.Date, ev.Location, ev.Organizer, ev.ParticipantCount, ev.SportType);
            }
        }

        private void RefreshParticipationGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            var participations = Database.GetParticipations();
            foreach (var p in participations)
            {
                grid.Rows.Add(p.Id, p.EventName, p.StudentName, p.Result, p.Award, p.Rank, p.AddedBy, p.Date);
            }
        }

        private void InitializeEventsTab(TabPage tab)
        {
            Button addEventBtn = new Button();
            addEventBtn.Text = "Добавить мероприятие";
            addEventBtn.BackColor = Color.FromArgb(32, 178, 170);
            addEventBtn.ForeColor = Color.White;
            addEventBtn.FlatStyle = FlatStyle.Flat;
            addEventBtn.FlatAppearance.BorderSize = 0;
            addEventBtn.Size = new Size(200, 35);
            addEventBtn.Location = new Point(10, 10);
            addEventBtn.Cursor = Cursors.Hand;
            addEventBtn.Click += (s, e) => OpenEventForm();
            tab.Controls.Add(addEventBtn);

            Button editEventBtn = new Button();
            editEventBtn.Text = "Редактировать";
            editEventBtn.BackColor = Color.FromArgb(32, 178, 170);
            editEventBtn.ForeColor = Color.White;
            editEventBtn.FlatStyle = FlatStyle.Flat;
            editEventBtn.FlatAppearance.BorderSize = 0;
            editEventBtn.Size = new Size(150, 35);
            editEventBtn.Location = new Point(220, 10);
            editEventBtn.Cursor = Cursors.Hand;
            tab.Controls.Add(editEventBtn);

            DataGridView grid = new DataGridView();
            grid.Location = new Point(10, 55);
            grid.Size = new Size(tab.Width - 30, tab.Height - 75);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;

            grid.Columns.Add("Id", "№");
            grid.Columns.Add("Name", "Название");
            grid.Columns.Add("Date", "Дата");
            grid.Columns.Add("Location", "Место проведения");
            grid.Columns.Add("Organizer", "Организатор");
            grid.Columns.Add("ParticipantCount", "Количество участников");
            grid.Columns.Add("SportType", "Вид спорта");

            RefreshEventGrid(grid);
            tab.Controls.Add(grid);
        }

        private void RefreshEventGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (var ev in Database.Events)
            {
                grid.Rows.Add(ev.Id, ev.Name, ev.Date, ev.Location, ev.Organizer, ev.ParticipantCount, ev.SportType);
            }
        }

        private void InitializeParticipationsTab(TabPage tab)
        {
            Button addParticipationBtn = new Button();
            addParticipationBtn.Text = "Добавить участие";
            addParticipationBtn.BackColor = Color.FromArgb(32, 178, 170);
            addParticipationBtn.ForeColor = Color.White;
            addParticipationBtn.FlatStyle = FlatStyle.Flat;
            addParticipationBtn.FlatAppearance.BorderSize = 0;
            addParticipationBtn.Size = new Size(180, 35);
            addParticipationBtn.Location = new Point(10, 10);
            addParticipationBtn.Cursor = Cursors.Hand;
            addParticipationBtn.Click += (s, e) => OpenParticipationForm();
            tab.Controls.Add(addParticipationBtn);

            Button editParticipationBtn = new Button();
            editParticipationBtn.Text = "Редактировать";
            editParticipationBtn.BackColor = Color.FromArgb(32, 178, 170);
            editParticipationBtn.ForeColor = Color.White;
            editParticipationBtn.FlatStyle = FlatStyle.Flat;
            editParticipationBtn.FlatAppearance.BorderSize = 0;
            editParticipationBtn.Size = new Size(150, 35);
            editParticipationBtn.Location = new Point(200, 10);
            editParticipationBtn.Cursor = Cursors.Hand;
            tab.Controls.Add(editParticipationBtn);

            DataGridView grid = new DataGridView();
            grid.Location = new Point(10, 55);
            grid.Size = new Size(tab.Width - 30, tab.Height - 75);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;

            grid.Columns.Add("Id", "№");
            grid.Columns.Add("EventName", "Мероприятие");
            grid.Columns.Add("StudentName", "Студент");
            grid.Columns.Add("Result", "Результат");
            grid.Columns.Add("Award", "Награда");
            grid.Columns.Add("Rank", "Разряд");
            grid.Columns.Add("AddedBy", "Внесен пользователем");
            grid.Columns.Add("Date", "Дата");

            RefreshParticipationGrid(grid);
            tab.Controls.Add(grid);
        }

        private void RefreshParticipationGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (var p in Database.Participations)
            {
                grid.Rows.Add(p.Id, p.EventName, p.StudentName, p.Result, p.Award, p.Rank, p.AddedBy, p.Date);
            }
        }

        private void OpenStudentForm()
        {
            // Здесь можно открыть форму добавления студента
            MessageBox.Show("Форма добавления студента", "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenEventForm()
        {
            MessageBox.Show("Форма добавления мероприятия", "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenParticipationForm()
        {
            MessageBox.Show("Форма добавления участия", "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            Database.CurrentUser = null;
            this.Close();
        }
    }
}