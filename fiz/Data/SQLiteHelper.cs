using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using fiz.Models;
using BCrypt.Net;
using Microsoft.Data.Sqlite;

namespace fiz.Data
{
    public static class SQLiteHelper
    {
        public static string ConnectionString { get; private set; }

        public static void Initialize()
        {
            string dbPath = Path.Combine(Application.StartupPath, "database.db");

            try
            {
                ConnectionString = $"Data Source={dbPath}";

                CreateTables();
                CreateAdmin();
                InsertTestData();
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CreateTables()
        {
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();

                string[] tables = {
                    @"CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Login TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL
                    )",
                    @"CREATE TABLE IF NOT EXISTS Students (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FullName TEXT NOT NULL,
                        Faculty TEXT NOT NULL,
                        ""Group"" TEXT NOT NULL,
                        StudentCardNumber TEXT NOT NULL,
                        BirthDate TEXT NOT NULL,
                        ContactInfo TEXT NOT NULL
                    )",
                    @"CREATE TABLE IF NOT EXISTS Events (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Date TEXT NOT NULL,
                        Location TEXT NOT NULL,
                        Organizer TEXT NOT NULL,
                        SportType TEXT NOT NULL,
                        ParticipantCount INTEGER NOT NULL
                    )",
                    @"CREATE TABLE IF NOT EXISTS Participations (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EventName TEXT NOT NULL,
                        StudentName TEXT NOT NULL,
                        Result TEXT NOT NULL,
                        Award TEXT NOT NULL,
                        Rank TEXT NOT NULL,
                        AddedBy TEXT NOT NULL,
                        Date TEXT NOT NULL
                    )"
                };

