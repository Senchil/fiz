using System.Collections.Generic;
using fiz.Models;

namespace fiz.Data
{
    public static class Database
    {
        // Текущий пользователь
        public static User CurrentUser { get; set; }

        // Методы для получения данных из БД
        public static List<Student> GetStudents() => SQLiteHelper.GetAllStudents();
        public static List<Event> GetEvents() => SQLiteHelper.GetAllEvents();
        public static List<Participation> GetParticipations() => SQLiteHelper.GetAllParticipations();

        // Методы для добавления
        public static void AddStudent(Student student) => SQLiteHelper.AddStudent(student);
        public static void AddEvent(Event ev) => SQLiteHelper.AddEvent(ev);
        public static void AddParticipation(Participation participation) => SQLiteHelper.AddParticipation(participation);

        // Авторизация и регистрация
        public static User Authenticate(string login, string password) => SQLiteHelper.AuthenticateUser(login, password);
        public static bool Register(string login, string password) => SQLiteHelper.RegisterUser(login, password);
    }
}