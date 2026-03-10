using fiz.Data;
using fiz.Forms;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace fiz
{
    public partial class LoginForm : Form
    {
        private RoundedTextBox textBoxLogin;
        private RoundedTextBox textBoxPassword;
        private RoundedButton buttonLogin;
        private LinkLabel linkRegister;
        private Panel headerPanel;

        public LoginForm()
        {
            //InitializeComponent();
            InitializeCustomControls();
        }

        private void InitializeCustomControls()
        {
            this.Text = "Авторизация";
            this.Size = new Size(600, 500);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Шапка
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

            // Логин
            textBoxLogin = new RoundedTextBox();
            textBoxLogin.PlaceholderText = "Логин";
            textBoxLogin.BackColor = Color.FromArgb(240, 240, 240);
            textBoxLogin.ForeColor = Color.Gray;
            textBoxLogin.Size = new Size(250, 35);
            textBoxLogin.Location = new Point((this.ClientSize.Width - 250) / 2, 120);
            textBoxLogin.Anchor = AnchorStyles.Top;

            // Пароль
            textBoxPassword = new RoundedTextBox();
            textBoxPassword.PlaceholderText = "Пароль";
            textBoxPassword.BackColor = Color.FromArgb(240, 240, 240);
            textBoxPassword.ForeColor = Color.Gray;
            textBoxPassword.Size = new Size(250, 35);
            textBoxPassword.Location = new Point((this.ClientSize.Width - 250) / 2, 175);
            textBoxPassword.Anchor = AnchorStyles.Top;

            // Кнопка Войти
            buttonLogin = new RoundedButton();
            buttonLogin.Text = "Войти";
            buttonLogin.BackColor = Color.FromArgb(32, 178, 170);
            buttonLogin.ForeColor = Color.White;
            buttonLogin.FlatStyle = FlatStyle.Flat;
            buttonLogin.FlatAppearance.BorderSize = 0;
            buttonLogin.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            buttonLogin.Size = new Size(250, 40);
            buttonLogin.Location = new Point((this.ClientSize.Width - 250) / 2, 240);
            buttonLogin.Anchor = AnchorStyles.Top;
            buttonLogin.Cursor = Cursors.Hand;
            buttonLogin.Click += ButtonLogin_Click;

            // Ссылка на регистрацию
            linkRegister = new LinkLabel();
            linkRegister.Text = "Регистрация";
            linkRegister.LinkColor = Color.FromArgb(32, 178, 170);
            linkRegister.ActiveLinkColor = Color.DarkCyan;
            linkRegister.VisitedLinkColor = Color.Teal;
            linkRegister.Size = new Size(100, 23);
            linkRegister.Location = new Point((this.ClientSize.Width - 100) / 2, 295);
            linkRegister.Anchor = AnchorStyles.Top;
            linkRegister.LinkClicked += LinkRegister_LinkClicked;

            this.Controls.Add(textBoxLogin);
            this.Controls.Add(textBoxPassword);
            this.Controls.Add(buttonLogin);
            this.Controls.Add(linkRegister);
            this.Controls.Add(headerPanel);
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text.Trim();
            string password = textBoxPassword.Text.Trim();

            var user = Database.Authenticate(login, password);

            if (user != null)
            {
                Database.CurrentUser = user;
                this.Hide();
                MainForm mainForm = new MainForm();
                mainForm.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LinkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog();
            this.Show();
        }
    }
}