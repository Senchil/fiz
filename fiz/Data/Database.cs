using System.Collections.Generic;
using fiz.Models;

namespace fiz.Data
{
    public static class Database
    {
        // Текущий пользователь (может быть null если не авторизован)
        public static User? CurrentUser { get; set; }

        // ========== АВТОРИЗАЦИЯ И РЕГИСТРАЦИЯ ==========

        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        public static User? Authenticate(string login, string password) =>
            SQLiteHelper.AuthenticateUser(login, password);

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        public static bool Register(string login, string password) =>
            SQLiteHelper.RegisterUser(login, password);

        // ========== СТУДЕНТЫ ==========

        /// <summary>
        /// Получить всех студентов
        /// </summary>
        public static List<Student> GetStudents() =>
            SQLiteHelper.GetAllStudents();

        /// <summary>
        /// Добавить студента
        /// </summary>
        public static void AddStudent(Student student) =>
            SQLiteHelper.AddStudent(student);

        /// <summary>
        /// Обновить данные студента
        /// </summary>
        public static void UpdateStudent(Student student) =>
            SQLiteHelper.UpdateStudent(student);

        /// <summary>
        /// Удалить студента по ID
        /// </summary>
        public static void DeleteStudent(int id) =>
            SQLiteHelper.DeleteStudent(id);

        // ========== МЕРОПРИЯТИЯ ==========

        /// <summary>
        /// Получить все мероприятия
        /// </summary>
        public static List<Event> GetEvents() =>
            SQLiteHelper.GetAllEvents();

        /// <summary>
        /// Добавить мероприятие
        /// </summary>
        public static void AddEvent(Event ev) =>
            SQLiteHelper.AddEvent(ev);

        // ========== УЧАСТИЯ ==========

        /// <summary>
        /// Получить все участия
        /// </summary>
        public static List<Participation> GetParticipations() =>
            SQLiteHelper.GetAllParticipations();

        /// <summary>
        /// Добавить участие
        /// </summary>
        public static void AddParticipation(Participation p) =>
            SQLiteHelper.AddParticipation(p);
    }
}