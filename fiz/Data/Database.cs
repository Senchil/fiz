using System;
using System.Collections.Generic;
using fiz.Models;

namespace fiz.Data
{
    public static class Database
    {
        public static User? CurrentUser { get; set; }

        public static User? Authenticate(string login, string password) =>
            SQLiteHelper.AuthenticateUser(login, password);

        public static bool Register(string login, string password, string role = "user") =>
            SQLiteHelper.RegisterUser(login, password, role);

        // ========== СТУДЕНТЫ ==========

        public static List<Student> GetStudents() =>
            SQLiteHelper.GetAllStudents();  // Все видят всех студентов

        public static void AddStudent(Student student) =>
            SQLiteHelper.AddStudent(student);

        public static void UpdateStudent(Student student)
        {
            // Admin может редактировать всё
            if (CurrentUser?.Role == "admin")
            {
                SQLiteHelper.UpdateStudent(student);
                return;
            }

            // User может редактировать только свои записи
            var existing = SQLiteHelper.GetStudentById(student.Id);
            if (existing?.CreatedBy != CurrentUser?.Login)
                throw new UnauthorizedAccessException("Вы можете редактировать только свои записи");

            SQLiteHelper.UpdateStudent(student);
        }

        public static void DeleteStudent(int id)
        {
            if (CurrentUser?.Role == "admin")
            {
                SQLiteHelper.DeleteStudent(id);
                return;
            }

            var existing = SQLiteHelper.GetStudentById(id);
            if (existing?.CreatedBy != CurrentUser?.Login)
                throw new UnauthorizedAccessException("Вы можете удалять только свои записи");

            SQLiteHelper.DeleteStudent(id);
        }

        // ========== МЕРОПРИЯТИЯ ==========

        public static List<Event> GetEvents() =>
            SQLiteHelper.GetAllEvents();  // Все видят все мероприятия

        public static void AddEvent(Event ev) =>
            SQLiteHelper.AddEvent(ev);

        public static void UpdateEvent(Event ev)
        {
            if (CurrentUser?.Role == "admin")
            {
                SQLiteHelper.UpdateEvent(ev);
                return;
            }

            var existing = SQLiteHelper.GetEventById(ev.Id);
            if (existing?.CreatedBy != CurrentUser?.Login)
                throw new UnauthorizedAccessException("Вы можете редактировать только свои записи");

            SQLiteHelper.UpdateEvent(ev);
        }

        public static void DeleteEvent(int id)
        {
            if (CurrentUser?.Role == "admin")
            {
                SQLiteHelper.DeleteEvent(id);
                return;
            }

            var existing = SQLiteHelper.GetEventById(id);
            if (existing?.CreatedBy != CurrentUser?.Login)
                throw new UnauthorizedAccessException("Вы можете удалять только свои записи");

            SQLiteHelper.DeleteEvent(id);
        }

        // ========== УЧАСТИЯ ==========

        public static List<Participation> GetParticipations() =>
            SQLiteHelper.GetAllParticipations();  // Все видят все участия

        public static void AddParticipation(Participation p) =>
            SQLiteHelper.AddParticipation(p);

        public static void UpdateParticipation(Participation p)
        {
            if (CurrentUser?.Role == "admin")
            {
                SQLiteHelper.UpdateParticipation(p);
                return;
            }

            var existing = SQLiteHelper.GetParticipationById(p.Id);
            if (existing?.AddedBy != CurrentUser?.Login)
                throw new UnauthorizedAccessException("Вы можете редактировать только свои записи");

            SQLiteHelper.UpdateParticipation(p);
        }

        public static void DeleteParticipation(int id)
        {
            if (CurrentUser?.Role == "admin")
            {
                SQLiteHelper.DeleteParticipation(id);
                return;
            }

            var existing = SQLiteHelper.GetParticipationById(id);
            if (existing?.AddedBy != CurrentUser?.Login)
                throw new UnauthorizedAccessException("Вы можете удалять только свои записи");

            SQLiteHelper.DeleteParticipation(id);
        }
    }
}