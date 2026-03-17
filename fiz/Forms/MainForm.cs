using System;
using System.Drawing;
using System.Windows.Forms;
using fiz.Data;
using fiz.Forms;
using fiz.Models;

namespace fiz
{
    public partial class MainForm : Form
    {
        private TabControl mainTabControl;
        private Panel headerPanel;
        private Label welcomeLabel;
        private Button logoutButton;

        private DataGridView? studentsGrid;
        private DataGridView? eventsGrid;
        private DataGridView? participationsGrid;

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
            addBtn.Click += (s, e) => AddStudent();

            Button editBtn = CreateActionButton("Редактировать", 200, 10, 150, 35);
            editBtn.Click += (s, e) => EditSelectedStudent();

            Button deleteBtn = CreateActionButton("Удалить", 360, 10, 120, 35);
            deleteBtn.Click += (s, e) => DeleteSelectedStudent();

            studentsGrid = CreateGrid(10, 55, tab);
            studentsGrid.Columns.AddRange(
                new DataGridViewColumn[] {
                    CreateColumn("Id", "№", 40),
                    CreateColumn("FullName", "ФИО", 200),
                    CreateColumn("Faculty", "Факультет", 120),
                    CreateColumn("Group", "Группа", 80),
                    CreateColumn("StudentCardNumber", "Номер студ. билета", 120),
                    CreateColumn("BirthDate", "Дата рождения", 100),
                    CreateColumn("ContactInfo", "Контактные данные", 150)
                });

            RefreshStudentGrid(studentsGrid);

            tab.Controls.AddRange(new Control[] { addBtn, editBtn, deleteBtn, studentsGrid });
        }

