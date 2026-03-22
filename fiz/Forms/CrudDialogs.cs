using System;
using System.Drawing;
using System.Windows.Forms;
using fiz.Models;

namespace fiz.Forms
{
    internal static class CrudDialogs
    {
        public static bool TryEditStudent(IWin32Window owner, Student? existing, out Student result)
        {
            var s = existing ?? new Student();

            using var form = CreateBaseForm(existing == null ? "Добавить студента" : "Редактировать студента", "Данные студента");
            var layout = CreateFieldsLayout(form);

            var tbFullName = AddRoundedRow(layout, "ФИО", s.FullName, "Введите ФИО студента");
            var tbFaculty = AddRoundedRow(layout, "Факультет", s.Faculty, "Факультет");
            var tbGroup = AddRoundedRow(layout, "Группа", s.Group, "Например, ИИ‑23");
            var tbCard = AddRoundedRow(layout, "Номер студ. билета", s.StudentCardNumber, "Номер студенческого");
            var dpBirth = AddDateRow(layout, "Дата рождения", s.BirthDate);
            var tbContact = AddRoundedRow(layout, "Контактные данные", s.ContactInfo, "Телефон / email");

            if (ShowDialogWithButtons(form) != DialogResult.OK)
            {
                result = s;
                return false;
            }

            var fullName = tbFullName.Text.Trim();
            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show(owner, "ФИО не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                result = s;
                return false;
            }

            s.FullName = fullName;
            s.Faculty = tbFaculty.Text.Trim();
            s.Group = tbGroup.Text.Trim();
            s.StudentCardNumber = tbCard.Text.Trim();
            s.BirthDate = dpBirth.Value;
            s.ContactInfo = tbContact.Text.Trim();

            result = s;
            return true;
        }

        public static bool TryEditEvent(IWin32Window owner, Event? existing, out Event result)
        {
            var ev = existing ?? new Event();

            using var form = CreateBaseForm(existing == null ? "Добавить мероприятие" : "Редактировать мероприятие", "Данные мероприятия");
            var layout = CreateFieldsLayout(form);

            var tbName = AddRoundedRow(layout, "Название", ev.Name, "Название соревнования");
            var dpDate = AddDateRow(layout, "Дата", ev.Date);
            var tbLocation = AddRoundedRow(layout, "Место проведения", ev.Location, "Спортивный зал / стадион");
            var tbOrganizer = AddRoundedRow(layout, "Организатор", ev.Organizer, "Кафедра / факультет");
            var tbSport = AddRoundedRow(layout, "Вид спорта", ev.SportType, "Например, Плавание");
            var numCount = AddNumericRow(layout, "Участников", ev.ParticipantCount);

            var cbLevel = AddComboRow(layout, "Уровень", new[] { "Региональный", "Межрегиональный", "Всероссийский", "Международный" }, ev.Level);
            var cbType = AddComboRow(layout, "Тип", new[] { "Обычное", "Комплексное" }, ev.EventType);
            var cbOnBase = AddCheckRow(layout, "Проводилось на базе", ev.IsOnBase);
            var cbOfficial = AddCheckRow(layout, "Официальное", ev.IsOfficial);

            if (ShowDialogWithButtons(form) != DialogResult.OK)
            {
                result = ev;
                return false;
            }

            var name = tbName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(owner, "Название не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                result = ev;
                return false;
            }

            ev.Name = name;
            ev.Date = dpDate.Value;
            ev.Location = tbLocation.Text.Trim();
            ev.Organizer = tbOrganizer.Text.Trim();
            ev.SportType = tbSport.Text.Trim();
            ev.ParticipantCount = (int)numCount.Value;
            ev.Level = cbLevel.SelectedItem?.ToString();
            ev.EventType = cbType.SelectedItem?.ToString();
            ev.IsOnBase = cbOnBase.Checked;
            ev.IsOfficial = cbOfficial.Checked;

            result = ev;
            return true;
        }

