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
            InitializeCustomControls();
        }

        private void InitializeCustomControls()
        {
            this.Text = "Спортивные достижения студентов";
            this.Size = new Size(1200, 700);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            // ===== ШАПКА =====
            headerPanel = new Panel
            {
                BackColor = Color.FromArgb(32, 178, 170),
                Dock = DockStyle.Top,
                Height = 60
            };

            Label titleLabel = new Label
            {
                Text = "Спортивные достижения студентов",
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold),
                AutoSize = false,
                Dock = DockStyle.Left,
                Width = 400,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            headerPanel.Controls.Add(titleLabel);

            welcomeLabel = new Label
            {
                Text = $"Админ: {Database.CurrentUser?.Login}",
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 11F),
                AutoSize = true,
                Location = new Point(450, 18)
            };
            headerPanel.Controls.Add(welcomeLabel);

            logoutButton = new Button
            {
                Text = "Выйти",
                BackColor = Color.White,
                ForeColor = Color.FromArgb(32, 178, 170),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                Size = new Size(100, 35),
                Location = new Point(this.Width - 130, 12),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            logoutButton.Click += LogoutButton_Click;
            headerPanel.Controls.Add(logoutButton);

            // ===== ТАБЫ =====
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft Sans Serif", 11F),
                ItemSize = new Size(120, 40)
            };

            TabPage studentsTab = new TabPage("Студенты") { Padding = new Padding(10) };
            TabPage eventsTab = new TabPage("Мероприятия") { Padding = new Padding(10) };
            TabPage participationsTab = new TabPage("Участия") { Padding = new Padding(10) };

            mainTabControl.TabPages.AddRange(new[] { studentsTab, eventsTab, participationsTab });

            InitializeStudentsTab(studentsTab);
            InitializeEventsTab(eventsTab);
            InitializeParticipationsTab(participationsTab);

            this.Controls.Add(mainTabControl);
            this.Controls.Add(headerPanel);
        }

        // ===== СТУДЕНТЫ =====
        private void InitializeStudentsTab(TabPage tab)
        {
            Button addBtn = CreateActionButton("Добавить студента", 10, 10, 180, 35);
            addBtn.Click += (s, e) => OpenStudentForm();

            Button editBtn = CreateActionButton("Редактировать", 200, 10, 150, 35);

            DataGridView grid = CreateGrid(10, 55, tab);
            grid.Columns.AddRange(
                new DataGridViewColumn[] {
                    CreateColumn("Id", "№", 40),
                    CreateColumn("FullName", "ФИО", 200),
                    CreateColumn("Faculty", "Факультет", 120),
                    CreateColumn("Group", "Группа", 80),
                    CreateColumn("StudentCardNumber", "Номер студ. билета", 120),
                    CreateColumn("BirthDate", "Дата рождения", 100),
                    CreateColumn("ContactInfo", "Контактные данные", 150)
                });

            RefreshStudentGrid(grid);

            tab.Controls.AddRange(new Control[] { addBtn, editBtn, grid });
        }

        private void RefreshStudentGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (var s in Database.GetStudents())
            {
                grid.Rows.Add(s.Id, s.FullName, s.Faculty, s.Group,
                    s.StudentCardNumber, s.BirthDate, s.ContactInfo);
            }
        }

        // ===== МЕРОПРИЯТИЯ =====
        private void InitializeEventsTab(TabPage tab)
        {
            Button addBtn = CreateActionButton("Добавить мероприятие", 10, 10, 200, 35);
            addBtn.Click += (s, e) => OpenEventForm();

            Button editBtn = CreateActionButton("Редактировать", 220, 10, 150, 35);

            DataGridView grid = CreateGrid(10, 55, tab);
            grid.Columns.AddRange(
                new DataGridViewColumn[] {
                    CreateColumn("Id", "№", 40),
                    CreateColumn("Name", "Название", 200),
                    CreateColumn("Date", "Дата", 100),
                    CreateColumn("Location", "Место проведения", 150),
                    CreateColumn("Organizer", "Организатор", 120),
                    CreateColumn("ParticipantCount", "Участников", 80),
                    CreateColumn("SportType", "Вид спорта", 120)
                });

            RefreshEventGrid(grid);

            tab.Controls.AddRange(new Control[] { addBtn, editBtn, grid });
        }

        private void RefreshEventGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (var ev in Database.GetEvents())
            {
                grid.Rows.Add(ev.Id, ev.Name, ev.Date, ev.Location,
                    ev.Organizer, ev.ParticipantCount, ev.SportType);
            }
        }

        // ===== УЧАСТИЯ =====
        private void InitializeParticipationsTab(TabPage tab)
        {
            Button addBtn = CreateActionButton("Добавить участие", 10, 10, 180, 35);
            addBtn.Click += (s, e) => OpenParticipationForm();

            Button editBtn = CreateActionButton("Редактировать", 200, 10, 150, 35);

            DataGridView grid = CreateGrid(10, 55, tab);
            grid.Columns.AddRange(
                new DataGridViewColumn[] {
                    CreateColumn("Id", "№", 40),
                    CreateColumn("EventName", "Мероприятие", 150),
                    CreateColumn("StudentName", "Студент", 200),
                    CreateColumn("Result", "Результат", 120),
                    CreateColumn("Award", "Награда", 100),
                    CreateColumn("Rank", "Разряд", 100),
                    CreateColumn("AddedBy", "Внёс", 100),
                    CreateColumn("Date", "Дата", 100)
                });

            RefreshParticipationGrid(grid);

            tab.Controls.AddRange(new Control[] { addBtn, editBtn, grid });
        }

        private void RefreshParticipationGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (var p in Database.GetParticipations())
            {
                grid.Rows.Add(p.Id, p.EventName, p.StudentName, p.Result,
                    p.Award, p.Rank, p.AddedBy, p.Date);
            }
        }

        // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ =====
        private Button CreateActionButton(string text, int x, int y, int w, int h)
        {
            return new Button
            {
                Text = text,
                BackColor = Color.FromArgb(32, 178, 170),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Microsoft Sans Serif", 10F),
                Size = new Size(w, h),
                Location = new Point(x, y),
                Cursor = Cursors.Hand
            };
        }

        private DataGridView CreateGrid(int x, int y, TabPage tab)
        {
            return new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(tab.Width - 30, tab.Height - 75),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
        }

        private DataGridViewColumn CreateColumn(string name, string header, int width)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                Width = width
            };
        }

        // ===== ОБРАБОТЧИКИ =====
        private void OpenStudentForm() =>
            MessageBox.Show("Форма добавления студента", "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void OpenEventForm() =>
            MessageBox.Show("Форма добавления мероприятия", "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void OpenParticipationForm() =>
            MessageBox.Show("Форма добавления участия", "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            Database.CurrentUser = null;
            this.Close();
        }
    }
}