        private void RefreshStudentGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (var s in Database.GetStudents())
            {
                grid.Rows.Add(s.Id, s.FullName, s.Faculty, s.Group,
                    s.StudentCardNumber, s.BirthDate.ToString("dd.MM.yyyy"), s.ContactInfo);
            }
        }

        // ===== МЕРОПРИЯТИЯ =====
        private void InitializeEventsTab(TabPage tab)
        {
            Button addBtn = CreateActionButton("Добавить мероприятие", 10, 10, 200, 35);
            addBtn.Click += (s, e) => AddEvent();

            Button editBtn = CreateActionButton("Редактировать", 220, 10, 150, 35);
            editBtn.Click += (s, e) => EditSelectedEvent();

            Button deleteBtn = CreateActionButton("Удалить", 380, 10, 120, 35);
            deleteBtn.Click += (s, e) => DeleteSelectedEvent();

            eventsGrid = CreateGrid(10, 55, tab);
            eventsGrid.Columns.AddRange(
                new DataGridViewColumn[] {
                    CreateColumn("Id", "№", 40),
                    CreateColumn("Name", "Название", 200),
                    CreateColumn("Date", "Дата", 100),
                    CreateColumn("Location", "Место проведения", 150),
                    CreateColumn("Organizer", "Организатор", 120),
                    CreateColumn("ParticipantCount", "Участников", 80),
                    CreateColumn("SportType", "Вид спорта", 120)
                });

            RefreshEventGrid(eventsGrid);

            tab.Controls.AddRange(new Control[] { addBtn, editBtn, deleteBtn, eventsGrid });
        }

        private void RefreshEventGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (var ev in Database.GetEvents())
            {
                grid.Rows.Add(ev.Id, ev.Name, ev.Date.ToString("dd.MM.yyyy"), ev.Location,
                    ev.Organizer, ev.ParticipantCount, ev.SportType);
            }
        }

        // ===== УЧАСТИЯ =====
        private void InitializeParticipationsTab(TabPage tab)
        {
            Button addBtn = CreateActionButton("Добавить участие", 10, 10, 180, 35);
            addBtn.Click += (s, e) => AddParticipation();

            Button editBtn = CreateActionButton("Редактировать", 200, 10, 150, 35);
            editBtn.Click += (s, e) => EditSelectedParticipation();

            Button deleteBtn = CreateActionButton("Удалить", 360, 10, 120, 35);
            deleteBtn.Click += (s, e) => DeleteSelectedParticipation();

            participationsGrid = CreateGrid(10, 55, tab);
            participationsGrid.Columns.AddRange(
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

            RefreshParticipationGrid(participationsGrid);

            tab.Controls.AddRange(new Control[] { addBtn, editBtn, deleteBtn, participationsGrid });
        }

        private void RefreshParticipationGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (var p in Database.GetParticipations())
            {
                grid.Rows.Add(p.Id, p.EventName, p.StudentName, p.Result,
                    p.Award, p.Rank, p.AddedBy, p.Date.ToString("dd.MM.yyyy"));
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

        // ===== ОБРАБОТЧИКИ / CRUD =====
        private void AddStudent()
        {
            if (!CrudDialogs.TryEditStudent(this, null, out var s)) return;
            Database.AddStudent(s);
            if (studentsGrid != null) RefreshStudentGrid(studentsGrid);
        }

        private void EditSelectedStudent()
        {
            if (studentsGrid == null || studentsGrid.SelectedRows.Count == 0) return;
            var row = studentsGrid.SelectedRows[0];
            var s = new Student
            {
                Id = Convert.ToInt32(row.Cells[0].Value),
                FullName = Convert.ToString(row.Cells[1].Value) ?? "",
                Faculty = Convert.ToString(row.Cells[2].Value) ?? "",
                Group = Convert.ToString(row.Cells[3].Value) ?? "",
                StudentCardNumber = Convert.ToString(row.Cells[4].Value) ?? "",
                BirthDate = DateTime.Parse(Convert.ToString(row.Cells[5].Value) ?? DateTime.Now.ToString("dd.MM.yyyy")),
                ContactInfo = Convert.ToString(row.Cells[6].Value) ?? ""
            };

            if (!CrudDialogs.TryEditStudent(this, s, out var edited)) return;
            Database.UpdateStudent(edited);
            RefreshStudentGrid(studentsGrid);
        }

        private void DeleteSelectedStudent()
        {
            if (studentsGrid == null || studentsGrid.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(studentsGrid.SelectedRows[0].Cells[0].Value);
            if (MessageBox.Show(this, "Удалить выбранного студента?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            Database.DeleteStudent(id);
            RefreshStudentGrid(studentsGrid);
        }

        private void AddEvent()
        {
            if (!CrudDialogs.TryEditEvent(this, null, out var ev)) return;
            Database.AddEvent(ev);
            if (eventsGrid != null) RefreshEventGrid(eventsGrid);
        }

        private void EditSelectedEvent()
        {
            if (eventsGrid == null || eventsGrid.SelectedRows.Count == 0) return;
            var row = eventsGrid.SelectedRows[0];
            var ev = new Event
            {
                Id = Convert.ToInt32(row.Cells[0].Value),
                Name = Convert.ToString(row.Cells[1].Value) ?? "",
                Date = DateTime.Parse(Convert.ToString(row.Cells[2].Value) ?? DateTime.Now.ToString("dd.MM.yyyy")),
                Location = Convert.ToString(row.Cells[3].Value) ?? "",
                Organizer = Convert.ToString(row.Cells[4].Value) ?? "",
                ParticipantCount = Convert.ToInt32(row.Cells[5].Value),
                SportType = Convert.ToString(row.Cells[6].Value) ?? ""
            };

            if (!CrudDialogs.TryEditEvent(this, ev, out var edited)) return;
            Database.UpdateEvent(edited);
            RefreshEventGrid(eventsGrid);
        }

        private void DeleteSelectedEvent()
        {
            if (eventsGrid == null || eventsGrid.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(eventsGrid.SelectedRows[0].Cells[0].Value);
            if (MessageBox.Show(this, "Удалить выбранное мероприятие?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            Database.DeleteEvent(id);
            RefreshEventGrid(eventsGrid);
        }

        private void AddParticipation()
        {
            if (!CrudDialogs.TryEditParticipation(this, null, out var p)) return;
            Database.AddParticipation(p);
            if (participationsGrid != null) RefreshParticipationGrid(participationsGrid);
        }

        private void EditSelectedParticipation()
        {
            if (participationsGrid == null || participationsGrid.SelectedRows.Count == 0) return;
            var row = participationsGrid.SelectedRows[0];
            var p = new Participation
            {
                Id = Convert.ToInt32(row.Cells[0].Value),
                EventName = Convert.ToString(row.Cells[1].Value) ?? "",
                StudentName = Convert.ToString(row.Cells[2].Value) ?? "",
                Result = Convert.ToString(row.Cells[3].Value) ?? "",
                Award = Convert.ToString(row.Cells[4].Value) ?? "",
                Rank = Convert.ToString(row.Cells[5].Value) ?? "",
                AddedBy = Convert.ToString(row.Cells[6].Value) ?? "",
                Date = DateTime.Parse(Convert.ToString(row.Cells[7].Value) ?? DateTime.Now.ToString("dd.MM.yyyy"))
            };

            if (!CrudDialogs.TryEditParticipation(this, p, out var edited)) return;
            Database.UpdateParticipation(edited);
            RefreshParticipationGrid(participationsGrid);
        }

        private void DeleteSelectedParticipation()
        {
            if (participationsGrid == null || participationsGrid.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(participationsGrid.SelectedRows[0].Cells[0].Value);
            if (MessageBox.Show(this, "Удалить выбранное участие?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            Database.DeleteParticipation(id);
            RefreshParticipationGrid(participationsGrid);
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            Database.CurrentUser = null;
            this.Close();
        }
    }
}