        public static bool TryEditParticipation(IWin32Window owner, Participation? existing, out Participation result)
        {
            var p = existing ?? new Participation();

            using var form = CreateBaseForm(existing == null ? "Добавить участие" : "Редактировать участие", "Данные участия");
            var layout = CreateFieldsLayout(form);

            var tbEvent = AddRoundedRow(layout, "Мероприятие", p.EventName, "Название мероприятия");
            var tbStudent = AddRoundedRow(layout, "Студент", p.StudentName, "ФИО студента");
            var tbResult = AddRoundedRow(layout, "Результат", p.Result, "Например, Призовое место");
            var tbAward = AddRoundedRow(layout, "Награда", p.Award, "Кубок / медаль / грамота");
            var tbRank = AddRoundedRow(layout, "Разряд", p.Rank, "Разряд / категория");
            var tbAddedBy = AddRoundedRow(layout, "Внёс", p.AddedBy, "Ответственный пользователь");
            var dpDate = AddDateRow(layout, "Дата", p.Date);

            if (ShowDialogWithButtons(form) != DialogResult.OK)
            {
                result = p;
                return false;
            }

            var eventName = tbEvent.Text.Trim();
            var studentName = tbStudent.Text.Trim();
            if (string.IsNullOrWhiteSpace(eventName) || string.IsNullOrWhiteSpace(studentName))
            {
                MessageBox.Show(owner, "Поля \"Мероприятие\" и \"Студент\" обязательны.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                result = p;
                return false;
            }

            p.EventName = eventName;
            p.StudentName = studentName;
            p.Result = tbResult.Text.Trim();
            p.Award = tbAward.Text.Trim();
            p.Rank = tbRank.Text.Trim();
            p.AddedBy = tbAddedBy.Text.Trim();
            p.Date = dpDate.Value;

            result = p;
            return true;
        }

        private static Form CreateBaseForm(string title, string sectionTitle)
        {
            var form = new Form
            {
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                ClientSize = new Size(640, 520),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var headerPanel = new Panel
            {
                BackColor = Color.FromArgb(32, 178, 170),
                Dock = DockStyle.Top,
                Height = 50
            };

            var headerLabel = new Label
            {
                Text = "Спортивные достижения студентов",
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            headerPanel.Controls.Add(headerLabel);

            var sectionLabel = new Label
            {
                Text = sectionTitle,
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point((form.ClientSize.Width - 200) / 2, 70)
            };

            form.Controls.Add(sectionLabel);
            form.Controls.Add(headerPanel);

            return form;
        }

        private static TableLayoutPanel CreateFieldsLayout(Form form)
        {
            var layout = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 0,
                AutoSize = false,
                Width = 360,
                Height = form.ClientSize.Height - 160,
                Location = new Point((form.ClientSize.Width - 360) / 2, 100),
                BackColor = Color.White
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            form.Controls.Add(layout);
            return layout;
        }

        private static RoundedTextBox AddRoundedRow(TableLayoutPanel layout, string labelText, string value, string placeholder)
        {
            var label = new Label
            {
                Text = labelText,
                Font = new Font("Microsoft Sans Serif", 9F),
                ForeColor = Color.Black,
                AutoSize = true,
                Margin = new Padding(0, 8, 0, 2)
            };

            var tb = new RoundedTextBox
            {
                PlaceholderText = placeholder,
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.Gray,
                Size = new Size(360, 38),
                Multiline = false,
                TextAlign = HorizontalAlignment.Left,
                Font = new Font("Microsoft Sans Serif", 10.5f)
            };

            if (!string.IsNullOrEmpty(value))
            {
                tb.Text = value;
            }

            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(label);
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(tb);

            return tb;
        }

        private static DateTimePicker AddDateRow(TableLayoutPanel layout, string labelText, DateTime? value)
        {
            var label = new Label
            {
                Text = labelText,
                Font = new Font("Microsoft Sans Serif", 9F),
                ForeColor = Color.Black,
                AutoSize = true,
                Margin = new Padding(0, 8, 0, 2)
            };

            var picker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 360,
                Value = (value.HasValue && value.Value > DateTimePicker.MinimumDateTime)
                        ? value.Value
                        : DateTime.Today
            };

            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(label);
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(picker);

            return picker;
        }

        private static NumericUpDown AddNumericRow(TableLayoutPanel layout, string labelText, int value)
        {
            var label = new Label
            {
                Text = labelText,
                Font = new Font("Microsoft Sans Serif", 9F),
                ForeColor = Color.Black,
                AutoSize = true,
                Margin = new Padding(0, 8, 0, 2)
            };

            var num = new NumericUpDown
            {
                Width = 360,
                Minimum = 0,
                Maximum = 100000,
                Value = value < 0 ? 0 : value
            };

            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(label);
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(num);

            return num;
        }

        private static DialogResult ShowDialogWithButtons(Form form)
        {
            var okButton = new RoundedButton
            {
                Text = "Сохранить",
                BackColor = Color.FromArgb(32, 178, 170),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                Size = new Size(180, 36),
                Location = new Point(form.ClientSize.Width / 2 - 190, form.ClientSize.Height - 60),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            okButton.FlatAppearance.BorderSize = 0;

            var cancelButton = new RoundedButton
            {
                Text = "Отмена",
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.FromArgb(32, 178, 170),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold),
                Size = new Size(180, 36),
                Location = new Point(form.ClientSize.Width / 2 + 10, form.ClientSize.Height - 60),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            cancelButton.FlatAppearance.BorderSize = 0;

            form.Controls.Add(okButton);
            form.Controls.Add(cancelButton);

            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;

            return form.ShowDialog();
        }
        private static ComboBox AddComboRow(TableLayoutPanel layout, string labelText, string[] items, string selectedValue)
        {
            var label = new Label
            {
                Text = labelText,
                Font = new Font("Microsoft Sans Serif", 9F),
                AutoSize = true,
                Margin = new Padding(0, 8, 0, 2)
            };

            var cb = new ComboBox
            {
                Width = 360,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cb.Items.AddRange(items);
            if (!string.IsNullOrEmpty(selectedValue) && cb.Items.Contains(selectedValue))
                cb.SelectedItem = selectedValue;
            else if (items.Length > 0)
                cb.SelectedIndex = 0;

            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(label);
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(cb);

            return cb;
        }

        private static CheckBox AddCheckRow(TableLayoutPanel layout, string text, bool isChecked)
        {
            var cb = new CheckBox
            {
                Text = text,
                Width = 360,
                Height = 30,
                Checked = isChecked
            };

            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(cb);

            return cb;
        }
    }
}