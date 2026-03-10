using System;
using System.Windows.Forms;
using fiz.Data;

namespace fiz
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ИНИЦИАЛИЗИРУЕМ БД ПЕРЕД ЗАПУСКОМ!
            SQLiteHelper.Initialize();

            Application.Run(new LoginForm());
        }
    }
}