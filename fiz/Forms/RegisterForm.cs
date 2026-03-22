using System;
using System.Drawing;
using System.Windows.Forms;
using fiz.Data;
using fiz.Models;

namespace fiz
{
    public partial class RegisterForm : Form
    {
        private RoundedTextBox textBoxLogin;
        private RoundedTextBox textBoxPassword;
        private RoundedTextBox textBoxPasswordConfirm;
        private ComboBox cbRole;
        private RoundedButton buttonRegister;
        private Panel headerPanel;

        public RegisterForm()
        {
            InitializeCustomControls();
        }

        private void InitializeCustomControls()
        {
            this.Text = "Регистрация";
            this.Size = new Size(600, 500);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;

            headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(32, 178, 170);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 50;

            Label titleLabel = new Label();
            titleLabel.Text = "Спортивные достижения студентов";
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            titleLabel.AutoSize = false;
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            headerPanel.Controls.Add(titleLabel);

            textBoxLogin = new RoundedTextBox();
            textBoxLogin.PlaceholderText = "Логин";
            textBoxLogin.BackColor = Color.FromArgb(240, 240, 240);
            textBoxLogin.ForeColor = Color.Gray;
            textBoxLogin.Size = new Size(250, 35);
            textBoxLogin.Location = new Point((this.ClientSize.Width - 250) / 2, 80);
            textBoxLogin.Anchor = AnchorStyles.Top;

            textBoxPassword = new RoundedTextBox();
            textBoxPassword.PlaceholderText = "Пароль";
            textBoxPassword.PasswordChar = '*';
            textBoxPassword.ShowPasswordToggle = true;
            textBoxPassword.BackColor = Color.FromArgb(240, 240, 240);
            textBoxPassword.ForeColor = Color.Gray;
            textBoxPassword.Size = new Size(250, 35);
            textBoxPassword.Location = new Point((this.ClientSize.Width - 250) / 2, 135);
            textBoxPassword.Anchor = AnchorStyles.Top;

            textBoxPasswordConfirm = new RoundedTextBox();
            textBoxPasswordConfirm.PlaceholderText = "Повторите пароль";
            textBoxPasswordConfirm.PasswordChar = '*';
            textBoxPasswordConfirm.ShowPasswordToggle = true;
            textBoxPasswordConfirm.BackColor = Color.FromArgb(240, 240, 240);
            textBoxPasswordConfirm.ForeColor = Color.Gray;
            textBoxPasswordConfirm.Size = new Size(250, 35);
            textBoxPasswordConfirm.Location = new Point((this.ClientSize.Width - 250) / 2, 190);
            textBoxPasswordConfirm.Anchor = AnchorStyles.Top;

            cbRole = new ComboBox();
            cbRole.Items.AddRange(new[] { "user", "admin" });
            cbRole.SelectedIndex = 0;
            cbRole.Size = new Size(250, 30);
            cbRole.Location = new Point((this.ClientSize.Width - 250) / 2, 245);
            cbRole.DropDownStyle = ComboBoxStyle.DropDownList;

            buttonRegister = new RoundedButton();
            buttonRegister.Text = "Зарегистрироваться";
            buttonRegister.BackColor = Color.FromArgb(32, 178, 170);
            buttonRegister.ForeColor = Color.White;
            buttonRegister.FlatStyle = FlatStyle.Flat;
            buttonRegister.FlatAppearance.BorderSize = 0;
            buttonRegister.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            buttonRegister.Size = new Size(250, 40);
            buttonRegister.Location = new Point((this.ClientSize.Width - 250) / 2, 295);
            buttonRegister.Anchor = AnchorStyles.Top;
            buttonRegister.Cursor = Cursors.Hand;
            buttonRegister.Click += ButtonRegister_Click;

            this.Controls.Add(textBoxLogin);
            this.Controls.Add(textBoxPassword);
            this.Controls.Add(textBoxPasswordConfirm);
            this.Controls.Add(cbRole);
            this.Controls.Add(buttonRegister);
            this.Controls.Add(headerPanel);
        }

        private void ButtonRegister_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string password = textBoxPassword.Text.Trim();
            string confirm = textBoxPasswordConfirm.Text.Trim();
            string role = cbRole.SelectedItem?.ToString() ?? "user";

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (login.Length < 3)
            {
                MessageBox.Show("Логин должен быть не менее 3 символов!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен быть не менее 6 символов!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool success = Database.Register(login, password, role);

            if (success)
            {
                MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Пользователь уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}