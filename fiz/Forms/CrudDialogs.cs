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

            using var form = CreateDialog(existing == null ? "Добавить студента" : "Редактировать студента", 520);
            var layout = CreateLayout(form);

            var tbFullName = AddRow(layout, "ФИО", s.FullName);
            var tbFaculty = AddRow(layout, "Факультет", s.Faculty);
            var tbGroup = AddRow(layout, "Группа", s.Group);
            var tbCard = AddRow(layout, "Номер студ. билета", s.StudentCardNumber);
            var tbBirth = AddRow(layout, "Дата рождения", s.BirthDate);
            var tbContact = AddRow(layout, "Контактные данные", s.ContactInfo);

            if (form.ShowDialog(owner) != DialogResult.OK)
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
            s.BirthDate = tbBirth.Text.Trim();
            s.ContactInfo = tbContact.Text.Trim();

            result = s;
            return true;
        }

        public static bool TryEditEvent(IWin32Window owner, Event? existing, out Event result)
        {
            var ev = existing ?? new Event();

            using var form = CreateDialog(existing == null ? "Добавить мероприятие" : "Редактировать мероприятие", 500);
            var layout = CreateLayout(form);

            var tbName = AddRow(layout, "Название", ev.Name);
            var tbDate = AddRow(layout, "Дата", ev.Date);
            var tbLocation = AddRow(layout, "Место проведения", ev.Location);
            var tbOrganizer = AddRow(layout, "Организатор", ev.Organizer);
            var tbSport = AddRow(layout, "Вид спорта", ev.SportType);
            var tbCount = AddRow(layout, "Участников", ev.ParticipantCount == 0 ? "" : ev.ParticipantCount.ToString());

            if (form.ShowDialog(owner) != DialogResult.OK)
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

            if (!int.TryParse(tbCount.Text.Trim(), out var count) || count < 0)
            {
                MessageBox.Show(owner, "Поле 'Участников' должно быть целым числом (0 или больше).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                result = ev;
                return false;
            }

            ev.Name = name;
            ev.Date = tbDate.Text.Trim();
            ev.Location = tbLocation.Text.Trim();
            ev.Organizer = tbOrganizer.Text.Trim();
            ev.SportType = tbSport.Text.Trim();
            ev.ParticipantCount = count;

            result = ev;
            return true;
        }

        public static bool TryEditParticipation(IWin32Window owner, Participation? existing, out Participation result)
        {
            var p = existing ?? new Participation();

            using var form = CreateDialog(existing == null ? "Добавить участие" : "Редактировать участие", 520);
            var layout = CreateLayout(form);

            var tbEvent = AddRow(layout, "Мероприятие", p.EventName);
            var tbStudent = AddRow(layout, "Студент", p.StudentName);
            var tbResult = AddRow(layout, "Результат", p.Result);
            var tbAward = AddRow(layout, "Награда", p.Award);
            var tbRank = AddRow(layout, "Разряд", p.Rank);
            var tbAddedBy = AddRow(layout, "Внёс", p.AddedBy);
            var tbDate = AddRow(layout, "Дата", p.Date);

            if (form.ShowDialog(owner) != DialogResult.OK)
            {
                result = p;
                return false;
            }

            var eventName = tbEvent.Text.Trim();
            var studentName = tbStudent.Text.Trim();
            if (string.IsNullOrWhiteSpace(eventName) || string.IsNullOrWhiteSpace(studentName))
            {
                MessageBox.Show(owner, "Поля 'Мероприятие' и 'Студент' обязательны.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                result = p;
                return false;
            }

            p.EventName = eventName;
            p.StudentName = studentName;
            p.Result = tbResult.Text.Trim();
            p.Award = tbAward.Text.Trim();
            p.Rank = tbRank.Text.Trim();
            p.AddedBy = tbAddedBy.Text.Trim();
            p.Date = tbDate.Text.Trim();

            result = p;
            return true;
        }

        private static Form CreateDialog(string title, int height)
        {
            return new Form
            {
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ClientSize = new Size(560, height)
            };
        }

        private static TableLayoutPanel CreateLayout(Form form)
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 0,
                Padding = new Padding(12),
                AutoScroll = true
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(12),
                Height = 55
            };

            var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 100 };
            var cancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 100 };
            buttons.Controls.Add(ok);
            buttons.Controls.Add(cancel);

            form.Controls.Add(layout);
            form.Controls.Add(buttons);

            form.AcceptButton = ok;
            form.CancelButton = cancel;

            return layout;
        }

        private static TextBox AddRow(TableLayoutPanel layout, string label, string value)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowCount += 1;

            var rowIndex = layout.RowCount - 1;

            var lbl = new Label
            {
                Text = label,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 8, 8, 8),
                AutoSize = true
            };

            var tb = new TextBox
            {
                Dock = DockStyle.Top,
                Text = value ?? string.Empty
            };

            layout.Controls.Add(lbl, 0, rowIndex);
            layout.Controls.Add(tb, 1, rowIndex);

            return tb;
        }
    }
}