                foreach (var table in tables)
                {
                    using (var cmd = new SqliteCommand(table, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CreateAdmin()
        {
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();

                var cmd = new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Login='admin'", conn);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count == 0)
                {
                    string hash = BCrypt.Net.BCrypt.HashPassword("admin");
                    cmd.CommandText = "INSERT INTO Users (Login, PasswordHash, CreatedAt) VALUES ('admin', @hash, @date)";
                    cmd.Parameters.AddWithValue("@hash", hash);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void InsertTestData()
        {
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();

                var cmd = new SqliteCommand("SELECT COUNT(*) FROM Students", conn);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count == 0)
                {
                    cmd.CommandText = @"INSERT INTO Students (FullName, Faculty, ""Group"", StudentCardNumber, BirthDate, ContactInfo) 
                                        VALUES ('Черкасский Данил Сергеевич', 'ФМИЗ', 'ИИ-23', '1337', '04.02.2005', 'danil@gmail.com')";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"INSERT INTO Students (FullName, Faculty, ""Group"", StudentCardNumber, BirthDate, ContactInfo) 
                                        VALUES ('Петров Петр Петрович', 'ФИ', 'ИО-21', '1338', '31.01.2003', '88005553535')";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"INSERT INTO Events (Name, Date, Location, Organizer, SportType, ParticipantCount) 
                                        VALUES ('Кубок КГПИ', '27.02.2026', 'СК ''Олимп''', 'Факультетский', 'Плавание', 25)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"INSERT INTO Participations (EventName, StudentName, Result, Award, Rank, AddedBy, Date) 
                                        VALUES ('Кубок КГПИ', 'Черкасский Данил Сергеевич', 'Призовое место', 'Медаль', '1-ый разряд', 'ivanov', '05.03.2026')";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ========== АВТОРИЗАЦИЯ И РЕГИСТРАЦИЯ ==========

        public static User AuthenticateUser(string login, string password)
        {
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand("SELECT * FROM Users WHERE Login = @login", conn);
                cmd.Parameters.AddWithValue("@login", login);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string hash = reader["PasswordHash"].ToString();
                        if (BCrypt.Net.BCrypt.Verify(password, hash))
                        {
                            return new User
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Login = reader["Login"].ToString(),
                                Password = hash
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static bool RegisterUser(string login, string password)
        {
            try
            {
                using (var conn = new SqliteConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqliteCommand("SELECT COUNT(*) FROM Users WHERE Login = @login", conn);
                    cmd.Parameters.AddWithValue("@login", login);

                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        return false;

                    string hash = BCrypt.Net.BCrypt.HashPassword(password);
                    cmd.CommandText = "INSERT INTO Users (Login, PasswordHash, CreatedAt) VALUES (@login, @hash, @date)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@hash", hash);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        // ========== СТУДЕНТЫ ==========

        public static List<Student> GetAllStudents()
        {
            var list = new List<Student>();
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand("SELECT * FROM Students", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Student
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            FullName = reader["FullName"].ToString(),
                            Faculty = reader["Faculty"].ToString(),
                            Group = reader["Group"].ToString(),
                            StudentCardNumber = reader["StudentCardNumber"].ToString(),
                            BirthDate = reader["BirthDate"].ToString(),
                            ContactInfo = reader["ContactInfo"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public static void AddStudent(Student s)
        {
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand(@"INSERT INTO Students (FullName, Faculty, ""Group"", StudentCardNumber, BirthDate, ContactInfo) 
                                              VALUES (@FullName, @Faculty, @Group, @StudentCardNumber, @BirthDate, @ContactInfo)", conn);
                cmd.Parameters.AddWithValue("@FullName", s.FullName);
                cmd.Parameters.AddWithValue("@Faculty", s.Faculty);
                cmd.Parameters.AddWithValue("@Group", s.Group);
                cmd.Parameters.AddWithValue("@StudentCardNumber", s.StudentCardNumber);
                cmd.Parameters.AddWithValue("@BirthDate", s.BirthDate);
                cmd.Parameters.AddWithValue("@ContactInfo", s.ContactInfo);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateStudent(Student s)
        {
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand(@"UPDATE Students SET FullName = @FullName, Faculty = @Faculty, 
                                              ""Group"" = @Group, StudentCardNumber = @StudentCardNumber, 
                                              BirthDate = @BirthDate, ContactInfo = @ContactInfo WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", s.Id);
                cmd.Parameters.AddWithValue("@FullName", s.FullName);
                cmd.Parameters.AddWithValue("@Faculty", s.Faculty);
                cmd.Parameters.AddWithValue("@Group", s.Group);
                cmd.Parameters.AddWithValue("@StudentCardNumber", s.StudentCardNumber);
                cmd.Parameters.AddWithValue("@BirthDate", s.BirthDate);
                cmd.Parameters.AddWithValue("@ContactInfo", s.ContactInfo);
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteStudent(int id)
        {
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand("DELETE FROM Students WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ========== МЕРОПРИЯТИЯ ==========

        public static List<Event> GetAllEvents()
        {
            var list = new List<Event>();
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand("SELECT * FROM Events", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Event
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Date = reader["Date"].ToString(),
                            Location = reader["Location"].ToString(),
                            Organizer = reader["Organizer"].ToString(),
                            SportType = reader["SportType"].ToString(),
                            ParticipantCount = Convert.ToInt32(reader["ParticipantCount"])
                        });
                    }
                }
            }
            return list;
        }

        public static void AddEvent(Event ev)
        {
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand(@"INSERT INTO Events (Name, Date, Location, Organizer, SportType, ParticipantCount) 
                                              VALUES (@Name, @Date, @Location, @Organizer, @SportType, @ParticipantCount)", conn);
                cmd.Parameters.AddWithValue("@Name", ev.Name);
                cmd.Parameters.AddWithValue("@Date", ev.Date);
                cmd.Parameters.AddWithValue("@Location", ev.Location);
                cmd.Parameters.AddWithValue("@Organizer", ev.Organizer);
                cmd.Parameters.AddWithValue("@SportType", ev.SportType);
                cmd.Parameters.AddWithValue("@ParticipantCount", ev.ParticipantCount);
                cmd.ExecuteNonQuery();
            }
        }

        // ========== УЧАСТИЯ ==========

        public static List<Participation> GetAllParticipations()
        {
            var list = new List<Participation>();
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand("SELECT * FROM Participations", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Participation
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            EventName = reader["EventName"].ToString(),
                            StudentName = reader["StudentName"].ToString(),
                            Result = reader["Result"].ToString(),
                            Award = reader["Award"].ToString(),
                            Rank = reader["Rank"].ToString(),
                            AddedBy = reader["AddedBy"].ToString(),
                            Date = reader["Date"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public static void AddParticipation(Participation p)
        {
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqliteCommand(@"INSERT INTO Participations (EventName, StudentName, Result, Award, Rank, AddedBy, Date) 
                                              VALUES (@EventName, @StudentName, @Result, @Award, @Rank, @AddedBy, @Date)", conn);
                cmd.Parameters.AddWithValue("@EventName", p.EventName);
                cmd.Parameters.AddWithValue("@StudentName", p.StudentName);
                cmd.Parameters.AddWithValue("@Result", p.Result);
                cmd.Parameters.AddWithValue("@Award", p.Award);
                cmd.Parameters.AddWithValue("@Rank", p.Rank);
                cmd.Parameters.AddWithValue("@AddedBy", p.AddedBy);
                cmd.Parameters.AddWithValue("@Date", p.Date);
                cmd.ExecuteNonQuery();
            }
        }
    }
}