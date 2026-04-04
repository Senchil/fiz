using fiz.Data;
using fiz.Forms;
using fiz.Models;
using ClosedXML.Excel;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

        private TextBox searchBox;
        private ComboBox facultyFilter;
        private Button searchBtn;
        private Button resetBtn;
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

            string roleText = Database.CurrentUser?.Role == "admin" ? "Админ" : "Пользователь";
            welcomeLabel = new Label
            {
                Text = $"{roleText}: {Database.CurrentUser?.Login}",
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
            // Панель фильтров
            var filterPanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(tab.Width - 30, 45),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label searchLabel = new Label
            {
                Text = "Поиск:",
                Location = new Point(5, 12),
                AutoSize = true
            };

            searchBox = new TextBox
            {
                Location = new Point(55, 8),
                Width = 200,
                Font = new Font("Microsoft Sans Serif", 10F)
            };

            Label facultyLabel = new Label
            {
                Text = "Факультет:",
                Location = new Point(280, 12),  // было 270, сдвинули вправо
                AutoSize = true
            };

            facultyFilter = new ComboBox
            {
                Location = new Point(365, 8),   // было 345, сдвинули вправо
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            facultyFilter.Items.Add("Все");

            var faculties = Database.GetStudents().Select(s => s.Faculty).Distinct().OrderBy(f => f);
            foreach (var f in faculties)
                facultyFilter.Items.Add(f);
            facultyFilter.SelectedIndex = 0;

            searchBtn = new Button
            {
                Text = "Найти",
                Location = new Point(530, 8),   // было 510, сдвинули вправо
                Width = 80,
                Height = 30,
                BackColor = Color.FromArgb(32, 178, 170),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            searchBtn.Click += (s, e) => FilterStudents();

            resetBtn = new Button
            {
                Text = "Сброс",
                Location = new Point(620, 8),   // было 600, сдвинули вправо
                Width = 80,
                Height = 30,
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            resetBtn.Click += (s, e) => ResetStudentFilter();
            studentsGrid = CreateGrid(10, 125, tab);
            filterPanel.Controls.AddRange(new Control[] { searchLabel, searchBox, facultyLabel, facultyFilter, searchBtn, resetBtn });

            Button addBtn = CreateActionButton("Добавить студента", 10, 80, 180, 35);
            addBtn.Click += (s, e) => AddStudent();

            Button editBtn = CreateActionButton("Редактировать", 200, 80, 150, 35);
            editBtn.Click += (s, e) => EditSelectedStudent();

            Button deleteBtn = CreateActionButton("Удалить", 360, 80, 120, 35);
            deleteBtn.Click += (s, e) => DeleteSelectedStudent();

            Button tocsvBtn = CreateActionButton(".csv", 520, 80, 120, 35);
            tocsvBtn.Click += (s, e) => Report_grid_To_csv(studentsGrid);

            Button toxlsxBtn = CreateActionButton(".xlsx", 680, 80, 120, 35);
            toxlsxBtn.Click += (s, e) => Report_grid_To_xlsx(studentsGrid);

            
            studentsGrid.Columns.AddRange(
                new DataGridViewColumn[] {
            CreateColumn("Id", "№", 40),
            CreateColumn("FullName", "ФИО", 200),
            CreateColumn("Faculty", "Факультет", 120),
            CreateColumn("Group", "Группа", 80),
            CreateColumn("StudentCardNumber", "Номер студ. билета", 120),
            CreateColumn("BirthDate", "Дата рождения", 100),
            CreateColumn("ContactInfo", "Контактные данные", 150),
            CreateColumn("CreatedBy", "Создал", 100)
                });

            RefreshStudentGrid(studentsGrid);

            tab.Controls.AddRange(new Control[] { filterPanel, addBtn, editBtn, deleteBtn, tocsvBtn, toxlsxBtn, studentsGrid });
        }

        // ===== МЕРОПРИЯТИЯ =====
        private void InitializeEventsTab(TabPage tab)
        {
            eventsGrid = CreateGrid(10, 55, tab);
            Button addBtn = CreateActionButton("Добавить мероприятие", 10, 10, 200, 35);
            addBtn.Click += (s, e) => AddEvent();

            Button editBtn = CreateActionButton("Редактировать", 220, 10, 150, 35);
            editBtn.Click += (s, e) => EditSelectedEvent();

            Button deleteBtn = CreateActionButton("Удалить", 380, 10, 120, 35);
            deleteBtn.Click += (s, e) => DeleteSelectedEvent();

            Button tocsvBtn = CreateActionButton(".csv", 520, 10, 120, 35);
            tocsvBtn.Click += (s, e) => Report_grid_To_csv(eventsGrid);

            Button toxlsxBtn = CreateActionButton(".xlsx", 680, 10, 120, 35);
            toxlsxBtn.Click += (s, e) => Report_grid_To_xlsx(eventsGrid);

            
            eventsGrid.Columns.AddRange(
                new DataGridViewColumn[] {
                    CreateColumn("Id", "№", 40),
                    CreateColumn("Name", "Название", 180),
                    CreateColumn("Date", "Дата", 90),
                    CreateColumn("Location", "Место", 120),
                    CreateColumn("Organizer", "Организатор", 120),
                    CreateColumn("ParticipantCount", "Участников", 70),
                    CreateColumn("SportType", "Вид спорта", 100),
                    CreateColumn("Level", "Уровень", 100),
                    CreateColumn("EventType", "Тип", 80),
                    CreateColumn("IsOfficial", "Офиц.", 60),
                    CreateColumn("CreatedBy", "Создал", 100)
                });

            RefreshEventGrid(eventsGrid);
            tab.Controls.AddRange(new Control[] { addBtn, editBtn, deleteBtn, tocsvBtn, toxlsxBtn, eventsGrid });
        }

        private void RefreshEventGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (var ev in Database.GetEvents())
            {
                grid.Rows.Add(
                    ev.Id,
                    ev.Name,
                    ev.Date.ToString("dd.MM.yyyy"),
                    ev.Location,
                    ev.Organizer,
                    ev.ParticipantCount,
                    ev.SportType,
                    ev.Level,
                    ev.EventType,
                    ev.IsOfficial ? "Да" : "Нет",
                    ev.CreatedBy
                );
            }
        }

        // ===== УЧАСТИЯ =====
        private void InitializeParticipationsTab(TabPage tab)
        {
            participationsGrid = CreateGrid(10, 55, tab);
            Button addBtn = CreateActionButton("Добавить участие", 10, 10, 180, 35);
            addBtn.Click += (s, e) => AddParticipation();

            Button editBtn = CreateActionButton("Редактировать", 200, 10, 150, 35);
            editBtn.Click += (s, e) => EditSelectedParticipation();

            Button deleteBtn = CreateActionButton("Удалить", 360, 10, 120, 35);
            deleteBtn.Click += (s, e) => DeleteSelectedParticipation();

            Button tocsvBtn = CreateActionButton(".csv", 520, 10, 120, 35);
            tocsvBtn.Click += (s, e) => Report_grid_To_csv(participationsGrid);

            Button toxlsxBtn = CreateActionButton(".xlsx", 680, 10, 120, 35);
            toxlsxBtn.Click += (s, e) => Report_grid_To_xlsx(participationsGrid);

                        
            participationsGrid.Columns.AddRange(
                new DataGridViewColumn[] {
                    CreateColumn("Id", "№", 40),
                    CreateColumn("EventName", "Мероприятие", 150),
                    CreateColumn("StudentName", "Студент", 200),
                    CreateColumn("Result", "Результат", 120),
                    CreateColumn("Award", "Награда", 100),
                    CreateColumn("Rank", "Разряд", 100),
                    CreateColumn("AddedBy", "Создал", 100),
                    CreateColumn("Date", "Дата", 90),
                    CreateColumn("UpdatedBy", "Изменил", 100),
                    CreateColumn("UpdatedAt", "Дата изм.", 120)
                });

            RefreshParticipationGrid(participationsGrid);

            tab.Controls.AddRange(new Control[] { addBtn, editBtn, deleteBtn, tocsvBtn, toxlsxBtn, participationsGrid });
        }

        private void RefreshParticipationGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (var p in Database.GetParticipations())
            {
                grid.Rows.Add(
                    p.Id,
                    p.EventName,
                    p.StudentName,
                    p.Result,
                    p.Award,
                    p.Rank,
                    p.AddedBy,
                    p.Date.ToString("dd.MM.yyyy"),
                    p.UpdatedBy,
                    p.UpdatedAt?.ToString("dd.MM.yyyy HH:mm:ss") ?? ""
                );
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
                ContactInfo = Convert.ToString(row.Cells[6].Value) ?? "",
                CreatedBy = Convert.ToString(row.Cells[7].Value) ?? ""
            };

            try
            {
                if (!CrudDialogs.TryEditStudent(this, s, out var edited)) return;
                Database.UpdateStudent(edited);
                RefreshStudentGrid(studentsGrid);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(this, ex.Message, "Доступ запрещён",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteSelectedStudent()
        {
            if (studentsGrid == null || studentsGrid.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(studentsGrid.SelectedRows[0].Cells[0].Value);

            try
            {
                if (MessageBox.Show(this, "Удалить выбранного студента?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                Database.DeleteStudent(id);
                RefreshStudentGrid(studentsGrid);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(this, ex.Message, "Доступ запрещён",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                SportType = Convert.ToString(row.Cells[6].Value) ?? "",
                Level = Convert.ToString(row.Cells[7].Value) ?? "",
                EventType = Convert.ToString(row.Cells[8].Value) ?? "",
                IsOfficial = Convert.ToString(row.Cells[9].Value) == "Да",
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
                Date = DateTime.Parse(Convert.ToString(row.Cells[7].Value) ?? DateTime.Now.ToString("dd.MM.yyyy")),
                UpdatedBy = Convert.ToString(row.Cells[8].Value) ?? "",
                UpdatedAt = !string.IsNullOrEmpty(Convert.ToString(row.Cells[9].Value)) ? DateTime.Parse(Convert.ToString(row.Cells[9].Value)) : (DateTime?)null
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

        private void FilterStudents()
        {
            var all = Database.GetStudents();
            var filtered = all.AsEnumerable();

            string search = searchBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(s =>
                    s.FullName.ToLower().Contains(search) ||
                    s.StudentCardNumber.Contains(search));
            }

            if (facultyFilter.SelectedItem != null && facultyFilter.SelectedItem.ToString() != "Все")
            {
                string faculty = facultyFilter.SelectedItem.ToString();
                filtered = filtered.Where(s => s.Faculty == faculty);
            }

            studentsGrid.Rows.Clear();
            foreach (var s in filtered)
            {
                studentsGrid.Rows.Add(s.Id, s.FullName, s.Faculty, s.Group,
                    s.StudentCardNumber, s.BirthDate.ToString("dd.MM.yyyy"),
                    s.ContactInfo, s.CreatedBy);
            }
        }

        private void ResetStudentFilter()
        {
            searchBox.Text = "";
            facultyFilter.SelectedIndex = 0;
            RefreshStudentGrid(studentsGrid);
        }

        private void RefreshStudentGrid(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (var s in Database.GetStudents())
            {
                grid.Rows.Add(s.Id, s.FullName, s.Faculty, s.Group,
                    s.StudentCardNumber, s.BirthDate.ToString("dd.MM.yyyy"),
                    s.ContactInfo, s.CreatedBy);
            }
        }
        public void Report_grid_To_csv(DataGridView dgv)
        {
            try
            {
                string fileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

                StringBuilder csvContent = new StringBuilder();

                // Добавляем заголовки ВСЕХ столбцов
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    csvContent.Append(EscapeCsvField(dgv.Columns[i].HeaderText));
                    if (i < dgv.Columns.Count - 1)
                        csvContent.Append(",");
                }
                csvContent.AppendLine();

                // Добавляем строки данных
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            string cellValue = row.Cells[i].Value?.ToString() ?? string.Empty;
                            csvContent.Append(EscapeCsvField(cellValue));

                            if (i < dgv.Columns.Count - 1)
                                csvContent.Append(",");
                        }
                        csvContent.AppendLine();
                    }
                }

                File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8);
                MessageBox.Show($"CSV файл успешно сохранен!\nПуть: {filePath}", "Успех",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении CSV: {ex.Message}", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Report_grid_To_xlsx(DataGridView dgv)
        {
            try
            {
                string fileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Report");

                    // Добавляем заголовки ВСЕХ столбцов
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = dgv.Columns[i].HeaderText;
                    }

                    // Стиль для заголовков
                    var headerRange = worksheet.Range(1, 1, 1, dgv.Columns.Count);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Добавляем данные
                    int currentRow = 2;
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            for (int i = 0; i < dgv.Columns.Count; i++)
                            {
                                object cellValue = row.Cells[i].Value;
                                var cell = worksheet.Cell(currentRow, i + 1);

                                if (cellValue == null || cellValue == DBNull.Value)
                                {
                                    cell.Value = "";
                                }
                                else if (cellValue is DateTime dateTime)
                                {
                                    cell.Value = dateTime;
                                    cell.Style.DateFormat.Format = "dd.MM.yyyy HH:mm:ss";
                                }
                                else if (cellValue is decimal || cellValue is double || cellValue is float)
                                {
                                    cell.Value = Convert.ToDouble(cellValue);
                                    cell.Style.NumberFormat.Format = "#,##0.00";
                                }
                                else
                                {
                                    cell.Value = cellValue.ToString();
                                }
                            }
                            currentRow++;
                        }
                    }

                    // Автоподбор ширины колонок
                    worksheet.Columns().AdjustToContents();

                    // Сохраняем файл
                    workbook.SaveAs(filePath);
                }

                MessageBox.Show($"XLSX файл успешно сохранен!\nПуть: {filePath}", "Успех",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Опционально: открыть папку с файлом
                System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{filePath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении XLSX: {ex.Message}", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            if ((field.Contains(",") || field.Contains("\"") || field.Contains("\n")) &&
                (!field.StartsWith("\"") && !field.EndsWith("\"")))
            {
                field = field.Replace("\"", "\"\"");
                return $"\"{field}\"";
            }
            return field;
        }
    }
}
