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

        public static List<Student> GetStudents() =>
            SQLiteHelper.GetAllStudents();

        public static void AddStudent(Student student) =>
            SQLiteHelper.AddStudent(student);

        public static void UpdateStudent(Student student) =>
            SQLiteHelper.UpdateStudent(student);

        public static void DeleteStudent(int id) =>
            SQLiteHelper.DeleteStudent(id);

        public static List<Event> GetEvents() =>
            SQLiteHelper.GetAllEvents();

        public static void AddEvent(Event ev) =>
            SQLiteHelper.AddEvent(ev);

        public static void UpdateEvent(Event ev) =>
            SQLiteHelper.UpdateEvent(ev);

        public static void DeleteEvent(int id) =>
            SQLiteHelper.DeleteEvent(id);

        public static List<Participation> GetParticipations() =>
            SQLiteHelper.GetAllParticipations();

        public static void AddParticipation(Participation p) =>
            SQLiteHelper.AddParticipation(p);

        public static void UpdateParticipation(Participation p) =>
            SQLiteHelper.UpdateParticipation(p);

        public static void DeleteParticipation(int id) =>
            SQLiteHelper.DeleteParticipation(id);

        public static List<Rank> GetRanks() =>
            SQLiteHelper.GetAllRanks();

        public static void AddStudentRank(StudentRank sr) =>
            SQLiteHelper.AddStudentRank(sr);

        public static List<StudentRank> GetStudentRanks(int studentId) =>
            SQLiteHelper.GetStudentRanks(studentId);

        public static void DeleteStudentRank(int id) =>
            SQLiteHelper.DeleteStudentRank(id);
    }
}